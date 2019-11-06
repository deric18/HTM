using HTM.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HTM.Encoders
{
    public class IntegerEncoder
    {
        private int _n;
        private int _w;
        private SDR _outputSdr;        
        private Random rnd;

        public IntegerEncoder(int N, int W)
        {
            _n = N;
            _w = W;
            rnd = new Random();
        }

        public SDR SparsifyInput(uint inputs)
        {            
            throw new System.NotImplementedException();
        }
    }
}
