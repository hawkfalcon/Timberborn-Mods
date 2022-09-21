using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.ToolPanelSystem;

namespace UnifiedFactions
{
    [Configurator(SceneEntrypoint.InGame)]
    public class UnifiedFactionsConfigurator : IConfigurator
    {
		
		private class FactionTogglePanelModuleProvider : IProvider<ToolPanelModule>
		{
			private readonly FactionTogglePanel _factionTogglePanel;

			public FactionTogglePanelModuleProvider(FactionTogglePanel factionTogglePanel)
			{
				_factionTogglePanel = factionTogglePanel;
			}

			public ToolPanelModule Get()
			{
				ToolPanelModule.Builder builder = new();
				builder.AddFragment(_factionTogglePanel, 9);
				return builder.Build();
			}
		}

		public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<FactionTogglePanel>().AsSingleton();
            containerDefinition.MultiBind<ToolPanelModule>().ToProvider<FactionTogglePanelModuleProvider>().AsSingleton();
        }
    }
}