using HTM.Models;
using System;

namespace HTM.Algorithms
{
    internal static class Interval
    {               
        public static Position3D PredictNewRandomSynapseWithoutInterval(Position3D pos, char dimension, uint blockRadius)
        {
            Position3D toRet = pos;

            switch(dimension)
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
                        toRet.Y = GetRand(pos.Y - blockRadius, pos.Y + blockRadius); ;                     
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
                        toRet.Y = GetRand(pos.Y - blockRadius, pos.Y + blockRadius); ;
                        toRet.Z = GetRand(pos.Z - blockRadius, pos.Z + blockRadius);
                        break;
                    }
                default: break;

            }

            return ConnectionTable.SingleTon.IsPositionAvailable(toRet) ? toRet : PredictNewRandomSynapseWithoutInterval(pos, dimension, blockRadius);

        }

        public static Position3D PredictNewRandomSynapseWithInterval(Position3D pos, char dimension, uint blockRadius)
        {
            Position3D toRet = pos;
            //TODO : Forming intervals for negative indexed coordiantes , reducing blockId's for such coordinates as such ,...

            switch (dimension)
            {
                case 'x':
                case 'X':
                    {
                        if(pos.X - blockRadius >= 0  || pos.X + blockRadius < CPM.GetInstance.BCP.NumXperBlock)
                        {
                            throw new Exception("Called PredictNewRandomSynapseWithInterval without neccesity for interval");
                        }
                        //Compute interval 
                        toRet.X = GetRand()                        
                        break;
                    }
                case 'y':
                case 'Y': break;
                case 'z':
                case 'Z': break;
                case 'A':
                case 'a': break;
                default: break;

            }
        }

        //iLow, iLowMid, iHighMid, iHigh
        private static uint PredictNewRandomSynapse(uint i1, uint i2, uint i3, uint i4)
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
