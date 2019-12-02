using HTM.Enums;
using HTM.Models;
using System.Collections.Generic;

namespace HTM.Algorithms
{
    public class ConnectionTable
    {
        private char[,][,] cMap;            //keeps track of every connection point in the 
        private ulong counter;
        private Dictionary<SegmentID, bool> axonalEndPoints;            //True for Axon , False for Dendrite
        private Dictionary<Position4D, SegmentID> dendriticEndPoints;
        private Dictionary<uint, List<SegmentID>> temporalLines;
        private Dictionary<uint, List<SegmentID>> axonalLines;

        public ConnectionTable(uint w, uint x, uint y, uint z)
        {
            counter = 0;
            axonalEndPoints = new Dictionary<SegmentID, bool>();
            dendriticEndPoints = new Dictionary<Position4D, SegmentID>();
              cMap = new char[x, y][, ];
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
        /// <returns></returns>
        public ConnectionType ClaimPosition(Position4D pos, EndPointType eType, SegmentID claimerSegID)
        {
            switch(cMap[pos.X,pos.Y][pos.Z,pos.PosID])
            {
                case 'A': //Available                   
                    switch(eType)
                    {
                        case EndPointType.Axon: AxonClaim(pos);break;
                        case EndPointType.Dendrite: DendriteClaim(pos);break;
                        default:break;
                    }
                    break;
                case 'D'://Dendrite
                    switch(eType)
                    {
                        case EndPointType.Axon://Axon claiming a dendritc position 
                            SegmentID seg;
                            if(dendriticEndPoints.TryGetValue(pos, out seg))
                            {
                                CPM.GetInstance.AddConnection(seg, pos);
                                CPM.GetInstance.AddConnection(claimerSegID, pos);
                            }
                            break;
                        default:
                            return ConnectionType.NotAvailable;                            
                    }
                    break;
                case 'a':
                    switch (eType)
                    {
                        //Inform both neurons of the established connection and mark the character to unavilable
                    }
                    break;//Axon
                case 'N':break;//Not Available
                case 'T':break;//Temporal Line
                case 'P':break;//Apical Axon Line
            }
        }
        
        public void AxonClaim(Position4D pos)
        {
            bool val = false;
            if(!internalConnections.TryGetValue(pos, out val))
            {
                internalConnections.Add(pos, true);
            }
        }

        public void DendriteClaim(Position4D pos)
        {
            cMap[pos.X, pos.Y][pos.Z, pos.PosID] = 'D';
        }
                   
    
    }
}
