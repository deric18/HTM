namespace HTM.Algorithms
{
    using HTM.Models;
    /// <summary>
    /// Needed for scenarios to find out which synapses and neurons need to be supplied with voltage when a presynaptic neuron fired
    /// </summary>
    public class DoubleSegment
    {
        public SegmentID axonalSegmentID { get; private set; }
        public Position3D synapsePosition { get; private set; }
        public SegmentID dendriteISegmentD { get; private set; }

        public DoubleSegment(Position3D synapsePosition, SegmentID axonalSegmentID, SegmentID dendriticSegmentID)
        {
            this.synapsePosition = synapsePosition;
            this.axonalSegmentID = axonalSegmentID;
            this.dendriteISegmentD = dendriticSegmentID;
        }        
    }
}
