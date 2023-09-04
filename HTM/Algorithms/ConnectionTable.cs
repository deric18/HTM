//TODO : Complete Apical and Temporal axon lines.
//Get Rid off CMap.
//
using HTM.Enums;
using HTM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HTM.Algorithms
{
    public class ConnectionTable
    {
        private char[,,,] cMap;            //  A = Available , d - Occupied by Dendrite , a - Occupied by axon , N - Not Available
                                           //key : is always Position3D.GetStringIDWithBID();       
        private uint temporalCounter;
        private uint apicalCounter;
        private Dictionary<string, SegmentID> axonalEndPoints;          //holds only unconnected axonal ( active axonal connections)
        private Dictionary<string, SegmentID> dendriticEndPoints;       //holds only unconnected dendrites ( active dendritic connections)

        private Dictionary<string, DoubleSegment> synapses;

        public Dictionary<string,DoubleSegment> PredictedSegments { get; private set; }    //Needed by CPM to match up and grow synapses between firing and predicted neurons.
        public static ConnectionTable SingleTon;


        public static ConnectionTable Singleton(uint totalNumberOfBlocks = 0, BlockConfigProvider bcp = null)
        {
            if (SingleTon == null)
                SingleTon = new ConnectionTable(totalNumberOfBlocks, bcp);
            return SingleTon;
        }

        private ConnectionTable(uint numBlocks, BlockConfigProvider bcp)
        {            
            axonalEndPoints = new Dictionary<string, SegmentID>();          //holds all the due axonal connections.
            dendriticEndPoints = new Dictionary<string, SegmentID>();       //<position3d , corresponding segment id of the dendrite>.
            synapses = new Dictionary<string, DoubleSegment>();             //Holds all the synapses with respective synapses ID's.
            cMap = new char[numBlocks, bcp.NumXperBlock,bcp.NumYperBlock, bcp.NumZperBlock];
            PredictedSegments = new Dictionary<string, DoubleSegment>();


            for (uint block = 0; block < numBlocks; block++)
                for (uint i = 0; i < bcp.NumXperBlock; i++)
                    for (uint j = 0; j < bcp.NumYperBlock; j++)                    
                        for (uint k = 0; k < bcp.NumZperBlock; k++)
                        {
                            //if (cMap[block, i][j, k] == default(char))
                              //  cMap[block, i][j, k] = 'A';
                            cMap[block, i,j, k] = 'A';
                        }                                                           
        }

        /*TO Implement:
         * -when cpm processes a neuronal fire it gives all the positions to which the neuron fires , then cpm needs to get all thos positions and find out if there are any neurons connecting to the specific position and if so get there segment ids and tra
         *  potential to those segments
         *  CPM Constants:

         
             */

        //public void RecordFire(Position3D pos)
        //{
        //    if(synapses.TryGetValue(pos.StringIDWithBID, out DoubleSegment dubSeg))
        //    {
        //        Console.WriteLine("RECORD FIRE : Good News we found the synapse");

        //        if(PredictedSegments.TryGetValue(pos.StringIDWithBID, out var valueItem))
        //        {
        //            uint repeatCount = valueItem.Key;
        //            repeatCount++;
        //            synapses.Remove(pos.StringIDWithBID);
        //            valueItem = new KeyValuePair<uint, DoubleSegment>(repeatCount, valueItem.Value);
        //            PredictedSegments.Add(pos.StringIDWithBID, valueItem);
        //        }
        //        else
        //        {
        //            PredictedSegments.Add(pos.StringIDWithBID, new KeyValuePair<uint, DoubleSegment>(0, dubSeg));
        //        }
        //    }
        //    else
        //    {
        //        // Do Nothing & Return. Nothing to Record.
        //    }
        //}

        public int GetTotalSynapsesCount => synapses.Count;

        public Dictionary<string, DoubleSegment> GetAllPredictedSegments()
        {
            return PredictedSegments;
        }

        public void FlushPredictedSegments()
        {
            if (PredictedSegments.Count == 0)
                Console.WriteLine("WARNING : FlushPredictedSegments: Emptying empty Predicted Segments end of Cycle");
            else
                Console.WriteLine("FlushPredictedSegments : Succesfully Emptied all the Predicted Segments");

            this.PredictedSegments.Clear();
        }

        public char Position(uint blockID, uint x, uint y, uint z) => cMap[blockID, x,y, z];

        public bool IsPositionAvailable(Position3D pos) => (!dendriticEndPoints.TryGetValue(pos.StringIDWithBID, out SegmentID item) && !axonalEndPoints.TryGetValue(pos.StringIDWithBID, out SegmentID item1) && !synapses.TryGetValue(pos.StringIDWithBID, out DoubleSegment item2));

        public bool DoesConnectionExist(Segment seg1, Segment seg2)
        {
            
            if (seg1.Synapses.Count == 0 || seg2.Synapses.Count == 0)
                return false;

            foreach(var kvp in seg1.Synapses)
            {
                foreach(var kvp2 in seg2.Synapses)
                {
                    if(kvp.Key.Equals(kvp2.Key))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        private bool IsDendriteOccupied(Position3D pos) => (dendriticEndPoints.TryGetValue(pos.StringIDWithBID, out SegmentID item));

        private bool IsAxonOccupied(Position3D pos) => (axonalEndPoints.TryGetValue(pos.StringIDWithBID, out SegmentID item));

        private bool IsSynapsed(Position3D pos) => (synapses.TryGetValue(pos.StringIDWithBID, out DoubleSegment item));

        /// <summary>
        /// Position Claimer        
        /// </summary>
        /// <param name="pos">Position that is under claim investigation</param>
        /// <param name="claimerSegID">SegmentID</param>
        /// <param name="eType">EndpointType eType</param>
        /// <returns>ConnectionType to</returns>
        public ConnectionType ClaimPosition(Position3D pos, SegmentID claimerSegID, EndPointType eType)
        {
            /// Scenario 1: If the claimer is a dendrite and the position is empty we just give the position to them.  Most Probable scenario
            /// Scenario 2: If the claimer is a axon and the position is empty we just give the position to them.
            /// Scenario 3: If the claimer is a dendrite and the positio9n is occupied by an axonal seg then check for selfing , remove the axonal seg id from the dict ,mark the position as 'N' in cMap and return the seg id.
            /// Scenario 4: If the claimer is a axon and the position is occupied by the dendrite then we again check for selfing, and send connect signal to claimed neuron, mark the position and return bind signal to claiming neuron.            
            #region COMMENTED OUT

            //switch (cMap[pos.BID, pos.X,pos.Y,pos.Z])
            //{
            //    case 'A': //Available                   
            //        switch (eType)
            //        {
            //            case EndPointType.Axon:
            //                AxonClaim(pos, claimerSegID);
            //                cMap[pos.BID, pos.X,pos.Y, pos.Z] = 'a';
            //                ++openCounter;
            //                return new ConnectionType(CType.SuccesfullyClaimedByAxon);                            
            //            case EndPointType.Dendrite:
            //                DendriteClaim(pos, claimerSegID);
            //                cMap[pos.BID, pos.X,pos.Y, pos.Z] = 'D';
            //                ++openCounter;
            //                return new ConnectionType(CType.SuccesfullyClaimedByAxon);

            //            default: break;
            //        }
            //        ++openCounter;
            //        break;
            //    case 'D'://Dendrite
            //    case 'd':
            //        switch (eType)
            //        {
            //            case EndPointType.Axon://Axon claiming a dendritc position 
            //                AxonClaim(pos, claimerSegID);
            //                cMap[pos.BID, pos.X,pos.Y, pos.Z] = 'N';
            //                --openCounter;
            //                ++closedCounter;                            
            //                if(dendriticEndPoints.TryGetValue(pos.StringIDWithBID, out segid))
            //                    return new ConnectionType(CType.DendriteConnectedToAxon, segid);
            //                break;  
            //            default:
            //                return new ConnectionType(CType.NotAvailable);
            //        }
            //        break;
            //    case 'a'://Axon
            //        switch (eType)
            //        {
            //            case EndPointType.Dendrite:     //dendrite claiming an axon
            //                {
            //                    DendriteClaim(pos, claimerSegID);
            //                    cMap[pos.BID, pos.X,pos.Y, pos.Z] = 'N';
            //                    --openCounter;
            //                    ++closedCounter;                                
            //                    if (dendriticEndPoints.TryGetValue(pos.StringIDWithBID, out segid))
            //                        return new ConnectionType(CType.DendriteConnectedToAxon, segid);
            //                    break;
            //                }
            //            default:
            //                return new ConnectionType(CType.NotAvailable);
            //        }
            //        break;
            //    case 'T'://Temporal Axon Line
            //        {
            //            switch (eType)
            //            {
            //                case EndPointType.Dendrite:
            //                    {
            //                        DendriteClaim(pos, claimerSegID);
            //                        cMap[pos.BID, pos.X,pos.Y, pos.Z] = 'N';
            //                        ++temporalCounter;
            //                        --openCounter;                                    
            //                        if (dendriticEndPoints.TryGetValue(pos.StringIDWithBID, out segid))
            //                            return new ConnectionType(CType.DendriteConnectedToAxon, segid);
            //                        break;
            //                    }
            //                default:
            //                    return new ConnectionType(CType.NotAvailable);
            //            }
            //            break;
            //        }
            //    case 'P'://Apical Axon Line
            //        {
            //            switch (eType)
            //            {
            //                case EndPointType.Dendrite:
            //                    {
            //                        DendriteClaim(pos, claimerSegID);
            //                        cMap[pos.BID, pos.X,pos.Y, pos.Z] = 'N';
            //                        ++apicalCounter;
            //                        --openCounter;
            //                        if (dendriticEndPoints.TryGetValue(pos.StringIDWithBID, out segid))
            //                            return new ConnectionType(CType.DendriteConnectedToAxon, segid);
            //                        break;
            //                    }
            //                default:
            //                    return new ConnectionType(CType.NotAvailable);
            //            }
            //        }
            //        break;
            //    case 'N': return new ConnectionType(CType.NotAvailable);
            //}

            #endregion

            if(IsPositionAvailable(pos))
            {

                if(eType.Equals(EndPointType.Dendrite)) //Dendrite trying to claim an empty position
                {
                    DendriteClaim(pos, claimerSegID);
                    
                }
                else if(eType.Equals(EndPointType.Axon)) //Axon trying to claim an empty position
                {
                    AxonClaim(pos, claimerSegID);                        
                }                                                                
            }
            else
            {
                if (eType.Equals(EndPointType.Dendrite)) //Dendrite trying to claim an empty position
                {
                    DendriteClaim(pos, claimerSegID);
                }
                else if (eType.Equals(EndPointType.Axon)) //Axon trying to claim an empty position
                {
                    AxonClaim(pos, claimerSegID);
                }
            }

            return new ConnectionType(CType.NotAvailable);
        }


        /// <summary>
        /// Returns the dendrites position 3D object that segments axon is connected to.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal DoubleSegment InterfaceFire(string position)
        { 
            //Called by axonal endpoints to extract if any dendrites are connected to it it ll fire them
            if(synapses.TryGetValue(position, out var doubleSegment))
            {
                if(PredictedSegments.TryGetValue(position, out var ds))
                {                                        
                    PredictedSegments[position].IncrementHitcount();
                }
                else
                {
                    PredictedSegments.Add(position, doubleSegment);
                }

                return doubleSegment;
            }

            Console.WriteLine("Invalid Synapse ID : Synapse does NOT exist at this position");

            return null;
        }

        //Axonal is always the Claimer and Dendrite is always the Claimee
        private CType AxonClaim(Position3D pos, SegmentID claimerSegID)
        {            
            SegmentID claimeeSegID;
            if (dendriticEndPoints.TryGetValue(pos.StringIDWithBID, out claimeeSegID))
            {
                //form a synapse and get rid of both values in the network.
                if (CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).NeuronID.StringIDWithBID.Equals(CPM.GetInstance.GetNeuronFromSegmentID(claimeeSegID).NeuronID.StringIDWithBID))
                {
                    return CType.NotAvailable;        //Selfing
                }
                

                CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).AddNewConnection(pos, claimerSegID);     //inform claimer segment - axon
                CPM.GetInstance.GetNeuronFromSegmentID(claimeeSegID).AddNewConnection(pos, claimeeSegID);     //inform claimee segment - dendrite

                dendriticEndPoints.Remove(pos.StringIDWithBID);      //Remove active dendritic connections from dendritic dictionary

                synapses.Add(pos.StringIDWithBID, new DoubleSegment(pos, claimerSegID, claimeeSegID));

                return CType.AxonConnectedToDendrite;

            }
            else
            {
                axonalEndPoints.Add(pos.StringIDWithBID, claimerSegID);

                return CType.SuccesfullyClaimedByAxon;
            }            
        }

        //Axon is always the claimer and dendrite is always the claimee
        private CType DendriteClaim(Position3D pos, SegmentID claimeeSegID)      //A dendrite is claiming an axon
        {            
            SegmentID claimerSegID;
            if (axonalEndPoints.TryGetValue(pos.StringIDWithBID, out claimerSegID))
            {
                if (CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).NeuronID.Equals(CPM.GetInstance.GetNeuronFromSegmentID(claimeeSegID).NeuronID))
                {
                    return CType.NotAvailable;
                    //Check for Selfing
                };  
                CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).AddNewConnection(pos, claimerSegID);     //inform claimer segment - dendrite
                CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).AddNewConnection(pos, claimerSegID);     //inform claimee segment - axon
                axonalEndPoints.Remove(pos.StringIDWithBID);                                                         //flush existing axon                   
                synapses.Add(pos.StringIDWithBID, new DoubleSegment(pos, claimerSegID, claimeeSegID));                    //add synapse

                return CType.DendriteConnectedToAxon;
            }
            else
            {
                dendriticEndPoints.Add(pos.StringIDWithBID, claimerSegID);
                cMap[pos.BID, pos.X,pos.Y, pos.Z] = 'D';

                return CType.SuccesfullyClaimedByDendrite;
            }
        }        
    }
}
