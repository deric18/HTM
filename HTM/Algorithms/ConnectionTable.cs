using HTM.Enums;
using HTM.Models;
using System.Collections.Generic;

namespace HTM.Algorithms
{
    public class ConnectionTable
    {
        private char[,][,] cMap;            //keeps track of every connection point in the 
        private ulong counter;
        private Dictionary<Position4D, SegmentID> axonalEndPoints;
        private Dictionary<Position4D, SegmentID> dendriticEndPoints;

        public ConnectionTable(uint w, uint x, uint y, uint z)
        {
            counter = 0;
            axonalEndPoints = new Dictionary<Position4D, SegmentID>();
            dendriticEndPoints = new Dictionary<Position4D, SegmentID>();
            cMap = new char[x, y][,];
            for (int i = 0; i < w; i++)
                for (int j = 0; j < x; j++)
                    for (int k = 0; k < y; k++)
                        for (int l = 0; l < z; k++)
                            cMap[i, j][k, l] = 'A';

            //Draw Temporal Lines here


            //Draw Apical Lines here


        }

        /// <summary>
        /// Position Claimer
        /// Scenario 1: If the claimer is a dendrite and the position is empty we just give the position to them.
        /// Scenario 2: If the claimer is a axon and the position is empty we add the segment id to the dict and mark the position as 'A'.
        /// Scenario 3: If the claimer is a dendrite and the positio9n is occupied by an axonal seg then check for selfing , remove the axonal seg id from the dict ,mark the position as 'N' in cMap and return the seg id.
        /// Scenario 4: If the claimer is a axon and the position is occupied by the dendrite then we again check for selfing, and send connect signal to claimed neuron, mark the position and return bind signal to claiming neuron.
        /// </summary>
        /// <param name="pos">Position that is under claim investigation</param>
        /// <param name="cType">Connection Type</param>
        /// <returns>Connection Result</returns>
        public ConnectionType ClaimPosition(Position4D pos, SegmentID claimerSegID, EndPointType eType)
        {
            switch (cMap[pos.X, pos.Y][pos.Z, pos.PosID])
            {
                case 'A': //Available                   
                    switch (eType)
                    {
                        case EndPointType.Axon:
                            AxonClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'A';
                            break;
                        case EndPointType.Dendrite:
                            DendriteClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'D';
                            break;

                        default: break;
                    }
                    break;
                case 'D'://Dendrite
                    switch (eType)
                    {
                        case EndPointType.Axon://Axon claiming a dendritc position 
                            AxonClaim(pos, claimerSegID);
                            cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'N';
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
                                cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'N';
                                break;
                            }
                        default:
                            return ConnectionType.NotAvailable;
                    }
                    break;
                case 'T'://Temporal
                    {
                        switch (eType)
                        {
                            case EndPointType.Dendrite:
                                {
                                    DendriteClaim(pos, claimerSegID);
                                    cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'N';
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
                                    cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'N';
                                    return ConnectionType.ConnectedToAxon;
                                }
                            default:
                                return ConnectionType.NotAvailable;
                        }
                    }
                case 'N': return ConnectionType.NotAvailable; 
            }
        }

        public void AxonClaim(Position4D pos, SegmentID claimerSegID)
        {
            SegmentID seg;
            if (dendriticEndPoints.TryGetValue(pos, out seg))
            {
                CPM.GetInstance.AddConnection(seg, pos);         //inform dendritic segment
                CPM.GetInstance.AddConnection(claimerSegID, pos);//inform claiming segment
            }
        }

        public void DendriteClaim(Position4D pos, SegmentID claimerSegID)
        {
            cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'N';
            SegmentID seg;
            if (axonalEndPoints.TryGetValue(pos, out seg))
            {
                CPM.GetInstance.AddConnection(seg, pos);         //inform dendritic segment
                CPM.GetInstance.AddConnection(claimerSegID, pos);//inform claiming segment
            }
        }


    }
}
