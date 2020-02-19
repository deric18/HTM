using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Algorithms;

namespace HtmTest
{
    [TestClass]    
    public class TestSynapseBlockStructure
    {
        CPM instance;

        [TestInitialize]
        public void Initialze()
        {
            instance = CPM.GetInstance;
            instance.Initialize(10, 10, 10);
        }

        [TestMethod]
        public void TestBlockStructure()
        {
            ConnectionTable ctable = ConnectionTable.Singleton();
        }
        
    }
}
