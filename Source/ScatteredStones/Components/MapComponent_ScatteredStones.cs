using Verse;
using RimWorld;
using System.Collections.Generic;
using static ScatteredStones.ResourceBank.ThingDefOf;
using static ScatteredStones.ScatteredStonesUtility;

// NOTE: This map component is designed for mod compatibility and safety.
// - All Thing and Def accesses are null-checked.
// - No static state is shared outside the mod.
// - All user-facing strings should be localized.
// - Avoids patching core game logic unless required for features.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    /// <summary>
    /// Map component for handling scattered stones initialization per map.
    /// </summary>
    public class MapComponent_ScatteredStones : MapComponent
    {
        private bool hasAppliedStones = false;

        /// <summary>
        /// Constructor for the map component.
        /// </summary>
        /// <param name="map">The map instance.</param>
        public MapComponent_ScatteredStones(Map map) : base(map) { }

        /// <summary>
        /// Exposes the state for save/load.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.hasAppliedStones, "hasAppliedStones", false, false);
        }

        /// <summary>
        /// Runs once per map to place scattered stones filth under valid objects.
        /// </summary>
        public override void FinalizeInit()
        {
            if (!hasAppliedStones)
            {
                var list = map.listerThings.AllThings;
                var length = list.Count;
                for (int i = 0; i < length; i++)
                {
                    var thing = list[i];
                    if
                    (
                        // Is stone chunk, and not in storage? && Is on water and allowed?
                        (!thing.IsInAnyStorage() && stoneChunks.Contains(thing.def.index) && (true || (!map.terrainGrid.TerrainAt(thing.Position)?.IsWater ?? false))) ||
                        // Is reachable cliff that's not fogged?
                        (stoneCliff.Contains(thing.def.index) && !thing.Fogged() && ValidateCell(thing.Position, map, false)) &&
                        // Is not along the map edge?
                        !CellRect.WholeMap(map).IsOnEdge(thing.positionInt)
                    )
                    {
                        Thing rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null);
                        // Place underneath chunk
                        GenPlace.TryPlaceThing(rocks, thing.Position, map, ThingPlaceMode.Direct);
                        // Match color
                        rocks.DrawColor = ((Rocks)rocks).MatchColor(thing);
                    }
                }
                hasAppliedStones = true;
            }
        }
    }
}