namespace HTM.Algorithms
{
    using HTM.Models;
    public class DoubleSegment
    {
        public SegmentID Claimer { get; set; }
        public SegmentID Connector { get; set; }

        public DoubleSegment(SegmentID Claimer, SegmentID Connector)
        {
            this.Claimer = Claimer;
            this.Connector = Connector;
        }
    }
}
