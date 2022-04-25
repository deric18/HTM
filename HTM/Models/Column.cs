using HTM.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HTM.Models
{
    public class Column
    {
        internal List<Neuron> Neurons { get; private set; }
        internal Position2D ID { get; private set; }
        internal uint Size { get; private set; }
        

        internal Column(uint x, uint y, uint size)
        {
            Size = size;
            ID = new Position2D(x, y);
            for(uint i=0;i< Size; i++)
            {
                Neuron n = new Neuron(new Position3D(x,y,i));
                Neurons.Add(n);
                Initialize(n);
            }
        }

        private void Initialize(Neuron n)
        {
            //proximal segments always need to have higher initial synaptic connection potential.
            n.CreateProximalSegments();
        }

        private void SetupProximalSegments()
        {
            foreach (Neuron neuron in Neurons)
            {

            }
        }

        internal void Fire()
        {
            Neurons.ForEach(x => x.Fire());
        }

        internal Neuron GetMaxVoltageNeuronInColumn()
        {
            Neuron toRet = null;
            uint max = 0;
            Neurons.ForEach(q =>
            {
                if (max < q.Voltage)
                {
                    max = q.Voltage;
                    toRet = q;
                }
            });
            return toRet;
        }

        internal Neuron GetNeuron(uint z) => Neurons[Convert.ToInt32(z)];

        internal List<Position3D> GetFiringCellPositions()
        {
            var neronsList = Neurons.Where(pos => (pos.State == NeuronState.FIRED || pos.State == NeuronState.SPIKING)).ToList();
            List<Position3D> posList = new List<Position3D>();

            foreach(var neuron in neronsList)
            {
                posList.Add(neuron.NeuronID);
            }

            return posList;
        }
        
        internal string GetFiringCellRepresentation()
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

        internal List<Neuron> GetPredictedCells() => Neurons.Where(n => n.State != NeuronState.RESTING).ToList();        
    }
}
