using HarmonyLib;
using RimWorld;
using Verse;

namespace StuffMassMatters;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.StatOffsetFromGear))]
public static class StatWorker_StatOffsetFromGear
{
    public static void Prefix(Thing gear, StatDef stat)
    {
        if (!StuffMassMattersMod.Instance.Settings.AffectMovementSpeedFactors)
        {
            return;
        }

        if (stat != StatDefOf.MoveSpeed)
        {
            Main.CurrentThing = null;
            return;
        }

        Main.CurrentThing = gear;
    }
}