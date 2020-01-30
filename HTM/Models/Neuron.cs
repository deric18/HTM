using HTM.Enums;
using System.Collections.Generic;
using System;

namespace HTM.Models
{
    /// <summary>
    /// 1.Account for both excitatory and inhibitory neurons
    /// </summary>
    public class Neuron
    {
        private uint _voltage;
        private uint _totalSegments;
        public Position3D NeuronID { get; private set; }
        public NeuronState State { get; private set; }
        public Dictionary<uint, Segment> proximalSegments { get; private set; }      
        private List<Segment> _predictedSegments;                
        private Dictionary<Position3D, SegmentID> axonEndPoints; 
        private const uint NEURONAL_FIRE_VOLTAGE = 10;

        public Neuron(Position3D pos)
        {
            _voltage = 0;
            _totalSegments = 0;
            NeuronID = pos;
            State = NeuronState.RESTING;
            proximalSegments = new Dictionary<uint, Segment>();
            _predictedSegments = new List<Segment>();
            axonEndPoints = new Dictionary<Position3D, SegmentID>();
        }

        internal Segment GetSegment(SegmentID segID)
        {
            Segment seg;
            if (proximalSegments.TryGetValue(segID.ID, out seg))
                return seg;

            throw new InvalidOperationException("Invalid Segment ID Access");
        }        

        internal void Process(Position3D position, SegmentID segmentID, uint potential)
        {

        }

        internal void Fire()
        {
            //Supply firing voltage to all the connected synapses.
            //Always use Process method fro mthe neuron as the neuron needs to to strength updates on the segment.
            foreach(var kvp in axonEndPoints )
            {
                CPM.GetInstance.GetNeuronFromPositionID(kvp.Key).Process(kvp.Value, kvp.Key, InputPatternType.INTERNAL);
            }            
        }               

        internal void AddNewConnection(Position3D pos, SegmentID segmentID)
        {
            SegmentID segid;
            if(!axonEndPoints.TryGetValue(pos, out segid))
            {
                axonEndPoints.Add(pos, segmentID);
            }
        }

        //
        internal void Process(SegmentID segID, Position3D SynapseId, InputPatternType iType)
        {
            Segment s = GetSegment(segID);
            if(s.Process(NEURONAL_FIRE_VOLTAGE, SynapseId, iType))            
                _predictedSegments.Add(s);                                                                        
        }

        internal void Predict()
        {

        }           

        internal void Grow()
        {

        }
    }
}
