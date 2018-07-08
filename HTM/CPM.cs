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
            instance._longPredictedList = new List<Neuron>();
            instance._shortPredictedList = new List<Neuron>();

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
        /// All the Firing modules update the predicted list , changing the current state of the system.
        /// </summary>
        /// <param name="inputPattern"></param>
        /// <param name="iType"></param>
        public void Process(SDR inputPattern, InputPatternType iType)
        {
            switch(iType)
            {
                case InputPatternType.SPATIAL:
                    {
                        //Fetch the columns to fire and decide if to burst the whole column or fire specific neurons
                        //Fire the neurons and update predicted list
                        List<Position2D> firingPositions = inputPattern.GetActivePositions();

                        foreach(var col in firingPositions)
                        {                            
                            instance.ProcessColumn(col, iType);
                        } 
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        //Fetch , Fire , Update
                        List<Position2D> firingPositions = inputPattern.GetActivePositions();

                        foreach (var col in firingPositions)
                        {
                            instance.ProcessColumn(col, iType);
                        }
                        break;
                    }                    
                case InputPatternType.APICAL:
                    {
                        //Fetch , Fire , Update
                        List<Position2D> firingPositions = inputPattern.GetActivePositions();

                        foreach (var col in firingPositions)
                        {
                            instance.ProcessColumn(col, iType);
                        }
                        break;
                    }
            }            
        }        
        
        public string Predict()
        {
            string toReturn = null;

            foreach(var columnArray in Columns)
            {
                foreach(var column in columnArray)
                {
                    toReturn += column.GetFiringCellRepresentation();
                }
            }

            return toReturn;
        }
        
        private void Grow()
        {//ToDo
            //Give a GROW SIGNAL around the network 
            //Can always be tweaked and policies may be constructed for sending these signals based on how much a neuron/Segment has contributed.
        }
        
         
        private void ProcessColumn(Position2D pos, InputPatternType iType)
        {
            //check for predicted cells in the column and decide whther to burst or not
            //pick cells and fire
            //return List of positions 
            switch(iType)
            {
                case InputPatternType.SPATIAL:
                    {
                        Column col = GetSpatialColumn(pos);

                        List<Neuron> predictedCells = col.GetPredictedCells;

                        if (predictedCells.Count > 0)
                        {
                            //Regular Fire
                            foreach (var neuron in predictedCells)
                            {
                                _longPredictedList.AddRange(GetNeuronsFromPositions(neuron.Fire()));
                            }
                        }
                        else
                        {
                            //Bursting
                        }
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        //Bursts all the time
                        //Travel through the axonal line laterally and add them to longpredicted list , give them temporal voltage
                        Column col = GetTemporalColumn(pos);
                        break;
                    }
                case InputPatternType.APICAL:
                    {
                        //Bursts all the time
                        //Travel through the apical lines and add them to longpredictedlist , give them apical voltage.

                        break;
                    }
                default:break;
            }                        
        }       

        private IEnumerable<Neuron> GetNeuronsFromPositions(List<Position3D> list)
        {
            List<Neuron> toReturn = new List<Neuron>();

            foreach(var pos in list)
            {
                toReturn.Add(GetNeuronFromPosition(pos));
            }

            return toReturn;
        }

        internal static Position3D GetBound()
        {
            return new Position3D(int.Parse(ConfigurationManager.AppSettings["SEGMENT_XBOUND"]), int.Parse(ConfigurationManager.AppSettings["SEGMENT_YBOUND"]), int.Parse(ConfigurationManager.AppSettings["SEGMENT_ZBOUND"]));
        }

        #region HELPER METHODS 

        private Column GetSpatialColumn(Position2D position) => Columns[position.X][position.Y];

        private Neuron GetNeuronFromPosition(Position3D pos) => Columns[pos.X][pos.Y].GetNeuron(pos.Z);

        private Column GetTemporalColumn(Position2D pos)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
    