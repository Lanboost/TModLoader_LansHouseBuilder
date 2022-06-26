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
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = 5;
            Item.width = 20;
            Item.height = 20;
            //item.consumable = true;
        }
        

        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
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
                Main.LocalPlayer.GetModPlayer<HouseBuilderModPlayer>().houesType = 0;
            }

			return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddRecipeGroup("Wood", 1);
            recipe.Register();
        }
    }
}
