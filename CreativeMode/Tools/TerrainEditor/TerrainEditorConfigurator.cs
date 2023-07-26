using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.SpecificationSystem;
using TimberApi.ToolSystem;
using Timberborn.BottomBarSystem;
using Timberborn.MapEditor;
using Timberborn.ToolPanelSystem;

namespace CreativeMode.Tools.TerrainEditor
{
    [Configurator(SceneEntrypoint.InGame)]
    public class TerrainEditorConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<AbsoluteTerrainHeightBrushTool>().AsSingleton();
            containerDefinition.Bind<RelativeTerrainHeightBrushTool>().AsSingleton();
            containerDefinition.Bind<NaturalResourceSpawningBrushTool>().AsSingleton();
            containerDefinition.Bind<NaturalResourceRemovalBrushTool>().AsSingleton();
            
            containerDefinition.Bind<BrushShapeIterator>().AsSingleton();
            containerDefinition.Bind<NaturalResourceSpawner>().AsSingleton();
            containerDefinition.Bind<BrushProbabilityMap>().AsSingleton();
            
            
            containerDefinition.Bind<MapEditorButtons>().AsSingleton();

            containerDefinition.Bind<NaturalResourceLayerService>().AsSingleton();
            containerDefinition.Bind<NoStartingLocationAlertFragment>().AsSingleton();
            containerDefinition.Bind<BrushHeightPanel>().AsSingleton();
            containerDefinition.Bind<BrushSizePanel>().AsSingleton();
            containerDefinition.Bind<BrushShapePanel>().AsSingleton();
            containerDefinition.Bind<NaturalResourceSpawningBrushPanel>().AsSingleton();
            containerDefinition.Bind<NaturalResourceLayerToggle>().AsSingleton();
            
            containerDefinition.MultiBind<ToolPanelModule>().ToProvider<MapEditorConfigurator.ToolPanelModuleProvider>().AsSingleton();
            containerDefinition.MultiBind<BottomBarModule>().ToProvider<MapEditorConfigurator.BottomBarModuleProvider>().AsSingleton();
            
            containerDefinition.MultiBind<IToolFactory>().To<AbsoluteTerrainHeightBrushToolFactory>().AsSingleton();
            containerDefinition.MultiBind<IToolFactory>().To<RelativeTerrainHeightBrushToolFactory>().AsSingleton();
            containerDefinition.MultiBind<IToolFactory>().To<NaturalResourceSpawningBrushToolFactory>().AsSingleton();
            containerDefinition.MultiBind<IToolFactory>().To<NaturalResourceRemovalBrushToolFactory>().AsSingleton();
            containerDefinition.MultiBind<ISpecificationGenerator>().To<TerrainEditorGenerator>().AsSingleton();
        }
    }
}