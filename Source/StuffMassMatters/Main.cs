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
    public static readonly Dictionary<Tuple<ThingDef, ThingDef>, float> ThingSpeedFactors;

    public static Thing CurrentThing;

    static Main()
    {
        StuffCategoryThings = new Dictionary<StuffCategoryDef, List<ThingDef>>();
        ThingMasses = new Dictionary<Tuple<ThingDef, ThingDef>, float>();
        ThingSpeedFactors = new Dictionary<Tuple<ThingDef, ThingDef>, float>();
        foreach (var stuffCategoryDef in DefDatabase<StuffCategoryDef>.AllDefsListForReading)
        {
            StuffCategoryThings[stuffCategoryDef] = DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
                def.IsStuff && def.stuffProps?.categories.Contains(stuffCategoryDef) == true).ToList();
        }

        Log.Message($"[StuffMassMatters]: Cached {StuffCategoryThings.Count} stuffcategories");

        var harmony = new Harmony("Mlie.StuffMassMatters");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }


    public static float CalculateRelativeMoveSpeedFactor(Thing thing, float movespeedFactor)
    {
        if (movespeedFactor >= 0)
        {
            return movespeedFactor;
        }

        if (thing?.def == null)
        {
            return movespeedFactor;
        }

        if (!thing.def.MadeFromStuff)
        {
            return movespeedFactor;
        }

        if (thing.Stuff == null)
        {
            return movespeedFactor;
        }

        if (thing.def.stuffCategories == null || thing.def.stuffCategories.Any() == false)
        {
            return movespeedFactor;
        }

        var currentTuple = new Tuple<ThingDef, ThingDef>(thing.def, thing.Stuff);

        if (ThingSpeedFactors.ContainsKey(currentTuple))
        {
            return ThingSpeedFactors[currentTuple];
        }

        var canBeMadeFrom = new HashSet<ThingDef>();
        foreach (var stuffPropsCategory in thing.def.stuffCategories)
        {
            canBeMadeFrom.AddRange(StuffCategoryThings[stuffPropsCategory]);
        }

        var result = canBeMadeFrom.TryMaxBy(def => def.stuffProps.commonality, out var baseThing);
        if (!result)
        {
            ThingSpeedFactors[currentTuple] = movespeedFactor;
            return movespeedFactor;
        }

        var stuffMass = thing.Stuff.BaseMass;
        if (thing.Stuff.smallVolume)
        {
            stuffMass *= 75f;
        }

        ThingSpeedFactors[currentTuple] = movespeedFactor * (stuffMass / baseThing.BaseMass);

        return ThingSpeedFactors[currentTuple];
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