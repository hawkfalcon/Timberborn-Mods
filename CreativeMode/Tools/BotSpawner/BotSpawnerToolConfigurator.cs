using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.SpecificationSystem;
using TimberApi.ToolSystem;

namespace CreativeMode.Tools.BotSpawner
{
    [Configurator(SceneEntrypoint.InGame)]
    public class BotSpawnerToolConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<BotSpawnerTool>().AsSingleton();
            
            containerDefinition.MultiBind<IToolFactory>().To<BotSpawnerToolFactory>().AsSingleton();
            containerDefinition.MultiBind<ISpecificationGenerator>().To<BotSpawnerToolGenerator>().AsSingleton();
        }
    }
}