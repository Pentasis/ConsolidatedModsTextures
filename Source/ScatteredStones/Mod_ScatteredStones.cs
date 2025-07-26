using HarmonyLib;
using Verse;
using UnityEngine;
using System;
using static ConsolidatedMods.Textures.ScatteredStonesUtility;

namespace ConsolidatedMods.Textures
{
	public class Mod_ScatteredStones : Mod
	{
		public Mod_ScatteredStones(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			LongEventHandler.QueueLongEvent(() => Setup(), null, false, null);
		}

	}

}
