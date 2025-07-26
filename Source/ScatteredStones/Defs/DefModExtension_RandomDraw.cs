using Verse;
// NOTE: This mod extension is designed for mod compatibility and safety.
// - All fields are public for Def injection.
// - No static state is shared outside the mod.
// - All user-facing strings should be localized.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    public class RandomDraw : DefModExtension
    {
        public float minSize = 1f;
        public float maxSize = 1f;
        public float minSizeModified = 1f;
        public float maxSizeModified = 1f;
        public float offsetRange = 0f;
        public float offsetRangeModified = 0f;
    }
}