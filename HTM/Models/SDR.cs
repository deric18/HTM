using HTM.Enums;
using System.Collections.Generic;
using System.Linq;

namespace HTM.Models
{
    public class SDR
    {
        public uint Length { get; private set; }
        public uint Breadth { get; private set; }
        public List<Position2D> ActiveBits { get; private set; }
        public InputPatternType IType { get; set; }        

        public uint Size() => Length * Breadth;

        public SDR(uint length, uint breadth)
        {
            this.Length = length;
            this.Breadth = breadth;
            this.ActiveBits = new List<Position2D>();
        }

        public SDR(uint length, uint breadth, List<Position2D> activeBits)
        {           
            this.Length = length;
            this.Breadth = breadth;
            this.ActiveBits = activeBits;
        }       

        public SDR(string s, uint length, uint breadth)
        {
            Length = length;
            Breadth = breadth;
            ActiveBits = new List<Position2D>();            
            for (int i = 0; i < length; i++)
                for (int j = 0; j < breadth; j++)
                {
                    
                }
        }

        public bool IsUnionTo(SDR uniounTo)
        {
            if (Length != uniounTo.Length || uniounTo.Breadth != uniounTo.Breadth || ActiveBits.Count > uniounTo.ActiveBits.Count)
                return false;

            ActiveBits.OrderByDescending(x => x.X);
            uniounTo.ActiveBits.OrderByDescending(y => y.X);

            int counter = 0;
            foreach (var pos in ActiveBits)
            {
                Position2D uPos = uniounTo.ActiveBits[counter];
                if ((pos.X == uPos.X && pos.Y == uPos.Y) || uPos.X < pos.X)
                {
                    counter++;
                    continue;
                }
                else if(pos.X < uPos.X)
                {
                    return false;
                }
            }

            return true;
        }        
    }
}
