using Timberborn.ToolSystem;

namespace CreativeMode {
    public class MapEditorToolGroup : ToolGroup {
		private static readonly string RelativeTerrainHeightBrushImageKey = "RelativeTerrainHeightBrushIcon";

		public override string IconName => RelativeTerrainHeightBrushImageKey;

		public override string DisplayNameLocKey => "CreativeMode.ToolGroups.MapEditor";	
	}
}
