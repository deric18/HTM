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
    //Need methods for moving a specific number of positions left right down up and anywhere in between.
    public sealed class SynapseGenerator
    {
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

        private void SetBounds(Synapse pos)
        {//set all dimensional min and maxes
            xmin = MoveNPositionsX(false, rbr, pos.X);
            xmax = MoveNPositionsX(true, rbr, pos.X);
            ymin = MoveNPositionsY(false, rbr, pos.Y);
            ymax = MoveNPositionsY(true, rbr, pos.Y);
            zmin = MoveNPositionsZ(false, rbr, pos.Z);
            zmax = MoveNPositionsZ(true, rbr, pos.Z);
        }

        private Synapse PredictNewRandomSynapse()
        {//Use computed bounds to randomly predict a new position inside the random neuro block
            //Need to be redone.

            Random r = new Random(seed);
            Synapse synapse = new Synapse();
            int minimum = Convert.ToInt32(xmin);
            int maximum = Convert.ToInt32(xmax);

            synapse.X = (uint)r.Next(minimum, maximum);

            minimum = Convert.ToInt32(ymin);
            maximum = Convert.ToInt32(ymax);

            synapse.Y = (uint)r.Next(minimum, maximum);

            minimum = Convert.ToInt32(zmin);
            maximum = Convert.ToInt32(zmax);

            synapse.Z = (uint)r.Next(minimum, maximum);

            return synapse;
        }               

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
