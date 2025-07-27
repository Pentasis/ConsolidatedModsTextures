using HarmonyLib;
using Verse;
using static ConsolidatedModsTextures.ScatteredStones.Utility;

namespace ConsolidatedModsTextures.ScatteredStones
{
	public class ScatteredStones : Mod
	{
		public ScatteredStones(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			LongEventHandler.QueueLongEvent(() => Setup(), null, false, null);
		}

	}

}
