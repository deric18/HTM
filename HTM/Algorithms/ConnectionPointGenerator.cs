using System;
using System.Configuration;
using HTM.Models;

namespace HTM.Algorithms
{
    //Need methods for moving a specific number of positions left right down up and anywhere in between.
    public sealed class ConnectionPointGenerator
    {
        private uint cubeConstant;
        private uint ppl;
        private uint minX;
        private uint maxX;
        private uint minY;
        private uint maxY;
        private uint minZ;
        private uint maxZ;
        private uint dmten;

        private static ConnectionPointGenerator instance = null;
        /*API:
         * List<Position4D> GenerateNewRandomPositions(Position4D lastPosition);
         *  
        */

        public ConnectionPointGenerator()
        {
            cubeConstant = uint.Parse(ConfigurationManager.AppSettings["CUBECONSTANT"]);
            minX = maxX = minY = maxY = minZ = maxZ = 0;
            ppl = (uint.Parse(ConfigurationManager.AppSettings["PPB"])) / 10;
            dmten = 0;
        }

        public static ConnectionPointGenerator Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConnectionPointGenerator();

                return instance;
            }            
        }

        private void GetBounds(Position4D pos)
        { 
            //set appropriate positions to corner points where position is valid. jn
             //need to apply the princliples of cube geometry
             //need to account for boundary conditions
        }

        private void SetBounds(Position4D pos)
        {
            dmten = (pos.PosID / 10) * 10;
            if (Math.Abs(pos.PosID - cubeConstant) < dmten) 
            {//Needs to extend to lower X neuro block

            }

            if(Math.Abs(pos.PosID)  )
            {

            }
        }


        public static Position4D GetBoxedRandomPosition(Position4D pos)
        {
            if(ValidateVerticalBOunds(pos))
            {

            }
            if(ValidateHorizontalBOunds(pos))
            {

            }
        }


        private bool ValidateVerticalBounds(Position4D pos)
        {//make sure position is within the RANDOM BOUND BLOCK
            

        }

        private bool ValidateHorizontalBounds(Position4D pos)
        {
            return 
        }



    }
}
