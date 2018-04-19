using HTM.Enums;
using System.Collections.Generic;
using System;

namespace HTM.Models
{
    /// <summary>
    /// 1.Account for both excitatory and inhibitory neurons
    /// </summary>
    public class Neuron
    {
        public Position3D NeuronID { get; private set; }
        public NeuronState State { get; private set; }
        public Dictionary<int, Segment> Segments { get; private set; }      //index of each connection
        private List<Segment> _predictedSegments;        
        private List<Position3D> AxonList;
        private const uint NEURONAL_FIRE_VOLTAGE = 10;

        public Segment GetSegment(string segID)
        {
            Segment seg;
            if(segID.Contains("-"))
            {
                var items = segID.Split('-');
                int baseSeg = int.Parse(items[0]);
                if(Segments.TryGetValue(baseSeg, out seg))
                {                    
                    for(int i=1; i < items.Length; i++)
                    {
                        seg = seg.GetSegment(int.Parse(items[i]));
                    }
                    return seg;
                }             
            }
            throw new InvalidOperationException("seg ID : " + segID.ToString() + " is not present");
        }        

        internal List<Position3D> Fire()
        { 
            //return all the connected segment ids
            return AxonList;
        }               

        internal bool Process(string segID, Position3D firingNeuron, InputPatternType iType)
        {
            Segment s = GetSegment(segID);
            if(s.Process(NEURONAL_FIRE_VOLTAGE, firingNeuron, iType))
            {
                _predictedSegments.Add(s);
                s.FlushVoltage();                
                return true;
            }
            return false;
        }

        internal void PruneConnections()
        {
            foreach(var s in Segments.Values)
            {
                s.Prune();
            }
        }

        internal void Update()
        {        
            //Update Local
            //Send GROW signal to all connected segments
            //Prune Segments that have not fired for the time interval
        }                       

        internal void UpdateLocal()
        {
            //Update the strengths for all fired segments on previous timestamp            
        }
    }
}
