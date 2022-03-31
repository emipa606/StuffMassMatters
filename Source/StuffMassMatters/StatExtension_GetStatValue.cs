using HarmonyLib;
using RimWorld;
using Verse;

namespace StuffMassMatters;

[HarmonyPatch(typeof(StatExtension), "GetStatValue", typeof(Thing), typeof(StatDef), typeof(bool))]
public static class StatExtension_GetStatValue
{
    public static void Postfix(Thing thing, StatDef stat, ref float __result)
    {
        if (stat != StatDefOf.Mass)
        {
            return;
        }

        __result = Main.CalculateRelativeMass(thing, __result);
    }
}