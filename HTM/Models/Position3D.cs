namespace HTM.Models
{
    public class Position3D
    {       
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Position3D() { }

        public Position3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }       

        public  bool Equals(Position3D segId)
        {
            if (this.X.Equals(segId.X) && this.Y.Equals(segId.Y) && this.Z.Equals(segId.Z))
                return true;

            return false;
        }
    }
}

