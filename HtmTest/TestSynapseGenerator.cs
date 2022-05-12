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
                new Position3D(0,0,0,  0),
                new Position3D(9,0,0,  9),
                new Position3D(0,9,0, 90),
                new Position3D(9,9,0, 99),
                new Position3D(0,0,9, 900),
                new Position3D(9,0,9, 909),
                new Position3D(0,9,9, 990),
                new Position3D(9,9,9, 999)
            };


            foreach (var neuron in instance.GetNeuronsFromPositions(corePositionList))
            {
                Assert.AreEqual(neuron.ProximalSegmentList.Count + neuron.axonEndPoints.Count, 2);

                foreach (var seg in neuron.Segments.Values)
                {
                    Assert.AreEqual(seg.SegmentID.BasePosition.BID, neuron.NeuronID.BID);
                    Assert.AreEqual(seg.NeuronID.X, neuron.NeuronID.X);
                    Assert.AreEqual(seg.NeuronID.Y, neuron.NeuronID.Y);
                    Assert.AreEqual(seg.NeuronID.Z, neuron.NeuronID.Z);

                }
            }
        }


        [TestMethod]
        public void CreateProximalSegmentForDoubleBB()
        {

            var doubleBBPositionList = new List<Position3D>()
            {
                new Position3D(0,1,0, 10),
                new Position3D(9,1,9, 19),
                new Position3D(0,8,0, 80),
                new Position3D(9,8,0, 89),

                new Position3D(0,1,9, 910),
                new Position3D(9,1,9, 919),
                new Position3D(0,8,9, 980),
                new Position3D(9,8,9, 989)
            };

            foreach (var neuron in instance.GetNeuronsFromPositions(doubleBBPositionList))
            {
                Assert.AreEqual(neuron.ProximalSegmentList.Count + neuron.axonEndPoints.Count, 2);

                foreach (var seg in neuron.Segments.Values)
                {
                    Assert.AreEqual(seg.SegmentID.BasePosition.BID, neuron.NeuronID.BID);
                    Assert.AreEqual(seg.NeuronID.X, neuron.NeuronID.X);
                    Assert.AreEqual(seg.NeuronID.Y, neuron.NeuronID.Y);
                    Assert.AreEqual(seg.NeuronID.Z, neuron.NeuronID.Z);

                }
            }
        }

        [TestMethod]
        public void CreateProximalSegmentForSingleBB()
        {
            var sinlgeBBPositionList = new List<Position3D>()
            {                
                new Position3D(5,0,5, 505),
                new Position3D(5,5,0, 55),
                new Position3D(0,5,5, 550),
                new Position3D(9,5,5, 559),
                new Position3D(5,5,5, 555)
            };

            foreach (var neuron in instance.GetNeuronsFromPositions(sinlgeBBPositionList))
            {
                Assert.AreEqual(neuron.ProximalSegmentList.Count + neuron.axonEndPoints.Count, 4);

                foreach (var seg in neuron.Segments.Values)
                {
                    Assert.AreEqual(seg.SegmentID.BasePosition.BID, neuron.NeuronID.BID);
                    Assert.AreEqual(seg.NeuronID.X, neuron.NeuronID.X);
                    Assert.AreEqual(seg.NeuronID.Y, neuron.NeuronID.Y);
                    Assert.AreEqual(seg.NeuronID.Z, neuron.NeuronID.Z);

                }
            }
        }

        [TestMethod]
        public void CreateProximalSegmentForNormalBB()
        {
            var normalBBPositionList = new List<Position3D>()
            {
                new Position3D(1,1,1, 111),
                new Position3D(8,8,8, 888),
                new Position3D(8,1,1, 118),
                new Position3D(8,8,9, 988)
            };

            foreach (var neuron in instance.GetNeuronsFromPositions(normalBBPositionList))
            {
                Assert.AreEqual(neuron.ProximalSegmentList.Count + neuron.axonEndPoints.Count, 8); 

                foreach (var seg in neuron.Segments.Values)
                {
                    Assert.AreEqual(seg.SegmentID.BasePosition.BID, neuron.NeuronID.BID);
                    Assert.AreEqual(seg.NeuronID.X, neuron.NeuronID.X);
                    Assert.AreEqual(seg.NeuronID.Y, neuron.NeuronID.Y);
                    Assert.AreEqual(seg.NeuronID.Z, neuron.NeuronID.Z);

                }
            }
        }


        //[TestCleanup]
        //private TestCleanup()
        //{

        //}
    }
}
