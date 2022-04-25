//TODO : Complete Apical and Temporal axon lines.
//Mark the neuron positions also as NotFiniteNumberException available and mark them as 'D'
using HTM.Enums;
using HTM.Models;
using System;
using System.Collections.Generic;

namespace HTM.Algorithms
{
    public class ConnectionTable
    {
        private char[,][,] cMap;            //keeps track of every connection point in the 
        private ulong openCounter;
        private uint closedCounter;
        private uint temporalCounter;
        private uint apicalCounter;
        private Dictionary<string, SegmentID> axonalEndPoints;          //holds only unconnected axonal ( active axonal connections)
        private Dictionary<string, SegmentID> dendriticEndPoints;       //holds only unconnected dendrites ( active dendritic connections)
        private Dictionary<string, DoubleSegment> synapses;
        public static ConnectionTable SingleTon;


        public static ConnectionTable Singleton(uint w = 0, BlockConfigProvider bcp = null)
        {
            if (SingleTon == null)
                SingleTon = new ConnectionTable(w, bcp);
            return SingleTon;
        }

        private ConnectionTable(uint numBlocks, BlockConfigProvider bcp)
        {
            openCounter = closedCounter = temporalCounter = closedCounter = 0;
            axonalEndPoints = new Dictionary<string, SegmentID>();          //holds all the due axonal connections.
            dendriticEndPoints = new Dictionary<string, SegmentID>();       //<position3d , corresponding segment id of the dendrite>.
            synapses = new Dictionary<string, DoubleSegment>();             //Holds all the synapses with respective synapses ID's.
            cMap = new char[numBlocks, bcp.NumXperBlock][,];


            for (uint block = 0; block < numBlocks; block++)
                for (uint i = 0; i < bcp.NumXperBlock; i++)
                    for (uint j = 0; j < bcp.NumYperBlock; j++)
                        for (uint k = 0; k < bcp.NumZperBlock; k++)
                            cMap[block, i][j, k] = 'A';
            

        }

        /*TO Implement:
         * -when cpm processes a neuronal fire it gives all the positions to which the neuron fires , then cpm needs to get all thos positions and find out if there are any neurons connecting to the specific position and if so get there segment ids and tra
         *  potential to those segments
         *  CPM Constants:
         *  A = Available , D - Occupied by Dendrite , a - Occupied by axon , N - Not Available , 
         
             */

        public char Position(uint blockID, uint x, uint y, uint z) => cMap[blockID, x][y, z];

        public bool IsPositionAvailable(Position3D pos) => (cMap[pos.BID, pos.X][pos.Y, pos.Z] == 'A');
                
        /// <summary>
        /// Position Claimer        
        /// </summary>
        /// <param name="pos">Position that is under claim investigation</param>
        /// <param name="claimerSegID">SegmentID</param>
        /// <param name="eType">EndpointType eType</param>
        /// <returns>ConnectionType to</returns>
        public ConnectionType ClaimPosition(Position3D pos, SegmentID claimerSegID, EndPointType eType)
        {
            /// Scenario 1: If the claimer is a dendrite and the position is empty we just give the position to them.
            /// Scenario 2: If the claimer is a axon and the position is empty we add the segment id to the dict and mark the position as 'A'.
            /// Scenario 3: If the claimer is a dendrite and the positio9n is occupied by an axonal seg then check for selfing , remove the axonal seg id from the dict ,mark the position as 'N' in cMap and return the seg id.
            /// Scenario 4: If the claimer is a axon and the position is occupied by the dendrite then we again check for selfing, and send connect signal to claimed neuron, mark the position and return bind signal to claiming neuron.
            SegmentID segid;
            switch (cMap[pos.BID, pos.X][pos.Y,pos.Z])
            {
                case 'A': //Available                   
                    switch (eType)
                    {
                        case EndPointType.Axon:
                            AxonClaim(pos, claimerSegID);
                            cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'A';
                            ++openCounter;
                            return new ConnectionType(CType.SuccesfullyClaimed);                            
                        case EndPointType.Dendrite:
                            DendriteClaim(pos, claimerSegID);
                            cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'D';
                            ++openCounter;
                            return new ConnectionType(CType.SuccesfullyClaimed);

                        default: break;
                    }
                    ++openCounter;                    
                    break;
                case 'D'://Dendrite
                    switch (eType)
                    {
                        case EndPointType.Axon://Axon claiming a dendritc position 
                            AxonClaim(pos, claimerSegID);
                            cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'N';
                            --openCounter;
                            ++closedCounter;                            
                            if(dendriticEndPoints.TryGetValue(pos.StringIDWithoutBID, out segid))
                                return new ConnectionType(CType.ConnectedToDendrite, segid);
                            break;  
                        default:
                            return new ConnectionType(CType.NotAvailable);
                    }
                    break;
                case 'a'://Axon
                    switch (eType)
                    {
                        case EndPointType.Dendrite:     //dendrite claiming an axon
                            {
                                DendriteClaim(pos, claimerSegID);
                                cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'N';
                                --openCounter;
                                ++closedCounter;                                
                                if (dendriticEndPoints.TryGetValue(pos.StringIDWithoutBID, out segid))
                                    return new ConnectionType(CType.ConnectedToDendrite, segid);
                                break;
                            }
                        default:
                            return new ConnectionType(CType.NotAvailable);
                    }
                    break;
                case 'T'://Temporal Axon Line
                    {
                        switch (eType)
                        {
                            case EndPointType.Dendrite:
                                {
                                    DendriteClaim(pos, claimerSegID);
                                    cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'N';
                                    ++temporalCounter;
                                    --openCounter;                                    
                                    if (dendriticEndPoints.TryGetValue(pos.StringIDWithoutBID, out segid))
                                        return new ConnectionType(CType.ConnectedToAxon, segid);
                                    break;
                                }
                            default:
                                return new ConnectionType(CType.NotAvailable);
                        }
                        break;
                    }
                case 'P'://Apical Axon Line
                    {
                        switch (eType)
                        {
                            case EndPointType.Dendrite:
                                {
                                    DendriteClaim(pos, claimerSegID);
                                    cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'N';
                                    ++apicalCounter;
                                    --openCounter;
                                    if (dendriticEndPoints.TryGetValue(pos.StringIDWithoutBID, out segid))
                                        return new ConnectionType(CType.ConnectedToAxon, segid);
                                    break;
                                }
                            default:
                                return new ConnectionType(CType.NotAvailable);
                        }
                    }
                    break;
                case 'N': return new ConnectionType(CType.NotAvailable);
            }

            return new ConnectionType(CType.NotAvailable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal SegmentID InterfaceFire(string position)
        {
            if(synapses.TryGetValue(position, out DoubleSegment doubleObject))
            {
                return synapses[position].dendriteID;
            }

            return null;
        }

        private void AxonClaim(Position3D pos, SegmentID claimerSegID)
        {            
            SegmentID claimeeSegID;
            if (dendriticEndPoints.TryGetValue(pos.StringIDWithoutBID, out claimeeSegID))
            {
                //form a synapse and get rid of both values in the network.
                if (CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).NeuronID.StringIDWithoutBID.Equals(CPM.GetInstance.GetNeuronFromSegmentID(claimeeSegID).NeuronID.StringIDWithoutBID)) return;        //Selfing

                CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).AddNewConnection(pos, claimerSegID);     //inform claimer segment - axon
                CPM.GetInstance.GetNeuronFromSegmentID(claimeeSegID).AddNewConnection(pos, claimeeSegID);     //inform claimee segment - dendrite

                dendriticEndPoints.Remove(pos.StringIDWithoutBID);      //Remove active dendritic connections from dendritic dictionary

                synapses.Add(pos.StringIDWithoutBID, new DoubleSegment(claimerSegID, claimeeSegID));

                cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'N';

            }
            else
            {
                axonalEndPoints.Add(pos.StringIDWithoutBID, claimerSegID);
                cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'a';
            }            
        }

        //Axon is always the claimer and dendrite is always the claimee
        private void DendriteClaim(Position3D pos, SegmentID claimeeSegID)      //A dendrite is claiming an axon
        {            
            SegmentID claimerSegID;
            if (axonalEndPoints.TryGetValue(pos.StringIDWithoutBID, out claimerSegID))
            {
                if (CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).NeuronID.Equals(CPM.GetInstance.GetNeuronFromSegmentID(claimeeSegID).NeuronID)) return;        //Check for Selfing
                CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).AddNewConnection(pos, claimerSegID);     //inform claimer segment - dendrite
                CPM.GetInstance.GetNeuronFromSegmentID(claimerSegID).AddNewConnection(pos, claimerSegID);     //inform claimee segment - axon
                axonalEndPoints.Remove(pos.StringIDWithoutBID);                                                         //flush existing axon                   
                synapses.Add(pos.StringIDWithoutBID, new DoubleSegment(claimerSegID, claimeeSegID));                    //add synapse
                cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'N';                                                     //refresh connection point
            }
            else
            {
                dendriticEndPoints.Add(pos.StringIDWithoutBID, claimerSegID);
                cMap[pos.BID, pos.X][pos.Y, pos.Z] = 'd';
            }
        }        
    }
}
