using HarmonyLib;
using Timberborn.ScienceSystem;
using Timberborn.BlockObjectTools;
using Timberborn.ToolSystem;
using Timberborn.Options;
using UnityEngine.UIElements;
using TimberApi.ModSystem;
using TimberApi.ConsoleSystem;
using Timberborn.BuildingTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;

namespace CreativeMode {

    [HarmonyPatch]
    public class CreativeModePlugin : IModEntrypoint {

        public static bool Enabled = true;

        private static CreativeModeConfig config;

        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            config = mod.Configs.Get<CreativeModeConfig>();
            new Harmony("com.hawkfalcon.plugin.creativemode").PatchAll();
        }

        /*
         * Disables Science Costs
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildingUnlockingService), "Unlocked")]
        static bool BuildingUnlocker(ref bool __result) {
            if (!(Enabled && config.DisableScienceCost)) {
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
        static bool PlaceInstantly(BlockObject prefab)
        {
            if (Enabled && config.EnableInstantBuilding)
            {
                Building component = prefab.GetComponent<Building>();
                component._placeFinished = true;
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
