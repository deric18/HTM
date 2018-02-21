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
                    
        private Dictionary<Position4D, Position4D> _pos4dToSegmentMapper;               //Maps Synapses to connected Segment.

        private List<Position2D> _temporalInput;
        private List<Position2D> _spatialInput;
        
        private List<Neuron> _predictedList;
        
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
                Console.WriteLine("Out Of Memory! , Please reduce the dimensions of the Neuroblock Length : " + length + " Breadth : " + breadth + " Width : " + width);
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }
        }

        public SDR Predict()
        {
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
                                ProcessPositionList(distributionPoints, int.Parse(ConfigurationManager.AppSettings["TEMPORAL_BIASING_VOLTAGE"]));                                
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
                                ProcessPositionList(distributionPoints, int.Parse(ConfigurationManager.AppSettings["APICAL_BIASING_VOLTAGE"]));                                
                            }
                        }
                        _state = CPMState.APICAL_PREDICTED;
                        break;
                    }
            }            
        }
        

        private void ProcessPositionList(List<Position4D> firedSynapses, int voltage)
        {            
            foreach(var pos4d in firedSynapses)
            {
                Position4D segmentID;
                if(_pos4dToSegmentMapper.TryGetValue(pos4d, out segmentID))
                {
                    Segment predictedSegment = GetColumn(segmentID.w, segmentID.x).GetNeuron(pos4d.y).GetSegment(pos4d.z);
                    if (predictedSegment.Fire(voltage, pos4d))
                    {
                        Neuron n = GetColumn(segmentID.w, segmentID.x).GetNeuron(segmentID.y);
                        n.ChangeStateToPredicted();
                        _predictedList.Add(n);                        
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
                ProcessPositionList(toFire, int.Parse(ConfigurationManager.AppSettings["SPATIAL_FIRING_VOLTAGE"]));
            }            
        }

        #region HELPER METHODS 

        private bool CheckIfPositionIsConnected(Position4D connectionPoint)
        {
            Position4D SegmentID = null;
            if(_pos4dToSegmentMapper.TryGetValue(connectionPoint,out SegmentID))
            {
                return true;
            }
            return false;
        }

        private Segment GetSegmentFromPosition(Position4D pos4d) =>        
            GetColumn(pos4d.x, pos4d.y).GetNeuron(pos4d.z).GetSegment(pos4d.z);                    

        private Column GetColumn(int x, int y) =>        
            _columns[x][y];        

        public void RegisterSegment(string segID, Position4D pos3d)
        {
            return;
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
