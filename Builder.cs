using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace LansHouseBuilder
{
    class ItemCost
    {
        public int itemid;
        public int count;

        public ItemCost(int itemid, int count)
        {
            this.itemid = itemid;
            this.count = count;
        }

        public ItemCost clone()
        {
            return new ItemCost(this.itemid, this.count);
        }
    }

    class CostHandler
    {
        List<ItemCost> cost = new List<ItemCost>();


        public void Add(ItemCost cost)
        {
            bool found = false;
            foreach(var v in this.cost)
            {
                if(v.itemid == cost.itemid)
                {
                    v.count += cost.count;
                    found = true;
                    return;
                }
            }

            if(!found)
            {
                this.cost.Add(cost.clone());
            }
        }

        public void Add(ItemCost[] cost)
        {
            foreach (var v in cost)
            {
                this.Add(v);
            }
        }

        internal ItemCost[] ToArray()
        {
            return this.cost.ToArray();
        }
    }

    class HouseBlock
    {
        public int NetID;
        public ItemCost[] cost;

        public HouseBlock(int netID, ItemCost[] cost)
        {
            NetID = netID;
            this.cost = cost;
        }
    }


    class Builder
    {

        public static HouseBlock[][] houseblocks = new HouseBlock[][] {
                new HouseBlock[] {
                    new HouseBlock(TileID.WoodBlock, new ItemCost[] {new ItemCost(ItemID.Wood, 1) }),
                    new HouseBlock(TileID.Platforms, new ItemCost[] {new ItemCost(ItemID.Wood, 1)}),
                    new HouseBlock(TileID.WorkBenches, new ItemCost[] {new ItemCost(ItemID.Wood, 10)}),
                    new HouseBlock(TileID.Chairs, new ItemCost[] {new ItemCost(ItemID.Wood, 4)}),
                    new HouseBlock(TileID.Torches, new ItemCost[] { new ItemCost(ItemID.Gel, 1), new ItemCost(ItemID.Wood, 1) }),
                },
                new HouseBlock[] {
                    new HouseBlock(TileID.Stone, new ItemCost[] {new ItemCost(ItemID.StoneBlock, 1) }),
                    new HouseBlock(TileID.Platforms, new ItemCost[] {new ItemCost(ItemID.Wood, 1)}),
                    new HouseBlock(TileID.WorkBenches, new ItemCost[] {new ItemCost(ItemID.Wood, 10)}),
                    new HouseBlock(TileID.Chairs, new ItemCost[] {new ItemCost(ItemID.Wood, 4)}),
                    new HouseBlock(TileID.Torches, new ItemCost[] { new ItemCost(ItemID.Gel, 1), new ItemCost(ItemID.Wood, 1) }),
                },
            };

        public static HouseBlock[][] housewalls = new HouseBlock[][] {
                new HouseBlock[] {
                    new HouseBlock(WallID.Wood, new ItemCost[] {new ItemCost(ItemID.Wood, 1)}),
                },
                new HouseBlock[] {
                    new HouseBlock(WallID.Stone, new ItemCost[] {new ItemCost(ItemID.StoneBlock, 1)}),
                },
            };



        public static int[,] Tiles = new int[,]
        {
            {1,1,1,1,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,5,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,0,0,0,1},
            {1,3,0,4,1},
            {1,2,1,2,1}
        };

        public static int[,] Walls = new int[,]
        {
            {0,0,0,0,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,1,1,1,0},
            {0,0,0,0,0}
        };


        public static bool CanPlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
        {
            if(type == ItemID.WorkBench)
            {
                return CanPlaceTile(i, j, ItemID.DirtBlock) && CanPlaceTile(i+1, j, ItemID.DirtBlock);
            }
            return Collision.EmptyTile(i, j, false) && !Main.tile[i, j].active();
            
        }

        public static bool CanPlaceWall(int i, int j, int type)
        {
            return Main.tile[i, j].wall == WallID.None;
        }

        public static bool CanPayCost(ItemCost cost, Player player)
        {
            int count = 0;
            foreach (var v in player.inventory)
            {
                if (v != null)
                {
                    if (v.netID == cost.itemid)
                    {
                        count += v.stack;
                    }
                }
            }

            return count >= cost.count;
        }

        public static bool CanPayCost(ItemCost[] cost, Player player)
        {
            foreach (var v in cost)
            {
                if (!CanPayCost(v, player))
                {
					Item item = new Item();
					item.SetDefaults(v.itemid);

					Main.NewText("You need " + v.count + " of " + item.HoverName + " to build a house.");

                    return false;
                }
            }
            return true;
        }

        public static void PayCost(ItemCost cost, Player player)
        {
            int count = cost.count;

            foreach (var v in player.inventory)
            {
                if (v != null)
                {
                    if (v.netID == cost.itemid)
                    {
                        if (v.stack <= count)
                        {
                            count -= v.stack;
                            v.TurnToAir();
                        }
                        else
                        {
                            v.stack -= count;
                            break;
                        }
                    }
                }
            }
        }

        public static void PayCost(ItemCost[] cost, Player player)
        {
            foreach (var v in cost)
            {
                PayCost(v, player);
            }
        }


        public static bool BuildHouse(int tileX, int tileY, int type, bool local = false)
        {
            

            CostHandler ch = new CostHandler();

            int bound0 = Tiles.GetUpperBound(1) + 1;
            int bound1 = Tiles.GetUpperBound(0) + 1;
            bool build = false;
            if (local)
            {
                for (int x = 0; x < bound0; x++)
                {
                    for (int y = bound1 - 1; y >= 0; y--)
                    {
                        int cx = tileX + x;
                        int cy = tileY + y - bound1 + 1;

                        int id = Tiles[y, x] - 1;

                        if(id == -1)
                        {
                            if (!CanPlaceTile(cx, cy, TileID.Dirt))
                            {
                                Main.NewText("Failed! The house would interfere with blocks already present.", new Color(255, 0, 0));
                                return false;
                            }
                        }

                        for (int i = 0; i < houseblocks[type].Length; i++)
                        {
                            if (id == i)
                            {

                                if (Main.tile[cx, cy].type != houseblocks[type][id].NetID)
                                {
                                    if (!CanPlaceTile(cx, cy, houseblocks[type][id].NetID))
                                    {
                                        Main.NewText("Failed! The house would interfere with blocks already present.", new Color(255, 0, 0));
                                        return false;
                                    }

                                    ch.Add(houseblocks[type][id].cost);
                                    build = true;
                                }
                            }
                        }

                        id = Walls[y, x] - 1;
                        for (int i = 0; i < housewalls[type].Length; i++)
                        {
                            if (id == i)
                            {
                                if (Main.tile[cx, cy].wall != housewalls[type][id].NetID)
                                {
                                    if (!CanPlaceWall(cx, cy, housewalls[type][id].NetID))
                                    {
                                        Main.NewText("Failed! The house would interfere with blocks already present.", new Color(255, 0, 0));
                                        return false;
                                    }

                                    ch.Add(housewalls[type][id].cost);
                                    build = true;
                                }
                            }
                        }
                    }
                }

                if (!build)
                {
                    return false;
                }

                if (!CanPayCost(ch.ToArray(), Main.LocalPlayer))
                {
                    Main.NewText("Failed! You cannot pay for the house cost.", new Color(255, 0, 0));
                    return false;
                }
                PayCost(ch.ToArray(), Main.LocalPlayer);

                
            }


            for (int i = 0; i < houseblocks[type].Length; i++)
            {
                for (int x = 0; x < bound0; x++)
                {
                    for (int y = bound1 - 1; y >= 0; y--)
                    {
                        int cx = tileX + x;
                        int cy = tileY + y - bound1 + 1;

                        int id = Tiles[y, x] - 1;
                        if (id == i)
                        {

                            if (Main.tile[cx, cy].type != houseblocks[type][id].NetID)
                            {
                                WorldGen.PlaceTile(cx, cy, houseblocks[type][id].NetID);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < housewalls[type].Length; i++)
            {
                for (int x = 0; x < bound0; x++)
                {
                    for (int y = bound1 - 1; y >= 0; y--)
                    {
                        int cx = tileX + x;
                        int cy = tileY + y - bound1 + 1;

                        int id = Walls[y, x] - 1;
                        if (id == i)
                        {

                            if (Main.tile[cx, cy].wall != housewalls[type][id].NetID)
                            {
                                WorldGen.PlaceWall(cx, cy, housewalls[type][id].NetID);
                            }
                        }
                    }
                }
            }
            

            
            return true;
        }
    }
}
