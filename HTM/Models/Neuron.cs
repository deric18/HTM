using System.Collections.Generic;

namespace HTM.Models
{
    /// <summary>
    /// 1.Account for both excitatory and inhibitory neurons
    /// </summary>
    public class Neuron
    {
        private Dictionary<int, Segment> _segments;

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
    }
}
