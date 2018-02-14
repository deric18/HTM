using HTM.Models;
using System;
using System.Collections.Generic;

namespace HTM
{
    public class CPM
    {        
        private int _length;
        private int _breadth;
        private int _width;
        private List<Column> _columns;
        private List<Position3D> _temporalInput;
        private Dictionary<Position3D, string> _posToSegment;
        private List<Neuron> _predictedList;
        private static Dictionary<string, string> _segmentMappings;

        private CPM(int length, int breadth, int width)
        {
            _length = length;
            _breadth = breadth;
            _width = width;
            _segmentMappings = new Dictionary<string, string>();

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

        public void RegisterSegment(string segID, Position3D pos3d)
        {

        }

        public static bool CheckForSelfConnection(Position3D pos3d, string neuronID)
        {
            return false;
        }

        internal static void UpdateConnectionGraph(Position3D pos3d)
        {
            throw new NotImplementedException();
        }
    }
}
