using Verse;
using UnityEngine;
using System.Collections.Generic;
using static ConsolidatedMods.Textures.ResourceBank.ThingCategoryDefOf;
using static ConsolidatedMods.Textures.ResourceBank.ThingDefOf;

namespace ConsolidatedMods.Textures
{
    public static class ScatteredStonesUtility
    {
        public static HashSet<ushort> stoneChunks = new HashSet<ushort>(), stoneCliff = new HashSet<ushort>();

        public static void Setup()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (thingDef.thingCategories?.Contains(StoneChunks) ?? false) stoneChunks.Add(thingDef.index);
                else if ((thingDef.building?.isNaturalRock ?? false) && !thingDef.building.isResourceRock) stoneCliff.Add(thingDef.index);
            }
        }


        public static bool ValidateCell(IntVec3 pos, Map map, bool autoClean = false)
        {
            //Cache the local filth here to check if we even need to bother processing this cell
            List<Thing> localFilth = new List<Thing>();
            foreach (var item in map.thingGrid.ThingsListAtFast(pos))
            {
                if (item.def == Owl_Filth_Rocks) localFilth.Add(item);
            }
            if (autoClean && localFilth.Count == 0) return true;

            //Fetch adjacent cells and proceess them
            int i = 0;
            foreach (var cell in GenAdjFast.AdjacentCellsCardinal(pos))
            {
                if (!cell.InBounds(map)) continue;

                //Check for passibility and water...
                if (map.terrainGrid.TerrainAt(cell)?.IsWater == true)
                {
                    i = 4;
                    break;
                }
                //Add a "point" if conditions are met.
                foreach (var item in map.thingGrid.ThingsListAtFast(cell))
                {
                    if (item.def.fillPercent == 1 || item.def.passability != Traversability.Standable) ++i;
                }
            }
            //Check the score, delete/false if more than 3
            if (i > 3)
            {
                if (autoClean) foreach (var item in localFilth) item.DeSpawn();
                return false;
            }
            return true;
        }
    }
}
