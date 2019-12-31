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
        private uint leftBoundX;
        private uint rightBoundX;
        private uint leftBoundY;
        private uint rightBoundY;
        private uint leftBoundZ;
        private uint rightBoundZ;
        private uint dmten;

        private static SynapseGenerator instance = null;        

        public SynapseGenerator()
        {
            cubeConstant = uint.Parse(ConfigurationManager.AppSettings["CUBECONSTANT"]);
            leftBoundX = rightBoundX = leftBoundY = rightBoundY = leftBoundZ = rightBoundZ = 0;
            ppux = ppuy = ppuz = 0;
            dmten = 0;
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
            if(leftOrRight) //left
            {
                //for every reduction in x check if point has crossed to another block
                Synapse toRet = new Synapse(position);
                uint x = position.X;
                while(n>0 &&  )  //check if block is crossed and random block is reached.
                {

                }
            }
            else        //right
            {

            }
        }

        private Synapse MoveNPositionsY(bool leftOrRight, uint n, Synapse position)
        {

        }

        private void MoveNPositionsZ(bool leftorRight, uint n)
        {

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
