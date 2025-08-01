using System.Collections.Generic;
using UnityEngine;
using Verse;
using static ConsolidatedModsTextures.ScatteredStones.ModSettings_ScatteredStones;
using static ConsolidatedModsTextures.ScatteredStones.ResourceBank.ThingCategoryDefOf;
using static ConsolidatedModsTextures.ScatteredStones.ResourceBank.ThingDefOf;

namespace ConsolidatedModsTextures.ScatteredStones
{
	public static class Utility
	{
		public const float minScaleModifier = 1f;
		public const float maxScaleModifier = 1f;
		public const float offsetModifier = 1f;
		public const bool allowOnWater = true;
		public const bool minedFilth = true;
		public const bool neverDespawn = false;
		
		public static HashSet<ushort> stoneChunks = new HashSet<ushort>(), stoneCliff = new HashSet<ushort>();

		public static void Setup()
		{
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				if (thingDef.thingCategories?.Contains(StoneChunks) ?? false) stoneChunks.Add(thingDef.index);
				else if ((thingDef.building?.isNaturalRock ?? false) && !thingDef.building.isResourceRock) stoneCliff.Add(thingDef.index);
			}
            UpdateModifiers();
		}

		public static void UpdateModifiers()
		{
			foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				RandomDraw modX = def.GetModExtension<RandomDraw>();
				if (modX == null) continue;

				modX.minSizeModified = modX.minSize * minScaleModifier;
				modX.maxSizeModified = Mathf.Clamp(modX.maxSize * maxScaleModifier, modX.minSizeModified, 10);
				modX.offsetRangeModified = modX.offsetRange * offsetModifier;
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
                if(map.terrainGrid.TerrainAt(cell)?.IsWater == true)
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
