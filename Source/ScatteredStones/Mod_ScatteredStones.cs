using HarmonyLib;
using Verse;
using UnityEngine;
using System;


// NOTE: This entry point is designed for mod compatibility and safety.
// - Harmony patches are applied only to targeted methods.
// - No static state is shared outside the mod.
// - All user-facing strings should be localized.
// - Avoids patching core game logic unless required for features.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    /// <summary>
    /// Main mod class for Scattered Stones. Handles initialization and patching.
    /// </summary>
    public class Mod_ScatteredStones : Mod
    {
        /// <summary>
        /// Initializes the mod, applies Harmony patches, and sets up stone data.
        /// </summary>
        /// <param name="content">The mod content pack.</param>
        public Mod_ScatteredStones(ModContentPack content) : base(content)
        {
            new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
            LongEventHandler.QueueLongEvent(() => ScatteredStonesUtility.Setup(), null, false, null);
        }
    }
}
