using System;
using TimberApi.ToolSystem;
using Timberborn.ToolSystem;

namespace CreativeMode.Tools.BotSpawner
{
    public class BotSpawnerToolFactory : IToolFactory
    {
        public string Id => "BotSpawnerTool";
        private readonly BotSpawnerTool _botGeneratorTool;

        public BotSpawnerToolFactory(BotSpawnerTool botGeneratorTool)
        {
            _botGeneratorTool = botGeneratorTool;
        }

        public Tool Create(ToolSpecification toolSpecification, ToolGroup toolGroup = null)
        {
            _botGeneratorTool.Initialize(toolGroup);
            
            return _botGeneratorTool;
        }
    }
}