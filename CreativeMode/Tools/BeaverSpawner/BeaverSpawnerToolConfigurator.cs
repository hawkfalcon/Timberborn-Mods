using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.SpecificationSystem;
using TimberApi.ToolSystem;

namespace CreativeMode.Tools.BeaverSpawner
{
    [Configurator(SceneEntrypoint.InGame)]
    public class BeaverSpawnerToolConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<BeaverSpawnerTool>().AsSingleton();
            
            containerDefinition.MultiBind<IToolFactory>().To<BeaverSpawnerToolFactory>().AsSingleton();
            containerDefinition.MultiBind<ISpecificationGenerator>().To<BeaverSpawnerToolGenerator>().AsSingleton();
        }
    }
}