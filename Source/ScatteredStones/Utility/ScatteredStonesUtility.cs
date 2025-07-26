using Verse;
using UnityEngine;
using System.Collections.Generic;

using static ScatteredStones.ResourceBank.ThingCategoryDefOf;
using static ScatteredStones.ResourceBank.ThingDefOf;

// NOTE: This utility is designed to be safe for use with other mods.
// - All ThingDef and Thing accesses are null-checked.
// - No static state is modified outside of the mod's own data.
// - All Harmony patches should use Postfix/Prefix and avoid transpilers unless necessary.
// - Avoids patching core game logic unless required for feature.
//
// User Experience:
// - All user-facing strings should be localized via Keyed XML.
// - If adding new features, ensure they are toggleable or compatible with other mods.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    /// <summary>
    /// <summary>
    /// Utility methods and data for the Scattered Stones mod.
    /// 
    /// <para>Compatibility:</para>
    /// <list type="bullet">
    /// <item>Null checks are used for all map, thing, and def accesses.</item>
    /// <item>Does not patch core game logic except where necessary (see Patches.cs).</item>
    /// <item>All static data is mod-local and not shared globally.</item>
    /// </list>
    /// 
    /// <para>User Experience:</para>
    /// <list type="bullet">
    /// <item>All user-facing strings should be localized in Keyed XML.</item>
    /// <item>Features should be compatible with other mods and not break saves.</item>
    /// </list>
    /// </summary>
    /// </summary>
    public static class ScatteredStonesUtility
    {
        /// <summary>
        /// Set of ThingDef indices for stone chunks.
        /// </summary>
        public static readonly HashSet<ushort> stoneChunks = new HashSet<ushort>();
        public static readonly HashSet<ushort> stoneCliff = new HashSet<ushort>();

        /// <summary>
        /// Initializes the stone chunk and cliff sets from all ThingDefs.
        /// </summary>
        public static void Setup()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (thingDef == null) continue;
                if (thingDef.thingCategories?.Contains(StoneChunks) ?? false) stoneChunks.Add(thingDef.index);
                else if ((thingDef.building?.isNaturalRock ?? false) && !thingDef.building.isResourceRock) stoneCliff.Add(thingDef.index);
            }
            UpdateModifiers();
        }

        /// <summary>
        /// Updates the modified size and offset values for all RandomDraw mod extensions.
        /// </summary>
        public static void UpdateModifiers()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (def == null) continue;
                RandomDraw modX = def.GetModExtension<RandomDraw>();
                if (modX == null) continue;

                modX.minSizeModified = modX.minSize * 1f;
                modX.maxSizeModified = Mathf.Clamp(modX.maxSize * 1f, modX.minSizeModified, 10);
                modX.offsetRangeModified = modX.offsetRange * 1f;
            }
        }

        /// <summary>
        /// Validates a cell for filth placement and optionally cleans up unreachable filth.
        /// </summary>
        /// <param name="pos">The cell position to validate.</param>
        /// <param name="map">The map to check.</param>
        /// <param name="autoClean">If true, will remove filth if the cell is invalid.</param>
        /// <returns>True if the cell is valid for filth, otherwise false.</returns>
        public static bool ValidateCell(IntVec3 pos, Map map, bool autoClean = false)
        {
            // Cache the local filth here to check if we even need to bother processing this cell
            List<Thing> localFilth = new List<Thing>();
            if (map?.thingGrid == null) return false;
            foreach (var item in map.thingGrid.ThingsListAtFast(pos))
            {
                if (item?.def == null) continue;
                if (item.def == Owl_Filth_Rocks) localFilth.Add(item);
            }
            if (autoClean && localFilth.Count == 0) return true;

            // Fetch adjacent cells and process them
            int i = 0;
            foreach (var cell in GenAdjFast.AdjacentCellsCardinal(pos))
            {
                if (!cell.InBounds(map)) continue;
                if (map.terrainGrid?.TerrainAt(cell)?.IsWater == true)
                {
                    i = 4;
                    break;
                }
                foreach (var item in map.thingGrid.ThingsListAtFast(cell))
                {
                    if (item?.def == null) continue;
                    if (item.def.fillPercent == 1 || item.def.passability != Traversability.Standable) ++i;
                }
            }
            // Check the score, delete/false if more than 3
            if (i > 3)
            {
                if (autoClean) foreach (var item in localFilth) item.DeSpawn();
                return false;
            }
            return true;
        }
    }
}
