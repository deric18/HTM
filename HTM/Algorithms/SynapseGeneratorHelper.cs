using HTM.Models;
using System;

namespace HTM.Algorithms
{
    internal static class SynapseGeneratorHelper
    {       
        public static uint PredictNewRandomSynapseWithoutInterval(Position3D pos, char dimension, uint blockRadius)
        {
            uint toRet = 0;

            switch(dimension)
            {
                case 'x':
                case 'X':
                    {
                        toRet = GetRand(pos.X - blockRadius, pos.X + blockRadius);                        
                        break;
                    }
                case 'y':
                case 'Y':
                    {                        
                        toRet = GetRand(pos.Y - blockRadius, pos.Y + blockRadius);               
                        break;
                    }                    
                case 'z':
                case 'Z':
                    {                        
                        toRet = GetRand(pos.Z - blockRadius, pos.Z + blockRadius);
                        break;
                    }                                   
                default: break;

            }

            return toRet;

        }

        public static Position3D PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(Position3D pos, char dimension, uint blockRadius, uint? count = 0)
        {
            Position3D toRet = pos;

            switch (dimension)
            {
                case 'x':
                case 'X':
                    {
                        toRet.X = GetRand(pos.X - blockRadius, pos.X + blockRadius);
                        break;
                    }
                case 'y':
                case 'Y':
                    {
                        toRet.Y = GetRand(pos.Y - blockRadius, pos.Y + blockRadius);
                        break;
                    }
                case 'z':
                case 'Z':
                    {
                        toRet.Z = GetRand(pos.Z - blockRadius, pos.Z + blockRadius);
                        break;
                    }
                case 'A':
                case 'a':
                    {
                        toRet.X = GetRand(pos.X - blockRadius, pos.X + blockRadius);
                        toRet.Y = GetRand(pos.Y - blockRadius, pos.Y + blockRadius);
                        toRet.Z = GetRand(pos.Z - blockRadius, pos.Z + blockRadius);
                        toRet.BID = pos.BID;
                        toRet.cType = Enums.CType.ConnectedToAxon;
                        break;
                    }
                case 'D':
                case 'd':
                    {
                        toRet.X = GetRand(pos.X - blockRadius, pos.X + blockRadius);
                        toRet.Y = GetRand(pos.Y - blockRadius, pos.Y + blockRadius);
                        toRet.Z = GetRand(pos.Z - blockRadius, pos.Z + blockRadius);
                        toRet.BID = pos.BID;
                        toRet.cType = Enums.CType.ConnectedToDendrite;
                        break;
                    }
                default: break;

            }

            //just making sure we dont accidentlly pick the same position back to the caller)
            toRet = (toRet.X == pos.X && toRet.Y == pos.Y && toRet.Z == pos.Z ) ? PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(toRet, dimension, blockRadius, ++count) : toRet ;


            //making sure its available or recurse again.
            return ConnectionTable.SingleTon.IsPositionAvailable(toRet) ? toRet : PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(pos, dimension, blockRadius);

        }


        //not handled for core blocks yet!
       

        //iLow, iLowMid, iHighMid, iHigh
        internal static uint PredictRandomIntervalInteger(int i1, int i2, int i3, int i4)
        {//Use computed bounds to randomly predict a new position inside the random neuro block
            //Need to be redone.

            Random r = new Random();            

            uint rnd1 = (uint)r.Next(i1, i2);
            uint rnd2 = (uint)r.Next(i3, i4);

            return (rnd1 + rnd2) % 2 == 0 ? rnd1 : rnd2;
        }

        internal static uint PredictRandomIntervalInteger2(uint i1, uint i2, uint i3, uint i4)
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

        public static uint GetRand(uint min, uint max)
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
