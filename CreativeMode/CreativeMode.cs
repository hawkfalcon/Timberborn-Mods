using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Timberborn.ScienceSystem;
using Timberborn.BuildingTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;

namespace CreativeModePlugin {

    [BepInPlugin("com.hawkfalcon.plugin.creativemode", "Creative Mode", "1.0.0.0")]
    [HarmonyPatch]
    public class CreativeModePlugin : BaseUnityPlugin {
        private static ConfigEntry<bool> disableScienceCost;
        private static ConfigEntry<bool> enableInstantBuilding;

        public void OnEnable()
        {
            disableScienceCost = Config.Bind("General.Features",
               "DisableScienceCost", true, "Placing anything no longer requires science");
            enableInstantBuilding = Config.Bind("General.Features",
               "EnableInstantBuilding", true, "Anything that is placed is instantly built at no cost");

            var harmony = new Harmony("com.hawkfalcon.plugin.creativemode");
            harmony.PatchAll();
            Logger.LogInfo("Plugin Creative Mode is loaded!");
        }

        /*
         * Disables Science Costs
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildingUnlockingService), "Unlocked")]
        static bool BuildingUnlocker(ref bool __result) {
            if (disableScienceCost.Value) {
                 __result = true;
            }
            return !disableScienceCost.Value;
        }

        /*
         * Enables instant building
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildingPlacer), "Place")]
        static bool PlaceInstantly(BlockObject prefab) {
            if (enableInstantBuilding.Value) {
                Building component = prefab.GetComponent<Building>();
                component.PlaceFinished = true;
            }
            return true;
        }
    }
}
