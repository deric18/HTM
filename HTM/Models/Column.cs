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
            Neurons = new List<Neuron>();
            Size = size;
            ID = new Position2D(x, y);
            for(uint i=0;i< Size; i++)
            {
                Neuron n = new Neuron(new Position3D(x, y, i));         //Potential Bug Reolved. Incorrect Block Id Allocation can cause potential fuck up's hard to track! Lesson : give 100% focus to modules you are working on !!!!!
                Neurons.Add(n);
                Initialize(n);
            }
        }

        private void Initialize(Neuron n)
        {
            //proximal segments always need to have higher initial synaptic connection potential.
            if(n.NeuronID.BID == 910)
            {
                Console.WriteLine("Cath it!!!");
            }
            n.CreateProximalSegments();
        }       

        internal void Fire()
        {
            Neurons.ForEach(x => x.Fire());
        }

        internal List<Neuron> GetMaxVoltageNeuronInColumn()
        {
            List<Neuron> toRet = null;
            uint max = 0;
            Neuron n = null;
            Neurons.ForEach(q =>
            {
                if (max < q.Voltage)
                {
                    max = q.Voltage;
                    n = q;
                }
            });

            if (n == null)
                return Neurons;

            toRet.Add(n);

            return toRet;
        }

        public Neuron GetNeuron(uint z) => Neurons[Convert.ToInt32(z)];

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
