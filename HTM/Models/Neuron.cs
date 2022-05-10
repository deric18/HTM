﻿//TODO : CreateProximalSegment, AddSegment, Prune, Grow
//If more than 3 cycles and neuron has not fired then flushall the voltage that came in before 3 cycles.
using HTM.Enums;
using System.Collections.Generic;
using System;
using HTM.Algorithms;

namespace HTM.Models
{
    /// <summary>
    /// 1.Account for both excitatory and inhibitory neurons . Need to rethink about this, one argument is that this is not neccessary because HTM Model already accounts for this by 
    /// picking single neurons from one column with highest values using selective firing techniques.
    /// </summary>
    public class Neuron : IEqualityComparer<Neuron> , IEquatable<Neuron>
    {                
        internal uint Voltage { get; private set; }
        public Position3D NeuronID { get; private set; }
        internal NeuronState State { get; private set; }
        public  List<Position3D> ProximalSegmentList { get; private set; }     //list of proximal segments with higher connectivity threshold

        //Question : Should proximal Segments be also added to the segment List
        //Answer : No , coz you can make the logic in the method to look into both lists rather than duping data , bad practice!
        public Dictionary<string, Segment> Segments { get; private set; }      // List of all segments the neuron has
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
            ProximalSegmentList = new List<Position3D>();
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

            if (proximalSegList == null)
                proximalSegList = new List<Position3D>();
            uint i = 0;

            foreach(Position3D pos in proximalSegList)
            {
                Segment newSegment = null;
                //TODO : Need bit more reasearch about the SegmentType

                if(pos.cType == CType.SuccesfullyClaimedByAxon)
                {
                    newSegment = new Segment(pos, SegmentType.Axonal, NeuronID, i, null, false);
                    axonEndPoints.Add(pos);
                }
                else if(pos.cType == CType.SuccesfullyClaimedByDendrite)
                {
                    newSegment = new Segment(pos, SegmentType.Proximal, NeuronID, i, null, false);
                    ProximalSegmentList.Add(pos);
                }
                else if(pos.cType == CType.Synapse)
                {
                    throw new Exception();
                    //To be Done.
                }

                if(Segments.TryGetValue(newSegment.SegmentID.GetSegmentID , out Segment segment))
                {
                    //Segment exists at this position , this should not have happened unless a bug in CTable is messing up this part of the logic

                    throw new Exception("You Fucked Up BITCH!!!! CTable is messed Up" + newSegment.SegmentID.GetSegmentID);
                }

                Segments.Add(newSegment.SegmentID.GetSegmentID, newSegment);


                //TODO : Need to add these positions to synapses as well.
                i++;
            }
        }


        public void RegisterSubSegmentToNeuron(Segment segment)
        {
            if (segment.NeuronID.Equals(NeuronID))
            {
                if(Segments.TryGetValue(segment.SegmentID.GetSegmentID, out Segment seg))
                {
                    throw new Exception("ERROR : RegisterSubSegmentToNeuron : There is already a segment registered at this position");
                }

                Segments.Add(segment.SegmentID.GetSegmentID, segment);
            }
            else
            {
                throw new Exception("Invalid Neuron trying to be registered");
            }

        }

        internal Segment GetSegment(SegmentID segID)
        {            

            if (Segments.TryGetValue(segID.GetSegmentID, out Segment seg))
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

        /// <summary>
        /// Handles Firing of a Neuron
        /// </summary>
        internal void Fire()
        {
            foreach(var point in axonEndPoints)
            {
                //Get connection table and check if any of the axon end points has connections ./AIII 118
                //Collect those SegmentId's , get Neurons from those segmentID's
                //start calling methods on those neurons with there respective segmentID's

                DoubleSegment dSegment = _cpm.CTable.InterfaceFire(point.StringIDWithBID);

                //_cpm.AddtoPredictedList(point, sId, NEURONAL_FIRE_VOLTAGE);
                //_cpm.CTable.RecordFire(point);

                //Get the doublesegment asociated with this position and directly send a grow signal to the denditic neuron.
                _cpm.GetNeuronFromPositionID(dSegment.dendriteISegmentD.NeuronId).Grow( _cpm.GetSegmentFromSegmentID(dSegment.dendriteISegmentD), dSegment.synapsePosition);
            }
        }

        public string GetString() => NeuronID.StringIDWithBID;

        private void FlushVoltage() =>
            Voltage = 0;

        internal bool AddNewConnection(Position3D pos, SegmentID segmentID)
        {
            Segment segment;
            if(!Segments.TryGetValue(pos.StringIDWithBID, out segment))
            {
                segment.AddNewConnection(pos);
                return true;
            }
            return false;
        }

        internal void Grow(Segment seg, Position3D synapse)
        {
            //TODO:
            //growth signal comes from CPM when neuron exceeds fire index in the fire cycle , we add new positions to both axonal endpoints and dendritic segments.
            // Will need some details on which dendrites or axons are exactly firing.

            seg.Grow(synapse);
        }

        internal void Prune()
        {
            //Need neuron status tracking from CPM.
        }

        public bool Equals(Neuron x, Neuron y)
        {
            return x.NeuronID.StringIDWithBID.Equals(y.NeuronID.StringIDWithBID);
        }

        public int GetHashCode(Neuron obj)
        {
            return (int)(obj.NeuronID.X * obj.NeuronID.Y * obj.NeuronID.Z);
        }

        public bool Equals(Neuron obj)
        {
            return this.NeuronID.StringIDWithBID.Equals(obj.NeuronID.StringIDWithBID);
        }
    } 
}
