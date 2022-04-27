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
    public static readonly Dictionary<Thing, float> ThingMasses;

    static Main()
    {
        StuffCategoryThings = new Dictionary<StuffCategoryDef, List<ThingDef>>();
        ThingMasses = new Dictionary<Thing, float>();
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

        if (ThingMasses.ContainsKey(thing))
        {
            return ThingMasses[thing];
        }


        if (thing.def.stuffCategories == null || thing.def.stuffCategories.Any() == false)
        {
            ThingMasses[thing] = vanillaMass;
            return vanillaMass;
        }

        var canBeMadeFrom = new HashSet<ThingDef>();
        foreach (var stuffPropsCategory in thing.def.stuffCategories)
        {
            canBeMadeFrom.AddRange(StuffCategoryThings[stuffPropsCategory]);
        }

        var result = canBeMadeFrom.TryMaxBy(def => def.stuffProps.commonality, out var baseThing);
        if (!result)
        {
            ThingMasses[thing] = vanillaMass;
            return vanillaMass;
        }

        var stuffMass = thing.Stuff.BaseMass;
        if (thing.Stuff.smallVolume)
        {
            stuffMass *= 75f;
        }

        ThingMasses[thing] = vanillaMass * (stuffMass / baseThing.BaseMass);
        return ThingMasses[thing];
    }
}