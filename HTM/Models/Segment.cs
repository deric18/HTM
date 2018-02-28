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
        public Position2D NeuronID;
        private List<Position4D> _firingSynapses;
        public Position4D SegmentID { get; private set; }        
        private uint _sumVoltage;
        private Dictionary<Position4D, int> Connections;                
        private bool _hasSubSegments;
        private List<Segment> SubSegments;
        private uint _spikeCounter;
        private bool _fullyConnected;

        public Segment(Position2D neuronID, Position4D segmentID)
        {
            SegmentID = segmentID;
            NeuronID = neuronID;
            _sumVoltage = 0;
            Connections = new Dictionary<Position4D, int>();
            _hasSubSegments = false;
            _spikeCounter = 0;
            _firingSynapses = new List<Position4D>();            
        }
        
        public bool Process(uint voltage, Position4D pos4d)
        {
            _sumVoltage += voltage;            
            if(_sumVoltage > int.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]))
            {
                //NMDA SPIKE
                //Update pos3d strength'
                StrengthenConnections(_firingSynapses);                
                return true;
            }

            return false;
        }

        private void StrengthenConnections(List<Position4D> firingSynapses)
        {
            throw new NotImplementedException();
        }

        private void internalGrow(Position4D lastFiredSynapse)
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

        public void Grow(Position4D synapse)
        {
            int s = 99999;
            Connections.TryGetValue(synapse, out s);
            if (s != 99999)
            {
                internalGrow(synapse);
            }
            else
            {
                if (_hasSubSegments)
                {
                    foreach (var segment in SubSegments)
                    {
                        segment.Grow(synapse);                        
                    }
                }
            }

            return;
        }

        private void AddNewConnection()
        {
            Position4D pos4d = CPM.GetNextPositionForSegment();
            Connections.Add(pos4d, int.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]));
            CPM.UpdateConnectionGraph(pos4d);
        }        

        private void CreateNewBranch()
        {
            if (!_hasSubSegments)
            {
                _hasSubSegments = true;
                SubSegments = new List<Segment>();
            }
            Position4D baseSegmentPosition = CPM.GetNextPositionForSegment();
            Segment newSegment = new Segment(NeuronID, baseSegmentPosition);
            SubSegments.Add(newSegment);
            
        }        

        /// <summary>
        /// Return "1" if the fire is coming from segment else Returns "0" if the fire is coming from one of the connected synapses.
        /// </summary>
        /// <param name="sourcePosition"></param>
        /// <returns>bool</returns>
        private bool SegmentFire(Position4D source)
        {
           foreach(Segment s in SubSegments)
            {
                if (s.SegmentID.Equals(source))
                    return true;
            }

            return false;
        }
    }
}
