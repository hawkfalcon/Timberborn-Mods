using System.Collections.Generic;
using Newtonsoft.Json;
using TimberApi.SpecificationSystem;
using TimberApi.SpecificationSystem.SpecificationTypes;

namespace CreativeMode.Tools.TerrainEditor
{
    public class TerrainEditorGenerator : ISpecificationGenerator
    {
        public IEnumerable<ISpecification> Generate()
        {
            yield return new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                Id = "AbsoluteTerrainHeightBrushTool",
                GroupId = "CreativeMode",
                Layout = "Brown",
                Order = 20,
                Type = "AbsoluteTerrainHeightBrushTool",
                NameLocKey = "CreativeMode.ToolGroups",
                DescriptionLocKey = "CreativeMode.ToolGroups",
                Icon = "Sprites/BottomBar/AbsoluteTerrainHeightBrush",
                DevMode = false,
                Hidden = false,
                ToolSpecification = new { }
            }), "CreativeMode", "ToolSpecification");
            
            yield return new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                Id = "RelativeTerrainHeightBrushTool",
                GroupId = "CreativeMode",
                Layout = "Brown",
                Order = 21,
                Type = "RelativeTerrainHeightBrushTool",
                NameLocKey = "CreativeMode.ToolGroups",
                DescriptionLocKey = "CreativeMode.ToolGroups",
                Icon = "Sprites/BottomBar/RelativeTerrainHeightBrushIcon",
                DevMode = false,
                Hidden = false,
                ToolSpecification = new { }
            }), "CreativeMode", "ToolSpecification");
            
            yield return new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                Id = "NaturalResourceSpawningBrushTool",
                GroupId = "CreativeMode",
                Layout = "Brown",
                Order = 22,
                Type = "NaturalResourceSpawningBrushTool",
                NameLocKey = "CreativeMode.ToolGroups",
                DescriptionLocKey = "CreativeMode.ToolGroups",
                Icon = "Sprites/BottomBar/NaturalResourcesIcon",
                DevMode = false,
                Hidden = false,
                ToolSpecification = new { }
            }), "CreativeMode", "ToolSpecification");
            
            yield return new GeneratedSpecification(JsonConvert.SerializeObject(new
            {
                Id = "NaturalResourceRemovalBrushTool",
                GroupId = "CreativeMode",
                Layout = "Brown",
                Order = 23,
                Type = "NaturalResourceRemovalBrushTool",
                NameLocKey = "CreativeMode.ToolGroups",
                DescriptionLocKey = "CreativeMode.ToolGroups",
                Icon = "Sprites/BottomBar/RemoveNaturalResourcesIcon",
                DevMode = false,
                Hidden = false,
                ToolSpecification = new { }
            }), "CreativeMode", "ToolSpecification");
        }
    }
}