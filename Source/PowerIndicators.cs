using RimWorld;
using Verse;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

namespace ConsolidatedMods.Textures
{
    [StaticConstructorOnStartup]
    internal class PowerIndicators
    {
        private static readonly Texture2D powerIcon = ContentFinder<Texture2D>.Get(itemPath: "UI/Overlays/NeedsPower");

        private static readonly Dictionary<string, Texture2D> fuelIcon = new Dictionary<string, Texture2D>();

        static PowerIndicators()
        {
            Harmony harmony = new Harmony(id: "consolidatedmods.textures.powerindicators");
            harmony.Patch(original: AccessTools.Method(type: AccessTools.Method(type: typeof(Designator_Build), name: nameof(Designator_Build.GizmoOnGUI)).DeclaringType, name: nameof(Designator_Build.GizmoOnGUI)),
                                                        postfix: new HarmonyMethod(methodType: typeof(PowerIndicators), methodName: nameof(DrawIndicator)));
            harmony.Patch(original: AccessTools.Method(type: AccessTools.Method(type: typeof(Designator_Dropdown), name: nameof(Designator_Dropdown.GizmoOnGUI)).DeclaringType, name: nameof(Designator_Dropdown.GizmoOnGUI)),
                          postfix: new HarmonyMethod(methodType: typeof(PowerIndicators), methodName: nameof(DrawIndicator)));
        }

        public static void DrawIndicator(Command __instance, Vector2 topLeft)
        {

            Command activeDesignator = __instance;

            if (__instance is Designator_Dropdown dropdownDesignator)
                activeDesignator = Traverse.Create(dropdownDesignator).Field<Designator>("activeDesignator").Value;

            if (!(activeDesignator is Designator_Build buildDesignator) || !(buildDesignator.PlacingDef is ThingDef currentThingDef)) return;


            if (currentThingDef.ConnectToPower)
            {
                GUI.DrawTexture(new Rect(x: topLeft.x + __instance.GetWidth(maxWidth: float.MaxValue) - 32f / 3 * 2 / 4 * 3, y: topLeft.y, width: 32f / 3 * 2, height: 32f / 3 * 2), image: power, ScaleMode.ScaleToFit);

            }
            else if (currentThingDef.GetCompProperties<CompProperties_Refuelable>() is CompProperties_Refuelable refuelProps && refuelProps.fuelFilter.AllowedThingDefs.Any())
            {
                ThingDef def = refuelProps.fuelFilter.AllowedThingDefs.First();
                Graphic g = def.graphic;
                string path = g is Graphic_Collection gc ? Traverse.Create(root: gc).Field(name: "subGraphics").GetValue<Graphic[]>().Last().path : g.path;
                if (path.NullOrEmpty())
                    path = def.graphicData != null ? def.graphicData.texPath : Traverse.Create(root: ThingDefOf.Fire.graphic).Field(name: "subGraphics").GetValue<Graphic[]>().RandomElement().path;
                if (!fuel.ContainsKey(key: path))
                    fuel.Add(key: path, value: ContentFinder<Texture2D>.Get(itemPath: path));

                GUI.DrawTexture(new Rect(x: topLeft.x + __instance.GetWidth(maxWidth: float.MaxValue) - 32f / 4 * 3, y: topLeft.y, width: 32f / 3 * 2, height: 32f / 3 * 2), image: fuel[path], ScaleMode.ScaleToFit);
            }
        }
    }
}