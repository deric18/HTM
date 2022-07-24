﻿using System;
using System.Collections.Generic;
using HTM.Enums;

namespace HTM.Models
{
    //3Dimensional cordinate system to pin point neuronal synapses
    public class Position3D : IEquatable<Position3D>, ICloneable
    {                       
        public uint BID { get; set; }
        public uint X { get; set; }         //Position Index within the block 
        public uint Y { get; set; }         //Position Index within the block
        public uint Z { get; set; }         //Position Index within the block
        public CType cType { get; set; }    //CType for the Connection Point

        public Position3D() { }

        internal Position3D(Position3D pos) { }
        public Position3D(uint x, uint y, uint z)
        {
            X = x;
            Y = y;
            Z = z;
            this.BID = ComputeBID(x,Y,z);
            cType = CType.Available;
        }

        public Position3D(uint x, uint y, uint z, uint blockId)
        {            
            X = x;
            Y = y;
            Z = z;
            if(blockId == ComputeBID(x,Y,Z))
                this.BID = blockId;
            else
                this.BID = ComputeBID(x,Y,Z);
            cType = CType.Available;
        }
      

        public object Clone()
        {
            return new Position3D()
            {
                BID = this.BID,
                X = this.X,
                Y = this.Y,
                Z = this.Z,
                cType = this.cType
            };
        }

        //TODO : Fix the computation of the BID Bug
        private uint ComputeBID(uint x, uint y, uint z) =>              //Computes BlockID : VERIFIED
        (z * CPM.GetInstance.NumX * CPM.GetInstance.NumY + y * CPM.GetInstance.NumX + x);

        public  bool Equals(Position3D segId) =>         
            this.X.Equals(segId.X) && this.Y.Equals(segId.Y) && this.Z.Equals(segId.Z) && this.BID.Equals(segId.BID);

        internal string StringIDWithBID =>          //Only this is unqiue
            X.ToString() + "-" + Y.ToString() + "-" + Z.ToString() + "-" + BID.ToString();

        internal string StringIDWithoutBID =>       
            X.ToString() + "-" + Y.ToString() + "-" + Z.ToString();
        
        internal static Position3D GetPositionFromString(string str)
        {
            string[] carr = str.Split('-');
            Position3D pos;
            if(carr.Length == 4)
            {
                pos = new Position3D(Convert.ToUInt32(carr[0]), Convert.ToUInt32(carr[3]), Convert.ToUInt32(carr[2]), Convert.ToUInt32(carr[3]));
            }
            else
            {
                throw new Exception("Unable to convert position string to Position3D object");
            }

            return pos;
        }
        
    }
}

