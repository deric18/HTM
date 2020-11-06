using HTM.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HTM.Encoders
{
    public class UnsignedIntegerEncoder
    {
        private int _n;
        private int _w;
        private SDR _outputSdr;

        public UnsignedIntegerEncoder(uint minVal, uint maxVal)
        {
            if((CPM.GetInstance.NeuralSchema == Enums.NeuralSchema.FIVECROSSFIVE && maxVal > 300) && (CPM.GetInstance.NeuralSchema == Enums.NeuralSchema.TENCROSSTEN && maxVal > 4950))
            {
                throw new Exception("Invalid limits for the Nerual Schema Crated");
            }
            _n = (CPM.GetInstance.NeuralSchema == Enums.NeuralSchema.FIVECROSSFIVE) ? 300 : (CPM.GetInstance.NeuralSchema == Enums.NeuralSchema.TENCROSSTEN ? 4950 : 0);
            _w = ;
        }
        
        public SDR EncodeUnsignedInteger(uint scalar)
        {

        }

        public SDR SparsifyInput(uint inputs)
        {            
            throw new System.NotImplementedException();
        }
    }
}
