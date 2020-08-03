using System.Collections.Generic;

namespace HTM.Models
{
    public class BlockConfigProvider
    {
        public uint NumXperBlock { get; private set; }
        public uint NumYperBlock { get; private set; }
        public uint NumZperBlock { get; private set; }        

        //Total Number of Pouints, <Number of Points per Line (x), Number of lines(x) , Number of such files (z)> these are all for a single block
        public BlockConfigProvider(uint totalPointsPerBlock)
        {
            switch(totalPointsPerBlock)
            {
                case 1000:
                    {
                        NumXperBlock = NumYperBlock = NumZperBlock = 10;
                        break;
                    }
                case 10000:
                    {
                        NumXperBlock = 100;
                        NumYperBlock = 10;
                        NumZperBlock = 10;
                        break;
                    }
                case 100000:
                    {
                        NumXperBlock = 100;
                        NumYperBlock = 100;
                        NumZperBlock = 10;
                        break;
                    }
                case 1000000:
                    {
                        NumXperBlock = 100;
                        NumYperBlock = 100;
                        NumZperBlock = 100;
                        break;
                    }
                case 10000000:
                    {
                        NumXperBlock = 1000;
                        NumYperBlock = 100;
                        NumZperBlock = 100;
                        break;
                    }                    
            }            
        }
    }
}
