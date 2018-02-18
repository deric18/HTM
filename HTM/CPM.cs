using System;
using HTM.Models;
using System.Collections.Generic;
using HTM.Enums;

namespace HTM
{
    public class CPM
    {        
        private int _length;
        private int _breadth;
        private int _width;
        private NetworkState _state;
        private List<Column> _columns;
        
        private List<Position3D> _emptyPoints;
        private List<Position3D> _connectedPoints;
        private Dictionary<Position3D, string> _pos3dToSegmentMapper;

        private List<Position2D> _temporalInput;
        private List<Position2D> _spatialInput;

        
        private List<Neuron> _predictedList;
        
        private bool hasTemporalSignal;
        private bool hasSpatialSignal;
        private static Dictionary<Position2D, List<Position3D>> _temporalAxonLines;
        private static Dictionary<Position2D, List<Position3D>> _apicalAxonLines;

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
                for (int i = 0; i < breadth; i++)
                    for (int j = 0; j < width; j++)
                    {
                        Column toAdd = new Column(length);
                        _columns.Add(toAdd);
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

        private static List<Neuron> Predict(SDR biasingPattern, InputPatternType pType)
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
                                if(Fire(distributionPoints) != null)
                                {
                                    List<Neuron> toRetun = new List<Neuron>();

                                    return toRetun;
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

        private static List<Neuron> Fire(List<Position4D> distributionPoints)
        {
            foreach(var pos3d in distributionPoints)
            {
                Segment seg = GetSegment(dist)
            }
        }



        #region HELPER METHODS 

        private Segment GetSegmentFromPosition(Position4D pos4d)
        {

        }

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
