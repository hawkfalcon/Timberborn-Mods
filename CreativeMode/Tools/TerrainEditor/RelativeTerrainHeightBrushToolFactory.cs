using TimberApi.ToolSystem;
using Timberborn.MapEditor;
using Timberborn.ToolSystem;

namespace CreativeMode.Tools.TerrainEditor
{
    public class RelativeTerrainHeightBrushToolFactory : IToolFactory
    {
        public string Id => "RelativeTerrainHeightBrushTool";
        private readonly RelativeTerrainHeightBrushTool _beaverGeneratorTool;

        public RelativeTerrainHeightBrushToolFactory(RelativeTerrainHeightBrushTool beaverGeneratorTool)
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