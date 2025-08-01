using RimWorld;
using Verse;
using static ConsolidatedModsTextures.ScatteredStones.ResourceBank.ThingDefOf;
using static ConsolidatedModsTextures.ScatteredStones.Utility;

namespace ConsolidatedModsTextures.ScatteredStones
{
	public class ConsolidatedModsTextures : MapComponent
	{
        bool applied = false;
        public ConsolidatedModsTextures(Map map) : base(map) {}

        public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.applied, "applied", false, false);
		}

        public override void FinalizeInit()
        {
            //This should only ever run once per map.
            if (!applied)
            {
                var list = map.listerThings.AllThings;
                var length = list.Count;
                for (int i = 0; i < length; i++)
                {
                    var thing = list[i];
                    if
                    (
                        //Is stone chunk, and not in storage? && Is on water and allowed?
                        (!thing.IsInAnyStorage() && stoneChunks.Contains(thing.def.index) && (allowOnWater || (!map.terrainGrid.TerrainAt(thing.Position)?.IsWater ?? false))) ||
                        //Is reachable cliff that's not fogged?
                        (stoneCliff.Contains(thing.def.index) && !thing.Fogged() && ValidateCell(thing.Position, map, false)) && 
                        //Is not along the map edge?
                        !CellRect.WholeMap(map).IsOnEdge(thing.positionInt)
                    )
                    {
                        Thing rocks = ThingMaker.MakeThing(Owl_Filth_Rocks, null);
                        //Place underneath chunk
                        GenPlace.TryPlaceThing(rocks, thing.Position, map, ThingPlaceMode.Direct);
                        //Match color
                        rocks.DrawColor = ((RockFilth)rocks).MatchColor(thing);
                    }
                }
                applied = true;
            }
        }
    }
}