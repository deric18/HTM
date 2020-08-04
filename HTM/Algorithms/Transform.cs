﻿namespace HTM.Algorithms
{
    using System;
    using System.Configuration;
    using HTM.Models;

    //Notes: Make sure to make blockRadius to include the number of cells in a column to be multiplied with the size of the square radius.

    public class Transform
    {
        private static Transform Instance;

        public static Transform GetInstance
        {
            get
            {
                if(Instance == null)
                {
                    Instance = new Transform();
                }
                return Instance;
            }
        }

        private uint blockRadius;
        private uint num_rows_reg;
        private uint num_cols_reg;
        private uint num_files_reg;
        private uint numXPerBlock;
        private uint numYPerBlock;
        private uint numZPerBlock;
        bool basis_block_x = false;
        bool basis_block_y = false;
        bool basis_block_z = false;
        bool crossOver_X, crossOver_Y, crossOver_Z;

        private Transform()
        {
            blockRadius = Convert.ToUInt32(ConfigurationManager.AppSettings["BLOCKRADIUS"]);        //Radius for the random block 
            num_rows_reg = CPM.GetInstance.NumX;   //number of blocks per row
            num_cols_reg = CPM.GetInstance.NumY;   //number of blocks per column
            num_files_reg = CPM.GetInstance.NumZ;  //number of blocks per file
            numXPerBlock = CPM.GetInstance.BCP.NumXperBlock;
            numYPerBlock = CPM.GetInstance.BCP.NumYperBlock;
            numZPerBlock = CPM.GetInstance.BCP.NumZperBlock;
            crossOver_X = crossOver_Y = crossOver_Z = false;
            //based on above values , initialize and populate all the basic block modulos.

            //XL_BB_Mods = XR_BB_Mods = YU_BB_Mod = YD_BB_Mod = ZF_BB_Mod = ZB_BB_Mod = 0;
        }

        private bool XL_BB_Mods(uint bId) => (bId % num_cols_reg == 0);           //returns true if basis block else false
        private bool XR_BB_Mods(uint bId) => (bId % (num_cols_reg - 1)) == 0;
        private bool YU_BB_Mods(uint bId) => (((num_cols_reg * num_rows_reg) - num_cols_reg) <= bId) ? bId < (num_cols_reg * num_rows_reg) ? true : false : false;
        private bool YD_BB_Mods(uint bId) => (0 <= bId ? bId < num_cols_reg ? true : false : false);
        private bool ZF_BB_Mods(uint bId) => (0 <= bId ? 0   < num_rows_reg * num_cols_reg ? true : false : false);
        private bool ZB_BB_Mods(uint bId) => ((num_cols_reg * num_rows_reg * num_files_reg) - (num_rows_reg * num_cols_reg) <= bId ? bId < (num_rows_reg * num_cols_reg * num_files_reg) ? true : false : false);
        /// <summary>
        /// Checks if the RSB for the pos falls out of the existing block or not
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>True if doesnt cross over the block boundaries false otherwise</returns>
        private bool RSBCheckX(Position3D pos) => (pos.X - blockRadius < 0 || pos.X + blockRadius > numXPerBlock) ? true : false;
        private bool RSBCheckY(Position3D pos) => (pos.Y - blockRadius < 0 || pos.Y + blockRadius > numYPerBlock) ? true : false;
        private bool RSBCheckZ(Position3D pos) => (pos.Z - blockRadius < 0 || pos.Z + blockRadius > numZPerBlock) ? true : false;



        public Position3D PredictNewRandomPosition(Position3D basePosition)
        {
            /*
             * Basis Block 
             *  if yes
             *  then do random square within the block 
             *    if yes 
             *    then compute the intervals and generate new random positions for x,y,z & return new position
             *    else 
             *    then assign (0) for the respective basis block dimension and generate radom interval numbers for the rest of the dimension
             *  else no
             *  then compute the intervals and generate new random positions for x,y,z & return new position
             * */            
            basis_block_x = false;
            basis_block_y = false;
            basis_block_z = false;
            crossOver_X = crossOver_Y = crossOver_Z = false;
            #region VERIFICATIONS - SETTING FLAGS

            uint bId = basePosition.BID;


            basis_block_x = (XL_BB_Mods(bId) || XR_BB_Mods(bId)) ? true : false;
            crossOver_X = RSBCheckX(basePosition) ? true : false;


            basis_block_y = ((YU_BB_Mods(bId) || (YD_BB_Mods(bId)) ? true : false && RSBCheckY(basePosition)));
            crossOver_Y = RSBCheckY(basePosition) ? true : false;


            basis_block_z = ((ZF_BB_Mods(bId) || (ZB_BB_Mods(bId)) ? true : false && RSBCheckZ(basePosition)));
            crossOver_Z = RSBCheckZ(basePosition)  ? true : false;
            
            #endregion


            //most probable case will be that it is not a basis block so process them first n return the new position
            if (!basis_block_x && !basis_block_y && !basis_block_z)    //0 coordinate falls outside of 3/3 faces of the block //falls within the neuroblock.
            {
                //Randomly predict all the 3 positions using the PredictSynapseWithoutinterval method and return the position
                return TransformHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'A', blockRadius);
            }
            else if (basis_block_x && basis_block_y && basis_block_z) //CORE BASIS BLOCK This happens exactly at 8 blocks need to be careful to return an appropirate new predicted position to the caller.
            {
                /*
                 * figure out which core basis block it is and then 
                 * LLO,RLO,LUO,RUO - LLN,RLN,LUN,RUN
                 * create the adjusted RSB and predict coordinates.
                */
                //will do later
            }            
            else
            {                                                

            }


            return new Position3D();
            ////one coordinate falls outside of 1/3 faces;
            ////if the block is a basis block then predict it with z/y/z min of 0 and max of blockradius of x/y/z from x/y/z and for the other 2 points that falls inside of the block predict them using PredictSynapseWithoutInterval for both of them
            ////els if the block is not a basis block then predict the position with PredictSynapseWithanInterval method for the position and for other 2 points use the other method and finally return the position.
            //if(basis_block_x)
        }        


        public Position3D ComputeInterval(Position3D pos)
        {            
            //primary goal is to find out all the possible values a co-ordinate can have
            if (basis_block_x)       //Is Basis Block & Overlaps
            {
                int xmin = (int)(((-1) * numXPerBlock) + (pos.X - blockRadius));
                Interval interval = new Interval(xmin, (int)((-1) * numXPerBlock), 0, (int)pos.X);

            }
            else
            {

            }
            if (basis_block_y)
            {

            }
            else
            {

            }
            if (basis_block_z)
            {

            }
            else
            {

            }



        }
    }
}
