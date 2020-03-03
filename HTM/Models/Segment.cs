using System.Configuration;
using System.Collections.Generic;
using System;
using HTM.Enums;
using HTM.Algorithms;

namespace HTM.Models
{
    //NOTE: line : 103
    /// <summary>
    /// -If Proximal Polarization  of the system
    /// -NMDA
    /// -Segment Growth.
    /// </summary>
    public class Segment
    {                        
        public SegmentID SegmentId { get; private set; }
        public uint _sumVoltage { get; private set; }
       
       
        private bool _fullyConnected;
        private Dictionary<Position3D, uint> _synapses;     //uint helps in prunning anything thats zero is taken out and flushed to connection table.
        private Lazy<List<Segment>> SubSegments;        
        private List<Position3D> _predictedSynapses;
        private uint _lastAccesedCycle;                     //helps in prunning of segments

        private SegmentType sType;
        private static uint NMDA_Spike_Potential;
        private static uint MAX_Connection_Strength;
        private static uint NEW_SYNAPSE_CONNECTION_DEF;
        

        public Segment(SegmentID segmentID, SegmentType sType)
        {            
            this.SegmentId = segmentID;
            this.sType = sType;
//            _bType = BranchingTechnique.BranchBinary;
            _sumVoltage = 0;               
            _fullyConnected = false;
            _synapses = new Dictionary<Position3D, uint>();
            _lastAccesedCycle = 0;
            _predictedSynapses = new List<Position3D>();            
            NMDA_Spike_Potential = uint.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]);
            MAX_Connection_Strength = uint.Parse(ConfigurationManager.AppSettings["MAX_CONNECTION_STRENGTH"]);
            NEW_SYNAPSE_CONNECTION_DEF = uint.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]);
        }       

        internal Segment GetSegment(int v)
        {
            if(SubSegments.IsValueCreated)
            {
                return SubSegments.Value[v];
            }

            return null;
        }

        internal void AddNewConnection(Position3D pos) =>        
            _synapses.Add(pos, 5);        

        /// <summary>
        /// Predict if the segment will fire or not based incoming voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="firingNeuronId"></param>
        /// <returns></returns>
        public bool Process(uint voltage, Position3D synapseId, InputPatternType iType)
        {
            #region REMOVED
            switch (iType)
            {
                case InputPatternType.INTERNAL:
                    {
                        _internalVoltage += voltage;
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        _temporalVoltage += voltage;
                        break;
                    }
                case InputPatternType.APICAL:
                    {
                        _apicalVoltage += voltage;
                        break;
                    }
                default:break;
            }
            #endregion

            _sumVoltage += voltage;            

            if(_sumVoltage > NMDA_Spike_Potential)
            {
                //NMDA Spike
                //Strengthen firing Neuron Connection.
                uint connectionStrength;
                if(_synapses.TryGetValue(synapseId, out connectionStrength))
                {
                    if (connectionStrength < MAX_Connection_Strength)                        
                        _synapses[synapseId]++;                                                  //Debug and make sure this works fine
                }

                return true;
            }
            return false;
        }                    

        public void Fire()
        {
            if(this.sType.Equals(SegmentType.Axonal))
            {

            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Boxed Growth : Direction and boxed random connection
        /// 1.A Segment should have direction it grows and by default will have limits set to how far away the segment can predict its new connection.
        /// </summary>
        private void AddNewLocalConnection()
        {            
            //Make sure you are not connecting to an axon of your own neuron if its a new position
            /*Decide how to add new connection : Randomly pick connections from the segments local visibility radius and connect.
            */
            //If not branched and position is noand check if segment has max positions , add this position to a possibility list for future connection when the neuron losses and non used connection else if not max position then add new position,
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.                        
            //Alert Connection Table about the new position

            SynapseGenerator sg = new SynapseGenerator();
            Position3D newPosition;

            if (!_fullyConnected && (_synapses.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"])) && !DoesConnectionExist(newPosition) && !SelfConnection(newPosition))
            {
                newPosition = sg.PredictNewRandomSynapse(BasePostion);
                AddConnection(newPosition);
            }
            else if(!_fullyConnected && SubSegments.Value.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]) && !DoesSubSegmentExist(newPosition))
            {
                newPosition = sg.PredictNewRandomSynapse(BasePostion);
                CreateSubSegment(newPosition);
            }
            else
            {
                Console.WriteLine("Segment: " + PrintPosition(SegmentId) + "SEG ID: " + SegmentId.BID + "-" + " is Over Connected");                
                _fullyConnected = true;
                //log Information with details , Segment has reached a peak connection pathway , this is essentially a crucial segment for the whole region.
            }            
        }       

        private void PickLocalForNewSegment()
        {

        }

        public void Prune()
        {
            //Run through synaptic list to eliminate neurons with lowest connection strength
            //also run through the subsegments and check the lastaccessed time and there synapses.
            foreach(var s in _synapses)
            {
                if(s.Value <= int.Parse(ConfigurationManager.AppSettings["PRUNE_THRESHOLD"]))
                {
                    Console.WriteLine("Removing synapse to Neuron" + PrintPosition(s.Key));
                    _synapses.Remove(s.Key);
                }
            }            

            foreach(var segment in SubSegments.Value)
            {
                segment.Prune();
            }
        }

        private bool SelfConnection(Position3D newPosition)
        {
            
        }

        private void PrintSegmnetID()
        {
            Console.Write("X: {0} Y: {1} Z: {2}", SegmentId.X, SegmentId.Y, SegmentId.Z);
        }

        //private Position3D GetNewPositionFromBound(Position3D segmentBound)
        //{
        //    Random r = new Random(_seed);
        //    return new Position3D((uint)r.Next((int)BaseConnection.X, (int)segmentBound.X), (uint)r.Next((int)BaseConnection.Y, (int)segmentBound.Y), (uint)r.Next((int)BaseConnection.Z, (int)segmentBound.Z));
        //}                   

        private void AddConnection(Position3D newPosition) =>
            _synapses.Add(newPosition, NEW_SYNAPSE_CONNECTION_DEF);                            

        private void CreateSubSegment(Position3D basePosition)
        {
            if (!this.SubSegments.IsValueCreated)
            {
                int count = SubSegments.Value.Count;
                string newSegId = SegmentId.StringID + "-" + (++count).ToString();
                Segment newSegment = new Segment(new SegmentID(basePosition), sType);
                SubSegments.Value.Add(newSegment);
            }                        
        }

        internal void FlushVoltage()
        {
            _sumVoltage = 0;
            _predictedSynapses.Clear();
        }

        private string PrintPosition(Position3D pos4d)
        {
            return " X: " + pos4d.X.ToString() + " Y:" + pos4d.Y.ToString() + " Z:" + pos4d.Z.ToString();
        }


        //finds out if there are any other segments that have already created a sub segmetn here , this might need to create a standard of segment ID syntax and this function needs to be done at the base segment level.
        private bool DoesSubSegmentExist(Position3D newPosition)
        {
            foreach(var seg in SubSegments.Value)
            {
                if(seg.SegmentId.Equals(newPosition))
                {
                    return false;
                }
            }

            return true;
        }

        #region Private Methods

        /// <summary>
        /// This is a General Grow Signal
        /// Questions:
        /// -When to branch and when to GetNewConnection
        ///  --Role out a Round Robin.
        /// </summary>        
        //private void Grow()
        //{
        //    switch (_bType)
        //    {
        //        case BranchingTechnique.BranchBinary:
        //            _bType = BranchingTechnique.LeftBranch;
        //            break;
        //        case BranchingTechnique.LeftBranch:
        //            _bType = BranchingTechnique.RightBranch;
        //            break;
        //        case BranchingTechnique.RightBranch:
        //            _bType = BranchingTechnique.BranchBinary;
        //            break;
        //        default: break;
        //    }

        //    foreach (var segment in SubSegments.Value)
        //    {
        //        segment.Grow();
        //    }
        //}

        #endregion

        private bool DoesConnectionExist(Position3D pos)
        {
            uint val;
            if(_synapses.TryGetValue(pos, out val))
            {
                return true;
            }
            return false;
        }
    }
}
