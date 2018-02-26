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
        private NeuronState _state;
        private List<Position4D> basalConnections;
        private Dictionary<int, Segment> _segments;
        private List<Position4D> AxonList;

        public Segment GetSegment(int z)
        {
            Segment seg;
            if(_segments.TryGetValue(z, out seg))
            {
                return seg;
            }
            //log error;
            return null;
        }

        internal void ChangeStateToPredicted()
        {
            if(_state != NeuronState.FIRED)
            {
                _state = NeuronState.PREDICTED;
            }            
        }

        internal List<Position4D> Fire()
        {
            return AxonList;
        }

        internal void Grow(Position4D segmentId)
        {
            Segment s = GetSegment(segmentId.z);
            s.Grow(segmentId);
        }       

        internal NeuronState GetState()
        {
            return _state;
        }
    }
}
