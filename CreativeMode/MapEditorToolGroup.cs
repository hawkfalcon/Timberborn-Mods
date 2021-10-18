using Timberborn.ToolSystem;
using Timberborn.WaterSystemRendering;
namespace CreativeMode {
    public class MapEditorToolGroup : ToolGroup, IWaterHider {
		private static readonly string RelativeTerrainHeightBrushImageKey = "RelativeTerrainHeightBrushIcon";

		public override string IconName => RelativeTerrainHeightBrushImageKey;

		public override string DisplayNameLocKey => "CreativeMode.ToolGroups.MapEditor";	
	}
}
