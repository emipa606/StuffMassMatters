using HarmonyLib;
using RimWorld;
using Verse;

namespace StuffMassMatters;

[HarmonyPatch(typeof(StatExtension), "GetStatValueAbstract", typeof(BuildableDef), typeof(StatDef), typeof(ThingDef))]
public static class StatExtension_GetStatValueAbstract
{
    public static void Postfix(BuildableDef def, StatDef stat, ThingDef stuff, ref float __result)
    {
        if (stat != StatDefOf.Mass)
        {
            return;
        }

        if (stuff == null)
        {
            return;
        }

        __result = Main.CalculateRelativeMass((ThingDef)def, stuff, __result);
    }
}