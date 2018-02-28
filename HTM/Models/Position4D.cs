
namespace HTM.Models
{
    public class Position4D
    {
        public uint W { get; private set; }
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Z { get; private set; }

        public Position4D(uint w, uint x, uint y, uint z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }
    }
}
