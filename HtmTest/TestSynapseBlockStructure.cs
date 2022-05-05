namespace HtmTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using HTM;
    using HTM.Algorithms;
    using HTM.Models;

    [TestClass, Ignore]    
    public class TestSynapseBlockStructure
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
        public void CreateNewPositionBasisBlock_WithintheBlock()
        {
            //Create a synapses at  Edge blocks and check the created synapses are at within those blocks , also check cTable
            synGen = SynapseGenerator.GetInstance;
            Position3D basePosition = new Position3D(1, 1, 1, 1);

            Position3D newPredictedSynapse = synGen.PredictNewRandomPosition(basePosition, 3);




        }

        [TestMethod]
        public void CreateNewPositionAcrosXL_XR_YU_YD_ZF_ZD()
        {
            //Create a synapses at basis block and check the created synapses are at the within those blocks , also check cTable
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
