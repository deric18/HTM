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
        private List<Neuron> _longPredictedList;
        private List<Neuron> _shortPredictedList;
        
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

        /// <summary>
        /// Process incoming signal 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public SDR Predict(SDR input, )
        {
            //Get current predicted list
            //Generate SDR with this as firing pattern and return it..
            List<Position2D> _firingColumns = GetColumnsToProcess(input);

            throw new NotImplementedException();                        
        }
        
        public void Process(SDR inputPattern, InputPatternType iType)
        {
            switch(iType)
            {
                case InputPatternType.SPATIAL:
                    {             
                        //Fetch the columns to fire and decide if to burst the whole column or fire specific neurons
                        //Fire the neurons and update predicted list
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        //Fetch , Fire , Update
                        break;
                    }                    
                case InputPatternType.APICAL:
                    {
                        //Fetch , Fire , Update
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

        internal static Position3D GetBound()
        {
            return new Position3D(int.Parse(ConfigurationManager.AppSettings["SEGMENT_XBOUND"]), int.Parse(ConfigurationManager.AppSettings["SEGMENT_YBOUND"]), int.Parse(ConfigurationManager.AppSettings["SEGMENT_ZBOUND"]));
        }

        #region HELPER METHODS 

        private List<Position2D> GetColumnsToProcess(SDR input)
        {
            List<Position2D> toReturn = new List<Position2D>;

            return toReturn;
        }


        #endregion
    }
}
