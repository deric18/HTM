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
            HTM.SynapseManager cpm = new HTM.SynapseManager();

            SDR testInput = GetTestInput();
            SDR expectedOutput = GetTestOutput();


            cpm.Process(testInput, HTM.Enums.InputPatternType.SPATIAL);
            SDR observedOutput = cpm.Predict();


            Assert.AreEqual(observedOutput, expectedOutput);

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
