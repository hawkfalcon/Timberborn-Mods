using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Timberborn.FactionSystemGame;
using Timberborn.FactionSystem;
using Timberborn.AssetSystem;
using Timberborn.BlockObjectTools;
using Timberborn.ToolSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Timberborn.CoreUI;
using Timberborn.PrefabOptimization;
using Timberborn.PlantingUI;
using Timberborn.Planting;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using TimberbornAPI;
using UnifiedFactions;
using Timberborn.ToolPanelSystem;
using System.Collections.Immutable;

namespace UnifiedFactions {

    [BepInPlugin("com.hawkfalcon.plugin.unifiedfactions", "Unified Factions", "1.2.0")]
    [HarmonyPatch]
    public class UnifiedFactionsPlugin : BaseUnityPlugin {

        public static readonly List<string> DuplicateBuildings = new();
        private static IEnumerable<Object> factionObjectCache = null;

        private static ConfigEntry<bool> enableAllFactionBuildings;
        private static ConfigEntry<bool> enableFactionLetters;

        private static ICollection<string> _notCommon = new[] {
            "LogPile.Folktails", "WaterPump.Folktails", "MechanicalWaterPump.Folktails", "WaterWheel.Folktails", "Mine.Folktails"
        };

        public void OnEnable() {
            enableAllFactionBuildings = Config.Bind("General.Features",
               "EnableAllFactionBuildings", true, "Unlocks access to ALL faction buildings");
            enableFactionLetters = Config.Bind("General.Features",
               "EnableFactionLetters", true, "Show which faction each building belongs to in buttons");
           
            var harmony = new Harmony("com.hawkfalcon.plugin.unifiedfactions");
            harmony.PatchAll();

            TimberAPI.DependencyRegistry.AddConfigurator(new UnifiedFactionsConfigurator());
            Logger.LogInfo("Plugin Unified Factions is loaded!");
        }

        /*
         * Unlock all faction buildings
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FactionObjectCollection), "GetObjects")]
        static bool UnlockFactionUniques(FactionService ____factionService, IResourceAssetLoader ____resourceAssetLoader, ref IEnumerable<Object> __result) {
            if (!enableAllFactionBuildings.Value) {
                return true;
            }

            // The game doesn't cache these, we will
            if (factionObjectCache != null) {
                __result = factionObjectCache;
                return false;
            }

            List<string> buildings = new();
            foreach (FactionSpecification factionSpecification in ____factionService._factionSpecificationService._factions) {
                buildings.AddRange(factionSpecification.UniqueBuildings);
                buildings.AddRange(factionSpecification.CommonBuildings);

                // Keep track of all duplicate names
                foreach (string building in factionSpecification.CommonBuildings) {
                    string duplicateName = building.Substring(building.LastIndexOf('/') + 1);
                    // This is a lie, it is not Common
                    if (!_notCommon.Contains(duplicateName)) {
                        DuplicateBuildings.Add(duplicateName);
                    }
                }
            }
            __result = (from path in buildings select ____resourceAssetLoader.Load<GameObject>(path));
            factionObjectCache = __result;
            return false;
        }

        /**
         * Add faction letters to buttons
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ToolButton), "PostLoad")]
        static void AddFactionName(ToolButton __instance) {
            if (!enableFactionLetters.Value) {
                return;
            }
            if (__instance.Tool is BlockObjectTool blockObjectTool) {
                string name = blockObjectTool.Prefab.name;
                string text = "";

                if (name.Contains("Folktails")) {
                    text = "F";
                } else if (name.Contains("IronTeeth")) {
                    text = "I";
                }

                // Mark unique faction buildings
                if (!DuplicateBuildings.Contains(name) && text != "") {
                    text += "*";
                }

                Label label = new() {
                    text = text,
                    style = {
                        position = Position.Absolute,
                        top = 2,
                        left = 1,
                        width = 20,
                        backgroundColor = new Color(0f, 0f, 0f, 0f),
                        color = new Color(0.75f, 0.65f, 0.44f),
                        fontSize = 10
                    }
                };
                __instance.Root.Add(label);
            }
        }

        /**
         * Keep track of clicked tool group button, refresh to current state
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ToolGroupButton), "OnToolGroupEntered")]
        static void CurrentButtonGroup(ToolGroupButton __instance, ToolGroupEnteredEvent toolGroupEnteredEvent, ToolGroup ____toolGroup)
        {
            if (toolGroupEnteredEvent.ToolGroup == ____toolGroup)
            {
                ToolButtonModifier.CurrentToolGroupButton = __instance;
                ToolButtonModifier.RefreshSection(null);
            }
        }

        /**
         * Reorder ToolPanel to insert FactionTogglePanel earlier.
         * I want to add this functionality to TimberAPI at some point
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ToolPanel), "AddFragments")]
        static bool ReorderToolPanel(VisualElement root, ImmutableArray<ToolPanelModule> ____toolPanelModules)
        {
            Dictionary<float, IToolFragment> dictionary = new();
            ImmutableArray<ToolPanelModule>.Enumerator enumerator = ____toolPanelModules.GetEnumerator();
            while (enumerator.MoveNext())
            {
                foreach (KeyValuePair<int, IToolFragment> toolPanelFragment in enumerator.Current.ToolPanelFragments)
                {
                    float key = toolPanelFragment.Key;
                    if (toolPanelFragment.Value is FactionTogglePanel)
                    {
                        key = 2.5f;
                    }
                    dictionary.Add(key, toolPanelFragment.Value);
                }
            }
            foreach (float item in dictionary.Keys.OrderByDescending((float key) => key))
            {
                IToolFragment toolFragment = dictionary[item];
                root.Add(toolFragment.InitializeFragment());
            }
            return false;
        }

        // ALL THE LITTLE FIXES
        /**
         * Yes, I know I'm loading more than one atlas!
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AutoAtlaser), "TooManyAtlases")]
        static bool HideWarning(ref bool __result)
        {
            __result = false;
            return false;
        }

        /**
         * Fix a weird farm bug - Single->First (hopefully I can remove this in the future)
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlantingToolButtonFactory), "GetPlanterBuildingName")]
        static bool FarmFix(ref string __result, Plantable plantable, ObjectCollectionService ____objectCollectionService, ILoc ____loc) {
            string displayNameLocKey = ____objectCollectionService.GetAllMonoBehaviours<PlanterBuilding>().First((PlanterBuilding building) =>
                building.PlantableResourceGroup.Contains(plantable.ResourceGroup)).GetComponent<LabeledPrefab>().DisplayNameLocKey;
            __result = ____loc.T(displayNameLocKey);
            return false;
        }

        /**
         * Remove the 'Unique to this faction' text
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
    }
}
