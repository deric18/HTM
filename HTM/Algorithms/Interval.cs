using HTM.Models;
using System;

namespace HTM.Algorithms
{
    internal static class Interval
    {        
        //iLow, iLowMid, iHighMid, iHigh
        public static uint PredictNewRandomSynapse(uint i1, uint i2, uint i3, uint i4)
        {//Use computed bounds to randomly predict a new position inside the random neuro block
            //Need to be redone.

            Random r = new Random();
            int I1 = Convert.ToInt32(i1);
            int I2 = Convert.ToInt32(i2);
            int I3 = Convert.ToInt32(i3);
            int I4 = Convert.ToInt32(i4);

            uint rnd1 = (uint)r.Next(I1, I2);
            uint rnd2 = (uint)r.Next(I3, I4);

            return (rnd1 + rnd2) % 2 == 0 ? rnd1 : rnd2;
        }



        public static Position3D PredictNewRandomSynapseWithoutInterval(Position3D pos, char dimension, uint blockRadius)
        {
            Position3D toRet;

            switch(dimension)
            {
                case 'x':
                case 'X':
                    {
                        toRet.Z = GetRand()
                        toRet.Y = pos.Y;
                        toRet.Z = pos.Z;
                        break;
                    }
                case 'y':
                case 'Y':break;
                case 'z':
                case 'Z':break;
                case 'A':
                case 'a':break;
                default: break;

            }
        }        

        private static uint GetRand(uint min, uint max)
        {
            Random r = new Random();
            int I1 = Convert.ToInt32(min);
            int I2 = Convert.ToInt32(max);
            return (uint)r.Next(I1, I2);
        }

        public static uint GetRand(int seed, uint min, uint max)
        {
            Random r = new Random(seed);
            int I1 = Convert.ToInt32(min);
            int I2 = Convert.ToInt32(max);
            return (uint)r.Next(I1, I2);
        }
    }
}
