using System.Collections.Generic;
using Newtonsoft.Json;
using TimberApi.SpecificationSystem;
using TimberApi.SpecificationSystem.SpecificationTypes;

namespace CreativeMode.Tools.ToolsGroup
{
    public class ToolsGroupGenerator : ISpecificationGenerator
    {
        public IEnumerable<ISpecification> Generate()
        {
            yield return new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                Id = "CreativeMode",
                Layout = "Green",
                Order = 1,
                Type = "DefaultToolGroup",
                NameLocKey = "CreativeMode.ToolGroups",
                Icon = "Sprites/Avatars/FolktailsLogo",
                Section = "BottomBar",
                DevMode = false,
                Hidden = false,
                FallbackGroup = false,
                GroupInformation = new
                {
                    BottomBarSection = 0
                }
            }), "CreativeMode", "ToolGroupSpecification");
        }
    }
}