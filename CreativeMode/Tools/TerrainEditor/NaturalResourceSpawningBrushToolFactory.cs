using TimberApi.ToolSystem;
using Timberborn.MapEditor;
using Timberborn.ToolSystem;

namespace CreativeMode.Tools.TerrainEditor
{
    public class NaturalResourceSpawningBrushToolFactory : IToolFactory
    {
        public string Id => "NaturalResourceSpawningBrushTool";
        private readonly NaturalResourceSpawningBrushTool _beaverGeneratorTool;

        public NaturalResourceSpawningBrushToolFactory(NaturalResourceSpawningBrushTool beaverGeneratorTool)
        {
            _beaverGeneratorTool = beaverGeneratorTool;
        }

        public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup = null)
        {
            _beaverGeneratorTool.ToolGroup = toolGroup;
            
            return _beaverGeneratorTool;
        }
    }
}