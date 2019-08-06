using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HouseBuilder
{
	public class HouseBuilder : Mod
	{
		public HouseBuilder()
		{
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