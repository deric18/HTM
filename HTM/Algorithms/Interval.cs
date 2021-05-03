namespace HTM.Algorithms
{
    using System;
    internal class Interval
    {
        private uint i1;
        private uint i2;
        private uint i3;
        private uint i4;
        private bool isInited;
        public bool isBlockChanged;
        private uint hardCodedX;

        public Interval(uint x1min, uint x2max, uint y1min, uint y2max)
        {            
            this.i1 = x1min;
            this.i2 = x2max;
            this.i3 = y1min;
            this.i4 = y2max;
            this.isInited = true;
            this.isBlockChanged = false;
            this.hardCodedX = 0;
        }                                                               

        public Interval(uint x)
        {
            this.hardCodedX = x;
            this.isInited = false;
            this.isBlockChanged = false;            
        }

        public uint PredictRandomInteger(int? seed = null)
        {
            if (isInited)
            {
                Random r;

                r = seed == null ? new Random() : new Random((int)seed);
                
                uint rnd1 = i1 < i2 ? (uint)r.Next((int)i1, (int)i2) : (uint)r.Next((int)i2, (int)i1);
                uint rnd2 = i3 < i4 ? (uint)r.Next((int)i3, (int)i4) : (uint)r.Next((int)i4, (int)i3);
                uint retInt = (rnd1 + rnd2) % 2 == 0 ? rnd1 : rnd2;

                isBlockChanged = (i3 <= retInt && retInt >= i4) ? false : (i1 <= retInt && retInt >= i2);

                return retInt;
            }
            else
                return hardCodedX;
        }
    }
}