//TODO : Complete Apical and Temporal axon lines.

using HTM.Enums;
using HTM.Models;
using System.Collections.Generic;

namespace HTM.Algorithms
{
    public class SynapseTable
    {
        private char[,][] cMap;            //keeps track of every connection point in the 
        private ulong openCounter;
        private uint closedCounter;
        private uint temporalCounter;
        private uint apicalCounter;
        private Dictionary<Synapse, SegmentID> axonalEndPoints;
        private Dictionary<Synapse, SegmentID> dendriticEndPoints;
        private static SynapseTable synapseTable;

        private SynapseTable(uint x, uint y, uint z)
        {
            openCounter = closedCounter = temporalCounter = closedCounter = 0;
            axonalEndPoints = new Dictionary<Synapse, SegmentID>();
            dendriticEndPoints = new Dictionary<Synapse, SegmentID>();
            cMap = new char[x, y][];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    for (int k = 0; k < z; k++)                        
                        cMap[i, j][k] = 'A';

            //Draw Temporal Lines here


            //Draw Apical Lines here


        }

        public static SynapseTable Singleton(uint x = 0, uint y = 0, uint z = 0)
        {
            if(synapseTable == null)           
                synapseTable = new SynapseTable(x, y, z);            
            return synapseTable;
        }

        /// <summary>
        /// Position Claimer        
        /// </summary>
        /// <param name="pos">Position that is under claim investigation</param>
        /// <param name="claimerSegID">SegmentID</param>
        /// <param name="eType">EndpointType eType</param>
        /// <returns>Connection Result</returns>
        public ConnectionType ClaimPosition(Synapse pos, SegmentID claimerSegID, EndPointType eType)
        {
            /// Scenario 1: If the claimer is a dendrite and the position is empty we just give the position to them.
            /// Scenario 2: If the claimer is a axon and the position is empty we add the segment id to the dict and mark the position as 'A'.
            /// Scenario 3: If the claimer is a dendrite and the positio9n is occupied by an axonal seg then check for selfing , remove the axonal seg id from the dict ,mark the position as 'N' in cMap and return the seg id.
            /// Scenario 4: If the claimer is a axon and the position is occupied by the dendrite then we again check for selfing, and send connect signal to claimed neuron, mark the position and return bind signal to claiming neuron.
            switch (cMap[pos.X, pos.Y][pos.Z])
            {
                case 'A': //Available                   
                    switch (eType)
                    {
                        case EndPointType.Axon:
                            AxonClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z] = 'A';
                            ++openCounter;
                            break;
                        case EndPointType.Dendrite:
                            DendriteClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z] = 'D';
                            ++openCounter;
                            break;

                        default: break;
                    }
                    ++openCounter;
                    break;
                case 'D'://Dendrite
                    switch (eType)
                    {
                        case EndPointType.Axon://Axon claiming a dendritc position 
                            AxonClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z] = 'N';
                            --openCounter;
                            ++closedCounter;
                            return ConnectionType.ConnectedToDendrite;
                        default:
                            return ConnectionType.NotAvailable;
                    }
                case 'a'://Axon
                    switch (eType)
                    {
                        case EndPointType.Dendrite:
                            {
                                DendriteClaim(pos, claimerSegID);
                                cMap[pos.X, pos.Y][pos.Z] = 'N';
                                --openCounter;
                                ++closedCounter;
                                break;
                            }
                        default:
                            return ConnectionType.NotAvailable;
                    }
                    break;
                case 'T'://Temporal Axon Line
                    {
                        switch (eType)
                        {
                            case EndPointType.Dendrite:
                                {
                                    DendriteClaim(pos, claimerSegID);
                                    cMap[pos.X, pos.Y][pos.Z] = 'N';
                                    ++temporalCounter;
                                    --openCounter;
                                    return ConnectionType.ConnectedToAxon;
                                }
                            default:
                                return ConnectionType.NotAvailable;
                        }
                    }
                case 'P'://Apical Axon Line
                    {
                        switch (eType)
                        {
                            case EndPointType.Dendrite:
                                {
                                    DendriteClaim(pos, claimerSegID);
                                    cMap[pos.X, pos.Y][pos.Z] = 'N';
                                    ++apicalCounter;
                                    --openCounter;
                                    return ConnectionType.ConnectedToAxon;
                                }
                            default:
                                return ConnectionType.NotAvailable;
                        }
                    }
                case 'N': return ConnectionType.NotAvailable; 
            }

            return ConnectionType.NotAvailable;
        }

        private void AxonClaim(Synapse pos, SegmentID claimerSegID)
        {
            SegmentID seg;
            if (dendriticEndPoints.TryGetValue(pos, out seg))
            {
                SynapseManager.GetInstance.AddConnection(seg, pos);         //inform dendritic segment
                SynapseManager.GetInstance.AddConnection(claimerSegID, pos);//inform claiming segment
            }
        }

        private void DendriteClaim(Synapse pos, SegmentID claimerSegID)
        {
            cMap[pos.X, pos.Y][pos.Z] = 'N';
            SegmentID seg;
            if (axonalEndPoints.TryGetValue(pos, out seg))
            {
                SynapseManager.GetInstance.AddConnection(seg, pos);         //inform dendritic segment
                SynapseManager.GetInstance.AddConnection(claimerSegID, pos);//inform claiming segment
            }
        }


    }
}
