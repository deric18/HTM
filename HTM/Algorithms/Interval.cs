using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static uint PredictNewRandomSynapseWithoutInterval(uint i1, uint i2)
        {
            Random r = new Random();
            int I1 = Convert.ToInt32(i1);
            int I2 = Convert.ToInt32(i2);
            return (uint)r.Next(I1, I2);
        }
    }
}
