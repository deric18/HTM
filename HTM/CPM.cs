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
                    
        private Dictionary<Position4D, Position4D> segmentMapper;               //Maps Synapses to connected Segment.        
        
        private List<Position4D> _predictedList;
        
        public bool HasTemporalSignal { get; private set; }
        public bool HasSpatialSignal { get; private set; }
        public Dictionary<Position2D, List<Position4D>> TemporalAxonLines { get; private set; }
        public Dictionary<Position2D, List<Position4D>> ApicalAxonLines { get; private set; }

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

        public SDR Predict()
        {
            //Get current predicted list
            //Generate SDR with this as firing pattern and return it..
            SDR toReturn = new SDR();

            return toReturn;
        }
        
        public void Process(SDR inputPattern, InputPatternType pType)
        {
            switch(pType)
            {
                case InputPatternType.SPATIAL:
                    {
                        List<Position4D> firedSynapsePoints = new List<Position4D>();
                        foreach (var pos2d in inputPattern.GetActivePositions())
                        {
                           ProcessColumn(pos2d);
                        }                        
                        instance.State = CPMState.SPATIAL_FIRED;                        
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        foreach (var pos2d in inputPattern.GetActivePositions())
                        {
                            List<Position4D> distributionPoints;
                            if (instance.TemporalAxonLines.TryGetValue(pos2d, out distributionPoints))
                            {
                                ProcessPositionList(distributionPoints, uint.Parse(ConfigurationManager.AppSettings["TEMPORAL_BIASING_VOLTAGE"]));                                
                            }
                        }
                        instance.State = CPMState.TEMPROAL_PREDICTED;
                        break;
                    }                    
                case InputPatternType.APICAL:
                    {
                        foreach (var pos2d in inputPattern.GetActivePositions())
                        {
                            List<Position4D> distributionPoints;
                            if (instance.ApicalAxonLines.TryGetValue(pos2d, out distributionPoints))
                            {
                                ProcessPositionList(distributionPoints, uint.Parse(ConfigurationManager.AppSettings["APICAL_BIASING_VOLTAGE"]));                                
                            }
                        }
                        instance.State = CPMState.APICAL_PREDICTED;
                        break;
                    }
            }            
        }
        

        private void ProcessPositionList(List<Position4D> firedSynapses, uint voltage)
        {            
            foreach(var pos4d in firedSynapses)
            {
                Position4D segmentID;
                if(segmentMapper.TryGetValue(pos4d, out segmentID))
                {
                    Segment predictedSegment = GetColumn(segmentID.W, segmentID.X).GetNeuron((int)pos4d.Y).GetSegment(pos4d.Z);
                    if (predictedSegment.Process(voltage, pos4d))
                    {
                        Neuron n = GetColumn(segmentID.W, segmentID.X).GetNeuron((int)segmentID.Y);
                        n.ChangeStateToPredicted();                       
                        _predictedList.Add(predictedSegment.SegmentID);
                        n.Grow(pos4d);                                                                         //Send Grow Signal
                    }
                }
            }            
        }

        private void ProcessColumn(Position2D corticalColumn)
        {
            //check for predicted cells in the column
            //pick cells and fire
            //return List of positions 
            List<Position4D> toFire = new List<Position4D>();
            for(int i = 0; i < instance.Width; i++)
            {
                if(GetColumn(corticalColumn.X, corticalColumn.Y).GetNeuron(i).GetState() == NeuronState.PREDICTED)
                {
                    toFire.AddRange(GetColumn(corticalColumn.X, corticalColumn.Y).GetNeuron(i).Fire());
                }
            }

            if(toFire.Count > 0)
            {
                ProcessPositionList(toFire, uint.Parse(ConfigurationManager.AppSettings["SPATIAL_FIRING_VOLTAGE"]));
            }            
        }

        #region HELPER METHODS 
        public static Position4D GetNextPositionForSegment()
        {
            return CPM.GetNextRandomPosition(instance.Length, instance.Width, instance.Width, CubeConstant);
        }

        private static Position4D GetNextRandomPosition(int limitW, int limitX,int limitY,int limitZ)
        {
            Random rand = new Random();
            return new Position4D((uint)rand.Next(limitW), (uint)rand.Next(limitX), (uint)rand.Next(limitY), (uint)rand.Next(limitZ));
        }

        private bool CheckIfPositionIsConnected(Position4D connectionPoint)
        {
            Position4D SegmentID = null;
            if(segmentMapper.TryGetValue(connectionPoint,out SegmentID))
            {
                return true;
            }
            return false;
        }

        private Segment GetSegmentFromPosition(Position4D pos4d) =>        
            GetColumn(pos4d.W, pos4d.X).GetNeuron((int)pos4d.Y).GetSegment(pos4d.Z);                    

        private Column GetColumn(uint x, uint y) =>        
            instance.Columns[x][y];        

        public void UpdateSegmentMapper(string segID, Position4D pos3d)
        {
            throw new NotImplementedException();
        }

        public static bool CheckForSelfConnection(Position4D pos3d, Position2D neuronID)
        {
            return false;
        }

        internal static void UpdateConnectionGraph(Position4D pos3d)
        {
            //needed to make sure to see which connection points are empty and which ones are already taken 
            throw new NotImplementedException();
        }

        #endregion
    }
}
