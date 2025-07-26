using Verse;
// NOTE: This mod extension is designed for mod compatibility and safety.
// - All fields are public for Def injection.
// - No static state is shared outside the mod.
// - All user-facing strings should be localized.

namespace ConsolidatedMods.Textures.ScatteredStones
{
    public class RandomDrawExtension : DefModExtension
    {
        public float MinSize = 1f;
        public float MaxSize = 1f;
        public float MinSizeModified = 1f;
        public float MaxSizeModified = 1f;
        public float OffsetRange = 0f;
        public float OffsetRangeModified = 0f;
    }
}