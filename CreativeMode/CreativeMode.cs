using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Bindito.Core;
using Timberborn.ScienceSystem;
using Timberborn.BuildingTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.FactionSystemGame;
using Timberborn.FactionSystem;
using Timberborn.AssetSystem;
using Timberborn.MasterScene;
using Timberborn.BlockObjectTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CreativeMode;

namespace CreativeModePlugin {

    [BepInPlugin("com.hawkfalcon.plugin.creativemode", "Creative Mode", "1.2.0.0")]
    [HarmonyPatch]
    public class CreativeModePlugin : BaseUnityPlugin {
        private static ConfigEntry<bool> disableScienceCost;
        private static ConfigEntry<bool> enableInstantBuilding; 
        private static ConfigEntry<bool> enableAllFactionBuildings;
        private static ConfigEntry<bool> enableMapEditorTools;

        public void OnEnable() {
            disableScienceCost = Config.Bind("General.Features",
               "DisableScienceCost", true, "Placing anything no longer requires science");
            enableInstantBuilding = Config.Bind("General.Features",
               "EnableInstantBuilding", true, "Anything that is placed is instantly built at no cost");
            enableAllFactionBuildings = Config.Bind("General.Features",
               "EnableAllFactionBuildings", true, "Unlocks access to ALL faction buildings");
            enableMapEditorTools = Config.Bind("General.Features",
                "EnableMapEditorTools", true, "Unlocks many map editor tools while playing");

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
            if (!disableScienceCost.Value) {
                return true;
            }
            __result = true;
            return false;
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

        /*
         * Unlock all faction uniques
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FactionObjectCollection), "GetObjects")]
        static bool UnlockFactionUniques(FactionService ____factionService, IResourceAssetLoader ____resourceAssetLoader, ref IEnumerable<Object> __result) {
            if (!enableAllFactionBuildings.Value) {
                return true;
            }
            List<FactionSpecification> factions = Traverse.Create(____factionService)
                .Field("_factionSpecificationService").Field("_factions").GetValue() as List<FactionSpecification>;

            List<string> buildings = new();
            foreach (FactionSpecification factionSpecification in factions) {
                buildings.AddRange(factionSpecification.UniqueBuildings);
            }
            buildings.AddRange(____factionService.Current.CommonBuildings);
            __result = (from path in buildings select ____resourceAssetLoader.Load<GameObject>(path));
            return false;
        }

        /**
         * If everything is unique, nothing is unique
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UniqueBuildingCollection), "IsUnique")]
        static bool UnlockAllUniques(ref bool __result) {
            if (!enableAllFactionBuildings.Value) {
                return true;
            }
            __result = false;
            return false;
        }

        /**
         * Unlock the Delete everything tool (from Map Editor)
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AnyBlockObjectDeletionTool), "DevModeTool", MethodType.Getter)]
        static bool ShowDeleteTool(ref bool __result) {
            __result = false;
            return false;
        }

        /**
         * Inject our own buttons which are present in Map Editor
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MasterSceneConfigurator), "Configure")]
        static void InjectMapEditorButtons(IContainerDefinition containerDefinition) {
            if (enableMapEditorTools.Value) {
                containerDefinition.Bind<MapEditorGroupedButtons>().AsSingleton();
                containerDefinition.Install((IConfigurator)(object)new MapEditorButtonsConfigurator());
            }
        }
    }
}
