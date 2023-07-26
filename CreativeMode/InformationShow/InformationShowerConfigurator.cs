using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;

namespace CreativeMode.InformationShow
{
    [Configurator(SceneEntrypoint.InGame | SceneEntrypoint.MapEditor)]
    public class InformationShowerConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<InformationShowerPanel>().AsSingleton();
        }
    }
}