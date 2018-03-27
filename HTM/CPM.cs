using System;
using HTM.Models;
using System.Collections.Generic;
using HTM.Enums;
using System.Configuration;

namespace HTM
{
    public class CPM
    {
        public const int CubeConstant = 100;        
        public static volatile CPM instance;
        public static object syncRoot = new object();

        private CPM() { }

        public static CPM Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CPM();
                    }
                }

                return instance;
            }
        }

        public int Length { get; private set; }
        public int Breadth { get; private set; }
        public int Width { get; private set; }
        public CPMState State { get; private set; }
        public Column[][] Columns { get; private set; }        
                            
        
        private List<string> _predictedList;        
        
        public bool HasTemporalSignal { get; private set; }
        public bool HasSpatialSignal { get; private set; }        

        public static void Initialize(int length, int breadth, int width)
        {
            instance.Length = length;
            instance.Breadth = breadth;
            instance.Width = width;
            instance.State = CPMState.RESTING;
            instance.HasSpatialSignal = false;
            instance.HasTemporalSignal = false;            

            try
            {
                for (uint i = 0; i < length; i++)
                    for (uint j = 0; j < breadth; j++)
                    {
                        Column toAdd = new Column(i, j, width);
                        instance.Columns[i][j] = toAdd;
                    }
            }
            catch(Exception e)
            {
                Console.WriteLine("Out Of Memory Allocated for the Service via Operating System! , Please reduce the dimensions of the Neuroblock \n Length : " + length + "\nBreadth : " + breadth + "\nWidth : " + width);
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }
        }

        public SDR Process(SDR input)
        {
            //Get current predicted list
            //Generate SDR with this as firing pattern and return it..
            SDR toReturn = new SDR();

            return toReturn;
        }
        
        public void Predict(SDR inputPattern, InputPatternType iType)
        {
            switch(iType)
            {
                case InputPatternType.SPATIAL:
                    {                                        
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        
                        break;
                    }                    
                case InputPatternType.APICAL:
                    {                        
                        break;
                    }
            }            
        }
        
        private void Grow()
        {
            //Give a predict
        }
        

        private void ProcessColumn(Column col)
        {
            //check for predicted cells in the column and decide whther to burst or not
            //pick cells and fire
            //return List of positions 
            
        }

        #region HELPER METHODS 
        
        #endregion
    }
}
