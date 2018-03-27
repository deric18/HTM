namespace HTM.Models
{
    public class Position3D
    {
        private int v1;
        private int v2;
        private int v3;

        public Position3D() { }

        public Position3D(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
    }
}
