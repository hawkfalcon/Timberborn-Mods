using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.SpecificationSystem;

namespace CreativeMode.Tools.ToolsGroup
{
    [Configurator(SceneEntrypoint.InGame)]
    public class ToolsGroupConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.MultiBind<ISpecificationGenerator>().To<ToolsGroupGenerator>().AsSingleton();
        }
    }
}