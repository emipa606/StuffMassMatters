using Mlie;
using UnityEngine;
using Verse;

namespace StuffMassMatters;

[StaticConstructorOnStartup]
internal class StuffMassMattersMod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static StuffMassMattersMod instance;

    private static string currentVersion;

    /// <summary>
    ///     The private settings
    /// </summary>
    private StuffMassMattersSettings settings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public StuffMassMattersMod(ModContentPack content) : base(content)
    {
        instance = this;
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal StuffMassMattersSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = GetSettings<StuffMassMattersSettings>();
            }

            return settings;
        }
        set => settings = value;
    }

    /// <summary>
    ///     The title for the mod-settings
    /// </summary>
    /// <returns></returns>
    public override string SettingsCategory()
    {
        return "Stuff Mass Matters";
    }

    /// <summary>
    ///     The settings-window
    ///     For more info: https://rimworldwiki.com/wiki/Modding_Tutorials/ModSettings
    /// </summary>
    /// <param name="rect"></param>
    public override void DoSettingsWindowContents(Rect rect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(rect);
        listing_Standard.Gap();
        listing_Standard.CheckboxLabeled("SMM.AffectMovementSpeedFactors".Translate(),
            ref Settings.AffectMovementSpeedFactors, "SMM.AffectMovementSpeedFactorsTT".Translate());
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("SMM.modVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
        Settings.Write();
    }
}