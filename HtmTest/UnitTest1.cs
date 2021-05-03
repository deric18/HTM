using System;
using HTM.Models ;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HtmTest
{
    [TestClass]
    public class UnitTest1
    {
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
