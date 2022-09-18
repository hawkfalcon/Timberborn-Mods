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
using System.IO;
using Timberborn.Buildings;
using Timberborn.NeedSpecifications;

namespace UnifiedFactions {

    [BepInPlugin("com.hawkfalcon.plugin.unifiedfactions", "Unified Factions", "1.2.0")]
    [HarmonyPatch]
    public class UnifiedFactionsPlugin : BaseUnityPlugin {

        public static readonly BuildingVariantTracker BuildingVariants = new();

        private static IEnumerable<Object> factionObjectCache = null;

        private static ConfigEntry<bool> enableAllFactionBuildings;
        private static ConfigEntry<bool> enableFactionLetters;

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
            List<GameObject> buildings = new();

            // Get an ordered list of factions, starting with Current
            List<FactionSpecification> factionSpecifications = new();
            factionSpecifications.Add(____factionService.Current);
            foreach (FactionSpecification factionSpecification in ____factionService._factionSpecificationService._factions)
            {
                if (factionSpecification.Id != ____factionService.Current.Id)
                {
                    factionSpecifications.Add(factionSpecification);
                }
            }
            Dictionary<string, Prefab> backwardCompatableCurrent = new();
            foreach (FactionSpecification factionSpecification in factionSpecifications)
            {
                Debug.Log("Unified Factions - Adding Faction: " + factionSpecification.DisplayName);
                foreach (string prefabPath in factionSpecification.CommonBuildings.Concat(factionSpecification.UniqueBuildings))
                {
                    // Just don't show them :|
                    if (prefabPath.StartsWith("Buildings/Power/Dev")) continue;
                    GameObject gameObject = ____resourceAssetLoader.Load<GameObject>(prefabPath);
                    if (gameObject == null)
                    {
                        Debug.Log("GameObject " + prefabPath + " is empty, skipping!");
                        continue;
                    }
                    Prefab prefab = gameObject.GetComponent<Prefab>();
                    if (prefab == null)
                    {
                        Debug.Log("Prefab " + prefabPath + " is empty, skipping!");
                        continue;
                    }
                    string buildingName = prefab.PrefabName;
                    // For any faction other than current, we have to clear out the old prefab name to prevent duplicates
                    if (____factionService.Current.Id == factionSpecification.Id)
                    {
                        // track all current backward compatable prefabs
                        foreach (string backwardCompatableName in prefab._backwardCompatiblePrefabNames)
                        {
                            backwardCompatableCurrent[backwardCompatableName] = prefab;
                        }
                    }
                    else {
                        // clear out all non-current backward compatable prefab
                        prefab._backwardCompatiblePrefabNames = new string[0];
                        // clear out all current backward compatable prefab if we have to
                        if (backwardCompatableCurrent.TryGetValue(buildingName, out Prefab originalPrefab))
                        {
                            originalPrefab._backwardCompatiblePrefabNames = new string[0];
                        }
                    }

                    // Prevent duplicate prefab names, as we can't load them
                    if (!BuildingVariants.TryAdd(buildingName))
                    {
                        Debug.Log(prefabPath + " contains duplicate prefab name, skipping: " + buildingName);
                        continue;
                    }
                    buildings.Add(gameObject);    
                }
            }
            __result = buildings;
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
                string buildingName = blockObjectTool.Prefab.name;
                string text = "";

                if (buildingName.Contains("Folktails")) {
                    text = "F";
                } else if (buildingName.Contains("IronTeeth")) {
                    text = "I";
                }

                // Mark unique faction buildings
                if (!BuildingVariants.HasVariant(buildingName) && text != "") {
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

        /**
         * What is a Faction Need?
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NeedSpecification), "AvailableForAllFactions", MethodType.Getter)]
        static bool AllFactionsAllNeeds(ref bool __result)
        {
            __result = true;
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
