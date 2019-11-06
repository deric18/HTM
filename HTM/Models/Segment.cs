using System.Configuration;
using System.Collections.Generic;
using System;
using HTM.Enums;

namespace HTM.Models
{
    /// <summary>
    /// Process, Grow
    /// </summary>
    public class Segment
    {        
        public Position3D NeuronID { get; private set; }        
        public Vector BaseConnection { get; private set; }
        private BranchingTechnique _bType;
        private SegmentType _isProximal;
        private uint _sumVoltage;
        private uint _temporalVoltage;
        private uint _apicalVoltage;
        public string SegmentID { get; private set; }
        private bool _hasSubSegments;        
        private bool _fullyConnected;
        private int _seed;                
        private List<Segment> SubSegments;
        private Dictionary<Position3D, int> _synapticStrength;    //strength
        private List<Position3D> __lastTimeStampFiringConnections;        
        

        public Segment(Position3D neuronId, string segmentID, Vector baseVector, int seed)
        {
            NeuronID = neuronId;
            SegmentID = segmentID;
            _bType = BranchingTechnique.BranchBinary;
            _sumVoltage = 0;
            _temporalVoltage = 0;
            _apicalVoltage = 0;
            _hasSubSegments = false;
            _fullyConnected = false;
            _synapticStrength = new Dictionary<Position3D, int>();
            __lastTimeStampFiringConnections = new List<Position3D>();
            BaseConnection = baseVector;
            _seed = seed;
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
            switch(_bType)
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
                default:break;
            }

            foreach(var segment in SubSegments)
            {
                segment.SegmentGrow();
            }
        }

        #endregion

        internal Segment GetSegment(int v)
        {
            if(v < SubSegments.Count)
            {
                return SubSegments[v];
            }

            return null;
        }

        /// <summary>
        /// Predict if the segment will fire or not based incoming voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="firingNeuronId"></param>
        /// <returns></returns>
        public bool Process(uint voltage, Position3D firingNeuronId, InputPatternType iType)
        {
            switch (iType)
            {                
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

            if(_sumVoltage > int.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]))
            {
                //NMDA Spike
                //Strengthen firing Neuron Connection.
                int connectionStrength;
                if(_synapticStrength.TryGetValue(firingNeuronId, out connectionStrength))
                {
                    if (connectionStrength > int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTION_STRENGTH"]))
                        return true;
                    _synapticStrength[firingNeuronId]++;
                }
                return true;
            }

            return false;
        }               

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
            Position3D bounds = CPM.GetBound();
            Position3D newPosition = GetNewPositionFromBound(bounds);            

            if (!_fullyConnected && (_synapticStrength.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"])) && !DoesConnectionExist(newPosition) && !SelfConnection(newPosition))
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

        private bool SelfConnection(Position3D newPosition)
        {
            if (NeuronID.Equals(newPosition))
                return true;

            return false;
        }

        private Position3D GetNewPositionFromBound(Position3D segmentBound)
        {
            Random r = new Random(_seed);
            return new Position3D((uint)r.Next((int)BaseConnection.X, (int)segmentBound.X), (uint)r.Next((int)BaseConnection.Y, (int)segmentBound.Y), (uint)r.Next((int)BaseConnection.Z, (int)segmentBound.Z));
        }                   

        private void AddConnection(Position3D newPosition) =>
            _synapticStrength.Add(newPosition, int.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]));                            

        private void CreateSubSegment(Position3D basePosition)
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
            __lastTimeStampFiringConnections = new List<Position3D>();
        }

        private string PrintPosition(Position3D pos3d)
        {
            return " X: " + pos3d.X.ToString() + " Y:" + pos3d.Y.ToString() + " Z:" + pos3d.Z.ToString();
        }

        private bool DoesSubSegmentExist(Position3D newPosition)
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

        private bool DoesConnectionExist(Position3D pos)
        {
            int val;
            if(_synapticStrength.TryGetValue(pos, out val))
            {
                return true;
            }
            return false;
        }
    }
}
