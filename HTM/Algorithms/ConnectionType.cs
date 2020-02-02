namespace HTM.Algorithms
{
    using HTM.Enums;
    using HTM.Models;

    public class ConnectionType
    {
        public CType ConType { get; set; }
        public SegmentID connectedSegmentID { get; set; }

        public ConnectionType(CType ctype, SegmentID segid)
        {
            this.ConType = ctype;
            this.connectedSegmentID = segid;
        }

        public ConnectionType(CType ctype)
        {
            this.ConType = ctype;
            this.connectedSegmentID = null;
        }

    }
}
