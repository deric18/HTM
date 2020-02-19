using System;
using HTM.Models;
using HTM.Enums;
using System.Collections.Generic;
using HTM.Algorithms;

namespace HTM
{
    public class CPM
    {        
        private static volatile CPM instance;        

        private CPM() { }

        public static CPM GetInstance
        {
            get
            {
                if (instance == null)
                {                
                        if (instance == null)
                            instance = new CPM();                       
                }

                return instance;
            }
        }

        public uint NumX { get; private set; }
        public uint NumY{ get; private set; }        
        public uint NumZ { get; private set; }
        public uint NumBLocks { get; private set; }
        public CPMState State { get; private set; }
        public BlockConfigProvider BCP { get; private set; }
        public Column[][] Columns { get; private set; }
        private Dictionary<string, Neuron> _longPredictedList;        //why 2 lists ? whats the diff ???
        //private List<Neuron> _shortPredictedList;
        private bool _readySpatial;
        private bool _readyTemporal;
        private bool _readyApical;
        private ConnectionTable cTable;

        public void Initialize(uint x, uint y, uint z)
        {
            //Notes ToDo : BASED ON NUMBER OF CLOUMNS , ROWS AND FILES TO BE CREATED , CREATE THAT MANY BLOCKS IN ORDER WITH X-Y & THEN Z CO-ORDINATE SYSTEM WITH SYNAPSE GENERATOR AND SYNAPSE TABLE AND INTERVAL
            /*LOAD ONE OF THE CONFIGURATIONS FROM NumColumnsPerBlock ,  once loaded intialise the system appropriately 
             * Initialie the synapse table 
             * Intialise interval , SynapseGenerator
             **/           
            instance.NumX = x;
            instance.NumY = y;
            instance.NumZ = z;
            instance.State = CPMState.RESTING;            
            instance._longPredictedList = new Dictionary<string, Neuron>();
            //instance._shortPredictedList = new List<Neuron>();
            instance._readyApical = false;
            instance._readyTemporal = true;
            instance._readySpatial = false;

            try
            {
                for (uint i = 0; i < x; i++)
                    for (uint j = 0; j < y; j++)
                    {
                        Column toAdd = new Column(i, j, z);
                        instance.Columns[i][j] = toAdd;
                    }
            }
            catch(Exception e)
            {
                Console.WriteLine("Out Of Memory Allocated for the Service via Operating System! , Please reduce the dimensions of the Neuroblock \n NumRows : " + NumY + "\n NumColumns : " + NumX + "\n NumFiles : " + NumZ);
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }

            instance.BCP = new BlockConfigProvider(100000);
            instance.NumBLocks = 30;

            cTable = ConnectionTable.Singleton(NumBLocks, instance.BCP);
        }
        
        internal Neuron GetNeuronFromPositionID(Position3D pos) => Columns[pos.X][pos.Y].GetNeuron(pos.Z);
        internal Neuron GetNeuronFromSegmentID(SegmentID segId) => Columns[segId.X][segId.Y].GetNeuron(segId.Z);
          
        /// <summary>
        /// All the Firing modules update the predicted list , changing the current state of the system.
        /// </summary>
        /// <param name="inputPattern"></param>
        /// <param name="iType"></param>
        public void Process(SDR inputPattern)
        {
            switch(inputPattern.IType)
            {
                case InputPatternType.SPATIAL:
                    {
                        //Fetch the columns to fire and decide if to burst the whole column or fire specific neurons
                        //Fire the neurons and update predicted list
                        if (!_readySpatial)
                            throw new Exception("Invalid Input Pattern Type");

                        List<Position2D> firingPositions = inputPattern.ActiveBits;

                        foreach(var pos in firingPositions)
                        {                            
                            instance.ProcessColumn(pos.X, pos.Y, inputPattern.IType);
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

                        List<Position2D> firingPositions = inputPattern.ActiveBits;

                        foreach (var pos in firingPositions)
                        {
                            instance.ProcessColumn(pos.X, pos.Y, inputPattern.IType);
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

                        List<Position2D> firingPositions = inputPattern.ActiveBits;

                        foreach (var pos in firingPositions)
                        {
                            instance.ProcessColumn(pos.X, pos.Y, inputPattern.IType);
                        }
                        _readyApical = false;
                        _readyTemporal = true;
                        break;
                    }
            }            
        }        


        internal void Fire()
        {
            //get neurons from the predicted list and depending on there voltages & spatial columnar inhibition decide which ones should fire and which one should inhibit 
            //then get all the axonal endpoints of the firing neurons and using connection table get the connected segment IDs and supply voltage to them based on the firing mode.


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
        
         
        private void ProcessColumn(uint X, uint Y, InputPatternType iType)
        {
            //check if pos in predicted neuron list if so return else do the full charade.
            //check for predicted cells in the column and decide whther to burst or not
            //pick cells and fire
            //return List of positions 

            List<Neuron> predictedCells = instance.Columns[X][Y].GetPredictedCells();
            if(predictedCells.Count == 0 )
            {
                //Burst 
                instance.Columns[X][Y].Fire();
            }            
            else if(predictedCells.Count ==  1)
            {
                //fire                
                predictedCells[0].Fire();
            }
            else
            {
                //pick & fire

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

            for (uint i = 0; i < instance.Columns.Length; i++)
            {
                toReturn.Add(GetNeuronFromPosition(position2D.X, i, position2D.Y));
            }

            return toReturn;
        }

        #endregion
    }
}
    