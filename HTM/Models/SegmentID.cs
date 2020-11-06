/*
 * Segment ID Syntax : <<Neuron Position3D>--<Segment Number>--<Segment Position3D>> 
 */

namespace HTM.Models
{    
    public class SegmentID : Position3D
    {
        public uint SegmentNumber { get; private set; }
        public Position3D NeuronId { get; private set; }
        public Position3D Position { get; private set; }

        internal SegmentID(Position3D neuronId, uint segNum, Position3D newPosition) : base(newPosition)
        {
            NeuronId = neuronId;
            SegmentNumber = segNum;
            Position = newPosition;
        }        

        internal string GetSegmentID() => NeuronId.StringID + "--" + SegmentNumber.ToString() + "--" + Position.StringID;
    }
}
