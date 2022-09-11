using HTM.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HTM.Models
{
    public class SDR : IEquatable<SDR>
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

        public float CompareFloat(SDR firingPattern)
        {
            // first pattern is always the firing pattern and second pattern is the predicted pattern

            float matchFloat = 0;
            float unmatchFloat = 0;
            uint totalBits = (uint) (this.ActiveBits.Count + firingPattern.ActiveBits.Count);
            uint MatchingBits = 0;
            uint UnmatchingBits = 0;
            bool flag = false;

            foreach(var item in this.ActiveBits)
            {

                foreach( var item1 in firingPattern.ActiveBits)
                {
                    
                    if(item.Equals(item1)
                    {
                        MatchingBits++;
                        flag = true;
                        break;
                    }                    
                }

                if (!flag)                
                    UnmatchingBits++;

                flag = false;

            }


            matchFloat = (MatchingBits / totalBits) * 100;
            unmatchFloat = (unmatchFloat / totalBits) * 100;

            return matchFloat;
        }

        public bool Equals(SDR y)
        {
            if (this.Length == y.Length && this.Breadth == y.Breadth && this.ActiveBits.Count == y.ActiveBits.Count)
            {
                for (int i = 0; i < this.ActiveBits.Count; i++)
                {
                    if (this.ActiveBits[i] != y.ActiveBits[i])
                        return false;
                }
            }

            return true;
        }
    }
}
