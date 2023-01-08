using HarmonyLib;
using RimWorld;

namespace StuffMassMatters;

[HarmonyPatch(typeof(StatUtility), "GetStatOffsetFromList")]
public static class StatUtility_GetStatOffsetFromList
{
    public static void Postfix(StatDef stat, ref float __result)
    {
        if (!StuffMassMattersMod.instance.Settings.AffectMovementSpeedFactors || stat != StatDefOf.MoveSpeed ||
            Main.CurrentThing == null)
        {
            return;
        }

        __result = Main.CalculateRelativeMoveSpeedFactor(Main.CurrentThing, __result);
        Main.CurrentThing = null;
    }
}