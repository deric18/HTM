using System;
using HTM.Models;
using HTM.Enums;
using System.Configuration;
using System.Collections.Generic;

namespace HTM
{
    public class SynapseManager
    {        
        public static volatile SynapseManager instance;
        public static object syncRoot = new object();

        public SynapseManager() { }

        public static SynapseManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SynapseManager();   
                    }
                }

                return instance;
            }
        }

        public uint NumRows { get; private set; }
        public uint NumCols{ get; private set; }        
        public CPMState State { get; private set; }
        public Column[][] Columns { get; private set; }
        private List<Neuron> _longPredictedList;
        private List<Neuron> _shortPredictedList;
        private bool _readySpatial;
        private bool _readyTemporal;
        private bool _readyApical;                      

        public static void Initialize(ulong NumberofPoints)
        {
            //ToDo : Calculate the block size based on given number of points for the whole region and initialize the variables accordingly.
            instance.NumRows = length;
            instance.NumCols = breadth;            
            instance.State = CPMState.RESTING;            
            instance._longPredictedList = new List<Neuron>();
            instance._shortPredictedList = new List<Neuron>();
            instance._readyApical = false;
            instance._readyTemporal = true;
            instance._readySpatial = false;

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
                Console.WriteLine("Out Of Memory Allocated for the Service via Operating System! , Please reduce the dimensions of the Neuroblock \n NumRows : " + length + "\n NumColumns : " + breadth + "\n NumFiles : " + width);
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }
        }


        public Neuron GetNeuronFromPositionID(Position3D pos) => Columns[pos.X][pos.Y].GetNeuron(pos.Z);
        public Neuron GetNeuronFromSegmentID(SegmentID segId) => Columns[segId.X][segId.Y].GetNeuron(segId.Z);

        public void AddConnection(SegmentID segId, Position3D pos)
        {
            GetNeuronFromSegmentID(segId).AddNewConnectionToSegment(pos, segId);
        }
        

        /// <summary>
        /// All the Firing modules update the predicted list , changing the current state of the system.
        /// </summary>
        /// <param name="inputPattern"></param>
        /// <param name="iType"></param>
        public void Process(SDR inputPattern)
        {
            switch(inputPattern.iType)
            {
                case InputPatternType.SPATIAL:
                    {
                        //Fetch the columns to fire and decide if to burst the whole column or fire specific neurons
                        //Fire the neurons and update predicted list
                        if (!_readySpatial)
                            throw new Exception("Invalid Input Pattern Type");

                        List<Position2D> firingPositions = inputPattern.GetActivePositions;

                        foreach(var col in firingPositions)
                        {                            
                            instance.ProcessColumn(col, inputPattern.iType);
                        }
                        _readySpatial = false;
                        _readyApical = true;
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        //Fetch , Fire , Update
                        if (!_readyTemporal)
                            throw new Exception("Invalid Input Pattern Type");

                        List<Position2D> firingPositions = inputPattern.GetActivePositions;

                        foreach (var col in firingPositions)
                        {
                            instance.ProcessColumn(col, inputPattern.iType);
                        }
                        _readySpatial = true;
                        _readyTemporal = false;
                        break;
                    }                    
                case InputPatternType.APICAL:
                    {
                        //Fetch , Fire , Update

                        if (!_readyApical)
                            throw new Exception("Invalid Input Pattern Type");

                        List<Position2D> firingPositions = inputPattern.GetActivePositions;

                        foreach (var col in firingPositions)
                        {
                            instance.ProcessColumn(col, inputPattern.iType);
                        }
                        _readyApical = false;
                        _readyTemporal = true;
                        break;
                    }
            }            
        }        


        
        public SDR Predict()
        {
            SDR toReturn = null;

            foreach(var columnArray in Columns)
            {
                foreach(var column in columnArray)
                {
                    //toReturn += column.GetFiringCellRepresentation();
                }
            }

            return toReturn;
        }        
        
        private void Grow()
        {   //ToDo
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
                        List<Neuron> temporalColumn = GetTemporalColumn(pos);

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

        #region HELPER METHODS 

        private Column GetSpatialColumn(Position2D position) => Columns[position.X][position.Y];

        private Neuron GetNeuronFromPosition(uint x, uint y, uint z) => Columns[x][y].GetNeuron(z);

        private Neuron GetNeuronFromPosition(Position3D pos3d) => Columns[pos3d.X][pos3d.Y].GetNeuron(pos3d.Z);

        private List<Neuron> GetTemporalColumn(Position2D position2D)
        {
            List<Neuron> toReturn = new List<Neuron>();

            for(uint i=0; i<instance.Columns; i++)
            {
                toReturn.Add(GetNeuronFromPosition(position2D.X, i, position2D.Y));
            }

            return toReturn;
        }

        #endregion
    }
}
    