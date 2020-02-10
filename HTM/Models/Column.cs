using HTM.Enums;
using System.Collections.Generic;
using System.Linq;

namespace HTM.Models
{
    public class Column
    {
        public List<Neuron> Neurons { get; private set; }
        public Position2D ID { get; private set; }
        public uint Size { get; private set; }
        

        public Column(uint x, uint y, uint size)
        {
            Size = size;
            ID = new Position2D(x, y);
            for(uint i=0;i< Size; i++)
            {
                Neuron n = new Neuron(new Position3D(x,y,i));
                Neurons.Add(n);
            }
        }

        public void Fire()
        {

        }

        public Neuron GetNeuron(uint z) => Neurons[(int)z];

        public List<Neuron> GetPredictedCells()
        {
            return Neurons.Where(pos => (pos.State == NeuronState.PREDICTED || pos.State == NeuronState.SPIKING)).ToList();
        }
        
        public string GetFiringCellRepresentation()
        {
            string toReturn = null;
            foreach(var neuron in Neurons)
            {
                if(neuron.State.Equals(NeuronState.FIRED))
                {
                    toReturn += "1";
                }
                else
                {
                    toReturn += "0";
                }
            }
            return toReturn;
        }
    }
}
