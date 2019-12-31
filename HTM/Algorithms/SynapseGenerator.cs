/*Connection Point Generator *Static Singleton Class
 * Copyright © Deric Roshan Pinto
 * Author: Deric Roshan Pinto
 * Responsibilities:
 * -Takes care of logic for creating new connection points for any connection point in any block 
 * -Handles Randomness of each of the segments own signature
 * API:
 *  List<Position4D> GenerateNewRandomPositions(Position4D lastPosition);
 */

using System;
using System.Configuration;
using HTM.Models;

namespace HTM.Algorithms
{
    //Need methods for moving a specific number of positions left right down up and anywhere in between.
    public sealed class SynapseGenerator
    {
        private uint cubeConstant;
        private uint ppux;
        private uint ppuy;
        private uint ppuz;
        private uint ppd;                

        private static SynapseGenerator instance = null;        

        public SynapseGenerator()
        {   
            //set PPD, cubeconstant
            cubeConstant = uint.Parse(ConfigurationManager.AppSettings["CUBECONSTANT"]);            
            ppd = ppux = ppuy = ppuz = 0;            
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

        private void GetBounds(Synapse pos)
        {
            if (pos is null)
            {
                throw new ArgumentNullException(nameof(pos));
            }
            //set appropriate positions to corner points where position is valid.              
            //need to account for boundary conditions
        }

        private void SetFlags(Synapse position)
        {

        }

        private void SetBounds(Synapse pos)
        {

        }

        private Synapse MoveNPositionsX(bool leftOrRight, uint n, Synapse position)
        {
            Synapse toRet = new Synapse(position);
            uint x = position.X;
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
            position.X = x;
            return position;
        }

        private Synapse MoveNPositionsY(bool leftOrRight, uint n, Synapse position)
        {
            Synapse toRet = new Synapse(position);
            uint y = position.Y;
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
            position.Y = y;
            return position;
        }

        private Synapse MoveNPositionsZ(bool leftorRight, uint n, Synapse position)
        {
            Synapse toRet = new Synapse(position);
            uint z = position.Z;
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
            position.Z = z;
            return position;
        }

        public static Synapse GetBoxedRandomPosition(Synapse pos)
        {
            if(ValidateVerticalBOunds(pos))
            {

            }
            if(ValidateHorizontalBOunds(pos))
            {

            }
        }


        private bool ValidateVerticalBounds(Synapse pos)
        {//make sure position is within the RANDOM BOUND BLOCK
            

        }

        private bool ValidateHorizontalBounds(Synapse pos)
        {
            return 
        }
    }
}
