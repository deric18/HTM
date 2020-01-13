namespace HTM.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using HTM.Enums;
    using HTM.Models;

    public  class Transform
    {
        private List<uint> XL_BassisBlocks;
        private List<uint> XR_BassisBlocks;
        private List<uint> YU_BasisBlocks;
        private List<uint> YD_BasisBlocks;
        private List<uint> ZF_BasisBlocks;
        private List<uint> ZB_BasisBlocks;
        private uint blockRadius;
        private uint num_rows_reg;
        private uint num_cols_reg;

        public Transform()
        {
            blockRadius = Convert.ToUInt32(ConfigurationManager.AppSettings["BLOCKRADIUS"]);
            num_rows_reg = Convert.ToUInt32(ConfigurationManager.AppSettings["NUMBLOCKSPERROW"]);
            num_cols_reg = Convert.ToUInt32(ConfigurationManager.AppSettings["NUMBLOCKSPERCOL"]);            
            
            //based on above values , initialize and populate all the basic block lists.


        }
        public Position3D ComputeNPredictNewPosition(Position3D pos)
        {
            //Figure out basis blocks if any?
            //if so set corresponding coordiantes
            //predict non basis block coordinates
            //return new position



        }
    }
}
