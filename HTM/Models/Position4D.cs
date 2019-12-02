namespace HTM.Models
{
    public class Position4D
    {
        public uint PosID { get; private set; }
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Z { get; private set; }

        public Position4D() { }

        public Position4D(uint x, uint y, uint z, uint posId)
        {           
            X = x;
            Y = y;
            Z = z;
            PosID = posId;
        }        
    }
}
