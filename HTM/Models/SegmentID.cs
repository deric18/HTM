/*
 * Segment ID Syntax : <<Neuron Position3D>--<Segment Number>--<Segment Position3D>> 
 */

namespace HTM.Models
{    
    public class SegmentID : Position3D
    {
        private uint SegmentNumber;
        private Position3D NeuronId;
        private Position3D Position;

        internal SegmentID(Position3D neuronId, uint segNum, Position3D newPosition) : base(newPosition)
        {
            NeuronId = neuronId;
            SegmentNumber = segNum;
            Position = newPosition;
        }        

        internal string GetSegmentID() => NeuronId.StringID + "--" + SegmentNumber.ToString() + "--" + Position.StringID;
    }
}
