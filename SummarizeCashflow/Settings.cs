using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using TaleWorlds.Library;

namespace SummarizeCashflow
{
    public static class Settings
    {
        public static bool DebugModeEnabled = false;
        public static List<Group> IncomeGroups = new List<Group>();
        public static List<Group> ExpenseGroups = new List<Group>();

        public static void Load()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(BasePath.Name + "Modules/SummarizeCashflow/Settings.xml");
                XmlNode settings = xml.SelectSingleNode("Settings");

                DebugModeEnabled = bool.Parse(settings.SelectSingleNode("DebugModeEnabled").InnerText);

                XmlNode compatability = settings.SelectSingleNode("Entries");
                foreach (XmlNode group in compatability.SelectSingleNode("Income").SelectNodes("Group"))
                {
                    List<string> matches = new List<string>();
                    foreach (XmlNode match in group.SelectNodes("Match"))
                        matches.Add(match.InnerText);
                    IncomeGroups.Add(new Group(group.SelectSingleNode("Label").InnerText, matches));
                }
                foreach (XmlNode group in compatability.SelectSingleNode("Expenses").SelectNodes("Group"))
                {
                    List<string> matches = new List<string>();
                    foreach (XmlNode match in group.SelectNodes("Match"))
                        matches.Add(match.InnerText);
                    ExpenseGroups.Add(new Group(group.SelectSingleNode("Label").InnerText, matches));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("SummarizeCashflow could not load settings.\n\n" + e.ToString());
            }
        }
    }
}
