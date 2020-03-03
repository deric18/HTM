using System;

namespace HTM.Models
{
    //3Dimensional cordinate system to pin point neuronal synapses
    public class Position3D
    {                       
        public uint BID { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Z { get; set; }                

        public Position3D() { }

        internal Position3D(Position3D pos) { }
        public Position3D(uint x, uint y, uint z)
        {            
            X = x;
            Y = y;
            Z = z;
            this.BID = ComputeBID(x,y,z);
        }       

        private uint ComputeBID(uint x, uint y, uint z) =>              //Computes BlockID
        
            (z * CPM.GetInstance.NumX * CPM.GetInstance.NumY + y * CPM.GetInstance.NumX + x);        

        public  bool Equals(Position3D segId) =>         
            this.X.Equals(segId.X) && this.Y.Equals(segId.Y) && this.Z.Equals(segId.Z);

        internal string StringID =>        
            X.ToString() + "-" + Y.ToString() + "-" + Z.ToString() + "-" + BID.ToString();        
    }
}

