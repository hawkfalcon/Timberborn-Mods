using System.Reflection;
using TimberApi.ConfigSystem;

namespace CreativeMode
{
    public class CreativeModeConfig : IConfig
    {
        public bool Enabled = false;
        public bool InstantBuild = false;
        public bool ScienceCost = true;
        public string TimeFormat = "MM/dd/yyyy hh:mm ss";
        
        public ResumeButtons ResumeButtonClick = ResumeButtons.None;

        public string ConfigFileName => Assembly.GetExecutingAssembly().GetName().Name;
    }
}
