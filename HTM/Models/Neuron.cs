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
        private uint _voltage;
        private uint _totalSegments;
        public Position3D NeuronID { get; private set; }
        public NeuronState State { get; private set; }
        public Dictionary<Guid, Segment> proximalSegments { get; private set; }      
        private List<SegmentID> _predictedSegments;                
        public Dictionary<Position3D, SegmentID> axonEndPoints { get; private set; } 
        private const uint NEURONAL_FIRE_VOLTAGE = 10;
        private CPM _cpm;

        public Neuron(Position3D pos)
        {
            _voltage = 0;
            _totalSegments = 0;
            NeuronID = pos;
            State = NeuronState.RESTING;
            proximalSegments = new Dictionary<Guid, Segment>();
            _predictedSegments = new List<SegmentID>();
            axonEndPoints = new Dictionary<Position3D, SegmentID>();
            _cpm = CPM.GetInstance;
        }

        internal Segment GetSegment(SegmentID segID)
        {
            Segment seg;
            if (proximalSegments.TryGetValue(segID.Guid, out seg))
                return seg;

            throw new InvalidOperationException("Invalid Segment ID Access");
        }        

        /// <summary>
        /// Process Potential to segment and decide if you are gonna fire or not so CPM can add you to the prediction list.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="segmentID"></param>
        /// <param name="potential"></param>
        /// <returns></returns>
        internal bool Process(Position3D position, SegmentID segmentID, uint potential)
        {
            //Get the segment , supply the potential and see if it decides to fire (NMDA) or depolarise.
            //if NMDA add segment to predicted list and return response based on firing limits also after firing remember to flush the voltage.
            //if deplorised good , keep the potential.
            Segment seg = GetSegment(segmentID);
            if(seg.Process(potential, position, InputPatternType.INTERNAL))     //if NMDA
            {
                _voltage += seg._sumVoltage;
                seg.FlushVoltage();
                _predictedSegments.Add(seg.SegmentId);
                return true;
            }
            return false;
        }

        internal void Fire()
        {
            foreach(var kvp in axonEndPoints)
            {

            }
        }

        public string GetString() => NeuronID.GetString();

        private void FlushVoltage() =>        
            _voltage = 0;        

        internal void AddNewConnection(Position3D pos, SegmentID segmentID)
        {
            SegmentID segid;
            if(!axonEndPoints.TryGetValue(pos, out segid))
            {
                axonEndPoints.Add(pos, segmentID);
            }
        }

        internal void Grow()
        {
            //check which segment is growing the fastest and getting modre NMDA spikes and supply a grow signal
        }

        internal void PRune()
        {
            //check which segment is most underpeforming and send inhibit signal to such segments.
        }
    } 
}
