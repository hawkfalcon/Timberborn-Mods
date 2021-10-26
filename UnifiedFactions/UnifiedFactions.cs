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

namespace UnifiedFactionsPlugin {

    [BepInPlugin("com.hawkfalcon.plugin.unifiedfactions", "Unified Factions", "1.0.0")]
    [HarmonyPatch]
    public class UnifiedFactionsPlugin : BaseUnityPlugin {

        static ToolGroupButton currentToolGroupButton = null;
        static BlockObjectTool currentTool = null;

        static IEnumerable<Object> factionObjectCache = null;
        static readonly List<string> duplicateBuildings = new();

        // TODO: when they add more factions, make this an enum
        public static bool showFolktails = true;

        private static ConfigEntry<bool> enableAllFactionBuildings;
        private static ConfigEntry<bool> enableFactionLetters;

        public void OnEnable() {
            enableAllFactionBuildings = Config.Bind("General.Features",
               "EnableAllFactionBuildings", true, "Unlocks access to ALL faction buildings");
            enableFactionLetters = Config.Bind("General.Features",
               "EnableFactionLetters", true, "Show which faction each building belongs to in buttons");
           
            var harmony = new Harmony("com.hawkfalcon.plugin.unifiedfactions");
            harmony.PatchAll();
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
            foreach (FactionSpecification factionSpecification in GetFactions(____factionService)) {
                buildings.AddRange(factionSpecification.UniqueBuildings);
                buildings.AddRange(factionSpecification.CommonBuildings);

                // Keep track of all duplicate names
                foreach (string building in factionSpecification.CommonBuildings) {
                    string duplicateName = building.Substring(building.LastIndexOf('/') + 1);
                    // This is a lie, it is not Common
                    if (!duplicateName.Equals("LogPile.Folktails")) {
                        duplicateBuildings.Add(duplicateName);
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
                if (!duplicateBuildings.Contains(name) && text != "") {
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
         * Keep track of clicked tool group, refresh to current state
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ToolGroupButton), "OnToolGroupEntered")]
        static void CurrentButtonGroup(ToolGroupButton __instance, ToolGroupEnteredEvent toolGroupEnteredEvent, ToolGroup ____toolGroup) {
            if (toolGroupEnteredEvent.ToolGroup == ____toolGroup) {
                currentToolGroupButton = __instance;
                refreshSection(null);
            }
        }

        /**
         * Keep track of clicked tool, only show Toggle Button for duplicate buildings
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockObjectRotationPanel), "OnToolEntered")]
        static void CurrentButton(ToolEnteredEvent toolEnteredEvent, VisualElement ____root) {
            currentTool = toolEnteredEvent.Tool as BlockObjectTool;

            if (currentTool != null) {
                bool isDuplicate = duplicateBuildings.Contains(currentTool.Prefab.name);
                ____root.Children().Skip(2).First().ToggleDisplayStyle(isDuplicate);
            }
        }

        /**
         * Yes, I know I'm loading more than one atlas!
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AutoAtlaser), "TooManyAtlases")]
        static bool HideWarning(ref bool __result) {
            __result = false;
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

        /**
         * Add a Toggle Faction button to the top of the panel
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockObjectRotationPanel), "InitializeFragment")]
        static void AddToggleButton(ref VisualElement __result) {
            if (!enableAllFactionBuildings.Value) {
                return;
            }

            NineSliceButton button = new() {
                text = "Toggle Faction",
                classList = {
                    "unity-text-element",
                    "unity-button",
                    "button-game",
                    "block-object-rotation-panel__button"
                },
                style = {
                    color = new Color(0.75f, 0.65f, 0.44f)
                }
            };

            button.clicked += ToggleFaction;
            __result.Add(button);
        }

        static void ToggleFaction() {
            showFolktails = !showFolktails;
            if (currentToolGroupButton != null && currentTool != null) {
                refreshSection(currentTool.Prefab.name);
            }
        }

        /**
         * Hide and show duplicate buttons based on currently selected faction,
         * and automatically select the other tool
         */
        private static void refreshSection(string selectedToolName) {
            foreach (ToolButton button in currentToolGroupButton._toolButtons) {
                if (button.Tool is BlockObjectTool otherTool) {
                    string otherToolName = otherTool.Prefab.name;
                    if (selectedToolName != null) {
                        selectOtherTool(button, otherToolName, selectedToolName);
                    }
                    showOneDuplicate(otherToolName, button);
                }
            }
        }

        /**
         * Automatically select the tool of the other faction
         * [tool names are Building.FactionName]
         */
        private static void selectOtherTool(ToolButton button, string otherToolName, string selectedToolName) {
            string otherToolId = otherToolName.Split(".")[0];
            string selectedToolId = selectedToolName.Split(".")[0];
            if (otherToolId.Equals(selectedToolId) && !otherToolName.Equals(selectedToolName)) {
                button._toolManager.SwitchTool(button.Tool);
            }
        }

        /**
         * Hide the other duplicate button
         */
        private static bool showOneDuplicate(string name, ToolButton button) {
            if (duplicateBuildings.Contains(name) && button.ToolEnabled) {
                string factionId = showFolktails ? "Folktails" : "IronTeeth";
                bool visibility = name.Contains(factionId);
                button.Root.ToggleDisplayStyle(visibility);
                return true;
            }
            return false;
        }

        /**
         * Get Factions
         */
        private static List<FactionSpecification> GetFactions(FactionService factionService) {
            return Traverse.Create(factionService).Field("_factionSpecificationService").
                Field("_factions").GetValue() as List<FactionSpecification>;

        }
    }
}
