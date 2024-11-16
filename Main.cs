using HarmonyLib;
using Pathea.ActorNs;
using Pathea.MapNs;
using Pathea.TreasureRevealerNs;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace YentisMod
{
    public class Main
    {
        public static Settings settings;
        public static UnityModManager.ModEntry mod;

        public static bool enabled;
        public static string relicSensorFilter;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            mod = modEntry;

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool newEnabled)
        {
            enabled = newEnabled;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
            GUILayout.Label("Relic sensor filter");
            relicSensorFilter = GUILayout.TextField(relicSensorFilter, GUILayout.Width(100f));
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        [HarmonyPatch(typeof(Actor_IMap), "Pathea.MapNs.IMap.GetHoverInfo")]
        class Actor_IMap_GetHoverInfo_Patch
        {
            static void Postfix(Actor_IMap __instance, ref string __result)
            {
                if (!enabled || !settings.showNpcNames) return;
                if (__result != null) return;

                try
                {
                    var mActor = Traverse.Create(__instance).Field("mActor").GetValue() as Actor;
                    __result = mActor.ActorName;
                } catch (Exception e)
                {
                    mod.Logger.Error(e.ToString());
                }
            }
        }

        [HarmonyPatch(typeof(TreasureRevealerItem), "Init")]
        class TreasureRevealerItem_Init_Patch
        {
            static void Postfix(TreasureRevealerItem __instance)
            {
                if (!enabled || string.IsNullOrEmpty(relicSensorFilter)) return;

                var mItemLines = Traverse.Create(__instance).Field("mItemLines").GetValue() as TreasureRevealerItemLines;
                if (mItemLines.itemName.ToLower() != relicSensorFilter.ToLower()) mItemLines.itemName = "";
            }
        }
    }
}
