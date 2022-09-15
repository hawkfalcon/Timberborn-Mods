using System;
using Timberborn.BlockObjectTools;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using TimberbornAPI.EventSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnifiedFactions
{
	internal class FactionTogglePanel : EventListener, IToolFragment
	{
		private static BlockObjectTool currentTool = null;
		private VisualElement _root;

        public VisualElement InitializeFragment()
		{
			NineSliceButton button = new()
			{
				text = "Toggle Faction",
				classList = {
					"unity-text-element",
					"unity-button",
					"button-game",
					"block-object-rotation-panel__button"
				},
				style = {
					color = Color.white
				}
			};
			button.clicked += ToggleFaction;
			_root = button;
			Hide();
			return _root;
		}

        private static void ToggleFaction()
        {
			UnifiedFactionsPlugin.BuildingVariants.NextFaction();
            if (ToolButtonModifier.CurrentToolGroupButton != null && currentTool != null)
            {
				ToolButtonModifier.RefreshSection(currentTool.Prefab.name);
            }
        }

		[OnEvent]
		public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
		{
			bool shouldShow = false;
			if (toolEnteredEvent.Tool is BlockObjectTool tool)
			{
				currentTool = tool;
				string buildingName = currentTool.Prefab.name;
                shouldShow = UnifiedFactionsPlugin.BuildingVariants.HasVariant(buildingName);
			}
			_root.ToggleDisplayStyle(shouldShow);
		}

		[OnEvent]
		public void OnToolExited(ToolExitedEvent toolExitedEvent)
		{
			Hide();
		}

		private void Hide()
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
