using HTM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM.Algorithms
{
    internal class Vertice
    {
        Position3D pos;
        private uint blockRadius;
        Position3D[] square;        

        Vertice(Position3D pos)
        {
            this.pos = pos;
            square = new Position3D[8];            
            string s = ConfigurationManager.AppSettings["BLOCKRADIUS"];
            if (String.IsNullOrEmpty(s))
                throw new InvalidOperationException("BlcokRadius needs to be Configured");
            this.blockRadius = Convert.ToUInt32(s);
        }

        public Position3D PredictNewSynapse()
        {

        }

        private uint PredictX_Coordinate()
        {
            uint numRows = SynapseManager.GetInstance.YSize;
            uint numCols = SynapseManager.GetInstance.XZSize;
            if (pos.X - blockRadius * numRows <= 0) //Crossing into Left Block of X;
            {
                if (pos.BlockID % 10 != 0)
                {
                    
                }
                else
                {

                }

            }
            else if (pos.X + blockRadius * numRows >= numRows * numCols)
            {                
            }
            else
            {

            }



        }


        //will need an interval construct to store the coordinates if the point crosses the block
    }
}
