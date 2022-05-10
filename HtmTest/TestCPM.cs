using System;
using HTM.Models;
using HTM.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTM;
using HTM.Enums;
using System.Collections.Generic;
using NUnit;


namespace HtmTest
{
    [TestClass]
    public class TestCPM
    {
        CPM instance;
        ConnectionTable cTable;
        SynapseGenerator synGen;
        uint xyz = 0;

        [TestInitialize]
        public void Initialze()
        {
            xyz = 10;
            instance = CPM.GetInstance;
            instance.Initialize(xyz, 100000);            
            //cTable = ConnectionTable.Singleton();
        }


        [TestMethod]
        public void TestColumnStructure()
        {
            
            Neuron neuron = null;

            for (uint i=0;i<xyz;i++)
                for(uint j=0;j<xyz;j++)
                    for(uint k=0;k<xyz;k++)
                    {
                        neuron = instance.Columns[i,j].GetNeuron(k);
                        Assert.AreEqual(neuron.NeuronID.X, i);
                        Assert.AreEqual(neuron.NeuronID.Y, j);
                        Assert.AreEqual(neuron.NeuronID.Z, k);
                    }
        }


        [TestMethod, ExpectedException(typeof(Exception))]
        public void TestProcessCycle()
        {
            SDR dummy1 = GetNewSDR(InputPatternType.APICAL);
            SDR dummy2 = GetNewSDR(InputPatternType.SPATIAL);

            instance.Process(dummy1, dummy2);
        }        
               

        [TestMethod, Ignore]
        public void TestBlockConfigProvider()
        {

        }


        [TestMethod]
        public void TestGetIntersectionSet()
        {



        }

        private SDR GetNewSDR(InputPatternType iType)
        {
            List<Position2D> activebits = GetRandomPositionList(4);

            SDR toRet = new SDR(xyz, xyz, activebits);

            toRet.IType = iType;

            return toRet;
        }        



        private List<Position2D> GetRandomPositionList(int count)
        {
            List<Position2D> positionList = new List<Position2D>();

            for (int i = 0; i < count; i++)
                positionList.Add(new Position2D(Convert.ToUInt32(GetRand()), Convert.ToUInt32(GetRand())));

            return positionList;

        }

        private int GetRand()
        {

            Random r = new Random();
            return r.Next(0, (int)xyz);
        }
    }
}
