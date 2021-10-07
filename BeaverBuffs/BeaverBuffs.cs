using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Timberborn.BonusSystem;
using System.Collections.Generic;
using System;

namespace BeaverBuffsPlugin {

    [BepInPlugin("com.hawkfalcon.plugin.beaverbuffs", "Beaver Buffs", "1.0.0.0")]
    [HarmonyPatch]
    public class CreativeModePlugin : BaseUnityPlugin {
        private static Dictionary<BonusType, ConfigEntry<float>> BonusMultipliers = new();
        
        public void OnEnable() {
            foreach (BonusType bonusType in Enum.GetValues(typeof(BonusType))) {
                BonusMultipliers.Add(bonusType, Config.Bind("BeaverBuffs.Multipliers",
                   bonusType.ToString(), 2.0f, "Buffs all beavers " + bonusType.ToString()));
            }
           
            var harmony = new Harmony("com.hawkfalcon.plugin.beaverbuffs");
            harmony.PatchAll();
            Logger.LogInfo("Plugin Beaver Buffs is loaded!");
        }

        /*
         * Inject beaver buffs
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BonusManager), "Awake")]
        static void AddBuffs(BonusManager __instance) {
            List<BonusSpecification> buffs = new();
            foreach (BonusType bonusType in Enum.GetValues(typeof(BonusType))) {
                float multipler = BonusMultipliers[bonusType].Value;
                buffs.Add(new BonusSpecification(bonusType, multipler));
            }
            __instance.AddBonuses(buffs);
        }
    }
}
