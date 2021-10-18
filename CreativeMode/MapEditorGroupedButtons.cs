using Timberborn.MapEditor;
using Timberborn.BottomBarSystem;
using Timberborn.ToolSystem;
using System.Collections.Generic;

namespace CreativeMode {

    /**
     * Modified from MapEditorButtons
     */
    public class MapEditorGroupedButtons : IBottomBarElementsProvider {
        private static readonly string AbsoluteTerrainHeightBrushImageKey = "AbsoluteTerrainHeightBrush";

        private static readonly string RelativeTerrainHeightBrushImageKey = "RelativeTerrainHeightBrushIcon";

        private static readonly string NaturalResourcesSpawningImageKey = "NaturalResourcesIcon";

        private static readonly string NaturalResourcesRemovalImageKey = "RemoveNaturalResourcesIcon";

        private readonly ToolButtonFactory _toolButtonFactory;

        private readonly AbsoluteTerrainHeightBrushTool _absoluteTerrainHeightBrushTool;

        private readonly RelativeTerrainHeightBrushTool _relativeTerrainHeightBrushTool;

        private readonly NaturalResourceSpawningBrushTool _naturalResourceSpawningBrushTool;

        private readonly NaturalResourceRemovalBrushTool _naturalResourceRemovalBrushTool;

        public MapEditorGroupedButtons(ToolButtonFactory toolButtonFactory, AbsoluteTerrainHeightBrushTool absoluteTerrainHeightBrushTool, RelativeTerrainHeightBrushTool relativeTerrainHeightBrushTool, NaturalResourceSpawningBrushTool naturalResourceSpawningBrushTool, NaturalResourceRemovalBrushTool naturalResourceRemovalBrushTool) {
            _toolButtonFactory = toolButtonFactory;
            _absoluteTerrainHeightBrushTool = absoluteTerrainHeightBrushTool;
            _relativeTerrainHeightBrushTool = relativeTerrainHeightBrushTool;
            _naturalResourceSpawningBrushTool = naturalResourceSpawningBrushTool;
            _naturalResourceRemovalBrushTool = naturalResourceRemovalBrushTool;
        }

        public IEnumerable<BottomBarElement> GetElements() {
            ToolButton absoluteHeightToolButton = _toolButtonFactory.CreateGrouplessBlue(_absoluteTerrainHeightBrushTool, AbsoluteTerrainHeightBrushImageKey);
            yield return BottomBarElement.CreateSingleLevel(absoluteHeightToolButton.Root);
            ToolButton relativeHeightToolButton = _toolButtonFactory.CreateGrouplessBlue(_relativeTerrainHeightBrushTool, RelativeTerrainHeightBrushImageKey);
            yield return BottomBarElement.CreateSingleLevel(relativeHeightToolButton.Root);
            ToolButton resourceSpawningToolButton = _toolButtonFactory.CreateGrouplessBlue(_naturalResourceSpawningBrushTool, NaturalResourcesSpawningImageKey);
            yield return BottomBarElement.CreateSingleLevel(resourceSpawningToolButton.Root);
            ToolButton resourceRemovalToolButton = _toolButtonFactory.CreateGrouplessBlue(_naturalResourceRemovalBrushTool, NaturalResourcesRemovalImageKey);
            yield return BottomBarElement.CreateSingleLevel(resourceRemovalToolButton.Root);
        }
    }
}
