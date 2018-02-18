using System.Configuration;
using System.Collections.Generic;
using System;

namespace HTM.Models
{
    /// <summary>
    /// Process, Grow
    /// </summary>
    public class Segment
    {
        public Position4D _basePosition;
        public Position2D NeuronID;
        private bool _fullyConnected;
        public Position4D SegmentID;
        public string ID { get; private set; }
        private uint _sumVoltage;
        private Dictionary<Position4D, int> Connections;        
        private bool _hasSubSegments;
        private List<Segment> SubSegments;
        private uint _spikeCounter;

        public Segment(Position2D neuronID, Position4D segmentID, int id)
        {
            ID = segmentID + "-" + id.ToString();
            NeuronID = neuronID;
            _sumVoltage = 0;
            Connections = new Dictionary<Position4D, int>();
            _hasSubSegments = false;
            _spikeCounter = 0;
            _fullyConnected = false;
        }
        
        public bool Process(uint voltage, Position4D pos3d)
        {
            _sumVoltage += voltage;
            if(_sumVoltage > int.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]))
            {
                //NMDA SPIKE
                //Update pos3d strength'
                return true;
            }

            return false;
        }

        public void Grow()
        {
            //Make sure you are not connecting to an axon of your own neuron
            //If already bracnhed and maxed out on MAXBRANCH , dont branch and add new position to the highest spiking branch
            //If not branched and check if segment has max positions else create new branch 
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.
            if (Connections.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"]))
            {
                AddNewConnection();                                
            }
            else if(SubSegments?.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]))
            {                
                CreateNewBranch();
            }
            else
            {
                _fullyConnected = true;
            }            
        }

        private void AddNewConnection()
        {
            Position4D pos4d = GetNextPositionForSegment();
            Connections.Add(pos4d, int.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]));
            CPM.UpdateConnectionGraph(pos4d);
        }

        private Position4D GetNextPositionForSegment()
        {
            //Need to figure out connection direction and all that stuff
            Position4D pos4d = new Position4D();
            if (CPM.CheckForSelfConnection(pos4d, NeuronID))
            {
            }
            throw new NotImplementedException("Not Yet Implemented!!! He HE HE ");
        }

        private void CreateNewBranch()
        {
            if (!_hasSubSegments)
            {
                _hasSubSegments = true;
                SubSegments = new List<Segment>();
            }
            Position4D baseSegment = GetNextPositionForSegment();
            Segment newSegment = new Segment(NeuronID, ID, SubSegments.Count, baseSegment);
            SubSegments.Add(newSegment);
            
        }

    }
}
