using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM.Models
{
    public class SegmentID : Position3D
    {
        public Guid Guid { get; set; }        

        public SegmentID():base() { }

        public SegmentID(uint x, uint y, uint z, uint segmentid):base(x,y,z,segmentid)
        {
            Guid = new Guid();            
        }       
    }
}
