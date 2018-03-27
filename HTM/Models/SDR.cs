using System.Collections.Generic;

namespace HTM.Models
{
    public class SDR
    {
        public int Length { get; private set; }
        public int Breadth { get; private set; }
        public bool[][] Contents { get; private set; }

        public int Size()
        {
            return Length * Breadth;
        }

        public SDR()
        {

        }

        public SDR(int length, int breadth)
        {
            SDR toReturn = new SDR();
            toReturn.Length = length;
            toReturn.Breadth = breadth;
            Contents = new bool[length][];
        }

        public List<Position2D> GetActivePositions()
        {
            List<Position2D> toReturn = new List<Position2D>();            

            for (uint i=0; i < Length; i++)
                for(uint j=0; j < Breadth; j++)
                {
                    Position2D p = new Position2D();
                    if (Contents[i][j])
                    {
                        p.X = i;
                        p.Y = j;
                        toReturn.Add(p);
                    }
                }

            return toReturn;
        }

        public bool IsUnionTo(SDR sdr)
        {
            bool flag = true;
            for(int i = 0; i < sdr.Length; i++)
                for(int j = 0; j < sdr.Breadth; j++)
                    if(sdr.Contents[i][j])
                    {
                        if (!Contents[i][j])
                            flag = false;
                    }
            return flag;
        }        
    }
}
