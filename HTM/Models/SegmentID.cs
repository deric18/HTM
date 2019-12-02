using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM.Models
{
    public class SegmentID
    {
        public uint ID { get; private set; }
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Z { get; private set; }

        public SegmentID() { }

        public SegmentID(uint x, uint y, uint z, uint segmentid)
        {
            X = x;
            Y = y;
            Z = z;
            ID = segmentid;
        }       
    }
}
