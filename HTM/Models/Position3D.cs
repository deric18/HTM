using System;

namespace HTM.Models
{
    //3Dimensional cordinate system to pin point neuronal synapses
    public class Position3D
    {                       
        public uint BlockID { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Z { get; set; }

        public Position3D() { }

        public Position3D(Position3D pos, uint blockID)
        {            
            BlockID = blockID;
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }

        public Position3D(uint x, uint y, uint z)
        {            
            X = x;
            Y = y;
            Z = z;            
        }       

        public  bool Equals(Position3D segId) =>         
            this.X.Equals(segId.X) && this.Y.Equals(segId.Y) && this.Z.Equals(segId.Z);

        internal string GetString()
        {
            return X.ToString() + "-" + Y.ToString() + "-" + Z.ToString() + "-" + BlockID.ToString();
        }
    }
}

