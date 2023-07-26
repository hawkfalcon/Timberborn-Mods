using System.Collections.Generic;
using Newtonsoft.Json;
using TimberApi.SpecificationSystem;
using TimberApi.SpecificationSystem.SpecificationTypes;

namespace CreativeMode.Tools.BeaverSpawner
{
    public class BeaverSpawnerToolGenerator : ISpecificationGenerator
    {
        public IEnumerable<ISpecification> Generate()
        {
            yield return new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                Id = "BeaverSpawner",
                GroupId = "CreativeMode",
                Type = "BeaverSpawnerTool",
                Layout = "Brown",
                Order = 1,
                Icon = "Sprites/BottomBar/BeaverGeneratorTool",
                NameLocKey = "CAN NOT BE MODIFIED",
                DescriptionLocKey = "CAN NOT BE MODIFIED",
                Hidden = false,
                DevMode = false,
                ToolInformation = new { }
            }), "BeaverSpawner", "ToolSpecification");
        }
    }
}