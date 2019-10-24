using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static LansHouseBuilder.HouseBuilder;

namespace LansHouseBuilder
{
    class HouseItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Creates a basic and suitable house.");
            DisplayName.SetDefault("HouseBuilder (Wood)");
        }

        public override void SetDefaults()
        {
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = 5;
            item.width = 20;
            item.height = 20;
            //item.consumable = true;
        }
        

        public override bool UseItem(Player player)
        {
            if (player.whoAmI != Main.LocalPlayer.whoAmI)
            {
                return true;
            }


            Vector2 mousePosition = Main.MouseWorld;

            int tileX = (int)(mousePosition.X / 16f);
            int tileY = (int)(mousePosition.Y / 16f);

            if (Builder.BuildHouse(tileX, tileY, 0, true))
            {
                Main.LocalPlayer.GetModPlayer<HouseBuilderModPlayer>().houseTileX = tileX;
                Main.LocalPlayer.GetModPlayer<HouseBuilderModPlayer>().houseTileY = tileY;
            }

			return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wood, 1);
            recipe.anyWood = true;
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
