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
using Timberborn.MapEditor;
using Timberborn.BottomBarSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using Timberborn.BlockObjectTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    /**
     * Modified from MapEditorConfigurator
     */
    public class MapEditorButtonsConfigurator : IConfigurator {

        private class ToolPanelModuleProvider : IProvider<ToolPanelModule> {
            private readonly NaturalResourceSpawningBrushPanel _naturalResourceSpawningBrushPanel;

            private readonly BrushHeightPanel _brushHeightPanel;

            private readonly BrushSizePanel _brushSizePanel;

            private readonly BrushShapePanel _brushShapePanel;

            public ToolPanelModuleProvider(BrushHeightPanel brushHeightPanel, BrushSizePanel brushSizePanel, BrushShapePanel brushShapePanel, NaturalResourceSpawningBrushPanel naturalResourceSpawningBrushPanel) {
                _brushHeightPanel = brushHeightPanel;
                _brushSizePanel = brushSizePanel;
                _brushShapePanel = brushShapePanel;
                _naturalResourceSpawningBrushPanel = naturalResourceSpawningBrushPanel;
            }

            public ToolPanelModule Get() {
                ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
                builder.AddFragment(_naturalResourceSpawningBrushPanel, 5);
                builder.AddFragment(_brushHeightPanel, 6);
                builder.AddFragment(_brushSizePanel, 7);
                builder.AddFragment(_brushShapePanel, 8);
                return builder.Build();
            }
        }

        private class BottomBarModuleProvider : IProvider<BottomBarModule> {
            private readonly MapEditorGroupedButtons _mapEditorGroupedButtons;

            public BottomBarModuleProvider(MapEditorGroupedButtons mapEditorGroupButtons) {
                _mapEditorGroupedButtons = mapEditorGroupButtons;
            }

            public BottomBarModule Get() {
                BottomBarModule.Builder builder = new BottomBarModule.Builder();
                builder.AddMiddleSectionElements(_mapEditorGroupedButtons);
                return builder.Build();
            }
        }

        public void Configure(IContainerDefinition containerDefinition) {
            containerDefinition.Bind<NaturalResourceLayerService>().AsSingleton();
            containerDefinition.Bind<BrushProbabilityMap>().AsSingleton();
            containerDefinition.Bind<BrushShapeIterator>().AsSingleton();
            containerDefinition.Bind<NaturalResourceSpawner>().AsSingleton();
            containerDefinition.Bind<NoStartingLocationAlertFragment>().AsSingleton();
            containerDefinition.Bind<BrushHeightPanel>().AsSingleton();
            containerDefinition.Bind<BrushSizePanel>().AsSingleton();
            containerDefinition.Bind<BrushShapePanel>().AsSingleton();
            containerDefinition.Bind<NaturalResourceSpawningBrushPanel>().AsSingleton();
            containerDefinition.Bind<AbsoluteTerrainHeightBrushTool>().AsSingleton();
            containerDefinition.Bind<RelativeTerrainHeightBrushTool>().AsSingleton();
            containerDefinition.Bind<NaturalResourceSpawningBrushTool>().AsSingleton();
            containerDefinition.Bind<NaturalResourceRemovalBrushTool>().AsSingleton();
            containerDefinition.Bind<MapEditorButtons>().AsSingleton();
            containerDefinition.Bind<NaturalResourceLayerToggle>().AsSingleton();
            containerDefinition.MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
            containerDefinition.MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
        }

    }

    /**
     * Modified from MapEditorButtons
     */
    public class MapEditorGroupedButtons : IBottomBarElementsProvider {
        private static readonly string AbsoluteTerrainHeightBrushImageKey = "AbsoluteTerrainHeightBrush";

        private static readonly string RelativeTerrainHeightBrushImageKey = "RelativeTerrainHeightBrushIcon";

        private static readonly string NaturalResourcesSpawningImageKey = "NaturalResourcesIcon";

        private static readonly string NaturalResourcesRemovalImageKey = "RemoveNaturalResourcesIcon";

        private readonly ToolButtonFactory _toolButtonFactory;

        private readonly AbsoluteTerrainHeightBrushTool _absoluteTerrainHeightBrushTool;

        private readonly RelativeTerrainHeightBrushTool _relativeTerrainHeightBrushTool;

        private readonly NaturalResourceSpawningBrushTool _naturalResourceSpawningBrushTool;

        private readonly NaturalResourceRemovalBrushTool _naturalResourceRemovalBrushTool;

        public MapEditorGroupedButtons(ToolButtonFactory toolButtonFactory, AbsoluteTerrainHeightBrushTool absoluteTerrainHeightBrushTool, RelativeTerrainHeightBrushTool relativeTerrainHeightBrushTool, NaturalResourceSpawningBrushTool naturalResourceSpawningBrushTool, NaturalResourceRemovalBrushTool naturalResourceRemovalBrushTool) {
            _toolButtonFactory = toolButtonFactory;
            _absoluteTerrainHeightBrushTool = absoluteTerrainHeightBrushTool;
            _relativeTerrainHeightBrushTool = relativeTerrainHeightBrushTool;
            _naturalResourceSpawningBrushTool = naturalResourceSpawningBrushTool;
            _naturalResourceRemovalBrushTool = naturalResourceRemovalBrushTool;
        }

        public IEnumerable<BottomBarElement> GetElements() {
            ToolButton absoluteHeightToolButton = _toolButtonFactory.CreateGrouplessBlue(_absoluteTerrainHeightBrushTool, AbsoluteTerrainHeightBrushImageKey);
            yield return BottomBarElement.CreateSingleLevel(absoluteHeightToolButton.Root);
            ToolButton relativeHeightToolButton = _toolButtonFactory.CreateGrouplessBlue(_relativeTerrainHeightBrushTool, RelativeTerrainHeightBrushImageKey);
            yield return BottomBarElement.CreateSingleLevel(relativeHeightToolButton.Root);
            ToolButton resourceSpawningToolButton = _toolButtonFactory.CreateGrouplessBlue(_naturalResourceSpawningBrushTool, NaturalResourcesSpawningImageKey);
            yield return BottomBarElement.CreateSingleLevel(resourceSpawningToolButton.Root);
            ToolButton resourceRemovalToolButton = _toolButtonFactory.CreateGrouplessBlue(_naturalResourceRemovalBrushTool, NaturalResourcesRemovalImageKey);
            yield return BottomBarElement.CreateSingleLevel(resourceRemovalToolButton.Root);
        }
    }
}
