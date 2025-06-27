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
    private static readonly Dictionary<StuffCategoryDef, List<ThingDef>> stuffCategoryThings;
    private static readonly Dictionary<Tuple<ThingDef, ThingDef>, float> thingMasses;
    private static readonly Dictionary<Tuple<ThingDef, ThingDef>, float> thingSpeedFactors;

    public static Thing CurrentThing;

    static Main()
    {
        stuffCategoryThings = new Dictionary<StuffCategoryDef, List<ThingDef>>();
        thingMasses = new Dictionary<Tuple<ThingDef, ThingDef>, float>();
        thingSpeedFactors = new Dictionary<Tuple<ThingDef, ThingDef>, float>();
        foreach (var stuffCategoryDef in DefDatabase<StuffCategoryDef>.AllDefsListForReading)
        {
            stuffCategoryThings[stuffCategoryDef] = DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
                def.IsStuff && def.stuffProps?.categories.Contains(stuffCategoryDef) == true).ToList();
        }

        Log.Message($"[StuffMassMatters]: Cached {stuffCategoryThings.Count} stuffcategories");

        var harmony = new Harmony("Mlie.StuffMassMatters");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }


    public static float CalculateRelativeMoveSpeedFactor(Thing thing, float movespeedFactor)
    {
        if (movespeedFactor >= 0 || thing?.def is not { MadeFromStuff: true } || thing.Stuff == null ||
            thing.def.stuffCategories == null || !thing.def.stuffCategories.Any())
        {
            return movespeedFactor;
        }

        var currentTuple = new Tuple<ThingDef, ThingDef>(thing.def, thing.Stuff);

        if (thingSpeedFactors.TryGetValue(currentTuple, out var factor))
        {
            return factor;
        }

        var canBeMadeFrom = new HashSet<ThingDef>();
        foreach (var stuffPropsCategory in thing.def.stuffCategories)
        {
            canBeMadeFrom.AddRange(stuffCategoryThings[stuffPropsCategory]);
        }

        var result = canBeMadeFrom.TryMaxBy(def => def.stuffProps.commonality, out var baseThing);
        if (!result)
        {
            thingSpeedFactors[currentTuple] = movespeedFactor;
            return movespeedFactor;
        }

        var stuffMass = thing.Stuff.BaseMass;
        if (thing.Stuff.smallVolume)
        {
            stuffMass *= 75f;
        }

        thingSpeedFactors[currentTuple] = movespeedFactor * (stuffMass / baseThing.BaseMass);

        return thingSpeedFactors[currentTuple];
    }

    public static float CalculateRelativeMass(Thing thing, float vanillaMass)
    {
        if (thing?.def is not { MadeFromStuff: true } || thing.Stuff == null || thing.def.stuffCategories == null ||
            !thing.def.stuffCategories.Any())
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

        if (thingMasses.TryGetValue(currentTuple, out var mass))
        {
            return mass;
        }

        var canBeMadeFrom = new HashSet<ThingDef>();
        foreach (var stuffPropsCategory in thingDef.stuffCategories)
        {
            canBeMadeFrom.AddRange(stuffCategoryThings[stuffPropsCategory]);
        }

        var result = canBeMadeFrom.TryMaxBy(def => def.stuffProps.commonality, out var baseThing);
        if (!result)
        {
            thingMasses[currentTuple] = vanillaMass;
            return vanillaMass;
        }

        var stuffMass = stuff.BaseMass;
        if (stuff.smallVolume)
        {
            stuffMass *= 75f;
        }

        thingMasses[currentTuple] = vanillaMass * (stuffMass / baseThing.BaseMass);
        return thingMasses[currentTuple];
    }
}