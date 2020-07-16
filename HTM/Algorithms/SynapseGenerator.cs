/*Connection Point Generator (Static Singleton Class)
 * Copyright ©Decision Machines
 * Author: Deric Roshan Pinto
 * Responsibilities:
 * -Takes care of logic for creating new connection points for any connection point in any block 
 * -Handles Randomness of each of the segments own signature
 * API:
 *  List<Position4D> GenerateNewRandomPositions(Position4D lastPosition);
 *  TODO:
 *  Redo the logic of predicting new random positions
 */

using System;
using System.Configuration;
using HTM.Models;

namespace HTM.Algorithms
{
    //NOTE : Need to account for scenario where position predicted is already occupied and in such cases we need to compute the position.
    //       CPM.NumX is actually the count of number of columns in the block and CPM.NumY is the count of number of rows in the block , Line : 43
    /// <summary>
    /// To use this API , Intialise this object to ur segment pos3d and and call the setbounds method and call PredictNJEwRAndomSynapse Method , easy as it looks. thats it!
    /// </summary>
    public sealed class SynapseGenerator
    {
        private uint xzSize;
        private uint ySize;        
        private uint rbr;
        private uint xmin;
        private uint xmax;
        private uint ymin;
        private uint ymax;
        private uint zmin;
        private uint zmax;
        private readonly uint ppd;
        private int seed;

        private static SynapseGenerator instance = null;        

        public SynapseGenerator()
        {
            //set PPD, cubeconstant
            xzSize = CPM.GetInstance.NumX;
            ySize = CPM.GetInstance.NumY;
            rbr = uint.Parse(ConfigurationManager.AppSettings["RBR"]);            
            ppd = xmin = xmax = ymin = ymax = zmin = zmax = 0;            
        }

        public static SynapseGenerator Instance
        {
            get
            {
                if (instance == null)
                    instance = new SynapseGenerator();

                return instance;
            }            
        }

        private void SetBounds(Position3D pos)
        {//set all dimensional min and maxes
            xmin = MoveNPositionsX(false, rbr, pos.X);
            xmax = MoveNPositionsX(true, rbr, pos.X);
            ymin = MoveNPositionsY(false, rbr, pos.Y);
            ymax = MoveNPositionsY(true, rbr, pos.Y);
            zmin = MoveNPositionsZ(false, rbr, pos.Z);
            zmax = MoveNPositionsZ(true, rbr, pos.Z);
        }

        /// <summary>
        /// SetBounds on the original position 
        /// </summary>
        /// <param name="basePosition"></param>
        /// <returns></returns>
        public Position3D PredictNewRandomSynapse(Position3D basePosition)
        {//Use computed bounds to randomly predict a new position inside the random neuro block
            //Need to be redone.
            SetBounds(basePosition);
            Random r = new Random(seed);
            Position3D synapse = new Position3D();
            

            //Check if the generated RNB falls completely inside the block or or edge points fall outside
            //If it falls inside predict 3 new random positions and return the new synapse
            //else if it falls outside block 
            //  -check if its a basis block or not and the part that falls outside the block is not corresponding to the basis block
            //   -if its a basis block that is falling outside then initilise that dimension of the point to zero and compute the remaining 2 dimensions
            //   -else compute the dimensions which are falling outside and initialise the one falling insdie
            //  -If its not a basis block then compute the intervals for the dimensions falling outside the block and also just randomly get the ones that are not failling outside and return the new synapse.


            return synapse;
        }               
        //Need to account for respective block Id changes , need some math to account for this.
        //x only accounts for x 
        private uint MoveNPositionsX(bool leftOrRight, uint n, uint X)
        {            
            uint x = X;
            if (leftOrRight) //left
            {
                //for every reduction in x check if point has crossed to another block                
                while( n > 0 && x > 0)  //check if block is crossed and random block is reached.
                {
                    n--;
                    x--;
                }
            }
            else        //right
            {
                while (n > 0 && x < ppd)  //check if block is crossed and random block is reached.
                {
                    n--;
                    x++;
                }
            }
            return x;
        }

        private uint MoveNPositionsY(bool leftOrRight, uint n, uint Y)
        {            
            uint y = Y;
            if (leftOrRight) //left
            {
                //for every reduction in x check if point has crossed to another block                
                while (n > 0 && y > 0)  //check if block is crossed and random block is reached.
                {
                    n--;
                    y--;
                }
            }
            else        //right
            {
                while (n > 0 && y < ppd)  //check if block is crossed and random block is reached.
                {
                    n--;
                    y++;
                }
            }
            return y;
        }

        private uint MoveNPositionsZ(bool leftorRight, uint n, uint Z)
        {            
            uint z = Z;
            if (leftorRight) //left
            {
                //for every reduction in x check if point has crossed to another block                
                while (n > 0 && z > 0)  //check if block is crossed and random block is reached.
                {
                    n--;
                    z--;
                }
            }
            else        //right
            {
                while (n > 0 && z < ppd)  //check if block is crossed and random block is reached.
                {
                    n--;
                    z++;
                }
            }
            return z;
        }
    }
}
