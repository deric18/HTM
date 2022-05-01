/*
 * Segment ID Syntax : <<Neuron Position3D>--<Segment Number>--<Segment Base Position-Position3D>> 
 */

namespace HTM.Models
{    
    /// <summary>
    /// Used to uniquely represent every single Segment in the Region
    /// </summary>
    public class SegmentID : Position3D
    {
        //Should also be able to include sub segments.
        public uint SegmentNumber { get; private set; }
        public Position3D NeuronId { get; private set; }
        public Position3D BasePosition { get; private set; }
        public string LineageString { get; private set; }

        public bool IsSubSegment { get; private set; }

        internal SegmentID(Position3D neuronId, uint segNum, Position3D basePositionSegment) : base(basePositionSegment)
        {
            NeuronId = neuronId;
            SegmentNumber = segNum;
            BasePosition = basePositionSegment;
            IsSubSegment = false;
            LineageString = null;
        }        

        internal SegmentID(Position3D neuronId, uint segNum, Position3D basePositionSegment, string LineageString) : base(basePositionSegment)
        {
            NeuronId = neuronId;
            SegmentNumber = segNum;
            BasePosition = basePositionSegment;
            IsSubSegment = true;
            this.LineageString = LineageString;
        }


        internal string GetSegmentID() => NeuronId.StringIDWithBID + "/" + SegmentNumber.ToString() + "/" + BasePosition.StringIDWithBID;
    }
}
