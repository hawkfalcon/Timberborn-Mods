using System;
using Timberborn.BlockObjectTools;
using Timberborn.CoreUI;
using Timberborn.ToolSystem;

namespace UnifiedFactions
{
    public class ToolButtonModifier
    {
        // TODO: when they add more factions, make this an enum
        public static bool ShowFolktails = true;

        public static ToolGroupButton CurrentToolGroupButton = null;

        /**
         * Hide and show duplicate buttons based on currently selected faction,
         * and automatically select the other tool
         */
        public static void RefreshSection(string selectedToolName)
        {
            foreach (ToolButton button in CurrentToolGroupButton._toolButtons)
            {
                if (button.Tool is BlockObjectTool otherTool)
                {
                    string otherToolName = otherTool.Prefab.name;
                    if (selectedToolName != null)
                    {
                        SelectOtherTool(button, otherToolName, selectedToolName);
                    }
                    ShowOneDuplicate(otherToolName, button);
                }
            }
        }

        /**
         * Automatically select the tool of the other faction
         * [tool names are Building.FactionName]
         */
        private static void SelectOtherTool(ToolButton button, string otherToolName, string selectedToolName)
        {
            string otherToolId = otherToolName.Split(".")[0];
            string selectedToolId = selectedToolName.Split(".")[0];
            if (otherToolId.Equals(selectedToolId) && !otherToolName.Equals(selectedToolName))
            {
                button._toolManager.SwitchTool(button.Tool);
            }
        }

        /**
         * Hide the other duplicate button
         */
        private static bool ShowOneDuplicate(string name, ToolButton button)
        {
            if (UnifiedFactionsPlugin.ToggleableBuildings.Contains(name) && button.ToolEnabled)
            {
                string factionId = ShowFolktails ? "Folktails" : "IronTeeth";
                bool visibility = name.Contains(factionId);
                button.Root.ToggleDisplayStyle(visibility);
                return true;
            }
            return false;
        }
    }
}

