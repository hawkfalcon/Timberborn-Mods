using System;
using System.Collections.Generic;
using Timberborn.Buildings;

namespace UnifiedFactions
{
    public class BuildingVariantTracker
    {
        private int currentFaction = 0;

        private readonly Dictionary<string, List<string>> buildingVariants = new();

        // Add, preventing duplicates
        public bool TryAdd(string buildingName)
        {
            string buildingId = getId(buildingName);
            List<string> buildingVariant;
            if (!buildingVariants.TryGetValue(buildingId, out buildingVariant))
            {
                buildingVariant = new();
                buildingVariants.Add(buildingId, buildingVariant);
            }

            if (buildingVariant.Contains(buildingName)) return false;
            buildingVariant.Add(buildingName);
            return true;
        }

        // Is Toggleable?
        public bool HasVariant(string buildingName)
        {
            string buildingId = getId(buildingName);
            if (buildingVariants.TryGetValue(buildingId, out List<String> buildingVariant))
            {
                return buildingVariant.Count > 1;
            }
            return false;
        }

        public void NextFaction()
        {
            if (currentFaction == 0) currentFaction = 1;
            else currentFaction = 0;
        }

        public bool IsVisible(string buildingName)
        {
            string buildingId = getId(buildingName);
            if (buildingVariants.TryGetValue(buildingId, out List<String> buildingVariant))
            {
                return buildingVariant[currentFaction].Equals(buildingName);
            }
            return true;
        }

        private string getId(string buildingName)
        {
            return buildingName.Split(".")[0];
        }
    }
}

