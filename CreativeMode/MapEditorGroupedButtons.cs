using Timberborn.MapEditor;
using Timberborn.BottomBarSystem;
using Timberborn.ToolSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockObjectTools;
using System.Collections.Generic;

namespace CreativeMode { 

    /**
     * Modified from MapEditorButtons
     */
    public class MapEditorGroupedButtons : IBottomBarElementProvider {
        private static readonly string AbsoluteTerrainHeightBrushImageKey = "AbsoluteTerrainHeightBrush";

        private static readonly string RelativeTerrainHeightBrushImageKey = "RelativeTerrainHeightBrushIcon";

        private static readonly string NaturalResourcesSpawningImageKey = "NaturalResourcesIcon";

        private static readonly string NaturalResourcesRemovalImageKey = "RemoveNaturalResourcesIcon";

        private static readonly string MapEditorToolGroupKey = "MapEditor";

        private readonly MapEditorToolGroup _mapEditorToolGroup;

        private readonly ToolButtonFactory _toolButtonFactory;

        private readonly ToolGroupButtonFactory _toolGroupButtonFactory;

        private readonly ToolGroupSpecificationService _toolGroupSpecificationService;

        private readonly BlockObjectToolGroupSpecificationService _blockObjectToolGroupSpecificationService;

        private readonly BlockObjectToolGroupFactory _blockObjectToolGroupFactory;

        private readonly BlockObjectToolButtonFactory _blockObjectToolButtonFactory;

        private readonly AbsoluteTerrainHeightBrushTool _absoluteTerrainHeightBrushTool;

        private readonly RelativeTerrainHeightBrushTool _relativeTerrainHeightBrushTool;

        private readonly NaturalResourceSpawningBrushTool _naturalResourceSpawningBrushTool;

        private readonly NaturalResourceRemovalBrushTool _naturalResourceRemovalBrushTool;

        public MapEditorGroupedButtons(
            MapEditorToolGroup mapEditorToolGroup,
            ToolButtonFactory toolButtonFactory,
            ToolGroupButtonFactory toolGroupButtonFactory,
            ToolGroupSpecificationService toolGroupSpecificationService,
            BlockObjectToolGroupSpecificationService blockObjectToolGroupSpecificationService,
            BlockObjectToolGroupFactory blockObjectToolGroupFactory,
            BlockObjectToolButtonFactory blockObjectToolButtonFactory,
            AbsoluteTerrainHeightBrushTool absoluteTerrainHeightBrushTool,
            RelativeTerrainHeightBrushTool relativeTerrainHeightBrushTool,
            NaturalResourceSpawningBrushTool naturalResourceSpawningBrushTool,
            NaturalResourceRemovalBrushTool naturalResourceRemovalBrushTool) {
            _mapEditorToolGroup = mapEditorToolGroup;
            _toolButtonFactory = toolButtonFactory;
            _toolGroupButtonFactory = toolGroupButtonFactory;
            _toolGroupSpecificationService = toolGroupSpecificationService;
            _blockObjectToolGroupSpecificationService = blockObjectToolGroupSpecificationService;
            _blockObjectToolGroupFactory = blockObjectToolGroupFactory;
            _blockObjectToolButtonFactory = blockObjectToolButtonFactory;
            _absoluteTerrainHeightBrushTool = absoluteTerrainHeightBrushTool;
            _relativeTerrainHeightBrushTool = relativeTerrainHeightBrushTool;
            _naturalResourceSpawningBrushTool = naturalResourceSpawningBrushTool;
            _naturalResourceRemovalBrushTool = naturalResourceRemovalBrushTool;
        }

        public BottomBarElement GetElement() {
            ToolGroupButton group = _toolGroupButtonFactory.CreateBlue(_mapEditorToolGroup);
            AddTool(_absoluteTerrainHeightBrushTool, AbsoluteTerrainHeightBrushImageKey, group);
            AddTool(_relativeTerrainHeightBrushTool, RelativeTerrainHeightBrushImageKey, group);
            AddTool(_naturalResourceSpawningBrushTool, NaturalResourcesSpawningImageKey, group);
            AddTool(_naturalResourceRemovalBrushTool, NaturalResourcesRemovalImageKey, group);

            ToolGroupSpecification mapEditorBlockObjectsGroup = _toolGroupSpecificationService.GetToolGroupSpecification(MapEditorToolGroupKey);
            IEnumerable<PlaceableBlockObject> blockObjectsGroup = _blockObjectToolGroupSpecificationService.GetBlockObjectsFromGroup(mapEditorBlockObjectsGroup);
            foreach (PlaceableBlockObject placeableBlockObject in blockObjectsGroup) {
                ToolButton placeableBlockObjectTool = _blockObjectToolButtonFactory.Create(placeableBlockObject, _mapEditorToolGroup, group.ToolButtonsElement);
                group.AddTool(placeableBlockObjectTool);
            }

            return BottomBarElement.CreateMultiLevel(group.Root, group.ToolButtonsElement);
        }

        private void AddTool(Tool tool, string imageName, ToolGroupButton group) {
            ToolButton button = _toolButtonFactory.Create(tool, imageName, group.ToolButtonsElement);
            group.AddTool(button);
        }
    }
}
