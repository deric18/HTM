using System;
using HTM.Models;
using System.Collections.Generic;
using HTM.Enums;
using System.Configuration;

namespace HTM
{
    public class CPM
    {        
        private int _length;
        private int _breadth;
        private int _width;
        private CPMState _state;
        private Column[][] _columns;
                    
        private Dictionary<Position4D, Position4D> segmentMapper;               //Maps Synapses to connected Segment.

        private List<Position2D> _temporalInput;
        private List<Position2D> _spatialInput;
        
        private List<Position4D> _predictedList;
        
        private bool hasTemporalSignal;
        private bool hasSpatialSignal;
        private static Dictionary<Position2D, List<Position4D>> _temporalAxonLines;
        private static Dictionary<Position2D, List<Position4D>> _apicalAxonLines;

        private CPM(int length, int breadth, int width)
        {
            _length = length;
            _breadth = breadth;
            _width = width;
            _state = CPMState.RESTING;
            hasSpatialSignal = false;
            hasTemporalSignal = false;

            try
            {
                for (int i = 0; i < length; i++)
                    for (int j = 0; j < breadth; j++)
                    {
                        Column toAdd = new Column(i, j, width);
                        _columns[i][j] = toAdd;
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
                        _state = CPMState.SPATIAL_FIRED;                        
                        break;
                    }
                case InputPatternType.TEMPORAL:
                    {
                        foreach (var pos2d in inputPattern.GetActivePositions())
                        {
                            List<Position4D> distributionPoints;
                            if (_temporalAxonLines.TryGetValue(pos2d, out distributionPoints))
                            {
                                ProcessPositionList(distributionPoints, uint.Parse(ConfigurationManager.AppSettings["TEMPORAL_BIASING_VOLTAGE"]));                                
                            }
                        }
                        _state = CPMState.TEMPROAL_PREDICTED;
                        break;
                    }                    
                case InputPatternType.APICAL:
                    {
                        foreach (var pos2d in inputPattern.GetActivePositions())
                        {
                            List<Position4D> distributionPoints;
                            if (_apicalAxonLines.TryGetValue(pos2d, out distributionPoints))
                            {
                                ProcessPositionList(distributionPoints, uint.Parse(ConfigurationManager.AppSettings["APICAL_BIASING_VOLTAGE"]));                                
                            }
                        }
                        _state = CPMState.APICAL_PREDICTED;
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
                    Segment predictedSegment = GetColumn(segmentID.w, segmentID.x).GetNeuron(pos4d.y).GetSegment(pos4d.z);
                    if (predictedSegment.Process(voltage, pos4d))
                    {
                        Neuron n = GetColumn(segmentID.w, segmentID.x).GetNeuron(segmentID.y);
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
            for(int i = 0; i < _width; i++)
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

        private static uint RandomNumberGenerator(int limit)
        {
            Random rand = new Random();
            return (uint)(rand.Next(1, limit));
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
            GetColumn(pos4d.x, pos4d.y).GetNeuron(pos4d.z).GetSegment(pos4d.z);                    

        private Column GetColumn(int x, int y) =>        
            _columns[x][y];        

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
