using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Algorithms;
using HTM.Models;
using System.Collections.Generic;

namespace HtmTest
{

    [TestClass]
    internal class TestSynapseGenerator
    {
        CPM instance;
        ConnectionTable cTable;

        [TestInitialize]
        public void Initialze()
        {
            instance = CPM.GetInstance;
            instance.Initialize(10, 100000);
            cTable = instance.CTable;            
        }


        [TestMethod]
        public void CreateProximalSegmentForCoreBB()
        {
            //we have 1000 neurons in a 3 dimensional space and we are trying to make sure if we send a CBB ew get the right number of axons and dendrite synapses with the same BID sent

            Position3D position3D = new Position3D(99, 99, 99, 999);
            Neuron neuron;

            for (uint i = 0; i < 10; i++)
                for (uint j = 0; j < 10; j++)
                    for (uint k = 0; k < 10; k++)
                    {
                        neuron = instance.Columns[i, j].GetNeuron(k);

                        foreach (var seg in neuron.Segments.Values)
                        {
                            Assert.Equals(seg.SegmentID.BasePosition.BID, neuron.NeuronID.BID);
                            Assert.Equals(seg.NeuronID.X, neuron.NeuronID.X);
                            Assert.Equals(seg.NeuronID.Y, neuron.NeuronID.Y);
                            Assert.Equals(seg.NeuronID.Z, neuron.NeuronID.Z);


                        }
                    }                       
        }

        [TestMethod]
        public void CreateProximalSegmentForDoubleBB()
        {

        }

        [TestMethod]
        public void CreateProximalSegmentForSingleBB()
        {

        }

        [TestMethod]
        public void CreateProximalSegmentForNormalBB()
        {

        }


        //[TestCleanup]
        //private TestCleanup()
        //{

        //}
    }
}
