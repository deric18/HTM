//changed the configurations to make the place blocks much more even and square
namespace HTM.Models
{
    public class BlockConfigProvider
    {
        public uint NumXperBlock { get; private set; }      //Number of Columns per Block
        public uint NumYperBlock { get; private set; }      //Number of Rows per Block
        public uint NumZperBlock { get; private set; }      //Number of Rows & Columns files per block

        //Total Number of Points, <Number of Points per Line (x), Number of lines(x) , Number of such files (z)> these are all for a single block
        public BlockConfigProvider(uint totalPointsPerBlock)
        {
            switch(totalPointsPerBlock)
            {
                case 1000:
                    {
                        NumXperBlock = NumYperBlock = NumZperBlock = 10;
                        break;
                    }
                case 10000:         //9261
                    {
                        NumXperBlock = 21;
                        NumYperBlock = 21;
                        NumZperBlock = 21;
                        break;
                    }
                default:
                case 100000:        //97336     //dEFAULT
                    {
                        NumXperBlock = 46;
                        NumYperBlock = 46;
                        NumZperBlock = 46;
                        break;
                    }
                case 1000000:       
                    {
                        NumXperBlock = 100;
                        NumYperBlock = 100;
                        NumZperBlock = 100;
                        break;
                    }
                case 10000000:      //9938375
                    {
                        NumXperBlock = 215;
                        NumYperBlock = 215;
                        NumZperBlock = 215;
                        break;
                    }                    
            }            
        }
    }
}
