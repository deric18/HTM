﻿using System.Configuration;
using System.Collections.Generic;
using System;

namespace HTM.Models
{
    /// <summary>
    /// Process, Grow
    /// </summary>
    public class Segment
    {                
        private Dictionary<Position3D, int> _segmentConnections;    //strength
        private List<Position3D> __lastTimeStampFiringConnections;
        public string SegmentID { get; private set; }        
        private uint _sumVoltage;
        private Dictionary<int, Segment> _subSegment;           //subsegment index   
        private bool _hasSubSegments;
        private List<Segment> SubSegments;                
        private bool _fullyConnected;

        public Segment(string segmentID)
        {
            SegmentID = segmentID;            
            _sumVoltage = 0;                  
            _hasSubSegments = false;
            _segmentConnections = new Dictionary<Position3D, int>();
            __lastTimeStampFiringConnections = new List<Position3D>();
        }

        internal Segment GetSegment(int v)
        {
            Segment seg;
            if(_subSegment.TryGetValue(v,out seg))
            {
                return seg;
            }

            throw new InvalidOperationException("seg ID : " + v.ToString() + " is not present");
        }

        /// <summary>
        /// Predict if the segment will fire or not based incoming voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="firingNeuronId"></param>
        /// <returns></returns>
        public bool Predict(uint voltage, Position3D firingNeuronId)
        {            
            _sumVoltage += voltage;            
            if(_sumVoltage > int.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]))
            {
                //NMDA Spike
                //Strengthen firing Neuron Connection.
                int connectionStrength;
                if(_segmentConnections.TryGetValue(firingNeuronId, out connectionStrength))
                {
                    if (connectionStrength > int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTION_STRENGTH"]))
                        return true;
                    _segmentConnections[firingNeuronId]++;
                }
                return true;
            }

            return false;
        }

        internal void FlushVoltage()
        {
            _sumVoltage = 0;
            __lastTimeStampFiringConnections = new List<Position3D>();
        }        
        

        /// <summary>
        /// Boxed Growth : Direction and boxed random connection
        /// 1.A Segment should have direction it grows and by default will have limits set to how far away the segment can predict its new connection.
        /// </summary>
        private void AddNewLocalConnection()
        {
            ///Should decide on its own to whether to add a new connection or not ?
            //Make sure you are not connecting to an axon of your own neuron if its a new position
            /*Decide how to add new connection : Check if there is any sub segments performing really well , if yes send grow signal to it
             * else check for any local connections to the segment which is performing really well 
             * else grow a new connection locally randomly.
            */
            //If not branched and position is noand check if segment has max positions , add this position to a possibility list for future connection when the neuron losses and non used connection else if not max position then add new position,
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.  
            Position3D newPosition = new Position3D();

            if (_segmentConnections.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"]))
            {
                AddConnection(newPosition);
            }
            else if(SubSegments?.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]))
            {                
                CreateSubSegment();
            }
            else
            {
                _fullyConnected = true;
                //log Information with details , Segment has reached a peak connection pathway , this is essentially a crucial segment for the whole region.
            }            
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
            AddNewLocalConnection();
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

        private void AddConnection(Position3D newPosition)
        {            
            _segmentConnections.Add(newPosition, int.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]));
            CPM.UpdateConnectionGraph(newPosition);
        }        

        private void CreateSubSegment()
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

        private string PrintPosition(Position3D pos3d)
        {
            return " X: " + pos3d.X.ToString() + " Y:" + pos3d.Y.ToString() + " Z:" + pos3d.Z.ToString();
        }
    }
}
