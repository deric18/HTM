namespace HTM.Algorithms
{
    using System;
    internal class Interval
    {
        private int i1;
        private int i2;
        private int i3;
        private int i4;
        private bool isInited;

        public Interval(int x1, int x2, int y1, int y2)
        {            
            this.i1 = x1;
            this.i2 = x2;
            this.i3 = y1;
            this.i4 = y2;
            this.isInited = true;
        }                                                               

        public uint PredictRandomInteger()
        {
            if (isInited)
            {
                Random r = new Random();
                uint rnd1 = (uint)r.Next(i1, i2);
                uint rnd2 = (uint)r.Next(i3, i4);

                return (rnd1 + rnd2) % 2 == 0 ? rnd1 : rnd2;
            }
            else
                throw new InvalidOperationException();
        }
    }
}
