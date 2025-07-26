using Verse;
using UnityEngine;
using RimWorld;
using System.Collections.Generic;
// NOTE: This custom graphic class is designed for mod compatibility and safety.
// - All Thing and Def accesses are null-checked where possible.
// - No static state is shared outside the mod.
// - All user-facing strings should be localized.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    public class RandomSpreadGraphic : Graphic_Random
    {
        public readonly Dictionary<int, float[]> SessionCache = new Dictionary<int, float[]>();

        public override void Print(SectionLayer layer, Thing thing, float extraRotation)
        {
            float sizeMultiplier = 1, randomRotation = 0, offsetX = 0, offsetY = 0;

            if (thing?.def == null) return;
            if (!SessionCache.ContainsKey(thing.thingIDNumber))
            {
                if (thing.def.HasModExtension<RandomDrawExtension>())
                {
                    int seed = thing.Position.GetHashCode();
                    RandomDrawExtension randomDraw = thing.def.GetModExtension<RandomDrawExtension>();
                    if (randomDraw != null)
                    {
                        randomRotation = Rand.RangeInclusiveSeeded(0, 360, seed);
                        sizeMultiplier = Rand.RangeSeeded(randomDraw.MinSizeModified, randomDraw.MaxSizeModified, seed);
                        offsetX = Rand.RangeSeeded(0 - randomDraw.OffsetRangeModified, 0 + randomDraw.OffsetRangeModified, seed);
                        offsetY = Rand.RangeSeeded(0 - randomDraw.OffsetRangeModified, 0 + randomDraw.OffsetRangeModified, seed);
                    }
                }
                SessionCache.Add(thing.thingIDNumber, new float[] { sizeMultiplier, randomRotation, offsetX, offsetY });
            }

            //Check if on a storage building
            if (thing.Map?.thingGrid?.ThingAt<Building_Storage>(thing.Position) != null)
            {
                SessionCache[thing.thingIDNumber][2] = SessionCache[thing.thingIDNumber][3] = 0f;
            }

            Vector3 center = thing.positionInt.ToVector3ShiftedWithAltitude(thing.def.altitudeLayer) + DrawOffset(thing.rotationInt) + new Vector3(SessionCache[thing.thingIDNumber][2], 0, SessionCache[thing.thingIDNumber][3]);
            Material mat = MatAt(thing.rotationInt, thing);
            Graphic.TryGetTextureAtlasReplacementInfo(mat, TextureAtlasGroup.Item, false, true, out mat, out Vector2[] uvs, out Color32 color);
            Printer_Plane.PrintPlane(layer, center, drawSize * SessionCache[thing.thingIDNumber][0], mat, SessionCache[thing.thingIDNumber][1], false, uvs, new Color32[]
            {
                color,
                color,
                color,
                color
            }, 0.01f, 0f);

            ShadowGraphic?.Print(layer, thing, 0f);
        }
    }
}