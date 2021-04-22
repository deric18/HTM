using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Algorithms;

namespace HtmTest
{
    [TestClass]    
    public class TestSynapseBlockStructure
    {
        CPM instance;
        ConnectionTable cTable;
        SynapseGenerator synGen;

        [TestInitialize]
        public void Initialze()
        {
            instance = CPM.GetInstance;
            instance.Initialize(10, 10, 10, 100000);
            cTable = ConnectionTable.Singleton();
        }
        

        [TestMethod]
        public void TestEdgeBlockSynapseCreation()
        {
            //Create a synapses at  Edge blocks and check the created synapses are at within those blocks , also cehck cTable
            synGen = SynapseGenerator.GetInstance;
            

        }

        [TestMethod]
        public void TestBasisBlockSynapseCreation()
        {
            //Create a synapses at basis block and check the created synapses are at the within those blocks , also cehck cTable
        }

        [TestMethod]
        public void TestGeneralSynapseCreation()
        {
            //Create synapses at non basis and edge block at the extreme corners of the block to check on probability to land on a synapse across the block 
        }

        [TestMethod]
        public void TestInterBlockSynapseCreation()
        {
            //Create a synapse at a point which is a in non basis . edge block and well within the Rasndom square block boundaries from the its block and post creation check the synapse created is within the block.
        }

    }
}
