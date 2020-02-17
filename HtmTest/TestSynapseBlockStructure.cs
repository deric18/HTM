using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HTM.Models;
using HTM;

namespace HtmTest
{
    [TestClass]    
    public class TestSynapseBlockStructure
    {
        CPM cpm = CPM.GetInstance;
        cpm.Initialize(10, 10, 10);
    }
}
