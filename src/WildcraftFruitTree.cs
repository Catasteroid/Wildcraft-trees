using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




namespace WildcraftFruitTrees
{
    public class WildcraftFruitTreeSystem : ModSystem
    {
        
        /*
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterBlockClass("BlockCoconutTree", typeof(BlockCoconutTree));
            //api.RegisterBlockClass("BlockCoconutFruit", typeof(BlockCoconutTree));
            //api.RegisterBlockEntityClass("BlockEntityCoconutFruit", typeof(BlockEntityCoconutFruit));
        }
        */
        
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.World.Logger.Notification("Start called, registering tree class");
            api.RegisterBlockClass("BlockCoconutTree", typeof(BlockCoconutTree));
            //api.RegisterBlockClass("BlockCoconutTree", typeof(BlockBananaTree));
        }

        /*
        public override void AssetsLoaded(ICoreAPI api)
        {
			api.RegisterBlockClass("BlockCoconutTree", typeof(BlockCoconutTree));
			api.World.Logger.Notification("AssetsLoaded called, registering tree class");
            base.AssetsLoaded(api);
        }
        */

        // I'm hoping that this will reduce the instance odrr
        public override double ExecuteOrder()
        {
            return 0.9;
        }
    }

    public class BlockCoconutTree : Block, ITreeGenerator
    {
        public Block trunkThickness6;
        public Block trunkThickness7;
        public Block trunkThickness8;
        public Block trunkThickness9;
        public Block trunkThickness10;
        //public Block trunkTopEmpty;
        public Block trunkTopFlowers;
        //public Block trunkTopUnripe;
        //public Block trunkTopRipe;
        public Block trunkTopFoliageYoung;
        public Block trunkTopFoliageMiddle;
        public Block trunkTopFoliageOld;

        public int minTrunkSize;
        public int maxTrunkSize;
        public int minTrunkSizeCap;
        public int maxTrunkSizeCap;
        public int foliageYoungSize;
        public int foliageMiddleSize;
        static Random rand = new Random();


        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            ICoreServerAPI sapi = api as ICoreServerAPI;
            if (sapi != null)
            {
                
                // Old shitty tree generator 
                //if (Code.Path.Equals("coconuttree-coconut-trunk10"))
                //{
                //    sapi.RegisterTreeGenerator(new AssetLocation("coconuttree-coconut-trunk10"), this);
                //}

                // If I learn of a function that allows you to get the first element of an assetlocation's code then I'll use it here
                // Until then we'll just have to check the last element and assume anything with this code 
                // and trunk size is the base block of the tree trunk
                if (Code.EndVariant().Equals("trunk10"))
                {
                    sapi.RegisterTreeGenerator(Code, this);
                }
            }

            if (trunkThickness6 == null)
            {
                IBlockAccessor blockAccess = api.World.BlockAccessor;

                //Code.FirstPathPart(1)
                //Variant["type"]
                
            
                trunkThickness6 = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-trunk6"));
                trunkThickness7 = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-trunk7"));
                trunkThickness8 = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-trunk8"));
                trunkThickness9 = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-trunk9"));
                trunkThickness10 = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-trunk10"));
                
                
                minTrunkSize = trunkThickness10.Attributes["minTrunkSize"].AsInt(4);
                maxTrunkSize = trunkThickness10.Attributes["maxTrunkSize"].AsInt(10);
                minTrunkSizeCap = trunkThickness10.Attributes["minTrunkSizeCap"].AsInt(2);
                maxTrunkSizeCap = trunkThickness10.Attributes["maxTrunkSizeCap"].AsInt(12);

                foliageYoungSize = trunkThickness10.Attributes["foliageYoungSizeMax"].AsInt(4);
                foliageMiddleSize = trunkThickness10.Attributes["foliageYoungSizeMax"].AsInt(8);

                trunkTopFlowers = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-trunktopflowers"));
                //trunkTopFlowering = blockAccess.GetBlock(new AssetLocation("coconuttree-coconut-trunktopflowering"));
                //trunkTopUnripe = blockAccess.GetBlock(new AssetLocation("coconuttree-coconut-trunktopunripe"));
                //trunkTopRipe = blockAccess.GetBlock(new AssetLocation("coconuttree-coconut-trunktopripe"));

                trunkTopFoliageYoung = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-foliageyoung"));
                trunkTopFoliageMiddle = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-foliagemiddle"));
                trunkTopFoliageOld = blockAccess.GetBlock(new AssetLocation("coconuttree-" + Variant["type"] + "-foliageold"));
            }
        }

        public override void OnDecalTesselation(IWorldAccessor world, MeshData decalMesh, BlockPos pos)
        {
            base.OnDecalTesselation(world, decalMesh, pos);
        }

        public override void OnJsonTesselation(ref MeshData sourceMesh, ref int[] lightRgbsByCorner, BlockPos pos, Block[] chunkExtBlocks, int extIndex3d)
        {
            base.OnJsonTesselation(ref sourceMesh, ref lightRgbsByCorner, pos, chunkExtBlocks, extIndex3d);

            if (this == trunkTopFoliageYoung || this == trunkTopFoliageMiddle || this == trunkTopFoliageOld)
            {
                for (int i = 0; i < sourceMesh.FlagsCount; i++)
                {
                    sourceMesh.Flags[i] = (sourceMesh.Flags[i] & ~VertexFlags.NormalBitMask) | BlockFacing.UP.NormalPackedFlags;
                }
            }
        }


        public string Type()
        {
            return Variant["type"];
        }


        //public void GrowTree(IBlockAccessor blockAccessor, BlockPos pos, bool skipForestFloor, float sizeModifier = 1, float vineGrowthChance = 0, float otherBlockChance = 1, int treesInChunkGenerated = 0)
        public void GrowTree(IBlockAccessor blockAccessor, BlockPos pos, TreeGenParams treegenParams)
        {
            //api.World.Logger.Notification("Attempting to spawn a palm tree, size: {0}, vines: {1}, otherblock: {2}, othertrees: {3}",sizeModifier,vineGrowthChance,otherBlockChance,treesInChunkGenerated);
            //float f = otherBlockChance == 0 ? 1 + (float)rand.NextDouble() * 2.0f : 1.5f + (float)rand.NextDouble() * 4f;
            //int quantity = GameMath.RoundRandom(rand, f);
            int quantity = 1;
            //api.World.Logger.Notification("Tree quantity: {0}",quantity);
            if (quantity == 1){
                GrowOneTree(blockAccessor, pos.UpCopy(), treegenParams.size, treegenParams.vinesGrowthChance);
            }
            
            while (quantity > 0)
            {
                GrowOneTree(blockAccessor, pos.UpCopy(), treegenParams.size, treegenParams.vinesGrowthChance);
                quantity--;
                // Potentially grow another one nearby
                pos.X += rand.Next(8) - 4;
                pos.Z += rand.Next(8) - 4;

                // Test up to 2 blocks up and down.
                bool foundSuitableBlock = false;
                for (int y = 2; y >= -2; y--)
                {
                    //Block block = blockAccessor.GetBlock(pos.X, pos.Y + y, pos.Z);
                    Block block = blockAccessor.GetBlock(pos.X,pos.Y,pos.Z,1);
                    if (block.Fertility > 0 && !blockAccessor.GetBlock(pos.X, pos.Y + y + 1, pos.Z,BlockLayersAccess.Fluid).IsLiquid())
                    {
                        pos.Y = pos.Y + y;
                        foundSuitableBlock = true;
                        break;
                    }
                }
                if (!foundSuitableBlock) break;
            }

            
        }

        private void GrowOneTree(IBlockAccessor blockAccessor, BlockPos upos, float sizeModifier, float vineGrowthChance)
        {
            //api.World.Logger.Notification("Growing a palm tree! Size: {0}, vines: {1}",sizeModifier,vineGrowthChance);
            Block[] thicknesses = new Block[5];
            thicknesses[0] = trunkThickness10;
            thicknesses[1] = trunkThickness9;
            thicknesses[2] = trunkThickness8;
            thicknesses[3] = trunkThickness7;
            thicknesses[4] = trunkThickness6;
           
            int maxthickness = GameMath.Min(4,rand.Next(5));
            //api.World.Logger.Notification("palm trunk thickness {0} chosen ({1})", maxthickness, thicknesses[maxthickness]);
            int height = GameMath.Clamp((int)(sizeModifier * (float)(minTrunkSize + rand.Next((maxTrunkSize - minTrunkSize) + 1))), minTrunkSizeCap, maxTrunkSizeCap);
            //api.World.Logger.Notification("palm size chosen: {0} (sizeModifier: {1}), size range (cap): {2}({3})-{4}({5})", height, sizeModifier, minTrunkSize, minTrunkSizeCap, maxTrunkSize, maxTrunkSizeCap);
            //api.World.Logger.Notification("Tree trunk segments, 1: {0}, 2: {1}, 3: {2}, 4: {3}, 5: {4}",trunkThickness6,trunkThickness7,trunkThickness8,trunkThickness9,trunkThickness10);
            //api.World.Logger.Notification("Tree max thickness: {0} ({1}), height: {2}",maxthickness,thicknesses[maxthickness],height);

            // Check that we can actually place the full tree length's blocks before we do anything heavy
            for (int i = 0; i <= (height+2); i++)  
            {
                Block toplaceblock = trunkThickness6;
                if (!blockAccessor.GetBlock(upos.X, upos.Y + i, upos.Z,BlockLayersAccess.SolidBlocks).IsReplacableBy(toplaceblock)){
                    //api.World.Logger.Notification("Failed to place palm due to obstruction {0} blocks up",i);
                    return;
                }
            }
            int[] trunkthicknesses = new int[height];
            int ty = 4;
            int tz = 1;
            for (int tx = height-1; tx >= 0; tx--)
            {
                //api.World.Logger.Notification("Trunk segment {0} {1}/{2}:",tx,tz,height);
                trunkthicknesses[tx] = GameMath.Max(ty,0);   
                //api.World.Logger.Notification("Trunk thickness set to {0}/{1} ({2})",ty,maxthickness,thicknesses[ty].Code.ToString());
                if (ty > maxthickness) ty--;
                tz++;

                /*
                if (height - tx < (maxthickness + 1))
                {
                    trunkthicknesses[tx] = height - tx;
                    api.World.Logger.Notification("Height ({0}) - tx ({1}) < 2 + maxthickness ({2}), thickness set to {3} ({4})",height,tx,maxthickness,height-tx,thicknesses[height-tx].Code.ToString());
                }
                else
                {
                    trunkthicknesses[tx] = 4-maxthickness;
                    api.World.Logger.Notification("Main stem, thickness set to {0} ({1})",maxthickness,thicknesses[4-maxthickness].Code.ToString());
                }
                */
                //api.World.Logger.Notification("Current palm trunk thickness array: {0}", trunkthicknesses.ToString());
            }
            //api.World.Logger.Notification("Resolved palm trunk thickness array: {0}", trunkthicknesses.ToString());

            
            // Future note when making fruiting palms- this is supposed to choose one of multiple variants of a fruiting/berry bush type block
            /*
            Block[] trunkTops = new Block[8];
            trunkTops[0] = trunkTopEmpty;
            trunkTops[1] = trunkTopFlowering;
            trunkTops[2] = trunkTopFlowering;
            trunkTops[3] = trunkTopUnripe;
            trunkTops[4] = trunkTopUnripe;
            trunkTops[5] = trunkTopRipe;
            trunkTops[6] = trunkTopRipe;
            trunkTops[7] = trunkTopRipe;
            //if (height == 1) trunkTop = trunkTopYoung;
            */

            

            for (int i = 0; i < height; i++)
            {
                //api.World.Logger.Error("shit and beans");
                Block toplaceblock = trunkThickness6;
                /*
                if (i <= height - 5) toplaceblock = trunkThickness10;
                if (i == height - 5) toplaceblock = trunkThickness9;
                if (i == height - 4) toplaceblock = trunkThickness8;
                if (i == height - 3) toplaceblock = trunkThickness7;
                if (i == height - 2) toplaceblock = trunkThickness6;
                if (i == height - 1) toplaceblock = trunkTops[GameMath.Min(7,rand.Next(8))];
                */
                //api.World.Logger.Error("attempting to place block {0}/{1}, trunkthicknesses length is {2}",i,height,trunkthicknesses.Length);
                toplaceblock = thicknesses[trunkthicknesses[i]];
                //api.World.Logger.Notification("placing palm trunk block: {0} at {1}", toplaceblock.Code,upos);
                blockAccessor.SetBlock(toplaceblock.BlockId, upos);
                upos.Up();
            }
            blockAccessor.SetBlock(trunkTopFlowers.BlockId, upos);
            upos.Up();
            //api.World.Logger.Notification("placing palm flower block: {0} at {1}", trunkTopFlowers.Code,upos);
            if (height <= foliageYoungSize)
            {
                //api.World.Logger.Notification("for height {0} placing young palm foliage block: {1} at {2}", height, trunkTopFoliageYoung.Code);
                blockAccessor.SetBlock(trunkTopFoliageYoung.BlockId, upos);
            }
            else if (height <= foliageMiddleSize)
            {
                //api.World.Logger.Notification("for height {0} placing middling/medium palm foliage block: {1} at {2}", height, trunkTopFoliageMiddle.Code,upos);
                blockAccessor.SetBlock(trunkTopFoliageMiddle.BlockId, upos);
            }
                // Assume it's larger than the foliageMiddleSize and place a mature/old foliage block
            else
            {
                //api.World.Logger.Notification("for height {0} placing mature/old palm foliage block: {1} at {2}", height, trunkTopFoliageOld.Code,upos);
                blockAccessor.SetBlock(trunkTopFoliageOld.BlockId, upos); 
            }
            //blockAccessor.SetBlock(trunkTopFoliage.BlockId, upos);
            //upos.Up();

            //api.World.Logger.Notification("placing palm foliage block: {0} at {1}", trunkTopFoliage.Code,upos);

            //api.World.Logger.Notification("Finished placing palm trunk blocks!");
        }

    }


    
}