namespace HTM.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using HTM.Enums;
    using HTM.Models;

    public  class Transform
    {        
        private uint blockRadius;
        private uint num_rows_reg;
        private uint num_cols_reg;

        public Transform()
        {
            blockRadius = Convert.ToUInt32(ConfigurationManager.AppSettings["BLOCKRADIUS"]);
            num_rows_reg = Convert.ToUInt32(ConfigurationManager.AppSettings["NUMBLOCKSPERROW"]);
            num_cols_reg = Convert.ToUInt32(ConfigurationManager.AppSettings["NUMBLOCKSPERCOL"]);

            //based on above values , initialize and populate all the basic block modulos.

        //       XL_BB_Mods = XR_BB_Mods = YU_BB_Mod = YD_BB_Mod = ZF_BB_Mod = ZB_BB_Mod = 0;
        }

        private bool XL_BB_Mods(uint blockid) => (blockid % num_cols_reg == 0);
        private bool XR_BB_Mods(uint blockid) => (blockid % (num_cols_reg -1)) == 0;
        private bool YU_BB_Mods(uint blockid) => (((num_cols_reg * num_rows_reg) - num_cols_reg) <= blockid) ? blockid < (num_cols_reg * num_rows_reg) ? true : false : false;
        private bool YD_BB_Mods(uint blockid) => (0 <= blockid ? blockid < num_cols_reg ? true : false : false);
        private bool ZF_BB_Mods(uint blockid) => (0 <= blockid ? 0 < num_rows_reg * num_cols_reg ? true : false : false);
        private bool ZB_BB_Mods(uint blockid) => ((num_cols_reg * num_rows_reg * num_rows_reg) - (num_rows_reg * num_cols_reg) <= blockid ? blockid < (num_rows_reg * num_cols_reg) ? true : false : false);

        public Position3D ComputeNPredictNewPosition(Position3D pos) 
        {
            /*
             * Basis Block 
             *  if yes
             *  then do random square within the block or not check 
             *    if yes 
             *    then compute the intervals and generate new random positions for x,y,z & return new position
             *    else no
             *    then assign (0) for the respective basis block dimension and generate radom interval numbers for the rest of the dimension
             *  else no
             *  then compute the intervals and generate new random positions for x,y,z & return new position
             * */
            Position3D newPosition = new Position3D(0,0,0); 
            uint b = pos.BlockID;
            if (XL_BB_Mods(b) && RSBCheckX(pos))
            {
                newPosition.BlockID = pos.BlockID;
                newPosition.X = 0;
            }
                        
        }

        private bool RSBCheckX(Position3D pos) => pos.X - blockRadius > 0 ? true : false;

        private bool RSBCheckY(Position3D pos) => pos.Y - blockRadius > 0 ? true : false;

        private bool RSBCheckZ(Position3D pos) => pos.Z - blockRadius > 0 ? true : false;
        
    }
}
