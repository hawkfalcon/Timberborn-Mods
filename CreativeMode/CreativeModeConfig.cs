using TimberApi.ConfigSystem;

namespace CreativeMode
{
    public class CreativeModeConfig : IConfig
    {
        public string ConfigFileName => "CreativeModeConfig";

        public bool EnableInstantBuilding = true;
        public bool DisableScienceCost = true;
    }
}
