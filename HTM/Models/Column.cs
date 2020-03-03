﻿using HTM.Enums;
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
            }
        }

        internal void Fire()
        {

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

        internal Neuron GetNeuron(uint z) => Neurons[(int)z];

        internal List<Neuron> GetPredictedCells()
        {
            return Neurons.Where(pos => (pos.State == NeuronState.PREDICTED || pos.State == NeuronState.SPIKING)).ToList();
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
    }
}
