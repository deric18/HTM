using HTM.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HTM.Encoders
{
    public class IntegerEncoder
    {        
        private int _w;
        private int _r;
        private SDR _outputSdr;        
        private Random rnd;

        public IntegerEncoder(int W, int R)
        {
            _w = W;
            _r = R;
            rnd = new Random();
        }

        public SDR SparsifyInput(uint inputs)
        {            
            throw new System.NotImplementedException();
        }
    }
}
