//TODO : CreateProximalSegment, AddSegment, Prune, Grow    
using HTM.Enums;
using System.Collections.Generic;
using System;

namespace HTM.Models
{
    /// <summary>
    /// 1.Account for both excitatory and inhibitory neurons . Need to rethink about this, one argument is that this is not neccessary because HTM Model already accounts for this by 
    /// picking single neurons from one column with highest values using selective firing techniques.
    /// </summary>
    public class Neuron
    {                
        internal uint Voltage { get; private set; }
        internal Position3D NeuronID { get; private set; }
        internal NeuronState State { get; private set; }
        private List<Position3D> _proximalSegmentList { get; set; }     //list of proximal segments with higher connectivity threshold
        private Dictionary<string, Segment> Segments { get; set; }      //List of all segments the neuron has
        private uint _totalSegments;                                    // total number of segments
        private List<SegmentID> _predictedSegments;                     
        public List<Position3D> axonEndPoints { get; private set; }
        public List<Position3D> dendriticEndPoints { get; private set; }
        private const uint NEURONAL_FIRE_VOLTAGE = 10;
        private CPM _cpm;

        public Neuron(Position3D pos)
        {
            Voltage = 0;
            _totalSegments = 0;
            NeuronID = pos;
            State = NeuronState.RESTING;
            Segments = new Dictionary<string, Segment>();
            _predictedSegments = new List<SegmentID>();
            axonEndPoints = new List<Position3D>();
            dendriticEndPoints = new List<Position3D>();
            _cpm = CPM.GetInstance;
        }        

        public void CreateProximalSegments()
        {
            //create 6 baseposition points on each side of the face of the neuron close to its nearby neurons ( atelast half way )
            //Create Segments with bae positions as the the once you recieve from synapseGenerator
            List<Position3D> proximalSegList;
            proximalSegList = (CPM.GetInstance.synapseGenerator.AddProximalSegment(NeuronID));
            uint i = 0;
            foreach(Position3D pos in proximalSegList)
            {
                Segment newSegment = null;
                //TODO : Need bit more reasearch about the SegmentType

                if(pos.cType == CType.ConnectedToAxon)
                {
                    newSegment = new Segment(pos, SegmentType.Axonal, NeuronID, i);
                    axonEndPoints.Add(pos);
                }
                else if(pos.cType == CType.ConnectedToDendrite)
                {
                    newSegment = new Segment(pos, SegmentType.Proximal, NeuronID, i);
                    _proximalSegmentList.Add(pos);
                }
                else if(pos.cType == CType.Synapse)
                {
                    throw new Exception();
                    //To be Done.
                }

                if(Segments.TryGetValue(newSegment.ComputeSegmentIDAsString() , out Segment segment))
                {
                    //Segment exists at this position , this should not have happened unless a bug in CTable is messing up this part of the logic

                    throw new Exception("You Fucked Up BITCH!!!! CTable mess" + newSegment.ComputeSegmentIDAsString());
                }

                Segments.Add(newSegment.ComputeSegmentIDAsString(), newSegment);

                i++;
            }
        }

        internal Segment GetSegment(SegmentID segID)
        {
            Segment seg;

            if (Segments.TryGetValue(segID.StringIDWithoutBID, out seg))
                return seg;

            throw new InvalidOperationException("Invalid Segment ID Access");
        }        

        /// <summary>
        /// Process Potential to segment and decide if you are gonna fire or not so CPM can add you to the prediction list.
        /// </summary>
        /// 
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
                Voltage += seg._sumVoltage;
                seg.FlushVoltage();
                _predictedSegments.Add(segmentID);
                return true;
            }
            return false;
        }

        internal void Fire()
        {
            foreach(var point in axonEndPoints)
            {
                //Get connection table and check if any of the axon end points has connections ./AIII 118
                //Collect those SegmentId's , get Neurons from those segmentID's
                //start calling methods on those neurons with there respective segmentID's

                SegmentID sId = _cpm.CTable.InterfaceFire(point.StringIDWithBID);
                _cpm.NeuronFire(point, sId, NEURONAL_FIRE_VOLTAGE);
            }
        }

        public string GetString() => NeuronID.StringIDWithoutBID;

        private void FlushVoltage() =>
            Voltage = 0;

        internal bool AddNewConnection(Position3D pos, SegmentID segmentID)
        {
            Segment segment;
            if(!Segments.TryGetValue(pos.StringIDWithoutBID, out segment))
            {
                segment.AddNewConnection(pos);
                return true;
            }
            return false;
        }

        internal void Grow()
        {
            //growth signal comes from CPM when neuron exceeds fire index in the fire cycle , we add new positions to both axonal endpoints and dendritic segments.
            // Will need some details on which dendrites or axons are exactly firing.

            ƒ

        }

        internal void Prune()
        {
            //Need neuron status tracking from CPM.
        }
    } 
}
