using HTM.Models;
using System;
using System.Configuration;

namespace HTM.Algorithms
{
    internal class Vertice
    {
        Position3D pos;
        private uint blockRadius;
        private uint numCols;
        private uint numRows;
        Vertice(Position3D pos)
        {
            this.pos = pos;
            this.numCols = CPM.GetInstance.NumCols;
            this.numRows = CPM.GetInstance.NumRows;
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
            uint numRows = CPM.GetInstance.NumCols;
            uint numCols = CPM.GetInstance.NumRows;
            if (pos.X - blockRadius <= 0) //Crossing into Left Block of X;
            {
                if (pos.BlockID % 10 != 0)  //X Basis Block
                {
                    //check for x transform or xz transform
                    return IntervalHelper.PredictNewRandomSynapseWithoutInterval(0, pos.X);
                }
                else       //Non  Basis Block
                {
                    //Need to redo the logic for coordinates                    
                    uint i1 = numCols - AbsoluteSub(blockRadius, pos.X);
                    return IntervalHelper.PredictNewRandomSynapse(i1, numCols, 0, pos.X);
                }

            }
            else if (pos.X + blockRadius * numRows >= numRows * numCols)
            {                 

            }
            else
            {

            }



        }

        private uint AbsoluteSub(uint u1, uint u2) => (u1 > u2) ? (u1 - u2) : (u2 - u1);


        //will need an interval construct to store the coordinates if the point crosses the block
    }
}
  