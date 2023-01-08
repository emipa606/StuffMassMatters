using Verse;

namespace StuffMassMatters;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class StuffMassMattersSettings : ModSettings
{
    public bool AffectMovementSpeedFactors = true;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref AffectMovementSpeedFactors, "AffectMovementSpeedFactors", true);
    }
}