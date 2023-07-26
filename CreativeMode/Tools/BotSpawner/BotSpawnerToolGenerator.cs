using System.Collections.Generic;
using Newtonsoft.Json;
using TimberApi.SpecificationSystem;
using TimberApi.SpecificationSystem.SpecificationTypes;

namespace CreativeMode.Tools.BotSpawner
{
    public class BotSpawnerToolGenerator : ISpecificationGenerator
    {
        public IEnumerable<ISpecification> Generate()
        {
            yield return new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                Id = "BotSpawner",
                GroupId = "CreativeMode",
                Type = "BotSpawnerTool",
                Layout = "Brown",
                Order = 2,
                Icon = "Sprites/BottomBar/BotGeneratorTool",
                NameLocKey = "CAN NOT BE MODIFIED",
                DescriptionLocKey = "CAN NOT BE MODIFIED",
                Hidden = false,
                DevMode = false,
                ToolInformation = new { }
            }), "BotSpawner", "ToolSpecification");
        }
    }
}