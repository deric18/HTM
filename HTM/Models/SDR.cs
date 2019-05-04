using HTM.Enums;
using System.Collections.Generic;

namespace HTM.Models
{
    public class SDR
    {
        public uint Length { get; private set; }
        public uint Breadth { get; private set; }
        public List<Position2D> ActiveBits { get; private set; }
        public InputPatternType iType { get; private set; }        

        public uint Size() => Length * Breadth;        

        private SDR()
        {

        }

        public SDR(uint length, uint breadth)
        {
            SDR toReturn = new SDR();
            toReturn.Length = length;
            toReturn.Breadth = breadth;
            ActiveBits = new List<Position2D>();            
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

        public bool IsUnionTo(SDR sdr1, SDR sdr2)
        {
            if (sdr1.Length != sdr2.Length || sdr1.Breadth != sdr2.Breadth)           
                return false;          
            
            foreach(var pos in ActiveBits)
            {
                if(!sdr2.ActiveBits[pos.X][pos.Y])
                {
                    return false;
                }
            }

            return true;
        }        
    }
}
