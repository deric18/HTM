using System.Collections.Generic;
using HTM.Interfaces;
using HTM.Models;

namespace HTM
{
    public class IntegerEncoder : IEncode
    {
        private int _numberOfPossibilities;
        private int _sdrLengthRequired;
        private SDR _outputSdr;

        public SDR SparsifyInput(List<int> inputs)
        {

            throw new System.NotImplementedException();
        }
    }
}
