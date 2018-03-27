namespace HTM.Models
{
    public class Position2D
    {
        public uint X { get; private set; }
        public uint Y { get; private set; }

        public Position2D(uint x, uint y)
        {
            X = x;
            Y = y;
        }
                
    }
}
