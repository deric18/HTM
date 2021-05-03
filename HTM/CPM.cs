﻿//Grow
using System;
using HTM.Models;
using HTM.Enums;
using System.Collections.Generic;
using HTM.Algorithms;

namespace HTM
{
    public class CPM
    {        
        private static CPM instance;        

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

        public uint NumX { get; private set; }      //Number of Neuron Rows in the region
        public uint NumY{ get; private set; }       //Number of Neuron Columns in the region
        public uint NumZ { get; private set; }      //Number of Neuron Files in the region
        public uint NumBlocks { get; private set; } //Number of Blocks in the Region        
        public BlockConfigProvider BCP { get; private set; }
        public Column[][] Columns { get; private set; }
        private List<Neuron> _predictedList;        
        //private List<Neuron> _shortPredictedList;
        private bool _readySpatial;
        private bool _readyTemporal;
        private bool _readyApical;        
        public ConnectionTable CTable { get; private set; }
        internal SynapseGenerator synapseGenerator;

        public void Initialize(uint xyz, uint pointsPerBlock = 0)
        {
            //Notes ToDo : BASED ON NUMBER OF CLOUMNS , ROWS AND FILES TO BE CREATED , CREATE THAT MANY BLOCKS IN ORDER WITH X-Y & THEN Z CO-ORDINATE SYSTEM WITH SYNAPSE GENERATOR AND SYNAPSE TABLE AND INTERVAL
            /*LOAD ONE OF THE CONFIGURATIONS FROM NumColumnsPerBlock ,  once loaded intialise the system appropriately 
             * Initialie the synapse table 
             * Intialise interval , SynapseGenerator
             **/           
            instance.NumX = xyz;
            instance.NumY = xyz;
            instance.NumZ = xyz;            

            instance._predictedList = new List<Neuron>();
            //instance._shortPredictedList = new List<Neuron>();
            instance._readyApical = false;
            instance._readyTemporal = false;
            instance._readySpatial = true;

            try
            {
                for (uint i = 0; i < xyz; i++)
                    for (uint j = 0; j < xyz; j++)
                    {
                        Column toAdd = new Column(i, j, xyz);
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

            instance.BCP = pointsPerBlock == 0 ? new BlockConfigProvider(100000) : new BlockConfigProvider(pointsPerBlock);
            instance.NumBlocks = x * y * z;

            instance.CTable = ConnectionTable.Singleton(NumBlocks, instance.BCP);
            synapseGenerator = SynapseGenerator.GetInstance;
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
                            instance.ColumnFire(Convert.ToUInt32(pos.X), Convert.ToUInt32(pos.Y), inputPattern.IType);
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
                            instance.ColumnFire(pos.X, pos.Y, inputPattern.IType);
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
                            instance.ColumnFire(pos.X, pos.Y, inputPattern.IType);
                        }
                        _readyApical = false;
                        _readyTemporal = true;
                        break;
                    }
            }            
        }


        //as per the name does a entire colmn fire if no predicted cells otherwise fires the predicted cells
        private void ColumnFire(uint X, uint Y, InputPatternType iType)
        {
            //check if pos is in predicted neuron list if so add potential and then return else do the full charade.
            //check for predicted cells in the column and decide whther to burst or not
            //pick cells and fire
            //return List of positions 

            List<Neuron> predictedCells = instance.Columns[X][Y].GetPredictedCells();
            if (predictedCells.Count == 0)
            {
                //Burst 
                instance.Columns[X][Y].Fire();
            }
            else if (predictedCells.Count == 1)
            {
                //fire                
                predictedCells[0].Fire();
            }
            else
            {
                //pick the cell with highest voltage & fire , inhibitting the others.
                Neuron toFire = instance.Columns[X][Y].GetMaxVoltageNeuronInColumn();
                toFire.Fire();
            }            

        }

        internal void NeuronFire(Position3D position, SegmentID segmentID, uint potential)
        {
            bool willFire = GetNeuronFromPosition(segmentID.NeuronId).Process(segmentID.BasePosition, segmentID, 10);

            if(willFire)
            {
                _predictedList.Add(GetNeuronFromPosition(segmentID.NeuronId));
            }            
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>output sdr should be 2D should only get 1 for columns that are firing and o if none of the cells in the column are firing</returns>
        public SDR Predict()
        {
            SDR toReturn = new SDR(NumX, NumY);

            for (int i=0; i<NumX; i++)
            {
                for(int j=0; j<NumY; j++)
                {
                    if (Columns[i][j].GetFiringCellPositions().Count > 0)
                        toReturn.ActiveBits.Add(Columns[i][j].ID);
                }
            }

            return toReturn;
        }        
        
        private void Grow()
        {   //ToDo
            //Give a GROW SIGNAL around the network 
            //Can always be tweaked and policies may be constructed for sending these signals based on how much a neuron/Segment has contributed.

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

        private Neuron GetNeuronFromSegmentID(string SegmentID)
        {
            string[] tokens = SegmentID?.Split('/');
            string[] neuronId = tokens[0]?.Split('-');
            if(neuronId.Length < 3)
            {
                throw new Exception("Corrupted stringId : Cannot extract NeuronId from SegmentID");
            }

            return GetNeuronFromPosition(Convert.ToUInt32(neuronId[0]), Convert.ToUInt32(neuronId[1]), Convert.ToUInt32(neuronId[2]));
        }

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
    