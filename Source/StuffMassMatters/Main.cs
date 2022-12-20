using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace StuffMassMatters;

[StaticConstructorOnStartup]
public static class Main
{
    public static readonly Dictionary<StuffCategoryDef, List<ThingDef>> StuffCategoryThings;
    public static readonly Dictionary<Tuple<ThingDef, ThingDef>, float> ThingMasses;

    static Main()
    {
        StuffCategoryThings = new Dictionary<StuffCategoryDef, List<ThingDef>>();
        ThingMasses = new Dictionary<Tuple<ThingDef, ThingDef>, float>();
        foreach (var stuffCategoryDef in DefDatabase<StuffCategoryDef>.AllDefsListForReading)
        {
            StuffCategoryThings[stuffCategoryDef] = DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
                def.IsStuff && def.stuffProps?.categories.Contains(stuffCategoryDef) == true).ToList();
        }

        Log.Message($"[StuffMassMatters]: Cached {StuffCategoryThings.Count} stuffcategories");

        var harmony = new Harmony("Mlie.StuffMassMatters");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static float CalculateRelativeMass(Thing thing, float vanillaMass)
    {
        if (thing?.def == null)
        {
            return vanillaMass;
        }

        if (!thing.def.MadeFromStuff)
        {
            return vanillaMass;
        }

        if (thing.Stuff == null)
        {
            return vanillaMass;
        }

        if (thing.def.stuffCategories == null || thing.def.stuffCategories.Any() == false)
        {
            return vanillaMass;
        }

        return CalculateRelativeMass(thing.def, thing.Stuff, vanillaMass);
    }

    public static float CalculateRelativeMass(ThingDef thingDef, ThingDef stuff, float vanillaMass)
    {
        if (thingDef == null)
        {
            return vanillaMass;
        }

        var currentTuple = new Tuple<ThingDef, ThingDef>(thingDef, stuff);

        if (ThingMasses.ContainsKey(currentTuple))
        {
            return ThingMasses[currentTuple];
        }

        var canBeMadeFrom = new HashSet<ThingDef>();
        foreach (var stuffPropsCategory in thingDef.stuffCategories)
        {
            canBeMadeFrom.AddRange(StuffCategoryThings[stuffPropsCategory]);
        }

        var result = canBeMadeFrom.TryMaxBy(def => def.stuffProps.commonality, out var baseThing);
        if (!result)
        {
            ThingMasses[currentTuple] = vanillaMass;
            return vanillaMass;
        }

        var stuffMass = stuff.BaseMass;
        if (stuff.smallVolume)
        {
            stuffMass *= 75f;
        }

        ThingMasses[currentTuple] = vanillaMass * (stuffMass / baseThing.BaseMass);
        return ThingMasses[currentTuple];
    }
}