using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Algorithms;
using HTM.Models;
using System.Collections.Generic;

namespace HtmTest
{

    [TestClass]
    public class TestSynapseGenerator
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

            List<Position3D> corePositionList = new List<Position3D>()
            {
                new Position3D(99, 99, 99, 999),
                new Position3D(9, 9, 0, 0),
                new Position3D(90, 0, 9, 0),
                new Position3D(0,0,0,0),
                new Position3D(999, 99, 99, 99),

            }
            Neuron neuron;

            
                        neuron = instance.Columns[i, j].GetNeuron(k);

                        foreach (var seg in neuron.Segments.Values)
                        {
                            Assert.AreEqual(seg.SegmentID.BasePosition.BID, neuron.NeuronID.BID);
                            Assert.AreEqual(seg.NeuronID.X, neuron.NeuronID.X);
                            Assert.AreEqual(seg.NeuronID.Y, neuron.NeuronID.Y);
                            Assert.AreEqual(seg.NeuronID.Z, neuron.NeuronID.Z);

                            

                        }


                        Assert.AreEqual(neuron.Segments.Count, 2);
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
