using HarmonyLib;
using System;
using System.Windows.Forms;
using TaleWorlds.MountAndBlade;

namespace SummarizeCashflow
{
    internal class Main : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Settings.Load();
            try
            {
                new Harmony("com.radthordax.bannerlord.summarizecashflow").PatchAll();
            }
            catch (Exception ex)
            {
                Exception innerException = ex.InnerException;
                MessageBox.Show(string.Format("Failed to apply the SummarizeCashflow patch:\n{0}\n{1}", ex.Message, innerException.Message));
            }
        }
    }
}
