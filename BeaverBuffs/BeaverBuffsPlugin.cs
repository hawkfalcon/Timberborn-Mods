using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Timberborn.BonusSystem;
using System.Collections.Generic;
using System;
using TimberbornAPI;
using TimberbornAPI.EntityActionSystem;
using Bindito.Core;
using UnityEngine;

namespace BeaverBuffs {

    [BepInPlugin("com.hawkfalcon.plugin.beaverbuffs", "Beaver Buffs", "1.1.0")]
    [HarmonyPatch]
    public class BeaverBuffsPlugin : BaseUnityPlugin
    {
        public static Dictionary<BonusType, ConfigEntry<float>> BonusMultipliers = new();
        
        public void OnEnable()
        {
            foreach (BonusType bonusType in Enum.GetValues(typeof(BonusType))) {
                BonusMultipliers.Add(bonusType, Config.Bind("BeaverBuffs.Multipliers",
                   bonusType.ToString(), 2.0f, "Buffs all beavers " + bonusType.ToString()));
            }

            TimberAPI.DependencyRegistry.AddConfigurator(new BeaverBuffsConfigurator());
            Logger.LogInfo("Plugin Beaver Buffs is loaded!");
        }
    }

    public class BeaverBuffsConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.MultiBind<IEntityAction>().To<BonusManagerEntityAction>().AsSingleton();
        }
    }

    public class BonusManagerEntityAction : IEntityAction
    {
        public void ApplyToEntity(GameObject entity)
        {
            BonusManager bonusManager = entity.GetComponent<BonusManager>();
            if (bonusManager == null)
                return;

            List<BonusSpecification> buffs = new();
            foreach (BonusType bonusType in Enum.GetValues(typeof(BonusType)))
            {
                float multipler = BeaverBuffsPlugin.BonusMultipliers[bonusType].Value;
                buffs.Add(new BonusSpecification(bonusType, multipler));
            }
            bonusManager.AddBonuses(buffs);
        }
    }
}
