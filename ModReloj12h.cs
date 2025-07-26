using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace ModReloj12h
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.DayTimeMoneyBox), nameof(StardewValley.Menus.DayTimeMoneyBox.draw), new[] { typeof(SpriteBatch) }),
                transpiler: new HarmonyMethod(typeof(DayTimeMoneyBoxPatch), nameof(DayTimeMoneyBoxPatch.Draw_Transpiler))
            );
        }
    }
}