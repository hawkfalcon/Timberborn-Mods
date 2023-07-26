using Timberborn.Bots;
using Timberborn.Coordinates;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ToolSystem;
using UnityEngine;

namespace CreativeMode.Tools.BotSpawner
{
    public class BotSpawnerTool : Tool, IInputProcessor
    {        
        private static readonly string CursorKey = "BeaverAvatarCursor";
        private static readonly int ManyBotsToAdd = 10;
        private readonly BotFactory _botFactory;
        private readonly InputService _inputService;
        private readonly CursorService _cursorService;
        private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;
        public override bool DevModeTool => false;
        
        private ToolDescription _toolDescription;
        private readonly ILoc _loc;

        public BotSpawnerTool(BotFactory botFactory, InputService inputService, CursorService cursorService, CursorCoordinatesPicker cursorCoordinatesPicker, ILoc loc)
        {
            _botFactory = botFactory;
            _inputService = inputService;
            _cursorService = cursorService;
            _cursorCoordinatesPicker = cursorCoordinatesPicker;
            _loc = loc;
        }
        
        public void Initialize(ToolGroup toolGroup)
        {
            var title = _loc.T("CreativeMode.BotSpawnerTool.Title");
            var description = _loc.T("CreativeMode.BotSpawnerTool.Description");
            var usage = _loc.T("CreativeMode.BotSpawnerTool.Usage");
            var builder = new ToolDescription.Builder(title).AddSection(usage).AddPrioritizedSection(description);
            
            _toolDescription = builder.Build();
            ToolGroup = toolGroup;
        }

        public bool ProcessInput()
        {
            if (_inputService.SelectionStart && !_inputService.MouseOverUI)
            {
                var isShiftHeld = _inputService.IsShiftHeld;
                var count = isShiftHeld ? ManyBotsToAdd : 1;
                
                Debug.Log(count);
                
                PlaceBots(count);
                return true;
            }
            return false;
        }

        public override void Enter()
        {
            _cursorService.SetCursor(CursorKey);
            _inputService.AddInputProcessor(this);
        }

        public override void Exit()
        {
            _cursorService.ResetCursor();
            _inputService.RemoveInputProcessor(this);
        }

        private void PlaceBots(int count)
        {
            Vector3Int? vector3Int = _cursorCoordinatesPicker.CursorCoordinates();
            
            if (vector3Int.HasValue)
            {
                var position = CoordinateSystem.GridToWorldCentered(vector3Int.Value);

                for (var i = 0; i < count; i++)
                {
                    _botFactory.Create(position);
                }
            }
        }
        
        public override ToolDescription Description()
        {
            return _toolDescription;
        }
    }
}