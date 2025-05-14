using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.MainWindow;

namespace LC_Localization_TaskValid.Mode_Handlers
{
    internal abstract class Shared
    {
        internal protected record DefaultValues
        {
            public double Height    { get; set; }
            public double Width     { get; set; }
            public double MinHeight { get; set; }
            public double MinWidth  { get; set; }
            public double MaxHeight { get; set; }
            public double MaxWidth  { get; set; }
        }

        internal protected static void AdjustUI(DefaultValues From)
        {
            MainControl.Width     = From.Width;
            MainControl.Height    = From.Height;
            MainControl.MinWidth  = From.MinWidth;
            MainControl.MaxWidth  = From.MaxWidth;
            MainControl.MaxHeight = From.MaxHeight;
            MainControl.MinHeight = From.MinHeight;
        }

        internal protected static void HideAllPreviewLayouts()
        {
            foreach(Grid PreviewLayoutChild in MainControl.PreviewLayouts.Children)
            {
                PreviewLayoutChild.Visibility = Visibility.Collapsed;
            }
        }
    }
}
