namespace HTM.Models
{
    public class Position4D
    {
        public uint PosID { get; private set; }
        public BlockID blockID { get; private set; }

        public Position4D() { }

        public Position4D(uint x, uint y, uint z, uint posId)
        {
            blockID = new BlockID(x, y, z);
            PosID = posId;
        }        zs
    }
}
