using HTM.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HTM.Encoders
{
    public class ByteEncoder
    {
        private int _n;
        private int _w;
        private SDR _outputSdr;

        public ByteEncoder(uint minVal, uint maxVal)
        {
            if(maxVal > 256 || minVal > maxVal)
            {
                throw new Exception("Invalid limits");
            }
            _n = (CPM.GetInstance.NeuralSchema == Enums.NeuralSchema.FIVECROSSFIVE) ? 300 : (CPM.GetInstance.NeuralSchema == Enums.NeuralSchema.TENCROSSTEN ? 4950 : 0);
            _w = ;




        }
        
        public SDR EncodeByte(byte byteVal)
        {

        }

        public SDR SparsifyInput(uint inputs)
        {            
            throw new System.NotImplementedException();
        }
    }
}
