namespace HTM.Algorithms
{
    using HTM.Models;
    public class DoubleSegment
    {
        public Position3D axonID { get; set; }
        public SegmentID dendriteID { get; set; }

        public DoubleSegment(Position3D Claimer, SegmentID Connector)
        {
            this.axonID = Claimer;
            this.dendriteID = Connector;
        }

        internal void InterfaceFire()
        {
            CPM.GetInstance.GetNeuronFromSegmentID();
        }
    }
}
