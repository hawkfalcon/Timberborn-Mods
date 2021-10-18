using Timberborn.MapEditor;
using Timberborn.BottomBarSystem;
using Timberborn.ToolPanelSystem;
using Bindito.Core;

namespace CreativeMode {
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
                builder.AddLeftSectionElement(_mapEditorGroupedButtons, 9);
                return builder.Build();
            }
        }

        public void Configure(IContainerDefinition containerDefinition) {
            containerDefinition.Bind<MapEditorToolGroup>().AsSingleton();
            containerDefinition.Bind<MapEditorButtons>().AsSingleton();

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
            containerDefinition.Bind<NaturalResourceLayerToggle>().AsSingleton();
            containerDefinition.MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
            containerDefinition.MultiBind<BottomBarModule>().ToProvider<BottomBarModuleProvider>().AsSingleton();
        }

    }
}
