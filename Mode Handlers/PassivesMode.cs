using System.Windows;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_TaskValid.Mode_Handlers.Shared;

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    internal abstract class PassivesMode
    {
        internal protected static DefaultValues Defaults = new()
        {
             Height = 550,
             Width = 1000,
             MinHeight = 550,
             MinWidth = 710.5,
             MaxHeight = 10000,
             MaxWidth = 1000,
        };

        internal protected static void TriggerSwitch()
        {
            {
                ManualTextLoadEvent = true;
            }


            PreviewUpdate_TargetSite = MainControl.PreviewLayout_Passives;
            HideAllPreviewLayouts();
            AdjustUI(Defaults);

            MainControl.PreviewLayoutGrid_Passives.Visibility = Visibility.Visible;
            MainControl.Editor.Text = "Some Passives text";

            {
                ManualTextLoadEvent = false;
            }
        }
    }
}
