//TODO
//1. Check multiplier for block radius for its dimensions 
//2. Check and register the generated position with connection Table
//3. Option to take seed in from segment Id's
//4. Computing Block ID for new position
namespace HTM.Algorithms
{
    using System;
    using System.Configuration;
    using HTM.Models;
    using HTM.Enums;
    using System.Collections.Generic;

    //Notes: Make sure to make blockRadius to include the number of cells in a column to be multiplied with the size of the square radius.

    public class SynapseGenerator
    {

        #region VARIABLES & INSTANCE GETTER
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
        BasisBlockType basisBlockType;

        #endregion


        #region PRIVATE METHODS & CONSTRUCTOR


        private SynapseGenerator()
        {
            blockRadius = Convert.ToUInt32(ConfigurationManager.AppSettings["BLOCKRADIUS"]);        //Radius for the random block 
            num_rows_reg = CPM.GetInstance.NumX;   //number of blocks per regional row
            num_cols_reg = CPM.GetInstance.NumY;   //number of blocks per regional column
            num_files_reg = CPM.GetInstance.NumZ;  //number of blocks per regional file
            numXPerBlock = CPM.GetInstance.BCP.NumXperBlock;
            numYPerBlock = CPM.GetInstance.BCP.NumYperBlock;
            numZPerBlock = CPM.GetInstance.BCP.NumZperBlock;
            crossOver_X_Left = crossOver_X_Right = crossOver_Y_Up = crossOver_Y_Down = crossOver_Z_Front = crossOver_Z_Back = false;
            isBlockChanges = false;
            newBlockId = 0;
            XOFFSET = 1;
            YOFFSET = num_cols_reg;
            ZOFFSET = (num_rows_reg * num_cols_reg);
            //based on above values , initialize and populate all the basic block modulos.
            basisBlockType = BasisBlockType.NotApplicable;
            //XL_BB_Mods = XR_BB_Mods = YU_BB_Mod = YD_BB_Mod = ZF_BB_Mod = ZB_BB_Mod = 0;
        }

        private bool XL_BB_Mods(uint bId) => (bId % num_cols_reg == 0);           //returns true if basis block else false
        private bool XR_BB_Mods(uint bId) => (bId % (num_cols_reg - 1)) == 0;
        private bool YU_BB_Mods(uint bId) => (((num_cols_reg * num_rows_reg) - num_cols_reg) <= bId) ? bId < (num_cols_reg * num_rows_reg) ? true : false : false;
        private bool YD_BB_Mods(uint bId) => (0 <= bId ? bId < num_cols_reg ? true : false : false);
        private bool ZF_BB_Mods(uint bId) => (0 <= bId ? 0   < num_rows_reg * num_cols_reg ? true : false : false);
        private bool ZB_BB_Mods(uint bId) => ((num_cols_reg * num_rows_reg * num_files_reg) - (num_rows_reg * num_cols_reg) <= bId ? bId < (num_rows_reg * num_cols_reg * num_files_reg) ? true : false : false);

        private bool X_BB_Mods(uint bId) => (XL_BB_Mods(bId) && XR_BB_Mods(bId));

        private bool Y_BB_Mods(uint bId) => (YU_BB_Mods(bId) && YD_BB_Mods(bId));

        private bool Z_BB_Mods(uint bId) => (ZF_BB_Mods(bId) && ZB_BB_Mods(bId));


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

        private void InitializeChecks(Position3D pos)
        {
            basisBlockType = IsSingleBasisBlock(pos) ? BasisBlockType.SingleBasisBlock : ( (IsCoreBlock(pos) ? BasisBlockType.CoreBasisBlock : (IsDoubleBasisBlock(pos) ? BasisBlockType.DoubleBasisBlock : BasisBlockType.NotApplicable)));
        }

        private bool IsCoreBlock(Position3D pos) => ((XL_BB_Mods(pos.BID) && YD_BB_Mods(pos.BID) && ZF_BB_Mods(pos.BID)) || (XR_BB_Mods(pos.BID) && YU_BB_Mods(pos.BID) && ZB_BB_Mods(pos.BID)));

        private bool IsDoubleBasisBlock(Position3D pos) => (X_BB_Mods(pos.BID) || Y_BB_Mods(pos.BID)) && (Y_BB_Mods(pos.BID) || Z_BB_Mods(pos.BID)) && (Z_BB_Mods(pos.BID) || X_BB_Mods(pos.BID));

        private bool IsSingleBasisBlock(Position3D pos)
        {
            int BBcount = 0;

            if (XL_BB_Mods(pos.BID))
                BBcount++;
            else if (XR_BB_Mods(pos.BID))
                BBcount++;
            else if (YU_BB_Mods(pos.BID))
                BBcount++;
            else if (YD_BB_Mods(pos.BID))
                BBcount++;
            else if (ZF_BB_Mods(pos.BID))
                BBcount++;
            else if (ZB_BB_Mods(pos.BID))
                BBcount++;

            return BBcount == 1;
        }

        #endregion


        #region PUBLIC API METHODS

        /// <summary>
        /// Positioning Logic : dont get the center point always get 4 point behind the center point , if something is registered already four points away , if its dendrite 
        /// put a axonal block or vice versa 
        /// core block : CREATE only 1 A & 1 D.
        /// double basis : CREATE 1 A & 1 D
        /// single basis block : 2 A & 2 D
        /// normal block: Create 4 A & 4 D
        /// </summary>
        /// <param name="neuronId"></param>
        /// <returns>Synpase position for the Neuron</returns>
        public List<Position3D> AddProximalSegment(Position3D neuronId)
        {
            List<Position3D> NewProximalSynapse = null;

            if (basisBlockType.Equals(BasisBlockType.NotApplicable))
                InitializeChecks(neuronId);

            switch(basisBlockType) 
            {
                case BasisBlockType.SingleBasisBlock:
                    {
                        NewProximalSynapse = ComputeProximalCoordinatesForSingleBB(neuronId);
                        break;
                    };

                case BasisBlockType.DoubleBasisBlock:
                    {
                        NewProximalSynapse = ComputeProximalCoordinatesForDoubleBB(neuronId);
                        break;
                    };
                case BasisBlockType.CoreBasisBlock:
                    {
                        NewProximalSynapse = ComputeProximalCoordinatesForCoreBB(neuronId);
                        break;
                    };
                case BasisBlockType.NormalBlock:
                    {
                        NewProximalSynapse = ComputeProximalCoordinatesForNormalBB(neuronId);
                        break;
                    };
                default:
                        break;
            };

            return NewProximalSynapse;

        }

        
        public Position3D PredictNewRandomPosition(Position3D basePosition, uint retryCount)
        {
            if(retryCount > 4)
            {
                Console.WriteLine("ERROR : ERROR : ERROR : ERROR : ERROR : RETRY TIMEOUT HIT THE LIMIT FOR MAX RETRY ON PREDICT NEW RANDOM POSITION!!!");
                return null;
            }
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

            newBlockId = basePosition.BID;


            basis_block_x = XL_BB_Mods(newBlockId) || XR_BB_Mods(newBlockId) ? true : false;
            crossOver_X_Left = RSBCheckX_Left(basePosition);
            crossOver_X_Right = RSBCheckX_Right(basePosition);


            basis_block_y = YU_BB_Mods(newBlockId) || YD_BB_Mods(newBlockId) ? true : false;
            crossOver_Y_Up = RSBCheckY_Up(basePosition);
            crossOver_Y_Down = RSBCheckY_Down(basePosition);


            basis_block_z = ZF_BB_Mods(newBlockId) || ZB_BB_Mods(newBlockId) ? true : false;
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

                Interval intervalY = ComputeBoundsY(basePosition, basis_block_y, crossOver_Y_Up, crossOver_Y_Down, blockRadius);

                Interval intervalZ = ComputeBoundsZ(basePosition, basis_block_z, crossOver_Z_Front, crossOver_Z_Back, blockRadius);


                if(!intervalX.isBlockChanged || !intervalY.isBlockChanged || !intervalZ.isBlockChanged)
                {
                    newPosition.BID = newBlockId;
                    newPosition.X = intervalX.PredictRandomInteger();
                    newPosition.Y = intervalY.PredictRandomInteger();
                    newPosition.Z = intervalZ.PredictRandomInteger();
                }

                if (!CPM.GetInstance.CTable.IsPositionAvailable(newPosition))
                    return PredictNewRandomPosition(basePosition, ++retryCount);

                return newPosition;
                //need to compute the new block Id


                /*
                 * figure out which core basis block it is and then 
                 * LLO,RLO,LUO,RUO - LLN,RLN,LUN,RUN
                 * create the adjusted RSB and predict coordinates.
                */
                //will do later
            }                        

            
            ////one coordinate falls outside of 1/3 faces;
            ////if the block is a basis block then predict it with z/y/z min of 0 and max of blockradius of x/y/z from x/y/z and for the other 2 points that falls inside of the block predict them using PredictSynapseWithoutInterval for both of them
            ////els if the block is not a basis block then predict the position with PredictSynapseWithanInterval method for the position and for other 2 points use the other method and finally return the position.
            //if(basis_block_x)
        }

        #endregion


        #region PRIVATE HELPER METHODS

        private List<Position3D> ComputeProximalCoordinatesForCoreBB(Position3D neuronPos)      // 1 AXON & 1 DENDRITE PER BLOCK
        {
            List<Position3D> toReturn = new List<Position3D>();
            Position3D axonPos = null;
            Position3D dendriticPos = null;

            //pick the center of the block and then create a random square block the size of the block and call create new random position , just in case perform check
            //if we are creating a position for axon and it falls over the central axon point then pick a different position. this is not needed because it wont happen CTable wont let that happen as the point is already registered
            Position3D blockCenter = CPM.GetInstance.BCP.BlockCenter;
            blockCenter.BID = neuronPos.BID;
            uint blockRadius = blockCenter.X;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);

            return toReturn;
        }

        private List<Position3D> ComputeProximalCoordinatesForNormalBlock(Position3D neuronId)     // 8 AXONS & 8 DENDRITES - 1 AXON AND DENDRITE PER BLOCK
        {
            List<Position3D> toReturn = new List<Position3D>();
            Position3D axonPos = null;
            Position3D dendriticPos = null;

            //pick the center of the block and then create a random square block the size of the block and call create new random position , just in case perform check
            //if we are creating a position for axon and it falls over the central axon point then pick a different position. this is not needed because it wont happen CTable wont let that happen as the point is already registered
            Position3D blockCenter = CPM.GetInstance.BCP.BlockCenter;
            blockCenter.BID = neuronId.BID;
            uint blockRadius = blockCenter.X;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);

            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            //current
            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);

            //-z offset
            blockCenter.BID = neuronId.BID - ZOFFSET;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);

            //-x offset
            blockCenter.BID = neuronId.BID - XOFFSET;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);

            //-y offset
            blockCenter.BID = neuronId.BID - YOFFSET;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);

            // -z,& -x
            blockCenter.BID = neuronId.BID - ZOFFSET - XOFFSET;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);


            // -z , & -y
            blockCenter.BID = neuronId.BID - ZOFFSET - YOFFSET;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);


            // -x , & -y
            blockCenter.BID = neuronId.BID - XOFFSET - YOFFSET;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);

            // -z , -x, & -y
            blockCenter.BID = neuronId.BID - ZOFFSET - YOFFSET - XOFFSET;
            axonPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'A', blockRadius);
            dendriticPos = SynapseGeneratorHelper.PredictNewRandomSynapseWithoutIntervalWithConnecctionCheck(blockCenter, 'D', blockRadius);

            toReturn.Add(axonPos);
            toReturn.Add(dendriticPos);

            //8 near by blocks

            //get blockID's of all the near by 8 blocks and start getting axonal & dendritic positions for all of them

            //need to do block id addition and substraction now :(

            // current
            // - z offset
            // - x offset
            // - y offset
            // -z & -x , -z & -y, -x & -y
            // -z & -x & -y



        }

        private List<Position3D> ComputeProximalCoordinatesForSingleBB(Position3D neuronId)
        {
            //There will be 8 different positions that needs to be predicted
            //4 axons and 4 dendrites
            //pick all the 8 neighbhouring blocks and start getting positions



            

            throw new NotImplementedException();
        }

        private List<Position3D> ComputeProximalCoordinatesForSingleBB(Position3D neuronPos)
        {

        }

        private List<Position3D> ComputeProximalCoordinatesForDoubleBB(Position3D neuronPos)
        {

        }

        private Position3D GetCentreOfGivenBlock(Position3D position)
        {

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
                    uint crossOver = (uint)Math.Abs(x - blockRadius);
                    uint x1 = 0;
                    uint x2 = x + blockRadius;
                    uint y1 = numXPerBlock - crossOver;
                    uint y2 = numXPerBlock;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId -= 1;
                    }

                }
                else if (crossOver_Right)
                {                    
                    uint x = basePosition.X;
                    //creating interval coordinates                    
                    uint crossOver = (uint)Math.Abs(numXPerBlock - (x + blockRadius));
                    uint x1 = x - blockRadius;
                    uint x2 = numXPerBlock;
                    uint y1 = 1;
                    uint y2 = crossOver;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId += 1;
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

                uint x = SynapseGeneratorHelper.GetRand(0, basePosition.X + blockRadius);
                isBlockChanges = false;
                boundedInterval = new Interval(x);
            }
            else if(XR_BB_Mods(basePosition.BID) && crossOver_Right)
            {
                uint x = SynapseGeneratorHelper.GetRand(basePosition.X - blockRadius , numXPerBlock);
                isBlockChanges = false;
                boundedInterval = new Interval(x);
            }
            else if(XR_BB_Mods(basePosition.BID) && crossOver_Left)
            {
                uint x = basePosition.X;
                //creating interval coordinates                    
                uint crossOver = (uint)Math.Abs(x - blockRadius);
                uint x1 = 0;
                uint x2 = x + blockRadius;
                uint y1 = numXPerBlock - crossOver;
                uint y2 = numXPerBlock;

                boundedInterval = new Interval(x1, x2, y1, y2);

                if (boundedInterval.isBlockChanged)
                {
                    isBlockChanges = true;
                    newBlockId -= 1;
                }
            }
            else if (XR_BB_Mods(basePosition.BID) && crossOver_Left)
            {
                uint x = basePosition.X;
                //creating interval coordinates                    
                uint crossOver = (uint)Math.Abs(numXPerBlock - (x + blockRadius));
                uint x1 = x - blockRadius;
                uint x2 = numXPerBlock;
                uint y1 = 1;
                uint y2 = crossOver;

                boundedInterval = new Interval(x1, x2, y1, y2);

                if (boundedInterval.isBlockChanged)
                {
                    isBlockChanges = true;
                    newBlockId += 1;
                }
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
                        newBlockId += (YOFFSET);
                    }

                }
                else if (crossOver_Down)
                {                    
                    uint y = basePosition.Y;
                    //creating interval coordinates
                    uint blockLengthY = CPM.GetInstance.BCP.NumYperBlock;
                    uint crossOver = (uint)Math.Abs(y - blockRadius);
                    uint x1 = 0;
                    uint x2 = y + blockRadius;
                    uint y1 = numYPerBlock - crossOver;
                    uint y2 = numYPerBlock;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId -= YOFFSET;
                    }
                }
                else    //if its not a basis block and doesnt cross over on X or on cross over on right!
                {
                    boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'Y', blockRadius));
                }
            }
            else if (YU_BB_Mods(basePosition.BID) && crossOver_Up)
            {
                // if isBasisBlock , need to figure out which side of the edge is the basis block is in ? if its on the left edge and the cross over is X then needs adjustments , same for other faces y ,z

                uint y = SynapseGeneratorHelper.GetRand(basePosition.Y - blockRadius, numYPerBlock);
                isBlockChanges = false;
                boundedInterval = new Interval(y);
            }
            else if (YD_BB_Mods(basePosition.BID) && crossOver_Down)
            {
                uint y = SynapseGeneratorHelper.GetRand(0, basePosition.Y + blockRadius);
                isBlockChanges = false;
                boundedInterval = new Interval(y);
            }
            else if(YU_BB_Mods(basePosition.BID) && crossOver_Down)     //Not a problem basis block on upper face but crossing over on below block
            {
                uint y = basePosition.Y;
                //creating interval coordinates
                uint blockLengthY = CPM.GetInstance.BCP.NumYperBlock;
                uint crossOver = (uint)Math.Abs(y - blockRadius);
                uint x1 = 0;
                uint x2 = y + blockRadius;
                uint y1 = numYPerBlock - crossOver;
                uint y2 = numYPerBlock;

                boundedInterval = new Interval(x1, x2, y1, y2);

                if (boundedInterval.isBlockChanged)
                {
                    isBlockChanges = true;
                    newBlockId -= YOFFSET;
                }
            }
            else if (YD_BB_Mods(basePosition.BID) && crossOver_Up)  //Not a problem basis block on down face but crossing over on upper block
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
                    newBlockId += (YOFFSET);
                }
            }
            else    //if it is basis block on Left face && crossing over on right is fine OR basis_block on right edge and crossing over on left is fine too!!!
            {
                boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'Y', blockRadius));
            }

            return boundedInterval;
        }

        private Interval ComputeBoundsZ(Position3D basePosition, bool isBasisBlock, bool crossOver_Front, bool crossOver_Back, uint blockRadius, bool? isCoreBlock = null)
        {
            Interval boundedInterval = null;

            //Create an interval object , compute bound on the crossover dimension 
            if (!isBasisBlock)
            {
                if (crossOver_Front)
                {
                    uint z = basePosition.Z;
                    //creating interval coordinates                    
                    uint crossOver = (uint)Math.Abs(z - blockRadius);
                    uint x1 = 0;
                    uint x2 = z;
                    uint y1 = numZPerBlock - crossOver;
                    uint y2 = numZPerBlock;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId += ZOFFSET;
                    }
                }
                else if (crossOver_Back)
                {
                    uint z = basePosition.Z;
                    //creating interval coordinates                    
                    uint crossOver = (uint)Math.Abs(z + blockRadius - numZPerBlock);
                    uint x1 = z - blockRadius;
                    uint x2 = numZPerBlock;
                    uint y1 = 0;
                    uint y2 = crossOver;

                    boundedInterval = new Interval(x1, x2, y1, y2);

                    if (boundedInterval.isBlockChanged)
                    {
                        isBlockChanges = true;
                        newBlockId -= ZOFFSET;
                    }
                }
                else    //if its not a basis block and doesnt cross over on X or on cross over on right!
                {
                    boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'Z', blockRadius));
                }

            }
            else if (ZF_BB_Mods(basePosition.BID) && crossOver_Front)
            {
                // if isBasisBlock , need to figure out which side of the edge is the basis block is in ? if its on the left edge and the cross over is X then needs adjustments , same for other faces y ,z

                uint z = SynapseGeneratorHelper.GetRand(0, basePosition.Z + numZPerBlock);  
                isBlockChanges = false;
                boundedInterval = new Interval(z);
            }
            else if (ZB_BB_Mods(basePosition.BID) && crossOver_Back)
            {
                uint x = SynapseGeneratorHelper.GetRand(basePosition.Z - blockRadius, numZPerBlock);
                isBlockChanges = false;
                boundedInterval = new Interval(x);
            }
            else if(ZF_BB_Mods(basePosition.BID) && crossOver_Back)
            {
                uint z = basePosition.Z;
                //creating interval coordinates                    
                uint crossOver = (uint)Math.Abs(z + blockRadius - numZPerBlock);
                uint x1 = z - blockRadius;
                uint x2 = numZPerBlock;
                uint y1 = 0;
                uint y2 = crossOver;

                boundedInterval = new Interval(x1, x2, y1, y2);

                if (boundedInterval.isBlockChanged)
                {
                    isBlockChanges = true;
                    newBlockId -= ZOFFSET;
                }
            }
            else if (ZF_BB_Mods(basePosition.BID) && crossOver_Front)
            {
                uint z = basePosition.Z;
                //creating interval coordinates                    
                uint crossOver = (uint)Math.Abs(z - blockRadius);
                uint x1 = 0;
                uint x2 = z;
                uint y1 = numZPerBlock - crossOver;
                uint y2 = numZPerBlock;

                boundedInterval = new Interval(x1, x2, y1, y2);

                if (boundedInterval.isBlockChanged)
                {
                    isBlockChanges = true;
                    newBlockId += ZOFFSET;
                }
            }
            else    //if it is basis block on Left face && crossing over on right is fine OR basis_block on right edge and crossing over on left is fine too!!!
            {
                boundedInterval = new Interval(SynapseGeneratorHelper.PredictNewRandomSynapseWithoutInterval(basePosition, 'Z', blockRadius));
            }

            return boundedInterval;
        }

        #endregion
    }
}
