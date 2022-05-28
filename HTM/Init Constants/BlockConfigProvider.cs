//changed the configurations to make the place blocks much more even and square
namespace HTM.Models
{
    public class BlockConfigProvider
    {
        public uint NumXperBlock { get; private set; }      //Number of Columns per Block
        public uint NumYperBlock { get; private set; }      //Number of Rows per Block
        public Position3D BlockCenter { get; private set; }

        //Total Number of Points, <Number of Points per Line (x), Number of lines(x) , Number of such files (z)> these are all for a single block
        public BlockConfigProvider(uint totalPointsPerBlock)
        {
            switch(totalPointsPerBlock)
            {
                case 1000:
                    {
                        NumXperBlock = NumYperBlock = NumZperBlock = 10;
                        BlockCenter = new Position3D(10 / 2, 10 / 2, 10 / 2);
                        break;
                    }
                case 10000:         //9261
                    {
                        NumXperBlock = 21;
                        NumYperBlock = 21;
                        NumZperBlock = 21;
                        BlockCenter = new Position3D(21 / 2, 21 / 2, 21 / 2);
                        break;
                    }
                default:
                case 100000:        //97336     //DEFAULT
                    {
                        NumXperBlock = 46;
                        NumYperBlock = 46;
                        NumZperBlock = 46;
                        BlockCenter = new Position3D(46 / 2, 46 / 2, 46 / 2);
                        break;
                    }
                case 1000000:       
                    {
                        NumXperBlock = 100;
                        NumYperBlock = 100;
                        NumZperBlock = 100;
                        BlockCenter = new Position3D(100 / 2, 100 / 2, 100 / 2);
                        break;
                    }
                case 10000000:      //9938375
                    {
                        NumXperBlock = 215;
                        NumYperBlock = 215;
                        NumZperBlock = 215;
                        BlockCenter = new Position3D(215 / 2, 215 / 2, 215 / 2);
                        break;
                    }                    
            }            
        }
    }
}
