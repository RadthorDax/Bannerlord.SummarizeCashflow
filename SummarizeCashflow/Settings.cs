using System.Xml;
using TaleWorlds.Library;

namespace SummarizeCashflow
{
    public static class Settings
    {
        public static bool DebugModeEnabled = false;

        public static void Load()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(BasePath.Name + "Modules/SummarizeCashflow/ModuleData/settings.xml");
                XmlNode settings = xml.SelectSingleNode("Settings");

                DebugModeEnabled = bool.Parse(settings.SelectSingleNode("DebugModeEnabled").InnerText);

            }
            catch { }
        }
    }
}
