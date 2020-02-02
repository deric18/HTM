﻿//TODO : Complete Apical and Temporal axon lines.

using HTM.Enums;
using HTM.Models;
using System;
using System.Collections.Generic;

namespace HTM.Algorithms
{
    public class ConnectionTable
    {
        private char[,][] cMap;            //keeps track of every connection point in the 
        private ulong openCounter;
        private uint closedCounter;
        private uint temporalCounter;
        private uint apicalCounter;
        private Dictionary<string, SegmentID> axonalEndPoints;
        private Dictionary<string, SegmentID> dendriticEndPoints;
        private Dictionary<string, DoubleSegment> synapses;
        private static ConnectionTable synapseTable;

        private ConnectionTable(uint x, uint y, uint z)
        {
            openCounter = closedCounter = temporalCounter = closedCounter = 0;
            axonalEndPoints = new Dictionary<string, SegmentID>();          //holds all the due axonal connections.
            dendriticEndPoints = new Dictionary<string, SegmentID>();       //<position3d , corresponding segment id of the dendrite>.
            synapses = new Dictionary<string, DoubleSegment>();             //Holds all the synapses with respective synapses ID's.
            cMap = new char[x, y][];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    for (int k = 0; k < z; k++)                        
                        cMap[i, j][k] = 'A';

            //Draw Temporal Lines here


            //Draw Apical Lines here


        }

        /*TO Implement:
         * -when cpm processes a neuronal fire it gives all the positions to which the neuron fires , then cpm needs to get all thos positions and find out if there are any neurons connecting to the specific position and if so get there segment ids and tra
         *  potential to those segments
         */

        public static ConnectionTable Singleton(uint x = 0, uint y = 0, uint z = 0)
        {
            if(synapseTable == null)           
                synapseTable = new ConnectionTable(x, y, z);            
            return synapseTable;
        }

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
            switch (cMap[pos.X, pos.Y][pos.Z])
            {
                case 'A': //Available                   
                    switch (eType)
                    {
                        case EndPointType.Axon:
                            AxonClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z] = 'A';
                            ++openCounter;
                            return new ConnectionType(CType.SuccesfullyOccupied);                            
                        case EndPointType.Dendrite:
                            DendriteClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z] = 'D';
                            ++openCounter;
                            return new ConnectionType(CType.SuccesfullyOccupied);

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
                            if(dendriticEndPoints.TryGetValue(pos.GetString(), out segid))
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
                                cMap[pos.X, pos.Y][pos.Z] = 'N';
                                --openCounter;
                                ++closedCounter;                                
                                if (dendriticEndPoints.TryGetValue(pos.GetString(), out segid))
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
                                    cMap[pos.X, pos.Y][pos.Z] = 'N';
                                    ++temporalCounter;
                                    --openCounter;                                    
                                    if (dendriticEndPoints.TryGetValue(pos.GetString(), out segid))
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
                                    cMap[pos.X, pos.Y][pos.Z] = 'N';
                                    ++apicalCounter;
                                    --openCounter;
                                    if (dendriticEndPoints.TryGetValue(pos.GetString(), out segid))
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

        private void AxonClaim(Position3D pos, SegmentID claimerSegID)
        {
            SegmentID seg;
            if (dendriticEndPoints.TryGetValue(pos.GetString(), out seg))
            {
                CPM.GetInstance.AddConnection(seg, pos);         //inform dendritic segment
                CPM.GetInstance.AddConnection(claimerSegID, pos);//inform claiming segment
            }
        }

        private void DendriteClaim(Position3D pos, SegmentID claimerSegID)
        {
            cMap[pos.X, pos.Y][pos.Z] = 'N';
            SegmentID seg;
            if (axonalEndPoints.TryGetValue(pos.GetString(), out seg))
            {
                CPM.GetInstance.AddConnection(seg, pos);         //inform dendritic segment
                CPM.GetInstance.AddConnection(claimerSegID, pos);//inform claiming segment
            }
        }


    }
}
