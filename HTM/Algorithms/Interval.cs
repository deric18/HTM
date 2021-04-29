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

        public Interval(uint x1, uint x2, uint y1, uint y2)
        {            
            this.i1 = x1;
            this.i2 = x2;
            this.i3 = y1;
            this.i4 = y2;
            this.isInited = true;
            this.isBlockChanged = false;
        }                                                               

        public uint PredictRandomInteger()
        {
            if (isInited)
            {
                Random r = new Random();
                uint rnd1 = (uint)r.Next((int)i1, (int)i2);
                uint rnd2 = (uint)r.Next((int)i3, (int)i4);
                uint retInt = (rnd1 + rnd2) % 2 == 0 ? rnd1 : rnd2;

                isBlockChanged = (i3 <= retInt && retInt >= i4) ? false : (i1 <= retInt && retInt >= i2);

                return retInt;
            }
            else
                throw new InvalidOperationException();
        }
    }
}
