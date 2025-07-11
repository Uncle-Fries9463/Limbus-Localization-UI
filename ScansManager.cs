using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.SettingsWindow;
using static System.Windows.Visibility;
using LC_Localization_Task_Absolute.Mode_Handlers;

namespace LC_Localization_Task_Absolute
{
    internal abstract class ScansManager
    {
        internal protected static void ToggleScanAreaView()
        {
            if (MainControl.SkillsPreviewFreeBordersCanvas.ActualHeight > 0)
            {
                if (MainControl.ScanAreaView_Skills.BorderThickness.Top == 0)
                {
                    // ACTIVATE
                    MainControl.SurfaceScrollPreview_Skills.ReconnectAsChildTo(MainControl.SkillsPreviewFreeBordersCanvas);
                    try
                    {
                        MainControl.SurfaceScrollPreview_Skills.Height = MainControl.SkillsPreviewFreeBordersCanvas.ActualHeight - 60;
                    }
                    catch { }
                    
                    if (Configurazione.DeltaConfig.ScanParameters.AreaWidth != 0)
                    {
                        MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Configurazione.DeltaConfig.ScanParameters.AreaWidth;
                    }
                    else
                    {
                        MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
                    }
                    
                    MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(2);
                    MainControl.ScanAreaView_Skills.Background = ToColor("#E1121212");

                    MainControl.PreviewScanButtonIndicator.Foreground = ToColor("#FF383838");
                    MainControl.MakeLimbusPreviewScan.IsHitTestVisible = false;

                    SettingsWindow.SettingsControl.ToggleScansPreview_I.Visibility = Visible;
                }
                else
                {
                    // HIDE
                    MainControl.SurfaceScrollPreview_Skills.ReconnectAsChildTo(MainControl.PreviewLayoutGrid_Skills);
                    MainControl.SurfaceScrollPreview_Skills.Height = double.NaN;

                    MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;

                    MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(0);
                    MainControl.ScanAreaView_Skills.Background = ToColor("#00121212");

                    MainControl.PreviewScanButtonIndicator.Foreground = ToColor("#FF9D9D9D");
                    MainControl.MakeLimbusPreviewScan.IsHitTestVisible = true;

                    SettingsWindow.SettingsControl.ToggleScansPreview_I.Visibility = Collapsed;
                }
            }
        }
    }
}
