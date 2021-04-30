//TODO
//1. Check multiplier for block radius for its dimensions 
//2. Check and register the generated position with connection Table
//3. Option to take seed in from segment Id's
namespace HTM.Algorithms
{
    using System;
    using System.Configuration;
    using HTM.Models;

    //Notes: Make sure to make blockRadius to include the number of cells in a column to be multiplied with the size of the square radius.

    public class SynapseGenerator
    {
        private static SynapseGenerator Instance;

        public static SynapseGenerator GetInstance
        {
            get
            {
                if(Instance == null)
                {
                    Instance = new SynapseGenerator();
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
        private bool isBlockChanges;
        private uint newBlockId;
        private uint numZPerBlock;
        private readonly uint YOFFSET;
        private readonly uint XOFFSET;
        private readonly uint ZOFFSET;
        bool basis_block_x = false;
        bool basis_block_y = false;
        bool basis_block_z = false;
        bool crossOver_X_Left, crossOver_X_Right, crossOver_Y_Up, crossOver_Y_Down, crossOver_Z_Front, crossOver_Z_Back;

        private SynapseGenerator()
        {
            blockRadius = Convert.ToUInt32(ConfigurationManager.AppSettings["BLOCKRADIUS"]);        //Radius for the random block 
            num_rows_reg = CPM.GetInstance.NumX;   //number of blocks per row
            num_cols_reg = CPM.GetInstance.NumY;   //number of blocks per column
            num_files_reg = CPM.GetInstance.NumZ;  //number of blocks per file
            numXPerBlock = CPM.GetInstance.BCP.NumXperBlock;
            numYPerBlock = CPM.GetInstance.BCP.NumYperBlock;
            numZPerBlock = CPM.GetInstance.BCP.NumZperBlock;
            crossOver_X_Left = crossOver_X_Right = crossOver_Y_Up = crossOver_Y_Down = crossOver_Z_Front = crossOver_Z_Back = false;
            isBlockChanges = false;
            newBlockId = 0;
            XOFFSET = 1;
            YOFFSET = num_cols_reg;
            ZOFFSET = num_files_reg;
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
        private bool RSBCheckX_Left(Position3D pos) => (pos.X - blockRadius < 0) ? true : false;
        private bool RSBCheckX_Right(Position3D pos) => (pos.X + blockRadius > 0) ? true : false;
        private bool RSBCheckY_Up(Position3D pos) => (pos.Y - blockRadius < 0 ) ? true : false;
        private bool RSBCheckY_Down(Position3D pos) => pos.Y + blockRadius > numYPerBlock ? true : false;
        private bool RSBCheckZ_Front(Position3D pos) => (pos.Z - blockRadius < 0) ? true : false;
        private bool RSBCheckZ_Back(Position3D pos) => (pos.Z + blockRadius > numZPerBlock) ? true : false;

        public Position3D PredictNewRandomPosition(Position3D basePosition, string claimerSegId)
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
            Position3D newPosition = new Position3D();
            
            #region VERIFICATIONS - SETTING FLAGS

            uint bId = basePosition.BID;


            basis_block_x = XL_BB_Mods(bId) || XR_BB_Mods(bId) ? true : false;
            crossOver_X_Left = RSBCheckX_Left(basePosition);
            crossOver_X_Right = RSBCheckX_Right(basePosition);


            basis_block_y = YU_BB_Mods(bId) || YD_BB_Mods(bId) ? true : false;
            crossOver_Y_Up = RSBCheckY_Up(basePosition);
            crossOver_Y_Down = RSBCheckY_Down(basePosition);


            basis_block_z = ZF_BB_Mods(bId) || ZB_BB_Mods(bId) ? true : false;
            crossOver_Z_Front = RSBCheckZ_Front(basePosition);
            crossOver_Z_Back = RSBCheckZ_Back(basePosition);

            #endregion


            //most probable case will be that it is not a basis block so process them first n return the new position
            if (!basis_block_x && !basis_block_y && !basis_block_z && !crossOver_X_Left && !crossOver_X_Right && !crossOver_Y_Up && !crossOver_Y_Down && !crossOver_Z_Front && !crossOver_Z_Back)    //0 coordinates falls outside of 3/3 faces of the block //falls within the neuroblock.
            {
                //Randomly predict all the 3 positions using the PredictSynapseWithoutinterval method and return the position
                return SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(basePosition, 'A', blockRadius);
            }
            else 
            {                       
                if (!basis_block_x && !crossOver_X_Left && !crossOver_X_Right)
                {
                    //Not a basis block and not crossing over
                    newPosition.X =  SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'X', blockRadius);
                }

                Interval intervalX = ComputeBoundsX(basePosition, basis_block_x, crossOver_X_Left, crossOver_X_Right, blockRadius);

                if ((!basis_block_y && !crossOver_Y) && (basis_block_y && !crossOver_Y))
                {
                    newPosition.Y = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'X', blockRadius);
                }
                else if (!basis_block_y && crossOver_Y)
                {
                    int ymin = (int)(((-1) * numYPerBlock) + (basePosition.Y - blockRadius));           //To Check what should be multiplied with blockRadius for connection point numbering convention
                    newPosition.Y = SynapseGeneratorHelper.PredictRandomIntervalInteger(ymin, (int)((-1) * numYPerBlock), 0, (int)basePosition.Y);
                }
                else if (basis_block_y && crossOver_Y)
                {
                    newPosition.Y = SynapseGeneratorHelper.GetRand(0, basePosition.Y);
                }

                if ( (!basis_block_z && !crossOver_Z) && (basis_block_z && !crossOver_Z))
                {
                    newPosition.Z = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'Z', blockRadius);
                }
                else if (!basis_block_z && crossOver_Z)
                {
                    int zmin = (int)(((-1) * numZPerBlock) + (basePosition.Z - blockRadius));           //To Check what should be multiplied with blockRadius for connection point numbering convention
                    newPosition.Z = SynapseGeneratorHelper.PredictRandomIntervalInteger(zmin, (int)((-1) * numZPerBlock), 0, (int)basePosition.Z);
                }
                else if (basis_block_z && crossOver_Z)
                {
                    newPosition.Z = SynapseGeneratorHelper.GetRand(0, basePosition.Z);
                }
                /*
                 * figure out which core basis block it is and then 
                 * LLO,RLO,LUO,RUO - LLN,RLN,LUN,RUN
                 * create the adjusted RSB and predict coordinates.
                */
                //will do later
            }                        


            return newPosition;
            ////one coordinate falls outside of 1/3 faces;
            ////if the block is a basis block then predict it with z/y/z min of 0 and max of blockradius of x/y/z from x/y/z and for the other 2 points that falls inside of the block predict them using PredictSynapseWithoutInterval for both of them
            ////els if the block is not a basis block then predict the position with PredictSynapseWithanInterval method for the position and for other 2 points use the other method and finally return the position.
            //if(basis_block_x)
        }

        private Interval ComputeBoundsX(Position3D basePosition, bool isBasisBlock, bool crossOver_Left, bool crossOver_Right, uint blockRadius, bool? isCoreBlock = null)
        {
            Interval boundedInterval = null;

            //Create an interval object , compute bound on the crossover dimension 
            if (!isBasisBlock)
            {
                if (crossOver_Left)
                {                    
                    uint x = basePosition.X;
                    //creating interval coordinates
                    uint blockLengthX = CPM.GetInstance.BCP.NumXperBlock;
                    uint crossOver = (uint)Math.Abs(x - blockRadius);
                    uint x1 = blockLengthX - crossOver;
                    uint x2 = blockLengthX - 1;
                    uint y1 = 0;
                    uint y2 = x;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId = basePosition.BID - 1;
                    }

                }
                else if (crossOver_Right)
                {
                    uint bid = basePosition.BID;
                    uint x = basePosition.X;
                    //creating interval coordinates
                    uint blockLengthX = CPM.GetInstance.BCP.NumXperBlock;
                    uint crossOver = (uint)Math.Abs(blockLengthX - (x + blockRadius));
                    uint x1 = x;
                    uint x2 = blockLengthX - 1;
                    uint y1 = 1;
                    uint y2 = crossOver;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId = basePosition.BID + 1;
                    }
                }
                else    //if its not a basis block and doesnt cross over on X or on cross over on right!
                {
                    boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'X', blockRadius));
                }
                    
            }
            else if(XL_BB_Mods(basePosition.BID) && crossOver_Left)
            {
                // if isBasisBlock , need to figure out which side of the edge is the basis block is in ? if its on the left edge and the cross over is X then needs adjustments , same for other faces y ,z

                uint x = SynapseGeneratorHelper.GetRand(0, basePosition.X);
                isBlockChanges = false;
                boundedInterval = new Interval(x);
            }
            else if(XR_BB_Mods(basePosition.BID) && crossOver_Right)
            {
                uint x = SynapseGeneratorHelper.GetRand(basePosition.X, numXPerBlock);
                isBlockChanges = false;
                boundedInterval = new Interval(x);
            }
            else    //if it is basis block on Left face && crossing over on right is fine OR basis_block on right edge and crossing over on left is fine too!!!
            {
                boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'X', blockRadius));
            }

            return boundedInterval;
        }

        private Interval ComputeBoundsY(Position3D basePosition, bool isBasisBlock, bool crossOver_Up, bool crossOver_Down, uint blockRadius, bool? isCoreBlock = null)
        {
            Interval boundedInterval = null;

            //Create an interval object , compute bound on the crossover dimension 
            if (!isBasisBlock)
            {
                if (crossOver_Up)
                {                    
                    uint y = basePosition.Y;
                    //creating interval coordinates                    
                    uint crossOver = (uint)Math.Abs(y + blockRadius);
                    uint x1 = y;
                    uint x2 = numYPerBlock - 1;
                    uint y1 = 0;
                    uint y2 = crossOver;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId = basePosition.BID + (YOFFSET);
                    }

                }
                else if (crossOver_Down)
                {                    
                    uint x = basePosition.X;
                    //creating interval coordinates
                    uint blockLengthX = CPM.GetInstance.BCP.NumXperBlock;
                    uint crossOver = (uint)Math.Abs(blockLengthX - (x + blockRadius));
                    uint x1 = x;
                    uint x2 = blockLengthX - 1;
                    uint y1 = 1;
                    uint y2 = crossOver;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId = basePosition.BID + 1;
                    }
                }
                else    //if its not a basis block and doesnt cross over on X or on cross over on right!
                {
                    boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'X', blockRadius));
                }

            }
            else if (XL_BB_Mods(basePosition.BID) && crossOver_Left)
            {
                // if isBasisBlock , need to figure out which side of the edge is the basis block is in ? if its on the left edge and the cross over is X then needs adjustments , same for other faces y ,z

                uint x = SynapseGeneratorHelper.GetRand(0, basePosition.X);
                isBlockChanges = false;
                boundedInterval = new Interval(x);
            }
            else if (XR_BB_Mods(basePosition.BID) && crossOver_Right)
            {
                uint x = SynapseGeneratorHelper.GetRand(basePosition.X, numXPerBlock);
                isBlockChanges = false;
                boundedInterval = new Interval(x);
            }
            else    //if it is basis block on Left face && crossing over on right is fine OR basis_block on right edge and crossing over on left is fine too!!!
            {
                boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'X', blockRadius));
            }

            return boundedInterval;
        }


    }
}
