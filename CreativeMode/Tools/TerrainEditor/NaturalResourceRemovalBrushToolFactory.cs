using TimberApi.ToolSystem;
using Timberborn.MapEditor;
using Timberborn.ToolSystem;

namespace CreativeMode.Tools.TerrainEditor
{
    public class NaturalResourceRemovalBrushToolFactory : IToolFactory
    {
        public string Id => "NaturalResourceRemovalBrushTool";
        private readonly NaturalResourceRemovalBrushTool _beaverGeneratorTool;

        public NaturalResourceRemovalBrushToolFactory(NaturalResourceRemovalBrushTool beaverGeneratorTool)
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