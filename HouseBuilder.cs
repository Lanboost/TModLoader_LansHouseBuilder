using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LansHouseBuilder
{
	public class HouseBuilder : Mod
	{
		public static int TorchRecipeGroup;
		public HouseBuilder()
		{
		}

		public override void Load()
		{
			base.Load();

			if (Main.netMode != NetmodeID.Server)
			{
				IL.Terraria.Main.DrawCursor += AddExtraDraw;
			}
		}

		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Torch", new int[]
			{
			ItemID.Torch,
			ItemID.IceTorch,
			ItemID.PurpleTorch,
			ItemID.YellowTorch,
			ItemID.BlueTorch,
			ItemID.GreenTorch,
			ItemID.RedTorch,
			ItemID.OrangeTorch,
			ItemID.WhiteTorch,
			ItemID.PinkTorch,
			ItemID.CursedTorch,
			ItemID.IchorTorch,
			ItemID.DemonTorch,
			ItemID.RainbowTorch,
			ItemID.UltrabrightTorch,
			ItemID.BoneTorch

			});
			TorchRecipeGroup = RecipeGroup.RegisterGroup("LansHouseBuilder:Torches", group);
		}


		public void AddExtraDraw(ILContext il)
		{
			var c = new ILCursor(il);

			var texture = this.GetTexture("housecursor");
			var texturestone = this.GetTexture("housecursorstone");
			
			c.EmitDelegate<Action>(delegate () {
				
				Vector2 mousePosition = Main.MouseWorld;

				if (Main.LocalPlayer == null || Main.LocalPlayer.HeldItem == null || mousePosition == null || Main.spriteBatch == null)
				{
					return;
				}

				var tileCoord = mousePosition.ToTileCoordinates();

				var displayCoord = tileCoord.ToWorldCoordinates() - Main.screenPosition;

				if (!Main.gameMenu)
				{
					if (texture != null && !texture.IsDisposed)
					{
						var item1 = this.GetItem("HouseItem");
						if (item1 != null && item1.item != null)
						{
							var id1 = item1.item.type;


							if (Main.LocalPlayer.HeldItem.type == id1)
							{
								Microsoft.Xna.Framework.Color color = new Color(255, 255, 255, 127);
								Main.spriteBatch.Draw(texture, new Vector2((float)displayCoord.X - 8, (float)displayCoord.Y - texture.Height + 8) + Vector2.One, null, color, 0f, default(Vector2), 1, SpriteEffects.None, 0f);
							}
						}
					}

					if (texturestone != null && !texturestone.IsDisposed)
					{
						var item2 = this.GetItem("HouseItemStone");
						if (item2 != null && item2.item != null)
						{
							var id2 = item2.item.type;

							if (Main.LocalPlayer.HeldItem.type == id2)
							{
								Microsoft.Xna.Framework.Color color = new Color(255, 255, 255, 127);
								Main.spriteBatch.Draw(texturestone, new Vector2((float)displayCoord.X - 8, (float)displayCoord.Y - texture.Height + 8) + Vector2.One, null, color, 0f, default(Vector2), 1, SpriteEffects.None, 0f);

							}
						}
					}
				}
			});

		}


		public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            HouseBuilderModMessageType msgType = (HouseBuilderModMessageType)reader.ReadByte();
            switch (msgType)
            {
                // This message sent by the server to initialize the Volcano Tremor on clients
                case HouseBuilderModMessageType.HouseBuilderBuildHouse:
                    {
                        int playernumber = reader.ReadByte();
                        int tileX = reader.ReadInt32();
                        int tileY = reader.ReadInt32();
                        int houseType = reader.ReadInt32();
                        Builder.BuildHouse(tileX, tileY, houseType);

                        if (Main.netMode == NetmodeID.Server)
                        {

                            var packet = GetPacket();
                            packet.Write((byte)HouseBuilderModMessageType.HouseBuilderBuildHouse);
                            packet.Write((byte)playernumber);
                            packet.Write(tileX);
                            packet.Write(tileY);
                            packet.Write(houseType);
                            
                            packet.Send(-1, playernumber);
                        }

                        break;
                    }
                
            }
        }



        internal enum HouseBuilderModMessageType : byte
        {
            HouseBuilderBuildHouse,
        }
    }
}