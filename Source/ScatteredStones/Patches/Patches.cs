
using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using static ScatteredStones.ResourceBank.ThingDefOf;

using static ScatteredStones.ScatteredStonesUtility;

// NOTE: All patches are designed for mod compatibility and safety.
// - Only targeted methods are patched, not core game logic unless required.
// - All Thing and Def accesses are null-checked where possible.
// - All user-facing strings should be localized.
// - Avoids patching core game logic unless required for features.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    #region Patch: Reset filth timer under mineables
    /// <summary>
    /// Resets the filth timer under mineables when mined to prevent immediate despawn.
    /// </summary>
    [HarmonyPatch(typeof(Mineable), nameof(Mineable.DestroyMined))]
    public class Patch_Destroy
    {
        public static void Prefix(Mineable __instance)
        {
            var list = __instance.Map?.thingGrid.ThingsListAtFast(__instance.Position);
            var length = list.Count;
            for (int i = 0; i < length; i++)
            {
                var item = list[i];
                if (item.def == Owl_Filth_Rocks) ((Filth)item).ThickenFilth();
            }
        }
    }
    #endregion

    #region Patch: Clean up unreachable filth after building placement
    /// <summary>
    /// Cleans up unreachable filth after a new building is spawned.
    /// </summary>
    [HarmonyPatch(typeof(Building), nameof(Building.SpawnSetup))]
    public class Patch_SpawnSetup
    {
        public static void Postfix(Map map, Building __instance)
        {
            if (__instance.def.passability != Traversability.Standable && map?.AgeInDays != 0 && __instance.positionInt != IntVec3.Invalid)
            {
                IntVec3[] list = GenAdjFast.AdjacentCellsCardinal(__instance).ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    var cell = list[i];
                    if (cell.InBounds(map) && !cell.Standable(map)) ScatteredStonesUtility.ValidateCell(cell, map, true);
                }
            }
        }
    }
    #endregion

    #region Patch: Place filth when something is mined
    /// <summary>
    /// Places filth under mined objects if appropriate.
    /// </summary>
    [HarmonyPatch(typeof(Mineable), nameof(Mineable.TrySpawnYield), new System.Type[] { typeof(Map), typeof(bool), typeof(Pawn) })]
    public class Patch_TrySpawnYield
    {
        public static void Postfix(Mineable __instance, Map map)
        {
            // Always true, as settings are removed
            List<Thing> list = map.thingGrid.ThingsListAtFast(__instance.positionInt);
            var length = list.Count;
            for (int i = 0; i < length; i++)
            {
                var item = list[i];
                if (stoneChunks.Contains(item.def.index))
                {
                    Rocks rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null) as Rocks;
                    GenPlace.TryPlaceThing(rocks, __instance.Position, map, ThingPlaceMode.Direct);
                    rocks.DrawColor = rocks.MatchColor(__instance);
                }
            }
        }
    }
    #endregion

    #region Patch: Prevent filth from being wiped by new spawns
    /// <summary>
    /// Prevents filth from being wiped when placing something on top of a chunk.
    /// </summary>
    [HarmonyPatch(typeof(GenSpawn), nameof(GenSpawn.SpawningWipes))]
    public class Patch_SpawningWipes
    {
        public static bool Postfix(bool __result, BuildableDef newEntDef, BuildableDef oldEntDef)
        {
            if (oldEntDef == ResourceBank.ThingDefOf.Owl_Filth_Rocks)
            {
                var index = newEntDef?.index ?? 0;
                if (stoneChunks.Contains(index) || stoneCliff.Contains(index)) return false;
            }
            return __result;
        }
    }
    #endregion

}
