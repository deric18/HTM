using HTM.Interfaces;
using HTM.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HTM.Encoders
{
    public class PixelEncoder
    {
        private int _numberOfPossibilities;
        private int _sdrLengthRequired;
        private SDR _outputSdr;
        private CPM _cpm;
        private Random rnd;

        public PixelEncoder()
        {
            _cpm = CPM.GetInstance;
            rnd = new Random();
        }

        public SDR SparsifyInput(List<Color> inputs)
        {
            SDR toReturn = new SDR();
            throw new System.NotImplementedException();
        }
    }
}
