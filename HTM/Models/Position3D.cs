namespace HTM.Models
{
    //3Dimensional cordinate system to pin point neurons
    public class Position3D
    {               
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Z { get; private set; }

        public Position3D() { }

        public Position3D(uint x, uint y, uint z)
        {            
            X = x;
            Y = y;
            Z = z;            
        }       

        public  bool Equals(Position3D segId) =>         
            this.X.Equals(segId.X) && this.Y.Equals(segId.Y) && this.Z.Equals(segId.Z);                    
    }
}

