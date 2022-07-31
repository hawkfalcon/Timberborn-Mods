using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Timberborn.ScienceSystem;
using Timberborn.BlockObjectTools;
using Timberborn.ToolSystem;
using Timberborn.Options;
using UnityEngine.UIElements;
using TimberbornAPI;

namespace CreativeMode {

    [BepInPlugin("com.hawkfalcon.plugin.creativemode", "Creative Mode", "1.5.0")]
    [HarmonyPatch]
    public class CreativeModePlugin : BaseUnityPlugin {

        public static bool Enabled = true;

        public static ConfigEntry<bool> EnableInstantBuilding;
        private static ConfigEntry<bool> disableScienceCost;
        private static ConfigEntry<bool> enableMapEditorTools;

        public void OnEnable() {
            EnableInstantBuilding = Config.Bind("General.Features",
               "EnableInstantBuilding", true, "Anything that is placed is instantly built at no cost");
            disableScienceCost = Config.Bind("General.Features",
               "DisableScienceCost", true, "Placing anything no longer requires science");
            enableMapEditorTools = Config.Bind("General.Features",
                "EnableMapEditorTools", true, "Unlocks many map editor tools while playing");

            TimberAPI.Localization.AddLabel("CreativeMode.ToolGroups.MapEditor", "Map editor tools");
            if (enableMapEditorTools.Value) {
                TimberAPI.DependencyRegistry.AddConfigurator(new CreativeModeConfigurator());
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
            if (!(Enabled && disableScienceCost.Value)) {
                return true;
            }
            __result = true;
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
            VisualElement root = __result.Query("OptionsBox");
            Button button = new() { classList = { "menu-button" } };

            button.text = "Toggle Creative Mode " + (Enabled ? "Off" : "On");
            button.clicked += () => ToggleCreativeMode(__instance);
            root.Insert(4, button);
        }

        public static void ToggleCreativeMode(OptionsBox optionsBox) {
            Enabled = !Enabled;
            optionsBox.ResumeClicked();
        }
    }
}
