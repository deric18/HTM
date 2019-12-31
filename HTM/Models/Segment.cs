using System.Configuration;
using System.Collections.Generic;
using System;
using HTM.Enums;

namespace HTM.Models
{
    /// <summary>
    /// -If Proximal Polarization  of the system
    /// -NMDA
    /// -Segment Growth.
    /// </summary>
    public class Segment
    {                        
        public SegmentID SegmentId { get; private set; }

        private BranchingTechnique _bType;
        private SegmentType sType;
        private uint _sumVoltage;
        private uint _temporalVoltage;
        private uint _apicalVoltage;
        private uint _internalVoltage;            
        private bool _fullyConnected;          
        private List<Segment> SubSegments;
        private Dictionary<Synapse, uint> _connections;    //strength
        private List<Synapse> _predictedSynapses;
        private static uint NMDA_Spike_Potential;
        private static uint MAX_Connection_Strength;
        private static uint NEW_SYNAPSE_CONNECTION_DEF;
        

        public Segment(SegmentID segmentID, Vector baseVector, int seed)
        {            
            SegmentId = segmentID;
            _bType = BranchingTechnique.BranchBinary;
            _sumVoltage = 0;
            _temporalVoltage = 0;
            _apicalVoltage = 0;            
            _fullyConnected = false;
            _connections = new Dictionary<Synapse, uint>();
            _predictedSynapses = new List<Synapse>();            
            NMDA_Spike_Potential = uint.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]);
            MAX_Connection_Strength = uint.Parse(ConfigurationManager.AppSettings["MAX_CONNECTION_STRENGTH"]);
            NEW_SYNAPSE_CONNECTION_DEF = uint.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]);
        }       

        internal Segment GetSegment(int v)
        {
            if(v < SubSegments.Count)
            {
                return SubSegments[v];
            }

            return null;
        }

        internal void AddNewConnection(Synapse pos) =>        
            _connections.Add(pos, 5);        

        /// <summary>
        /// Predict if the segment will fire or not based incoming voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="firingNeuronId"></param>
        /// <returns></returns>
        public bool Process(uint voltage, Synapse synapseId, InputPatternType iType)
        {
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

            _sumVoltage += voltage;            

            if(_sumVoltage > NMDA_Spike_Potential)
            {
                //NMDA Spike
                //Strengthen firing Neuron Connection.
                uint connectionStrength;
                if(_connections.TryGetValue(synapseId, out connectionStrength))
                {
                    if (connectionStrength < MAX_Connection_Strength)                        
                        _connections[synapseId]++;
                }

                return true;
            }
            return false;
        }                    

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Boxed Growth : Direction and boxed random connection
        /// 1.A Segment should have direction it grows and by default will have limits set to how far away the segment can predict its new connection.
        /// </summary>
        internal void AddNewLocalConnection()
        {            
            //Make sure you are not connecting to an axon of your own neuron if its a new position
            /*Decide how to add new connection : Randomly pick connections from the segments local visibility radius and connect.
            */
            //If not branched and position is noand check if segment has max positions , add this position to a possibility list for future connection when the neuron losses and non used connection else if not max position then add new position,
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.  
            Synapse bounds = SynapseManager.GetBound();
            Synapse newPosition = GetNewPositionFromBound(bounds);            

            if (!_fullyConnected && (_connections.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"])) && !DoesConnectionExist(newPosition) && !SelfConnection(newPosition))
            {
                AddConnection(newPosition);
            }
            else if(!_fullyConnected && SubSegments?.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]) && !DoesSubSegmentExist(newPosition))
            {                
                CreateSubSegment(newPosition);
            }
            else
            {
                Console.WriteLine("Segment " + PrintPosition(NeuronID) + "-" + SegmentID + " is Over Connected");                
                _fullyConnected = true;
                //log Information with details , Segment has reached a peak connection pathway , this is essentially a crucial segment for the whole region.
            }            
        }       

        public void Prune()
        {
            //Run through synaptic list to eliminate neurons with lowest connection strength
            foreach(var s in _synapticStrength)
            {
                if(s.Value <= int.Parse(ConfigurationManager.AppSettings["PRUNE_THRESHOLD"]))
                {
                    Console.WriteLine("Removing synapse to Neuron" + PrintPosition(s.Key));
                    _synapticStrength.Remove(s.Key);
                }
            }            

            foreach(var segment in SubSegments)
            {
                segment.Prune();
            }
        }

        private bool SelfConnection(Synapse newPosition)
        {
            if (NeuronID.Equals(newPosition))
                return true;

            return false;
        }

        //private Position3D GetNewPositionFromBound(Position3D segmentBound)
        //{
        //    Random r = new Random(_seed);
        //    return new Position3D((uint)r.Next((int)BaseConnection.X, (int)segmentBound.X), (uint)r.Next((int)BaseConnection.Y, (int)segmentBound.Y), (uint)r.Next((int)BaseConnection.Z, (int)segmentBound.Z));
        //}                   

        private void AddConnection(Synapse newPosition) =>
            _connections.Add(newPosition, NEW_SYNAPSE_CONNECTION_DEF);                            

        private void CreateSubSegment(Synapse basePosition)
        {
            if (!_hasSubSegments)
            {
                _hasSubSegments = true;
                SubSegments = new List<Segment>();
            }
            
            string newSegId = SegmentID + "-" + SubSegments.Count.ToString();
            Segment newSegment = new Segment(NeuronID, newSegId, basePosition, SubSegments.Count);
            SubSegments.Add(newSegment);            
        }

        internal void FlushVoltage()
        {
            _sumVoltage = 0;
            _predictedSynapses.Clear();
        }

        private string PrintPosition(Synapse pos4d)
        {
            return " X: " + pos4d.X.ToString() + " Y:" + pos4d.Y.ToString() + " Z:" + pos4d.Z.ToString();
        }

        private bool DoesSubSegmentExist(Synapse newPosition)
        {
            foreach(var seg in SubSegments)
            {
                if(seg.BaseConnection.Equals(newPosition))
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
        private void SegmentGrow()
        {
            switch (_bType)
            {
                case BranchingTechnique.BranchBinary:
                    _bType = BranchingTechnique.LeftBranch;
                    break;
                case BranchingTechnique.LeftBranch:
                    _bType = BranchingTechnique.RightBranch;
                    break;
                case BranchingTechnique.RightBranch:
                    _bType = BranchingTechnique.BranchBinary;
                    break;
                default: break;
            }

            foreach (var segment in SubSegments)
            {
                segment.SegmentGrow();
            }
        }

        #endregion

        private bool DoesConnectionExist(Synapse pos)
        {
            uint val;
            if(_connections.TryGetValue(pos, out val))
            {
                return true;
            }
            return false;
        }
    }
}
