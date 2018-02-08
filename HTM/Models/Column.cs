using System.Collections.Generic;

namespace HTM.Models
{
    class Column
    {
        List<Neuron> Neruons;
        Position2D ID;
        private int _length;

        public Column(int length)
        {
            _length = length;
            for(int i=0;i<length;i++)
            {
                Neuron n = new Neuron();

            }
        }
    }
}
