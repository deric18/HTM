using HTM.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HTM.Encoders
{
    public class IntegerEncoder
    {
        private int _numberOfPossibilities;
        private int _sdrLengthRequired;
        private SDR _outputSdr;
        private CPM _cpm;
        private Random rnd;

        public IntegerEncoder()
        {
            _cpm = CPM.GetInstance;
            rnd = new Random();
        }

        public SDR SparsifyInput(List<uint> inputs)
        {            
            throw new System.NotImplementedException();
        }
    }
}
