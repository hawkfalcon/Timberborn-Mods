using Timberborn.Beavers;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.ToolSystem;
using UnityEngine;

namespace CreativeMode.Tools.BeaverSpawner
{
    public class BeaverSpawnerTool : Tool, IInputProcessor
    {
        private static readonly string CursorKey = "BeaverAvatarCursor";
        private static readonly int ManyBeaversToAdd = 10;
        private readonly BeaverFactory _beaverFactory;
        private readonly InputService _inputService;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly CursorService _cursorService;
        private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;
        public override bool DevModeTool => false;
        
        private ToolDescription _toolDescription;
        private readonly ILoc _loc;

        public BeaverSpawnerTool(BeaverFactory beaverFactory, InputService inputService, IRandomNumberGenerator randomNumberGenerator, CursorService cursorService, CursorCoordinatesPicker cursorCoordinatesPicker, ILoc loc)
        {
            _beaverFactory = beaverFactory;
            _inputService = inputService;
            _randomNumberGenerator = randomNumberGenerator;
            _cursorService = cursorService;
            _cursorCoordinatesPicker = cursorCoordinatesPicker;
            _loc = loc;
        }
        
        public void Initialize(ToolGroup toolGroup)
        {
            var title = _loc.T("CreativeMode.BeaverSpawnerTool.Title");
            var description = _loc.T("CreativeMode.BeaverSpawnerTool.Description");
            var usage = _loc.T("CreativeMode.BeaverSpawnerTool.Usage");
            var builder = new ToolDescription.Builder(title).AddSection(usage).AddPrioritizedSection(description);
            
            _toolDescription = builder.Build();
            ToolGroup = toolGroup;
        }

        public bool ProcessInput()
        {
            if (_inputService.SelectionStart && !_inputService.MouseOverUI)
            {
                var isCtrlHeld = _inputService.IsCtrlHeld;
                var isShiftHeld = _inputService.IsShiftHeld;
                var count = isShiftHeld ? ManyBeaversToAdd : 1;
                
                PlaceBeavers(isCtrlHeld, count);
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

        private void PlaceBeavers(bool isChild, int count)
        {
            Vector3Int? vector3Int = _cursorCoordinatesPicker.CursorCoordinates();
            
            if (!vector3Int.HasValue)
            {
                return;
            }
            
            var position = CoordinateSystem.GridToWorldCentered(vector3Int.Value);
            
            for (var i = 0; i < count; i++)
            {
                var num = _randomNumberGenerator.Range(0f, 1f);
                
                if (isChild)
                {
                    _beaverFactory.CreateChild(position, num);
                }
                else
                {
                    _beaverFactory.CreateAdult(position, num);
                }
            }
        }
        
        public override ToolDescription Description()
        {
            return _toolDescription;
        }
    }
}