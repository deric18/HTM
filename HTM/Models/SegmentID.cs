/*
 * Segment ID Syntax : <<Neuron Position3D>--<Segment Number>--<Segment Position3D>> 
 */

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
