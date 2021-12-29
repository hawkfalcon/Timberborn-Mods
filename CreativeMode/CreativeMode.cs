using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Bindito.Core;
using Timberborn.ScienceSystem;
using Timberborn.BuildingTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.MasterScene;
using Timberborn.BlockObjectTools;
using Timberborn.Localization;
using Timberborn.ToolSystem;
using Timberborn.Options;
using System.Collections.Generic;
using System.Linq;
using CreativeMode;
using MonoMod.Utils;
using UnityEngine.UIElements;
using TimberbornAPI;

namespace CreativeModePlugin {

    [BepInPlugin("com.hawkfalcon.plugin.creativemode", "Creative Mode", "1.4.0")]
    [HarmonyPatch]
    public class CreativeModePlugin : BaseUnityPlugin {

        static bool creativeModeEnabled = true;

        private static ConfigEntry<bool> disableScienceCost;
        private static ConfigEntry<bool> enableInstantBuilding;
        private static ConfigEntry<bool> enableMapEditorTools;

        public void OnEnable() {
            disableScienceCost = Config.Bind("General.Features",
               "DisableScienceCost", true, "Placing anything no longer requires science");
            enableInstantBuilding = Config.Bind("General.Features",
               "EnableInstantBuilding", true, "Anything that is placed is instantly built at no cost");
            enableMapEditorTools = Config.Bind("General.Features",
                "EnableMapEditorTools", true, "Unlocks many map editor tools while playing");

            TimberAPI.Localization.AddLabel("CreativeMode.ToolGroups.MapEditor", "Map editor tools");
            if (enableMapEditorTools.Value) {
                TimberAPI.DependencyRegistry.AddConfigurator(new MapEditorButtonsConfigurator());
            }
            new Harmony("com.hawkfalcon.plugin.creativemode").PatchAll();

            Logger.LogInfo("Plugin Creative Mode is loaded!");
        }

        /*
         * Disables Science Costs
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildingUnlockingService), "Unlocked")]
        static bool BuildingUnlocker(ref bool __result) {
            if (!(creativeModeEnabled && disableScienceCost.Value)) {
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
            if (creativeModeEnabled && enableInstantBuilding.Value) {
                Building component = prefab.GetComponent<Building>();
                component.PlaceFinished = true;
            }
            return true;
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
         * Show the Ruins tool
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockObjectTool), "DevModeTool", MethodType.Getter)]
        static bool ShowRuinsTool(ref bool __result) {
            __result = false;
            return false;
        }

        /**
         * The [...] tool appears, let's remove it
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ToolGroupButton), "ContainsTool")]
        static void HideOldMapEditorTool(ToolGroup ____toolGroup, ref bool __result) {
            if (____toolGroup.DisplayNameLocKey.Equals("ToolGroups.MapEditor")) {
                __result = false;
            }
        }

        /**
         * Add a toggle button to the settings
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsBox), "GetPanel")]
        static void AddCreativeModeToggle(ref VisualElement __result, OptionsBox __instance) {
            VisualElement root = __result.Children().First();
            IEnumerable<string> buttonStyle = root.Children().First().GetClasses();

            string text = "Toggle Creative Mode " + (creativeModeEnabled ? "Off" : "On");
            Button button = CreateButton(text, buttonStyle);
            OptionsBox optionsBox = __instance;
            button.clicked += () => ToggleCreativeMode(__instance);
            root.Insert(4, button);
        }

        /**
         * Create a menu button
         */
        private static Button CreateButton(string name, IEnumerable<string> styles) {
            Button button = new();
            button.text = name;
            foreach (string style in styles) {
                button.AddToClassList(style);
            }
            return button;
        }

        public static void ToggleCreativeMode(OptionsBox optionsBox) {
            creativeModeEnabled = !creativeModeEnabled;
            optionsBox.ResumeClicked();
        }
    }
}
