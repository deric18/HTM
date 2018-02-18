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
        private NetworkState _state;
        private Column[][] _columns;
        
        private List<Position3D> _emptyPoints;
        private List<Position3D> _connectedPoints;
        private Dictionary<Position4D, Position4D> _pos4dToSegmentMapper;

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
            _state = NetworkState.RESTING;
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
        
        private List<Neuron> Predict(SDR biasingPattern, InputPatternType pType)
        {
            switch(pType)
            {
                case InputPatternType.TEMPORAL:
                    {
                        foreach (var pos2d in biasingPattern.GetActivePositions())
                        {
                            List<Position4D> distributionPoints;
                            if(_temporalAxonLines.TryGetValue(pos2d, out distributionPoints))
                            {
                                List<Neuron> precitedNeurons = Fire(distributionPoints);
                                if (precitedNeurons != null)
                                {                                    
                                    return precitedNeurons;
                                }
                            }
                        }
                    }
                    break;
                case InputPatternType.APICAL:
                    {
                        break;
                    }
            }

            return null;
        }

        private List<Neuron> Fire(List<Position4D> distributionPoints)
        {
            List<Neuron> toReturn = new List<Neuron>();
            foreach(var pos4d in distributionPoints)
            {
                Position4D segID;
                if(_pos4dToSegmentMapper.TryGetValue(pos4d, out segID))
                {
                    if(GetColumn(segID.w, segID.x).GetNeuron(pos4d.y).GetSegment(pos4d.z).Fire(int.Parse(ConfigurationManager.AppSettings["TEMPORAL_BIASING_VOLTAGE"]), pos4d))
                    {
                        toReturn.Add(GetColumn(segID.w, segID.x).GetNeuron(segID.y));
                    }
                }
            }
            return toReturn;
        }


        #region HELPER METHODS 

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
