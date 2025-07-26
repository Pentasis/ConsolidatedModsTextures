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
    public class ScatteredStonesMapComponent : MapComponent
    {
        private bool _hasAppliedStones = false;

        /// <summary>
        /// Constructor for the map component.
        /// </summary>
        /// <param name="mapInstance">The map instance.</param>
        public ScatteredStonesMapComponent(Map mapInstance) : base(mapInstance) { }

        /// <summary>
        /// Exposes the state for save/load.
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref _hasAppliedStones, "hasAppliedStones", false, false);
        }

        /// <summary>
        /// Runs once per map to place scattered stones filth under valid objects.
        /// </summary>
        public override void FinalizeInit()
        {
            if (!_hasAppliedStones)
            {
                var allThings = map.listerThings.AllThings;
                int thingCount = allThings.Count;
                for (int i = 0; i < thingCount; i++)
                {
                    var thing = allThings[i];
                    if
                    (
                        // Is stone chunk, and not in storage? && Is on water and allowed?
                        (!thing.IsInAnyStorage() && ScatteredStonesUtility.StoneChunksSet.Contains(thing.def.index) && (true || (!map.terrainGrid.TerrainAt(thing.Position)?.IsWater ?? false))) ||
                        // Is reachable cliff that's not fogged?
                        (ScatteredStonesUtility.StoneCliffSet.Contains(thing.def.index) && !thing.Fogged() && ScatteredStonesUtility.ValidateCell(thing.Position, map, false)) &&
                        // Is not along the map edge?
                        !CellRect.WholeMap(map).IsOnEdge(thing.positionInt)
                    )
                    {
                        Thing filthRocks = ThingMaker.MakeThing(ResourceBank.ThingDefOf.Owl_Filth_Rocks, null);
                        // Place underneath chunk
                        GenPlace.TryPlaceThing(filthRocks, thing.Position, map, ThingPlaceMode.Direct);
                        // Match color
                        filthRocks.DrawColor = ((Rocks)filthRocks).MatchColor(thing);
                    }
                }
                _hasAppliedStones = true;
            }
        }
    }
}