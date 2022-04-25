using Timberborn.Buildings;
using Timberborn.ConstructibleSystem;
using TimberbornAPI.EntityActionSystem;
using UnityEngine;

namespace CreativeMode
{
    public class BuildingPlacerEntityAction : IEntityAction
    {
        public void ApplyToEntity(GameObject entity)
        {
            Constructible constructable = entity.GetComponent<Constructible>();
            Building building = entity.GetComponent<Building>();

            // Skip entity if components are missing or building is already finished on place
            if (constructable == null || building == null || building.PlaceFinished || !constructable.IsUnfinished)
                return;

            if (!(CreativeModePlugin.Enabled && CreativeModePlugin.EnableInstantBuilding.Value))
                return;

            // Finish instantly
            constructable.Finish();
        }
    }
}

