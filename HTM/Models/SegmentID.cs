/*
 * Segment ID Syntax : <<Neuron Position3D>--<Segment Number>--<Segment Position3D>> 
 */

namespace HTM.Models
{    
    public class SegmentID : Position3D
    {
        public uint SegmentNumber { get; private set; }
        public Position3D NeuronId { get; private set; }
        public Position3D BasePosition { get; private set; }

        internal SegmentID(Position3D neuronId, uint segNum, Position3D basePositionSegment) : base(basePositionSegment)
        {
            NeuronId = neuronId;
            SegmentNumber = segNum;
            BasePosition = basePositionSegment;
        }        

        internal string GetSegmentID() => NeuronId.StringIDWithoutBID + "--" + SegmentNumber.ToString() + "--" + BasePosition.StringIDWithoutBID;
    }
}
