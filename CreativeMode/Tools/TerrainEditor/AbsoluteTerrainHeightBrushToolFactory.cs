using TimberApi.ToolSystem;
using Timberborn.MapEditor;
using Timberborn.ToolSystem;

namespace CreativeMode.Tools.TerrainEditor
{
    public class AbsoluteTerrainHeightBrushToolFactory : IToolFactory
    {
        public string Id => "AbsoluteTerrainHeightBrushTool";
        private readonly AbsoluteTerrainHeightBrushTool _beaverGeneratorTool;

        public AbsoluteTerrainHeightBrushToolFactory(AbsoluteTerrainHeightBrushTool beaverGeneratorTool)
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