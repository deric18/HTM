using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM.Models
{
    //2-Dimensional Cartesian cordinate system
    public class Position2D : IEquatable<Position3D>
    {
        public uint X { get; private set; }
        public uint Y { get; private set; }


        public Position2D(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public string GetString()
        {
            return X.ToString() + Y.ToString();
        }

        public bool Equals(Position3D nextPos)
        {
            return this.X == nextPos.X && this.Y == nextPos.Y ? true : false;
        }
    }
}
