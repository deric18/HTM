using System.Collections.Generic;

namespace HTM.Models
{
    public class SDR
    {
        public int Length { get; private set; }
        public int Breadth { get; private set; }
        public bool[][] _contents {get; private set; }
        public List<Position2D> GetActivePositions { get; private set; }

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
            _contents = new bool[length][];
            GetActivePositions = new List<Position2D>();
        }       

        public SDR(string s,int length, int breadth)
        {
            Length = length;
            Breadth = breadth;
            _contents = new bool[length][];
            GetActivePositions = new List<Position2D>();
            for (int i = 0; i < length; i++)
                for (int j = 0; j < breadth; j++)
                {
                    _contents[i][j] = s[i + j].Equals(0) ? false : true; GetActivePositions.Add(new Position2D((uint)i, (uint)j));
                }
        }

        public bool IsUnionTo(SDR sdr1, SDR sdr2)
        {
            if (sdr1.Length != sdr2.Length || sdr1.Breadth != sdr2.Breadth)
            {
                throw new System.Exception("Invalid SDR- Dimensions");
            }                        
            
            foreach(var pos in sdr1.GetActivePositions)
            {
                if(!sdr2._contents[pos.X][pos.Y])
                {
                    return false;
                }
            }

            return true;
        }        
    }
}
