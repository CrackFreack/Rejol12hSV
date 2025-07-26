using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ModReloj12h
{
    internal class DayTimeMoneyBoxPatch
    {
        private static readonly FieldInfo f_timeText = AccessTools.Field(typeof(DayTimeMoneyBox), "_timeText");
        private static readonly MethodInfo m_measureString = AccessTools.Method(typeof(SpriteFont), nameof(SpriteFont.MeasureString), new Type[] { typeof(StringBuilder) });
        private static readonly MethodInfo m_updateClockTime = AccessTools.Method(typeof(DayTimeMoneyBoxPatch), nameof(UpdateClockTime));

        public static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand as FieldInfo == f_timeText &&
                    codes[i + 1].opcode == OpCodes.Callvirt && codes[i + 1].operand as MethodInfo == m_measureString)
                {
                    // Insert before _timeText.MeasureString(...)
                    codes.Insert(i, new CodeInstruction(OpCodes.Call, m_updateClockTime));
                    codes.Insert(i, new CodeInstruction(OpCodes.Ldfld, f_timeText));
                    codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                    break;
                }
            }

            return codes;
        }

        public static void UpdateClockTime(StringBuilder builder)
        {
            builder.Clear();
            int time = Game1.timeOfDay;

            int hour = time / 100;
            int minute = time % 100;

            string suffix = (hour >= 12 && hour < 2400) ? "PM" : "AM";
            hour = hour % 12;
            if (hour == 0) hour = 12;

            builder.Append($"{hour}:{minute:00} {suffix}");
        }
    }
}
