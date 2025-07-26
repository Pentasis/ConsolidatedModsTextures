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
    public class Rocks : Filth
    {
        /// <summary>
        /// The color used for drawing this filth.
        /// </summary>
        private Color color;

        /// <summary>
        /// Gets or sets the draw color for this filth.
        /// </summary>
        public override Color DrawColor
        {
            get
            {
                if (this.color.a == 0f) this.color = this.MatchColor(null);
                return this.color;
            }
            set { this.color = value; }
        }

        /// <summary>
        /// Exposes the color data for save/load.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Color>(ref this.color, "color", ChunkGranite.graphicData.color, false);
        }

        /// <summary>
        /// Matches the color of a given thing, or finds a nearby chunk to match if null.
        /// </summary>
        /// <param name="matchToThis">The thing to match color to, or null.</param>
        /// <returns>The matched color.</returns>
        public Color MatchColor(Thing matchToThis)
        {
            if (matchToThis != null)
            {
                return matchToThis.DrawColor;
            }
            else if (this.positionInt != IntVec3.Invalid)
            {
                var list = this.Map.thingGrid.ThingsListAtFast(this.Position);
                var length = list.Count;
                for (int i = 0; i < length; i++)
                {
                    var item = list[i];
                    if (stoneChunks.Contains(item.def.index))
                    {
                        return item.DrawColor;
                    }
                }
            }
            // Default
            return ChunkGranite.graphicData.color;
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