using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Algorithms;
using HTM.Models;
using System.Collections.Generic;

namespace HtmTest
{
    public class TestNeuron
    {
        CPM instance;
        ConnectionTable cTable;
        SynapseGenerator synGen;

        [TestInitialize]
        public void Initialze()
        {
            instance = CPM.GetInstance;
            instance.Initialize(10, 100000);
            //cTable = ConnectionTable.Singleton();
        }

        [TestMethod, TestCategory("Neuron Tests")]
        public void TestFire()
        {
            Neuron neuron = instance.GetNeuronFromPosition(new Position3D(5, 5, 5));


            var segments = neuron.Segments.Values;

            foreach(var segment in segments)
            {
                //segment.
            }




        }


        [TestMethod, TestCategory("Neuron Tests")]
        public void TestGetSegment()
        {

        }


        [TestMethod, TestCategory("Neuron Tests")]
        public void TestRegisterSubSegment()
        {

        }


    }
}