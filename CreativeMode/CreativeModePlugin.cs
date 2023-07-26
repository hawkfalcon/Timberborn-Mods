using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CreativeMode.InformationShow;
using HarmonyLib;
using Newtonsoft.Json;
using Timberborn.BlockObjectTools;
using Timberborn.ToolSystem;
using Timberborn.Options;
using UnityEngine.UIElements;
using TimberApi.ModSystem;
using TimberApi.ConsoleSystem;
using TimberApi.DependencyContainerSystem;
using TimberApi.SpecificationSystem;
using TimberApi.SpecificationSystem.SpecificationTypes;
using TimberApi.ToolGroupSystem;
using Timberborn.BuildingTools;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.CursorToolSystem;
using Timberborn.GameScene;
using Timberborn.Localization;
using Timberborn.QuickNotificationSystem;
using Timberborn.WaterSourceSystemUI;
using UnityEngine;

/*
 ---------------------------------------------------------------------
    Mod Created by: https://github.com/hawkfalcon
    Updated by fork: https://github.com/AndRo-Marian
 ---------------------------------------------------------------------
*/

namespace CreativeMode
{
    [HarmonyPatch]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public class CreativeModePlugin : IModEntrypoint
    {
        private static CreativeModeConfig _config;
        private static Button _creativeButton;
        private static Button _scienceButton;
        private static Button _instantButton;
        private static ILoc _loc;
        
        private static InformationShowerPanel _infoPanel;
        private static QuickNotificationService _notification;

        #region Entry:

        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            var harmony = new Harmony("com.hawkfalcon.plugin.creativemode");
            
            _config = mod.Configs.Get<CreativeModeConfig>();
            
            harmony.PatchAll();
        }

        #endregion

        #region Localization:

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Loc), "Initialize")]
        private static void OnLanguageLoaded(Loc __instance)
        {
            // _loc = DependencyContainer.GetInstance<ILoc>();
            _loc = __instance;
        }

        #endregion
        #region Game initialize:

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NewGameInitializer), "Start")]
        private static void OnGameStarted(NewGameInitializer __instance)
        {
            _infoPanel = DependencyContainer.GetInstance<InformationShowerPanel>();
            _notification = DependencyContainer.GetInstance<QuickNotificationPanel>()._quickNotificationService;
        }

        #endregion
        #region Instant build:

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BuildingPlacer), "Place")]
        private static bool OnObjectPlace(BlockObject prefab)
        {
            var component = prefab.GetComponentFast<Building>();
            
            if (_config.Enabled && _config.InstantBuild)
            {
                component._placeFinished = true;
            }
            else if (component._placeFinished)
            {
                component._placeFinished = false;
            }
            return true;
        }

        #endregion
        #region No science cost:

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ToolButton), "OnButtonClicked")]
        private static bool OnToolClicked(ToolButton __instance, ClickEvent evt)
        {
            if (_config.Enabled && !_config.ScienceCost)
            {
                if (__instance.Tool.Locked)
                {
                    __instance.Tool.Locked = false;
                    __instance._toolManager.SwitchTool(__instance.Tool);
                    __instance.Tool.Locked = true;
                    return false;
                }
            }
            return true;
        }

        #endregion
        #region Game menu buttons:

        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsBox), "GetPanel")]
        private static void AddInGameMenuButtons(OptionsBox __instance, ref VisualElement __result)
        {
            var root = (VisualElement) __result.Query("OptionsBox");
            
            _creativeButton = new Button
            {
                classList = {"menu-button"},
                text = T("ToggleMode") + (_config.Enabled ? "On" : "Off")
            };
            _scienceButton = new Button
            {
                classList = {"menu-button"},
                text = T("ScienceCost") + (_config.ScienceCost ? "On" : "Off")
            };
            _instantButton = new Button
            {
                classList = {"menu-button"},
                text = T("InstantBuild") + (_config.InstantBuild ? "On" : "Off")
            };
            
            _creativeButton.clicked += () => ToggleCreativeMode(__instance);
            _scienceButton.clicked += () => ToggleSciencePoints(__instance);
            _instantButton.clicked += () => ToggleInstantBuild(__instance);

            root.Insert(4, _instantButton);
            root.Insert(4, _scienceButton);
            root.Insert(4, _creativeButton);
        }

        private static void ToggleCreativeMode(OptionsBox optionsBox)
        {
            _config.Enabled = !_config.Enabled;
            
            if ((_config.ResumeButtonClick & ResumeButtons.Creative) != 0) {
                optionsBox.ResumeClicked(null);
            } else {
                _creativeButton.text = T("ToggleMode") + (_config.Enabled ? "On" : "Off");
            }
            
            _notification.SendNotification(T("ToggleMode.Msg") + T(_config.Enabled ? "Enabled" : "Disabled"));
        }
        private static void ToggleSciencePoints(OptionsBox optionsBox)
        {
            _config.ScienceCost = !_config.ScienceCost;
            
            if ((_config.ResumeButtonClick & ResumeButtons.SciencePoints) != 0) {
                optionsBox.ResumeClicked(null);
            } else {
                _scienceButton.text = T("ScienceCost") + (_config.ScienceCost ? "On" : "Off");
            }
            
            _notification.SendNotification(T("ScienceCost.Msg") + T(_config.ScienceCost ? "Enabled" : "Disabled"));
        }
        private static void ToggleInstantBuild(OptionsBox optionsBox)
        {
            _config.InstantBuild = !_config.InstantBuild;
            
            if ((_config.ResumeButtonClick & ResumeButtons.InstantBuild) != 0) {
                optionsBox.ResumeClicked(null);
            } else {
                _instantButton.text = T("InstantBuild") + (_config.InstantBuild ? "On" : "Off");
            }
            
            _notification.SendNotification(T("InstantBuild.Msg") + T(_config.InstantBuild ? "Enabled" : "Disabled"));
        }

        #endregion
        #region Show tools:
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TimberbornGroupGenerator), "RuinsGroupDevelopment")]
        private static bool RuinsGroupDevelopment(ref ISpecification __result)
        {
            __result = new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                GroupId = "CreativeMode",
                Order = 31,
                DevMode = false
            }), "Ruins", "ToolGroupSpecification", isOriginal: false);
            return false;
        }
                
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TimberbornGroupGenerator), "MapEditorGroupDevelopment")]
        private static bool MapEditorGroupDevelopment(ref ISpecification __result)
        {
            __result = new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                GroupId = "CreativeMode",
                Icon = "Sprites/Bottombar/BuildingGroups/Other",
                Order = 32,
                DevMode = false
            }), "MapEditor", "ToolGroupSpecification", isOriginal: false);
            return false;
        }
                
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WaterSetting), "Visible", MethodType.Getter)]
        private static bool ShowWaterSetting(ref bool __result)
        {
            __result = _config.Enabled;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EntityBlockObjectDeletionTool), "DevModeTool", MethodType.Getter)]
        private static bool ShowDeletionTool(ref bool __result)
        {
            __result = false;
            return false;
        }
        
        /* - Power Wheel 10k Power */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockObjectTool), "DevModeTool", MethodType.Getter)]
        private static bool ShowDeveloperTools(ref bool __result)
        {
            __result = false;
            return false;
        }

        #endregion
        #region Information bar:

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CursorDebugger), "GetCoordinates")]
        public static void GetCursorCoordinates(CursorDebugger __instance, Ray ray)
        {
            var result = __instance._terrainPicker.PickTerrainCoordinates(ray);
            
            if (result == null) { return; }
            
            var builder = new StringBuilder();
            
            builder.Append(T("Height")).Append(result.Value.Coordinates.z + 1);
            builder.Append(" | ");
            builder.Append(DateTime.Now.ToString(_config.TimeFormat));
            
            _infoPanel.Text(builder.ToString());
        }

        #endregion

        #region Methods:

        /// <summary>
        /// Gets the translation of key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>string</returns>
        private static string T(string key)
        {
            return _loc.T("CreativeMode." + key);
        }

        #endregion
    }
}
