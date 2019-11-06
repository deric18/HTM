using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM.Models
{
    /// <summary>
    /// Vector = Scale + Direction
    /// </summary>
    public class Vector
    {
        //banking angle
        public uint BankingAngle { get; private set; }
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint Z { get; private set; }

        public Vector() { }

        public Vector(uint x, uint y, uint z, uint bankingangle)
        {
            BankingAngle = bankingangle;
            X = x;
            Y = y;
            Z = z;
        }

    }
}
