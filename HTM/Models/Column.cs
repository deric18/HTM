using HTM.Enums;
using System.Collections.Generic;
using System.Linq;

namespace HTM.Models
{
    public class Column
    {
        public List<Neuron> Neurons { get; private set; }
        public Position2D ID { get; private set; }
        public int Size { get; private set; }

        public Column(uint x, uint y, int size)
        {
            Size = size;
            ID = new Position2D(x, y);
            for(int i=0;i< Size; i++)
            {
                Neuron n = new Neuron();
                Neurons.Add(n);
            }
        }

        public Neuron GetNeuron(int z)
        {
            return Neurons[z];
        }

        public List<Neuron> GetPredictedCells => Neurons.Where(pos => pos.State == NeuronState.PREDICTED).ToList();        
    }
}
