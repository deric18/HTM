//Grow , Prune , lastAccesedCycle.
//Segment ID Syntax : << Neuron Position3D > -- < Segment Number > -- < Segment Position3D >>
using System.Configuration;
using System.Collections.Generic;
using System;
using HTM.Enums;
using HTM.Algorithms;

namespace HTM.Models
{
    /// <summary>
    /// -If Proximal Polarization  of the system
    /// -NMDA
    /// -Segment Growth.
    /// </summary>
    public class Segment
    {
        public Position3D BasePosition { get; private set; }    // Position where the Semgnet Originates and grows out of!.
        public uint _sumVoltage { get; private set; }
        public Position3D NeuronID { get; private set; }
        public string LineageString { get; private set; }
        public SegmentID SegmentID { get; private set; }
        private uint _segmentNumber;
        public bool didFireLastCycle { get; private set; }
        private bool _fullyConnected;
        public Dictionary<Position3D, uint> Synapses { get; private set; }     //uint helps in prunning anything thats zero is taken out and flushed to connection table.
        private Lazy<List<Segment>> SubSegments;
        private List<Position3D> _predictedSynapses;   //All predicted synapses are not NMDA Synapses but all NMDA Synapses has to be predicted at some point for it to be NMDA Spiking Synapse.
        private List<Position3D> _nmdapredictedSynapses;
        private ulong createdCycle;
        private ulong _lastAccesedCycle;                     //helps in prunning of segments
        private bool IsSubSegment;                          //True if yes , False if no
        private SegmentType sType;
        private static uint NMDA_Spike_Potential;
        private static uint MAX_Connection_Strength;
        private static uint DISTAL_NEW_SYNAPSE_STRENGTH;
        private static uint PROXIMAL_NEW_SYNAPSE_STRENGTH;
        private static uint NEW_SYNAPSE_CONNECTION_DEF;
        private uint MAX_SUBSEGMENTS_SEGMENT;

        public Segment(Position3D basePos, SegmentType sType, Position3D neuronID, uint segCount, string lineageString, bool isSubSegment = false)
        {            
            this._segmentNumber = segCount;
            this.IsSubSegment = isSubSegment;            
            SegmentID = isSubSegment ? new SegmentID(neuronID, segCount, basePos, lineageString + basePos.StringIDWithBID + segCount.ToString() ) : new SegmentID(neuronID, segCount, basePos);
            NeuronID = neuronID;
            this.BasePosition = basePos;
            this.sType = sType;
            this.createdCycle = CPM.GetInstance.currenCcycle;
            this._lastAccesedCycle = CPM.GetInstance.currenCcycle;
            this.didFireLastCycle = false;
            _sumVoltage = 0;               
            _fullyConnected = false;
            Synapses = new Dictionary<Position3D, uint>();            
            _lastAccesedCycle = 0;
            _predictedSynapses = new List<Position3D>();
            _nmdapredictedSynapses = new List<Position3D>();

            if (isSubSegment)
            {
                LineageString = lineageString + basePos.StringIDWithBID + segCount.ToString();
            }
            else
            {
                if(lineageString == null)
                {
                    LineageString = neuronID.StringIDWithBID + "/" + segCount.ToString() + "/" + BasePosition.StringIDWithBID;
                }                
            }
            

            NMDA_Spike_Potential = uint.Parse(ConfigurationManager.AppSettings["NMDA_SPIKE_POTENTIAL"]);
            MAX_Connection_Strength = uint.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_STRENGTH_FOR_SYNAPSE"]);
            MAX_SUBSEGMENTS_SEGMENT = uint.Parse(ConfigurationManager.AppSettings["MAX_SUBSEGMENTS_SEGMENT"]);
            NEW_SYNAPSE_CONNECTION_DEF = uint.Parse(ConfigurationManager.AppSettings["PRE_SYNAPTIC_CONNECTION_STRENGTH"]);
            PROXIMAL_NEW_SYNAPSE_STRENGTH = uint.Parse(ConfigurationManager.AppSettings["PROXIMAL_NEW_SYNAPSE_STRENGTH"]);
            DISTAL_NEW_SYNAPSE_STRENGTH = uint.Parse(ConfigurationManager.AppSettings["DISTAL_NEW_SYNAPSE_STRENGTH"]);

            Synapses.Add(basePos, PROXIMAL_NEW_SYNAPSE_STRENGTH);
        }



        //private string ComputeSegmentIDasString(Position3D neuronID, string segCount, Position3D basePos)
        //{
        //    return neuronID.StringIDWithoutBID + "/" + segCount + "/" + basePos.StringIDWithoutBID;
        //}        

        internal Segment GetSegment(int v)
        {
            if(SubSegments.IsValueCreated)
            {
                return SubSegments.Value[v];
            }

            return null;
        }

        public void Grow(bool onlyNMDA, uint numPulses = 1)
        {
            if(onlyNMDA)
            {
                if(_nmdapredictedSynapses.Count > 0)
                {
                    foreach(var pos in _nmdapredictedSynapses)
                    {
                        if (Synapses.TryGetValue(pos, out uint value))
                        {
                            Synapses[pos] += numPulses;
                        }
                        else
                        {
                            Console.WriteLine("EXCEPTION :: SEGMENT :: GROW :: PREDICTED SYNAPSE NOT PRESENT IN SYNAPSES LIST IN SEGMENT");
                        }
                    }
                }
                else if(_predictedSynapses.Count > 0)
                {
                    foreach( var pos in _predictedSynapses)
                    {
                        Synapses[pos] += numPulses;
                    }
                }
                else
                {
                    Console.WriteLine("INFORMATION :: NO NMDA Predicted Segments Found");
                }
            }
            else
            {
                if(Synapses.Count > 0)
                {
                    foreach(var kvp in Synapses)
                    {
                        Synapses[kvp.Key] += numPulses;
                    }
                }
                else if(SubSegments.IsValueCreated)
                {
                    Console.WriteLine("INFORMATIONAL :: GROWING SUB SEGMENT");
                    foreach(var subseg in SubSegments.Value)
                    {
                        subseg.Grow(onlyNMDA, numPulses);
                    }
                }
                else
                {
                    Console.WriteLine("WARNING :: NO Synapses UNDER this NEURON!!!");
                }
            }



        }

        public void GrowSingleNeuron(Position3D synapseId)
        {
            if (Synapses.TryGetValue(synapseId, out uint synapticStrength))
            {
                synapticStrength++;
                Synapses.Remove(synapseId);
                Synapses.Add(synapseId, synapticStrength);
            }
        }

        internal void FinishCycle()
        {
            _predictedSynapses.Clear();
            didFireLastCycle = false;
            FlushVoltage();
        }

        public void Prune()
        {
            //Run through synaptic list to eliminate neurons with lowest connection strength
            //also run through the subsegments and check the lastaccessed time and there synapses.
            foreach (var s in Synapses)
            {
                if (s.Value <= int.Parse(ConfigurationManager.AppSettings["PRUNE_THRESHOLD"]))
                {
                    Console.WriteLine("Removing synapse to Neuron" + PrintPosition(s.Key));
                    Synapses.Remove(s.Key);
                }
            }

            foreach (var segment in SubSegments.Value)
            {
                segment.Prune();
            }
        }


        internal void AddNewConnection(Position3D pos, SynapseType sType)
        {
            if(Synapses.TryGetValue(pos, out uint val))
            {
                Console.WriteLine("AddNewConnection : Segment : Synapse at this point already exists ! Somebody fucked up relly bad!! :( DEBUG NEEDED !!! THIS Should NOT HAVE HAPPENED");
                throw new Exception("You fucked up!!!");
            }
            else
            {
                if (sType.Equals(SynapseType.Distal))
                    Synapses.Add(pos, DISTAL_NEW_SYNAPSE_STRENGTH);
                else if (sType.Equals(SynapseType.Proximal))
                    Synapses.Add(pos, PROXIMAL_NEW_SYNAPSE_STRENGTH);
            }

        }
            
        /// <summary>
        /// Predict if the segment will fire or not based incoming voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="firingNeuronId"></param>
        /// <returns></returns>
        public bool Process(uint voltage, Position3D synapseId, InputPatternType iType)
        {
            #region REMOVED
            //switch (iType)
            //{
            //    case InputPatternType.INTERNAL:
            //        {
            //            _internalVoltage += voltage;
            //            break;
            //        }
            //    case InputPatternType.TEMPORAL:
            //        {
            //            _temporalVoltage += voltage;
            //            break;
            //        }
            //    case InputPatternType.APICAL:
            //        {
            //            _apicalVoltage += voltage;
            //            break;
            //        }
            //    default:break;
            //}
            #endregion

            this._lastAccesedCycle = CPM.GetInstance.currenCcycle;
            this.didFireLastCycle = true;
            if(Synapses.TryGetValue(synapseId, out uint sigStrength))
            {
                _sumVoltage += voltage * sigStrength;
                _predictedSynapses.Add(synapseId);
            }
            else
            {
                Console.WriteLine("PROCESS: SEGMENT :  Invalid FIRE , SYNAPSE DOES NOT EXIST!!!");
                throw new Exception("iNVALID fire()");
            }

            if(voltage >= NMDA_Spike_Potential)
            {
                _nmdapredictedSynapses.Add(synapseId);
                return true;
            }
            else
            {
                _predictedSynapses.Add(synapseId);
            }
            return false;
        }  
        
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// Boxed Growth : Direction and boxed random connection
        /// 1.A Segment should have direction it grows and by default will have limits set to how far away the segment can predict its new connection.
        /// </summary>
        private void AddNewLocalConnection()
        {
            //Make sure you are not connecting to an axon of your own neuron if its a new position
            /*Decide how to add new connection : Randomly pick connections from the segments local visibility radius and connect.
            */
            //If not branched and position is noand check if segment has max positions , add this position to a possibility list for future connection when the neuron losses and non used connection else if not max position then add new position,
            //Pick Suitable position (position next to the best firing position) need a method here to determine which direction the axon is growing and where to connect as such.                        
            //Alert Connection Table about the new position

            SynapseGenerator sg =  CPM.GetInstance.synapseGenerator;
            Position3D newPosition;

            if ((Synapses.Count < int.Parse(ConfigurationManager.AppSettings["MAX_CONNECTIONS_PER_SEGMENT"])))
            {//Below number of synapses threshold for segment 
                
                newPosition = sg.PredictNewRandomPosition(this.BasePosition);                                 

                ConnectionType cPos = CPM.GetInstance.CTable.ClaimPosition(newPosition, SegmentID, EndPointType.Dendrite);

                if(cPos.ConType.Equals(CType.NotAvailable))
                {
                    throw new Exception("Your Code Sucks !!!! This should never Happen , You Moron !!! ITs time to consider other careers !!! May be watch more BReaking Bad and start Selling Meth !!!!");
                }

                AddConnection(newPosition);
            } 
            else if(SubSegments.Value.Count < int.Parse(ConfigurationManager.AppSettings["MAX_SEGMENTS_PER_NEURON"]))
            {//Below number of subsegments threshold for segment 
                //handle logic for if this position is already marked in ctable then do a new position. basically dont do anything everything is already handled in ctable.
                newPosition = sg.PredictNewRandomPosition(this.BasePosition);

                ConnectionType cPos = CPM.GetInstance.CTable.ClaimPosition(newPosition, SegmentID, EndPointType.Dendrite);

                if (cPos.ConType.Equals(CType.NotAvailable))
                {
                    throw new Exception("Your Code Sucks !!!! This should never Happen , You Moron !!! ITs time to consider other careers !!! May be watch more BReaking Bad and start Selling Meth !!!!");
                }

                CreateSubSegment(newPosition);
            }
            else
            {//above both threshold print a msg and exit
                Console.WriteLine("Segment: " + PrintPosition(this.BasePosition) + "SEG ID: " + this.BasePosition.BID + "-" + " is Over Connected");                
                _fullyConnected = true;
                //log Information with details , Segment has reached a peak connection pathway , this is essentially a crucial segment for the whole region.
            }            
        }                      

        internal void FlushVoltage()
        {
            _sumVoltage = 0;
            _predictedSynapses.Clear();
        }

        private void PrintSegmenttID()
        {
            Console.Write(SegmentID.GetSegmentID);
        }

        //private Position3D GetNewPositionFromBound(Position3D segmentBound)
        //{
        //    Random r = new Random(_seed);
        //    return new Position3D((uint)r.Next((int)BaseConnection.X, (int)segmentBound.X), (uint)r.Next((int)BaseConnection.Y, (int)segmentBound.Y), (uint)r.Next((int)BaseConnection.Z, (int)segmentBound.Z));
        //}                   

        private void AddConnection(Position3D newPosition) =>
            Synapses.Add(newPosition, NEW_SYNAPSE_CONNECTION_DEF);                            

        private void CreateSubSegment(Position3D basePosition)
        {
            if (!this.SubSegments.IsValueCreated)
            {
                uint count = (uint)SubSegments.Value.Count;                
                Segment newSegment = new Segment(basePosition, this.sType, NeuronID, count, LineageString, true);
                
                Console.WriteLine("New Segment Added to Neuron " + this.NeuronID.StringIDWithoutBID + "Segment ID : ");
                newSegment.PrintSegmenttID();

                
                SubSegments.Value.Add(newSegment);

                //Need to register this subsegment in Neuron. otherwise it wont be able to send the grow signal
                CPM.GetInstance.GetNeuronFromSegmentID(this.SegmentID).RegisterSubSegmentToNeuron(newSegment);

            }
        }

      

        private string PrintPosition(Position3D pos4d)
        {
            return " X: " + pos4d.X.ToString() + " Y:" + pos4d.Y.ToString() + " Z:" + pos4d.Z.ToString();
        }

        
        //finds out if there are any other segments that have already created a sub segmetn here , this might need to create a standard of segment ID syntax and this function needs to be done at the base segment level.
        private bool DoesSubSegmentExist(Position3D newPosition)
        {
            foreach(var seg in SubSegments.Value)
            {
                if(seg.BasePosition.Equals(newPosition))
                {
                    return true;
                }
            }

            return false;
        }

        private bool DoesConnectionExist(Position3D pos)
        {
            //Do we have a synapse at this position already ?
            uint val;
            if (Synapses.TryGetValue(pos, out val))
            {
                return true;
            }
            return false;
        }
        
    }
}
