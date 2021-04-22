namespace HTM.Algorithms
{
    using HTM.Models;
    /// <summary>
    /// Needed for scenarios to find out which synapses and neurons need to be supplied with voltage when a presynaptic neuron fired
    /// </summary>
    public class DoubleSegment
    {
        public Position3D axonID { get; set; }
        public SegmentID dendriteID { get; set; }

        public DoubleSegment(Position3D Claimer, SegmentID Connector)
        {
            this.axonID = Claimer;
            this.dendriteID = Connector;
        }        
    }
}
