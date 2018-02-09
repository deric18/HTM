using System.Configuration;
using System.Collections.Generic;

namespace HTM.Models
{
    /// <summary>
    /// Process, Grow
    /// </summary>
    public class Segment
    {
        public string NeuronID {  get; private set; }
        public string ID { get; private set; }
        private uint _sumVoltage;
        private Dictionary<Position3D, int> Connections;        
        private bool _hasSubSegments;
        private List<Segment> SubSegments;
        private uint _spikeCounter;

        public Segment(string neuronID, string segmentID, int id)
        {
            ID = segmentID + "-" + id.ToString();
            NeuronID = neuronID;
            _sumVoltage = 0;
            Connections = new Dictionary<Position3D, int>();
            _hasSubSegments = false;
            _spikeCounter = 0;
        }
        
        public bool Process(uint voltage, Position3D pos3d)
        {
            _sumVoltage += voltage;
            if(_sumVoltage > int.Parse(ConfigurationManager.AppSettings["NMDASPIKEPOTENTIAL"]))
            {
                //NMDA SPIKE
                //Update pos3d strength
                return true;
            }

            return false;
        }

        public void Grow()
        {
            //Make sure you are not connecting to an axon of your own neuron
            //If already bracnhed and maxed out on MAXBRANCH , dont branch and add new position to the highest spiking branch
            //If not branched and check if segment has max positions else create new branch 
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.


        }

        private void CreateNewBranch()
        {
            if (!_hasSubSegments)
            {
                _hasSubSegments = true;
                SubSegments = new List<Segment>();
            }
            
            Segment newSegment = new Segment(NeuronID, ID, SubSegments.Count);
            SubSegments.Add(newSegment);
            
        }

    }
}
