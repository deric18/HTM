//Grow , Prune
//Segment ID Syntax : << Neuron Position3D > -- < Segment Number > -- < Segment Position3D >>
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
        public Position3D BasePosition { get; private set; }    // Position where the Semgnet Originates and grows out of!.
        public uint _sumVoltage { get; private set; }
        public Position3D NeuronID { get; private set; }
        private SegmentID _segID;
        private uint _segmentNumber;
        private bool _fullyConnected;
        private Dictionary<Position3D, uint> _synapses;     //uint helps in prunning anything thats zero is taken out and flushed to connection table.
        private Lazy<List<Segment>> SubSegments;        
        private List<Position3D> _predictedSynapses;
        private uint _lastAccesedCycle;                     //helps in prunning of segments

        private SegmentType sType;
        private static uint NMDA_Spike_Potential;
        private static uint MAX_Connection_Strength;
        private static uint NEW_SYNAPSE_CONNECTION_DEF;
        private uint MAX_SUBSEGMENTS_SEGMENT;

        public Segment(Position3D basePos, SegmentType sType, Position3D neuronID, uint segCount)
        {            
            this._segmentNumber = segCount;
            NeuronID = neuronID;
            this.BasePosition = basePos;
            this.sType = sType;
            _sumVoltage = 0;               
            _fullyConnected = false;
            _synapses = new Dictionary<Position3D, uint>();
            _lastAccesedCycle = 0;
            _predictedSynapses = new List<Position3D>();            
            NMDA_Spike_Potential = uint.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]);
            MAX_Connection_Strength = uint.Parse(ConfigurationManager.AppSettings["MAX_CONNECTION_STRENGTH"]);
            MAX_SUBSEGMENTS_SEGMENT = uint.Parse(ConfigurationManager.AppSettings["MAX_SUBSEGMENTS_SEGMENT"]);
            NEW_SYNAPSE_CONNECTION_DEF = uint.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]);
        }

        private string ComputeSegmentIDasString(Position3D neuronID, string segCount, Position3D basePos)
        {
            return neuronID.StringIDWithoutBID + "/" + segCount + "/" + basePos.StringIDWithoutBID;
        }

        public string ComputeSegmentIdAsString()
        {
            return NeuronID.StringIDWithoutBID + "/" + this._segmentNumber + BasePosition.StringIDWithoutBID;
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
            //switch (iType)
            //{
            //    case InputPatternType.INTERNAL:
            //        {
            //            _internalVoltage += voltage;
            //            break;
            //        }
            //    case InputPatternType.TEMPORAL:
            //        {
            //            _temporalVoltage += voltage;
            //            break;
            //        }
            //    case InputPatternType.APICAL:
            //        {
            //            _apicalVoltage += voltage;
            //            break;
            //        }
            //    default:break;
            //}
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

            SynapseGenerator sg =  CPM.GetInstance.synapseGenerator;
            Position3D newPosition;

            if ((_synapses.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"])))
            {//Below number of synapses threshold for segment 
                
                newPosition = sg.PredictNewRandomPosition(this.BasePosition);                                 

                ConnectionType cPos = CPM.GetInstance.CTable.ClaimPosition(newPosition, _segID, EndPointType.Dendrite);

                AddConnection(newPosition);
            } 
            else if(SubSegments.Value.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]))
            {//Below number of subsegments threshold for segment 
                //handle logic for if this position is already marked in ctable then do a new position. basically dont do anything everything is already handled in ctable.
                newPosition = sg.PredictNewRandomPosition(this.BasePosition);

                ConnectionType cPos = CPM.GetInstance.CTable.ClaimPosition(newPosition, _segID, EndPointType.Dendrite);

                CreateSubSegment(newPosition);
            }
            else
            {//above both threshold print a msg and exit
                Console.WriteLine("Segment: " + PrintPosition(this.BasePosition) + "SEG ID: " + this.BasePosition.BID + "-" + " is Over Connected");                
                _fullyConnected = true;
                //log Information with details , Segment has reached a peak connection pathway , this is essentially a crucial segment for the whole region.
            }            
        }              

        public void Grow(Position3D synapseId) 
        {
            //TBD
            if(_synapses.TryGetValue(synapseId, out uint synapticStrength))
            {
                synapticStrength++;
                _synapses.Remove(synapseId);
                _synapses.Add(synapseId, synapticStrength);
            }
            else
            {
                //Search in subsegments
                if(SubSegments.IsValueCreated)
                {

                }
                else
                {
                    //No Sub Segments created yet , The synapseId sent to grow does not exist here , Code is MEssed Up , This should never happen throw na ex

                    throw new Exception("ERROR ERROR !!! This should never happen ! : Synapse Id sent to grow doesnt exist in the segment!!!");
                }

            }


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

        private void PrintSegmnetID()
        {
            Console.Write(ComputeSegmentIdAsString());
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
                uint count = (uint)SubSegments.Value.Count;
                string newSegId = ComputeSegmentIdAsString() + "-" + (++count).ToString();
                Segment newSegment = new Segment(basePosition, this.sType, NeuronID, count);

                //Might also need to register this subsegment in Neuron. otherwise it wont be able to send the grow signal
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
                if(seg.ComputeSegmentIdAsString().Equals(newPosition))
                {
                    return false;
                }
            }

            return true;
        }

        private bool DoesConnectionExist(Position3D pos)
        {
            //Do we have a synapse at this position already ?
            uint val;
            if (_synapses.TryGetValue(pos, out val))
            {
                return true;
            }
            return false;
        }
        
    }
}
