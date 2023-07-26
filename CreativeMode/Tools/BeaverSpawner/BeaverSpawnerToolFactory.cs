using TimberApi.ToolSystem;
using Timberborn.ToolSystem;

namespace CreativeMode.Tools.BeaverSpawner
{
    public class BeaverSpawnerToolFactory : IToolFactory
    {
        public string Id => "BeaverSpawnerTool";
        private readonly BeaverSpawnerTool _beaverGeneratorTool;

        public BeaverSpawnerToolFactory(BeaverSpawnerTool beaverGeneratorTool)
        {
            _beaverGeneratorTool = beaverGeneratorTool;
        }

        public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup = null)
        {
            _beaverGeneratorTool.Initialize(toolGroup);
            
            return _beaverGeneratorTool;
        }
    }
}