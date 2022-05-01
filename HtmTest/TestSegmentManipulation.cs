using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Algorithms;
using HTM.Models;

namespace HtmTest
{
    internal class TestSegmentManipulation
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


        [TestMethod]
        public void TestProximalSegmentCreation()
        {

        }


        [TestMethod]
        public void TestMaxSynapseCreationOnSegment()
        {

        }

        [TestMethod]
        public void TestSynapseCreationOnSegment()
        {

        }


        [TestMethod]
        public void TestSubSegmentCreationOnSegment()
        {

        }


        [TestMethod]
        public void TestSynapseGrowthOnProperFiring()
        {

        }


    }
}
