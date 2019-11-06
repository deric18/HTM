using System;
using HTM.Models;

namespace HTM.Algorithms
{
    public static class Connector
    {
        public static Vector GetNewPosition(Segment s, Vector v)
        {
            return new Vector();
        }

        public static Tuple<Vector, Vector> BranchSegment(Segment s, Vector v)
        {            
            return new Tuple<Vector, Vector>(new Vector(), new Vector());
        }
    }
}
