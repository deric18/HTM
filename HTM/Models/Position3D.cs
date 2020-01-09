namespace HTM.Models
{
    //3Dimensional cordinate system to pin point neuronal synapses
    public class Synapse
    {               
        public uint BlockID { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Z { get; set; }

        public Synapse() { }

        public Synapse(Synapse pos, uint blockID)
        {
            BlockID = blockID;
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }

        public Synapse(uint x, uint y, uint z)
        {            
            X = x;
            Y = y;
            Z = z;            
        }       

        public  bool Equals(Synapse segId) =>         
            this.X.Equals(segId.X) && this.Y.Equals(segId.Y) && this.Z.Equals(segId.Z);                    
    }
}

