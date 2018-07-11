using System.Collections.Generic;

namespace HTM.Models
{
    public class SDR 
    {
        public uint Size { get; private set; }
        public uint[] Locations { get; private set; }
        public int  N { get; private set; }

        public SDR()
        {

        }

        public SDR(CPM inst, string s)
        {
            SDR toReturn = new SDR();
            toReturn.Size = (uint)s.Length;
            
            int j = 0;
            for(int i=0;i< s.Length;i++)            
                if (s[i].Equals("1"))
                    toReturn.Locations[j++] = (uint)i;                                 
        }

        public List<Position2D> GetActivePositions()
        {
            List<Position2D> toReturn = new List<Position2D>();
            int j = 0;
            for (uint i = 0; i < Size; i++)
            {                
                j =                 
                Position2D pos2d = new Position2D(i,j)
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
