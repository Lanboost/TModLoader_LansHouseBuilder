using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static LansHouseBuilder.HouseBuilder;

namespace LansHouseBuilder
{
    class HouseBuilderModPlayer:ModPlayer
    {

        public int houseTileX;
        public int houseTileY;
        public int houesType;

        public override void SendClientChanges(ModPlayer clientPlayer)
        {            

            if (houseTileX != -1)
            {
                var packet = mod.GetPacket();
                packet.Write((byte)HouseBuilderModMessageType.HouseBuilderBuildHouse);
                packet.Write((byte)Main.LocalPlayer.whoAmI);
                packet.Write(houseTileX);
                packet.Write(houseTileY);
                packet.Write(houesType);
                packet.Send();
                houseTileX = -1;
            }
        }
    }
}
