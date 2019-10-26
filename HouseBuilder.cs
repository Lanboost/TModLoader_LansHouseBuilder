using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LansHouseBuilder
{
	public class HouseBuilder : Mod
	{

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
		

		public void AddExtraDraw(ILContext il)
		{
			var c = new ILCursor(il);

			var texture = this.GetTexture("housecursor");

			var id1 = this.GetItem("HouseItem").item.type;
			var id2 = this.GetItem("HouseItemStone").item.type;

			c.EmitDelegate<Action>(delegate () {
				if (!texture.IsDisposed)
				{
					if (Main.LocalPlayer.HeldItem.type == id1)
					{
						Microsoft.Xna.Framework.Color color = new Color(255, 255, 255, 127);
						Main.spriteBatch.Draw(texture, new Vector2((float)Main.mouseX - 8, (float)Main.mouseY - texture.Height + 8) + Vector2.One, null, color, 0f, default(Vector2), 1, SpriteEffects.None, 0f);
					}
					else if (Main.LocalPlayer.HeldItem.type == id2)
					{
						Microsoft.Xna.Framework.Color color = new Color(255, 255, 255, 127);
						Main.spriteBatch.Draw(texture, new Vector2((float)Main.mouseX - 8, (float)Main.mouseY - texture.Height + 8) + Vector2.One, null, color, 0f, default(Vector2), 1, SpriteEffects.None, 0f);

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