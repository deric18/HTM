using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM.Models
{
    public class SegmentID : Position3D
    {        
        
        internal SegmentID():base() { }

        internal SegmentID(Position3D pos) : base(pos) { }

        internal SegmentID(uint x, uint y, uint z):base(x,y,z)
        {            
        }

        internal string GetStringID() => base.StringID;
    }
}
