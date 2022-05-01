using System;
using HTM.Models;
using HTM.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;

namespace HtmTest
{
    [TestClass]
    public class TestCPM
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
        public void TestNeuron()
        {
            HTM.CPM cpm = HTM.CPM.GetInstance;
            cpm.Initialize(10, 0);

            SDR testInput = GetTestInput();
            testInput.IType = HTM.Enums.InputPatternType.SPATIAL;
            SDR observedOutput = GetTestOutput();


            cpm.Process(testInput);
            SDR predictedOutput = cpm.Predict();


            Assert.AreEqual(predictedOutput, observedOutput);

        }

        private SDR GetTestOutput()
        {
            throw new NotImplementedException();
        }

        private SDR GetTestInput()
        {
            throw new NotImplementedException();
        }
    }
}
