using TimberApi.ConfigSystem;

namespace UnifiedFactions
{
    public class UnifiedFactionsConfig : IConfig
    {
        public string ConfigFileName => "UnifiedFactionsConfig";

        public bool EnableAllFactionBuildings = true;
        public bool EnableFactionLetters = true;
    }
}

