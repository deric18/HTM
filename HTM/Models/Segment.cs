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
        public Position4D CoreSegmentId { get; private set; }
        public Position3D NeuronID;
        private Dictionary<Position4D, int> _lastTimeStampFiringSynapses;
        private Position4D _candidateNewBranchPosition;
        public Position4D SegmentID { get; private set; }        
        private uint _sumVoltage;
        private Dictionary<Position4D, int> _positionConnections;
        private Dictionary<Position4D, int> _segmentConnections;                    //Gets updated every time after the growth cycle.
        private bool _hasSubSegments;
        private List<Segment> SubSegments;
        private uint _spikeCounter;
        private bool IsCore;
        private bool _fullyConnected;

        public Segment(Position3D neuronID, Position4D segmentID)
        {
            SegmentID = segmentID;
            NeuronID = neuronID;
            _sumVoltage = 0;
            IsCore = false;
            _positionConnections = new Dictionary<Position4D, int>();
            _segmentConnections = new Dictionary<Position4D, int>();
            _hasSubSegments = false;
            _spikeCounter = 0;
            _lastTimeStampFiringSynapses = new Dictionary<Position4D, int>();            
        }
        
        /// <summary>
        /// Processes Individual 4D Position
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="pos4d"></param>
        /// <returns></returns>
        public bool Process(uint voltage, Position4D pos4d)
        {
            int spikeCounter = 0;
            _lastTimeStampFiringSynapses.TryGetValue(pos4d, out spikeCounter);
            if(spikeCounter != 0)
            {
                _lastTimeStampFiringSynapses.Remove(pos4d);
                _lastTimeStampFiringSynapses.Add(pos4d, ++spikeCounter);
            }
            else
            {
                Console.WriteLine("This should never happen : Position4d :" + PrintPosition(pos4d) + " is not connected to the Segment : " + PrintPosition(SegmentID));
                throw new Exception();
            }
            _sumVoltage += voltage;            
            if(_sumVoltage > int.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]))
            {
                //NMDA SPIKE (alert CPM that this neuron will fire and CPM will in-turn alert the neuron about this semgnets fire for future update of its growth factor for the next growth cycle)
                //Increment counter                           
                return true;
            }

            return false;
        }

        private void UpdateLocalConnectionTable()
        {
            //Access _positionConnections and update strengths and flush _connectionStrength
            if(_lastTimeStampFiringSynapses.Count == 0)
            {
                //log info nothing to fire
                return;
            }

            foreach(var connection in _lastTimeStampFiringSynapses)
            {

            }
            throw new NotImplementedException();
        }
        /'
        
        private void InternalGrowth()
        {
            ///Should decide on its own to whether to add a new connection or not ?
            //Make sure you are not connecting to an axon of your own neuron if its a new position
            //If position is new position and already branched and maxed out on MAXBRANCH , dont branch , set flag fullyconnected to true and add new position as a new connection
            //If not branched and position is noand check if segment has max positions , add this position to a possibility list for future connection when the neuron losses and non used connection else if not max position then add new position,
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.
            Position4D newPosition = GetNextPosition();
            CPM.CheckForSelfConnection(newPosition, NeuronID);
            if (_segmentConnections.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"]))
            {
                AddNewLocalConnection(newPosition);
            }
            else if(SubSegments?.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]))
            {                
                CreateNewBranch();
            }
            else
            {
                _fullyConnected = true;
                //log Information with details
            }            
        }

        private void AddNewLocalConnection(Position4D newPosition)
        {
            //Maintain a Random number object local 
            throw new NotImplementedException();
        }

        private Position4D GetNextPosition()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// -Method gets called at every growth cycle
        /// -UpdateLocalConnectionTable : Update all local : Connection Tables
        /// -GrowSubSegments            : Updates all subsegments based on merit
        /// -InternalGrowth             : Decides if a new connection could be added , adds it if it can otherwise grows an existing nicely firing connection.
        /// </summary>
        /// <param name="synapse"></param>
        public void Grow()
        {            
            UpdateLocalConnectionTable();
            InternalGrowth();
            GrowSubSegments();            
            return;
        }        

        private void GrowSubSegments()
        {
            if (_hasSubSegments)

            {                
                foreach(var segment in SubSegments)
                {
                    segment.Grow();
                }
            }
            throw new NotImplementedException();
        }

        private void AddConnection()
        {
            Position4D pos4d = CPM.GetNextPositionForSegment(CoreSegmentId);
            _segmentConnections.Add(pos4d, int.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]));
            CPM.UpdateConnectionGraph(pos4d);
        }        

        private void CreateNewBranch()
        {
            if (!_hasSubSegments)
            {
                _hasSubSegments = true;
                SubSegments = new List<Segment>();
            }
            Position4D baseSegmentPosition = CPM.GetNextPositionForSegment(CoreSegmentId);
            Segment newSegment = new Segment(NeuronID, baseSegmentPosition);
            SubSegments.Add(newSegment);
            
        }        
        

        //Dont Use : Bad Tech
        private int GetMaxFiringSegmentCount()
        {
            //Figure out the highest firing subsegment fro _firingSynapses and send the highest firer measure;
            Position4D maxfirePosition;
            int maxCounter = 0;

            foreach(var pos in _lastTimeStampFiringSynapses)
            {
                if(pos.Value > maxCounter)
                {
                    maxfirePosition = pos.Key;
                    maxCounter = pos.Value;
                }
            }

            return maxCounter;
        }

        private int GetMaxPositionCount()
        {
            //Figure out the highest firing subsegment fro _firingSynapses and send the highest firer measure;
            Position4D maxfirePosition;
            int maxCounter = 0;

            foreach(var pos in _lastTimeStampFiringSynapses)
            {
                if (pos.Value > maxCounter)
                {
                    maxfirePosition = pos.Key;
                    maxCounter = pos.Value;
                }
            }

            return maxCounter;
        }


        private string PrintPosition(Position4D pos4d)
        {
            return pos4d.W.ToString() + " " + pos4d.X.ToString() + " " + pos4d.Y.ToString() + " " + pos4d.Z.ToString();
        }
    }
}
