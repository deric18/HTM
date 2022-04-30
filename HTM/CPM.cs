//Grow
using System;
using HTM.Models;
using HTM.Enums;
using System.Collections.Generic;
using System.Linq;
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
        private List<Neuron> _firingNeurons { get; set; }
        private List<KeyValuePair<Segment, Neuron>> _predictedList;        
        //private List<Neuron> _shortPredictedList;
        private bool _readySpatial;
        private bool _readyTemporal;
        private bool _readyApical;
        public ConnectionTable CTable { get; private set; }
        internal SynapseGenerator synapseGenerator;
        private ulong cycle;
        private SDR nextPattern;

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

            instance._predictedList = new List<KeyValuePair<Segment, Neuron>>();
            instance._firingNeurons = new List<Neuron>();
            //instance._shortPredictedList = new List<Neuron>();
            instance._readyApical = false;
            instance._readyTemporal = false;
            instance._readySpatial = true;
            instance.cycle = 0;
            instance.nextPattern = null;

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
            instance.CTable = ConnectionTable.Singleton(NumBlocks, instance.BCP);
            synapseGenerator = SynapseGenerator.GetInstance;

            //Setup all the proximal Segments of all the neurons
            //Setup and Register all the Spatial vertical Axon Lines
            //Setup all the Temproal Horizontal Axon Lines.

        }
        
        internal Neuron GetNeuronFromPositionID(Position3D pos) => Columns[pos.X][pos.Y].GetNeuron(pos.Z);
        internal Neuron GetNeuronFromSegmentID(SegmentID segId) => Columns[segId.X][segId.Y].GetNeuron(segId.Z);

        /// <summary>
        /// All the Firing modules update the predicted list , changing the current state of the system.
        /// </summary>
        /// <param name="firstPattern"></param>
        /// <param name="iType"></param>
        public void Process(SDR firstPattern, SDR secondPattern)
        {
            instance.nextPattern = secondPattern;
            switch(firstPattern.IType)
            {
                case InputPatternType.SPATIAL:
                    {
                        //Fetch the columns to fire and decide if to burst the whole column or fire specific neurons
                        //Fire the neurons and update predicted list
                        if (!_readySpatial)
                            throw new Exception("Invalid Input Pattern Type");

                        List<Position2D> firingPositions = firstPattern.ActiveBits;

                        foreach(var pos in firingPositions)
                        {                            
                            instance.ColumnFire(Convert.ToUInt32(pos.X), Convert.ToUInt32(pos.Y), firstPattern.IType);
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

                        List<Position2D> firingPositions = firstPattern.ActiveBits;

                        foreach (var pos in firingPositions)
                        {
                            instance.ColumnFire(pos.X, pos.Y, firstPattern.IType);
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

                        List<Position2D> firingPositions = firstPattern.ActiveBits;

                        foreach (var pos in firingPositions)
                        {
                            instance.ColumnFire(pos.X, pos.Y, firstPattern.IType);
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

                List<Neuron> toFire = instance.Columns[X][Y].GetMaxVoltageNeuronInColumn();

                toFire.ForEach(neuron => neuron.Fire());
            }            

        }

        internal void AddtoPredictedList(Position3D position, SegmentID segmentID, uint potential)
        {
            bool willFire = GetNeuronFromPosition(segmentID.NeuronId).Process(segmentID.BasePosition, segmentID, potential);

            if(willFire)
            {
                _predictedList.Add(new KeyValuePair<Segment, Neuron>(GetSegmentFromSegmentID(segmentID), GetNeuronFromSegmentID(segmentID)));
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


        /// <summary>
        /// 1.Neurons that are predicted in this cycle , all the neurons that contributed to the last cycle for these neurons to be predicted should be incremented.
        /// 2.System should be advanced enough to recognise any neurons that usually dont fire and are firing in this iteration , should be analysed.
        /// </summary>
        //private void Grow()
        //{
        //    //ToDo
        //    //Once the Firing Cycle has finished
        //    //Call Connection Tables & Get aLl the DoubleSegment Objects
        //    //Strengthen all the connections using the DoubleSegments.
        //    //var predictedSegments = instance.CTable.GetAllPredictedSegments();

        //    //for each item in predictedsegments contains a double segment and number of hits the segment has received
        //    //first order of business , get only the segments which belong to neurons which are going to fire this cycle and strengthen only those segments that have conrtibited
        //    //to the neuronal segments
        //    //Also if there too high of a count on one of the segments detect

        //    foreach (var item in GetIntersectionSet())
        //    {
        //        item.Key.Grow(item.Value, instance.CTable.InterfaceFire()

        //    }


        //    //Flush the predicted segment
        //    CPM.GetInstance.CTable.FlushPredictedSegments();

        //}


        /// <summary>
        /// Takes in an SDR and a keyvaluepair enumerable set and spits out list of neurons
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<Neuron, Segment>> GetIntersectionSet()
        {
            List<Neuron> list1 = GetPredictedNeuronsFromSDR(nextPattern);

            List<KeyValuePair<Neuron, Segment>> toRet = new List<KeyValuePair<Neuron, Segment>>();

            if (_predictedList.Count == 0)
            {
                Console.WriteLine("There are no predicted Neurons nor segments !! , THis is never supposed to happen!!!");
                throw new Exception("There are no predicted Neurons nor segments !! , THis is never supposed to happen!!!");
            }

           foreach(var kvp in _predictedList)
            {
                foreach(var neuron in list1)
                {
                    if(kvp.Value.Equals(neuron))
                    {
                        toRet.Add(new KeyValuePair<Neuron, Segment>(neuron, kvp.Key));
                    }
                }
            }

            return toRet;
        }

        private List<Neuron> GetPredictedNeuronsFromSDR(SDR pattern)
        {
            List<Neuron> toRet = new List<Neuron>();

            foreach(var item in pattern.ActiveBits)
            {
                toRet.AddRange(instance.Columns[item.X][item.Y].GetMaxVoltageNeuronInColumn());
            }

            return toRet;
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

        public Segment GetSegmentFromSegmentID(SegmentID segId) =>
            GetNeuronFromPositionID(segId.NeuronId).GetSegment(segId);

        #region HELPER METHODS

        //TODO:
        //private void SetupAllProximalNeuronalSegments()
        //{
        //    for(int i=0; i<NumX; i++)
        //    {
        //        for(int j=0; j<NumY; j++)
        //        {
        //            Columns[i][j].Initialize();
        //        }
        //    }
        //}


        //TODO:
        private void RegisterAxonLines()
        {

            //REgister Vertical Lines

            //Register Temporal Lines
        }

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
    