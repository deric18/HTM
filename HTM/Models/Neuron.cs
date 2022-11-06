//TODO : CreateProximalSegment, AddSegment, Prune, Grow
//If more than 3 cycles and neuron has not fired then flushall the voltage that came in before 3 cycles.

namespace HTM.Models
{

    using HTM.Enums;
    using System.Collections.Generic;
    using System;
    using HTM.Algorithms;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// 1.Account for both excitatory and inhibitory neurons . Need to rethink about this, one argument is that this is not neccessary because HTM Model already accounts for this by 
    /// picking single neurons from one column with highest values using selective firing techniques.
    /// </summary>
    public class Neuron : IEqualityComparer<Neuron>, IEquatable<Neuron>
    {
        internal uint Voltage { get; private set; }
        public Position3D NeuronID { get; private set; }
        internal NeuronState State { get; private set; }
        public List<Position3D> ProximalSegmentList { get; private set; }     //list of proximal segments with higher connectivity threshold

        //Question : Should proximal Segments be also added to the segment List
        //Answer : No , coz you can make the logic in the method to look into both lists rather than duping data , bad practice!
        public Dictionary<string, Segment> Segments { get; private set; }      // List of all segments the neuron has        
        private Dictionary<Position3D, Segment> _NMDApredictedSegments;
        private List<Segment> _FailedToFirePredictedSegments;
        public List<Position3D> axonEndPoints { get; private set; }
        public List<Position3D> dendriticEndPoints { get; private set; }

        private readonly uint NEURONAL_FIRE_VOLTAGE;
        private readonly uint AVERAGE_GROWTH_SIGNAL_STRENGTH;
        private readonly uint NMDA_SPIKE_POTENTIAL;

        private CPM _cpm;

        public Neuron(Position3D pos)
        {
            Voltage = 0;
            NeuronID = pos;
            State = NeuronState.RESTING;
            Segments = new Dictionary<string, Segment>();
            ProximalSegmentList = new List<Position3D>();
            _NMDApredictedSegments = new Dictionary<Position3D, Segment>();
            _FailedToFirePredictedSegments = new List<Segment>();
            axonEndPoints = new List<Position3D>();
            dendriticEndPoints = new List<Position3D>();
            _cpm = CPM.GetInstance;
            NEURONAL_FIRE_VOLTAGE = Convert.ToUInt32(ConfigurationManager.AppSettings["NEURONAL_FIRE_VOLTAGE"]);
            AVERAGE_GROWTH_SIGNAL_STRENGTH = Convert.ToUInt32(ConfigurationManager.AppSettings["AVERAGE_GROWTH_SIGNAL_STRENGTH"]);
            NMDA_SPIKE_POTENTIAL = Convert.ToUInt32(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]);
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
            if (seg.Process(potential, position, InputPatternType.INTERNAL))     //if NMDA
            {
                Voltage += seg._sumVoltage;
                _NMDApredictedSegments.Add(position, seg);
                this.State = NeuronState.PREDICTED;
                return true;
            }

            _FailedToFirePredictedSegments.Add(seg);

            return false;
        }

        /// <summary>
        /// check if any of the neruons axons are connected to any dendrite at all ?
        /// if so fire them , add the recipient neuron to the predicted list
        ///     if the predicted neuron fires next send a grow signal to that neuron ,also prune to those which havent fired in the last 45 cycles
        ///     else increase cycle count on each of these empty synapses
        /// else
        ///  send multiple grow singals to the neuron so it makes some connections.
        /// </summary>
        internal void Fire()
        {
            foreach (var point in axonEndPoints)
            {

                DoubleSegment dSegment = _cpm.CTable.InterfaceFire(point.StringIDWithBID);

                if (dSegment != null)
                {
                    var neuron = _cpm.GetNeuronFromSegmentID(dSegment.dendriteSegmentID);
                    neuron.Process(point, dSegment.dendriteSegmentID, NEURONAL_FIRE_VOLTAGE);
                }
                else
                {
                    Console.WriteLine("FIRE : NULL FIRE , Neuron does not have any valid Connections to fire");
                    //Neurons axons doesnt have any active connection to any dendrites near by.
                }
            }
        }


        internal void GrowDirect(Segment seg, Position3D synapse)
        {
            //TODO:
            //growth signal comes from CPM when neuron exceeds fire index in the fire cycle , we add new positions to both axonal endpoints and dendritic segments.
            // Will need some details on which dendrites or axons are exactly firing.

            seg.Grow(synapse);
            
        }

        /// <summary>
        /// increment synaptic strength on nmda & non nmda predicted segments
        /// send out a grow and prune signal to all the segments , to cut out unneccesary connection and grow more promissing ones.
        /// </summary>
        internal void Grow(bool boostOnlyNMDA, bool shouldPrune, uint connStrength)
        {
            //Increment synaptic strength on all the connections , more on ones that fired last cycle and less on ones that did not.
            if(boostOnlyNMDA)
            {
                List<Segment> firingSegments = Segments.Values.Where(q => q.didFireLastCycle == true).ToList();
                if(firingSegments.Count > 0)
                {
                    foreach(var seg in firingSegments)
                    {
                        seg.Grow();
                    }
                }

                }

        }


        internal void Audit()
        {
            //Go through all the segments and there respective connections and audit there hitcounters , if 0 for 50 cycles then prune them 
        }


        public void CreateProximalSegments()
        {
            //create 6 baseposition points on each side of the face of the neuron close to its nearby neurons ( atelast half way )
            //Create Segments with bae positions as the the once you recieve from synapseGenerator
            List<Position3D> proximalSegList;
            proximalSegList = (CPM.GetInstance.synapseGenerator.AddProximalSegment(NeuronID));

            if (proximalSegList == null)
                proximalSegList = new List<Position3D>();   //Avoiding Exceptions


            uint i = 0;

            if (NeuronID.BID == 111)
                Console.WriteLine("Stuff");

            foreach(Position3D pos in proximalSegList)
            {
                Segment newSegment = null;                

                try
                {
                    if (pos.cType == CType.SuccesfullyClaimedByAxon)
                    {
                        newSegment = new Segment(pos, SegmentType.Axonal, NeuronID, i, null, false);
                        axonEndPoints.Add(pos);
                    }
                    else if (pos.cType == CType.SuccesfullyClaimedByDendrite)
                    {                        
                        newSegment = new Segment(pos, SegmentType.Proximal, NeuronID, i, null, false);
                        ProximalSegmentList.Add(pos);
                    }
                    else if (pos.cType == CType.AxonConnectedToDendrite)
                    {
                        DoubleSegment douSeg = _cpm.CTable.InterfaceFire(pos.StringIDWithBID);

                        axonEndPoints.Add(douSeg.axonalSegmentID.BasePosition);
                        var segId = douSeg.axonalSegmentID;
                        newSegment = new Segment(segId.BasePosition, SegmentType.Axonal, segId.NeuronId, i, segId.GetSegmentID);
                        newSegment.AddNewConnection(pos, SynapseType.Proximal);
                        _cpm.GetSegmentFromSegmentID(douSeg.dendriteSegmentID).AddNewConnection(pos, SynapseType.Proximal);   //This is Very important as it adds the synapse to the dendrite as well
                        //Get the connecting Segment
                        // form a synapse 
                        // register both connections 
                        
                    }
                    else if(pos.cType == CType.DendriteConnectedToAxon)
                    {
                        DoubleSegment douSeg = _cpm.CTable.InterfaceFire(pos.StringIDWithBID);

                        dendriticEndPoints.Add(douSeg.dendriteSegmentID.BasePosition);
                        var segId = douSeg.dendriteSegmentID;
                        newSegment = new Segment(segId.BasePosition, SegmentType.Proximal, segId.NeuronId, i, segId.GetSegmentID);
                        newSegment.AddNewConnection(pos, SynapseType.Proximal);
                        _cpm.GetSegmentFromSegmentID(douSeg.axonalSegmentID).AddNewConnection(pos, SynapseType.Proximal);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("New Connection Synapse return type is incorrect" + e.Message);
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

        internal void FinishCycle()
        {
            FlushVoltage();
            _FailedToFirePredictedSegments.Clear();
            _NMDApredictedSegments.Clear();
        }

        public string GetString() => NeuronID.StringIDWithBID;

        private void FlushVoltage() =>
            Voltage = 0;

        internal bool AddNewConnection(Position3D pos, SegmentID segmentID)
        {
            Segment segment;
            if(!Segments.TryGetValue(pos.StringIDWithBID, out segment))
            {
                segment.AddNewConnection(pos, SynapseType.Distal);
                return true;
            }
            return false;
        }        

        internal bool CheckConnections(Neuron neuron)
        {
            bool toRet = false;

            foreach(var seg in this.Segments)
            {
                foreach(var seg2 in neuron.Segments)
                {
                    if(CPM.GetInstance.CTable.DoesConnectionExist(seg.Value , seg2.Value))
                    {
                        toRet = true;
                    }
                }
            }

            return toRet;
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
