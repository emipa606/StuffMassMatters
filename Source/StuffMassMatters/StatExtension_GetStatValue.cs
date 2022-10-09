using HarmonyLib;
using RimWorld;
using Verse;

namespace StuffMassMatters;

[HarmonyPatch(typeof(StatExtension), "GetStatValue")]
public static class StatExtension_GetStatValue
{
    public static void Postfix(Thing thing, StatDef stat, ref float __result)
    {
        if (stat != StatDefOf.Mass)
        {
            return;
        }

        if (thing == null)
        {
            return;
        }

        __result = Main.CalculateRelativeMass(thing, __result);
    }
}