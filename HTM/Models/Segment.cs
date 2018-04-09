using System.Configuration;
using System.Collections.Generic;
using System;

namespace HTM.Models
{
    /// <summary>
    /// Process, Grow
    /// </summary>
    public class Segment
    {        
        public Position3D NeuronID { get; private set; }
        public Position3D BaseConnection { get; private set; }
        private uint _sumVoltage;
        public string SegmentID { get; private set; }
        private bool _hasSubSegments;        
        private bool _fullyConnected;
        private int _seed;        
        
        private List<Segment> SubSegments;
        private Dictionary<Position3D, int> _synapticConnections;    //strength
        private List<Position3D> __lastTimeStampFiringConnections;        

        public Segment(Position3D neuronId, string segmentID, Position3D baseConnection, int seed)
        {
            NeuronID = neuronId;
            SegmentID = segmentID;            
            _sumVoltage = 0;                  
            _hasSubSegments = false;
            _fullyConnected = false;
            _synapticConnections = new Dictionary<Position3D, int>();
            __lastTimeStampFiringConnections = new List<Position3D>();
            BaseConnection = baseConnection;
            _seed = seed;
        }
        
        internal Segment GetSegment(int v)
        {
            if(v < SubSegments.Count)
            {
                return SubSegments[v];
            }

            throw new InvalidOperationException("seg ID : " + v.ToString() + " is not present");
        }

        /// <summary>
        /// Predict if the segment will fire or not based incoming voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="firingNeuronId"></param>
        /// <returns></returns>
        public bool Predict(uint voltage, Position3D firingNeuronId)
        {            
            _sumVoltage += voltage;            
            if(_sumVoltage > int.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]))
            {
                //NMDA Spike
                //Strengthen firing Neuron Connection.
                int connectionStrength;
                if(_synapticConnections.TryGetValue(firingNeuronId, out connectionStrength))
                {
                    if (connectionStrength > int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTION_STRENGTH"]))
                        return true;
                    _synapticConnections[firingNeuronId]++;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// -Method gets called at every growth cycle        
        /// -GrowSubSegments            : Updates all subsegments based on merit
        /// -InternalGrowth             : Decides if a new connection could be added , adds it if it can otherwise grows an existing nicely firing connection.
        /// </summary>        
        public void Grow()
        {
            AddNewLocalConnection();
            GrowSubSegments();                        
        }       

        /// <summary>
        /// Boxed Growth : Direction and boxed random connection
        /// 1.A Segment should have direction it grows and by default will have limits set to how far away the segment can predict its new connection.
        /// </summary>
        private void AddNewLocalConnection()
        {
            ///Should decide on its own to whether to add a new connection or not ?
            //Make sure you are not connecting to an axon of your own neuron if its a new position
            /*Decide how to add new connection : Check if there is any sub segments performing really well , if yes send grow signal to it
             * else check for any local connections to the segment which is performing really well 
             * else grow a new connection locally randomly.
            */
            //If not branched and position is noand check if segment has max positions , add this position to a possibility list for future connection when the neuron losses and non used connection else if not max position then add new position,
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.  
            Position3D bounds = CPM.GetBound();
            Position3D newPosition = GetNewPositionFromBound(bounds);            

            if ((_synapticConnections.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"])) && !DoesConnectionExist(newPosition) && !SelfConnection(newPosition))
            {
                AddConnection(newPosition);
            }
            else if(SubSegments?.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]) && !DoesSubSegmentExist(newPosition))
            {                
                CreateSubSegment(newPosition);
            }
            else
            {
                Console.WriteLine("Segment " + PrintPosition(NeuronID) + SegmentID + " is Over Connected");
                Console.ReadKey();
                _fullyConnected = true;
                //log Information with details , Segment has reached a peak connection pathway , this is essentially a crucial segment for the whole region.
            }            
        }

        private void GrowSubSegments()
        {
            if(_hasSubSegments)
            {
                foreach (var segment in SubSegments)
                {
                    segment.Grow();
                }
            }                    
        }

        public void Prune()
        {
            //Run through synaptic list to eliminate neurons with lowest connection strength
            foreach(var s in _synapticConnections)
            {
                if(s.Value <= int.Parse(ConfigurationManager.AppSettings["PRUNE_THRESHOLD"]))
                {
                    Console.WriteLine("Removing synapse to Neuron" + PrintPosition(s.Key));
                    _synapticConnections.Remove(s.Key);
                }
            }            

            foreach(var segment in SubSegments)
            {
                segment.Prune();
            }
        }

        private bool SelfConnection(Position3D newPosition)
        {
            if (NeuronID.Equals(newPosition))
                return true;

            return false;
        }

        private Position3D GetNewPositionFromBound(Position3D segmentBound)
        {
            Random r = new Random(_seed);
            return new Position3D(r.Next(BaseConnection.X, segmentBound.X), r.Next(BaseConnection.Y, segmentBound.Y), r.Next(BaseConnection.Z, segmentBound.Z));
        }                   

        private void AddConnection(Position3D newPosition) =>
            _synapticConnections.Add(newPosition, int.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]));                            

        private void CreateSubSegment(Position3D basePosition)
        {
            if (!_hasSubSegments)
            {
                _hasSubSegments = true;
                SubSegments = new List<Segment>();
            }
            
            string newSegId = SegmentID + "-" + SubSegments.Count.ToString();
            Segment newSegment = new Segment(NeuronID, newSegId, basePosition, SubSegments.Count);
            SubSegments.Add(newSegment);            
        }

        internal void FlushVoltage()
        {
            _sumVoltage = 0;
            __lastTimeStampFiringConnections = new List<Position3D>();
        }

        private string PrintPosition(Position3D pos3d)
        {
            return " X: " + pos3d.X.ToString() + " Y:" + pos3d.Y.ToString() + " Z:" + pos3d.Z.ToString();
        }

        private bool DoesSubSegmentExist(Position3D newPosition)
        {
            foreach(var seg in SubSegments)
            {
                if(seg.BaseConnection.Equals(newPosition))
                {
                    return false;
                }
            }

            return true;
        }

        private bool DoesConnectionExist(Position3D pos)
        {
            int val;
            if(_synapticConnections.TryGetValue(pos, out val))
            {
                return true;
            }
            return false;
        }
    }
}
