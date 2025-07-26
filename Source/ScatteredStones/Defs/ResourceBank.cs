using RimWorld;
using Verse;
// NOTE: This resource bank is designed for mod compatibility and safety.
// - All DefOfs are static and only used for mod-local references.
// - No static state is shared outside the mod.
// - All user-facing strings should be localized.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    public static class ResourceBank
    {
        [DefOf]
        public static class ThingDefOf
        {
            public static ThingDef Owl_Filth_Rocks;
            public static ThingDef ChunkGranite;
        }
        [DefOf]
        public static class ThingCategoryDefOf
        {
            public static ThingCategoryDef StoneChunks;
        }
    }
}