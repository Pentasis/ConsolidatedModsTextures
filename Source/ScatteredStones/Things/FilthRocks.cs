using RimWorld;
using UnityEngine;
using Verse;
using System.Diagnostics;
using static ScatteredStones.ScatteredStonesUtility;
using static ScatteredStones.ResourceBank.ThingDefOf;
// NOTE: This custom filth class is designed for mod compatibility and safety.
// - All Thing and Def accesses are null-checked where possible.
// - No static state is shared outside the mod.
// - All user-facing strings should be localized.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    /// <summary>
    /// Filth subclass for rocks, allowing dynamic color and custom behavior.
    /// </summary>
    public class FilthRocks : Filth
    {
        /// <summary>
        /// The color used for drawing this filth.
        /// </summary>
        private Color _drawColor;

        /// <summary>
        /// Gets or sets the draw color for this filth.
        /// </summary>
        public override Color DrawColor
        {
            get
            {
                if (_drawColor.a == 0f) _drawColor = this.MatchColor(null);
                return _drawColor;
            }
            set { _drawColor = value; }
        }

        /// <summary>
        /// Exposes the color data for save/load.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _drawColor, "color", ResourceBank.ThingDefOf.ChunkGranite.graphicData.color, false);
        }

        /// <summary>
        /// Matches the color of a given thing, or finds a nearby chunk to match if null.
        /// </summary>
        /// <param name="matchToThis">The thing to match color to, or null.</param>
        /// <returns>The matched color.</returns>
        public Color MatchColor(Thing thingToMatch)
        {
            if (thingToMatch != null)
            {
                return thingToMatch.DrawColor;
            }
            else if (this.positionInt != IntVec3.Invalid)
            {
                var thingsAtCell = this.Map.thingGrid.ThingsListAtFast(this.Position);
                int count = thingsAtCell.Count;
                for (int i = 0; i < count; i++)
                {
                    var thing = thingsAtCell[i];
                    if (ScatteredStonesUtility.StoneChunksSet.Contains(thing.def.index))
                    {
                        return thing.DrawColor;
                    }
                }
            }
            // Default
            return ResourceBank.ThingDefOf.ChunkGranite.graphicData.color;
        }

        /// <summary>
        /// Destroys the filth. (Custom logic for neverDespawn removed as settings are gone.)
        /// </summary>
        /// <param name="mode">The destroy mode.</param>
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
        }
    }
}