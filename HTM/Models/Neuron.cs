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
        public Position3D NeuronID { get; private set; }
        public NeuronState State { get; private set; }
        public Dictionary<int, Segment> Segments { get; private set; }
        private List<Position4D> _firedSegments;
        private Dictionary<Position4D, int> Weights { get; set; }
        private List<Position4D> AxonList;

        public Segment GetSegment(uint z)
        {
            Segment seg;
            if(Segments.TryGetValue((int)z, out seg))
            {
                return seg;
            }
            //log error;
            return null;
        }        

        internal List<Position4D> Fire()
        {
            return AxonList;
        }
        
        internal void Grow(Position4D pos4d)
        {

        }

        internal void Grow()
        {        
            foreach(var kvp in Segments)
            {
                kvp.Value.Grow();
            }
        }               

        internal void ChangeStateToPredicted()
        {
            if (State != NeuronState.FIRED)
            {
                State = NeuronState.PREDICTED;
            }
        }

        internal void UpdateLocal(Position4D segmentID)
        {
            if(!_firedSegments.Contains(segmentID))
            {
                _firedSegments.Add(segmentID);
            }
        }
    }
}
