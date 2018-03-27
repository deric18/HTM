using System.Collections.Generic;

namespace HTM.Models
{
    public class Column
    {
        List<Neuron> Neurons;
        Position2D ID;
        private int _size;

        public Column(uint x, uint y, int size)
        {
            _size = size;
            ID = new Position2D(x, y);
            for(int i=0;i< _size; i++)
            {
                Neuron n = new Neuron();
                Neurons.Add(n);
            }
        }

        public Neuron GetNeuron(int z)
        {
            return Neurons[z];
        }
    }
}
