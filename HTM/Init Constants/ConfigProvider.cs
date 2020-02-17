using System.Collections.Generic;

namespace HTM.Models
{
    public class BlockConfigProvider
    {
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Z { get; private set; }        

        //Total Number of Pouints, <Number of Pouints per Line (x), Number of lines(x) , Number of such files (z)> these are all for a single block
        public BlockConfigProvider(uint totalPoints)
        {
            switch(totalPoints)
            {
                case 1000:
                    {
                        X = Y = Z = 10;
                        break;
                    }
                case 10000:
                    {
                        X = 100;
                        Y = 10;
                        Z = 10;
                        break;
                    }
                case 100000:
                    {
                        X = 100;
                        Y = 100;
                        Z = 10;
                        break;
                    }
                case 1000000:
                    {
                        X = 100;
                        Y = 100;
                        Z = 100;
                        break;
                    }
                case 10000000:
                    {
                        X = 1000;
                        Y = 100;
                        Z = 100;
                        break;
                    }                    
            }            
        }
    }
}
