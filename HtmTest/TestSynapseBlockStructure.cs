using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Algorithms;

namespace HtmTest
{
    [TestClass]    
    public class TestSynapseBlockStructure
    {
        CPM instance;
        ConnectionTable _;

        [TestInitialize]
        public void Initialze()
        {
            instance = CPM.GetInstance;
            instance.Initialize(10, 10, 10);
            _ = ConnectionTable.Singleton();
        }

        [TestMethod]
        public void TestBlockStructure()
        {
            
        }
        
    }
}
