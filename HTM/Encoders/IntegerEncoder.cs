using HTM.Models;
using System.Collections.Generic;

namespace HTM.Encoders
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
