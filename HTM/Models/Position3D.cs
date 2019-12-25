namespace HTM.Models
{
    //3Dimensional cordinate system to pin point neurons
    public class BlockID
    {               
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Z { get; private set; }

        public BlockID() { }

        public BlockID(uint x, uint y, uint z)
        {            
            X = x;
            Y = y;
            Z = z;            
        }       

        public  bool Equals(BlockID segId) =>         
            this.X.Equals(segId.X) && this.Y.Equals(segId.Y) && this.Z.Equals(segId.Z);                    
    }
}

