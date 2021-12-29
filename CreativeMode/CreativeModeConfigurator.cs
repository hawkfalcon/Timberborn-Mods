using System;
using Bindito.Core;
using TimberbornAPI.EntityActionSystem;

namespace CreativeMode
{
    public class CreativeModeConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.MultiBind<IEntityAction>().To<BuildingPlacerEntityAction>().AsSingleton();
        }
    }
}

