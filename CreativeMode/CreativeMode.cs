using BepInEx;
using HarmonyLib;
using Timberborn.ScienceSystem;
using Timberborn.BuildingTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;

namespace CreativeModePlugin {
    [BepInPlugin("com.hawkfalcon.plugin.creativemode", "Creative Mode", "1.0.0.0")]
    public class CreativeModePlugin : BaseUnityPlugin {
        public void OnEnable() {
            var harmony = new Harmony("com.hawkfalcon.plugin.creativemode");
            harmony.PatchAll();
            Logger.LogInfo("Plugin Creative Mode is loaded!");
        }
    }

    [HarmonyPatch(typeof(BuildingUnlockingService), "Unlocked")]
    class PatchScience {
        static bool Prefix(ref bool __result) {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(BuildingPlacer), "Place")]
    class PatchPlace {
        static bool Prefix(BlockObject prefab) {
            Building component = prefab.GetComponent<Building>();
            component.PlaceFinished = true;
            return true;
        }
    }
}
