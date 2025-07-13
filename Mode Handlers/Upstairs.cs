﻿using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.MainWindow;

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    internal abstract class Upstairs
    {
        internal protected static SwitchedInterfaceProperties ActiveProperties = new()
        {
            Key = "E.G.O Gifts",
            DefaultValues = new()
            {
                Height = 550,
                Width = 1000,
                MinHeight = 464,
                MinWidth = 709.8,
                MaxHeight = 10000,
                MaxWidth = 1000,
            },
        };

        internal protected record SwitchedInterfaceProperties
        {
            public string Key { get; set; }
            public DefaultValues DefaultValues { get; set; }
        }

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
             MainControl.MinWidth = From.MinWidth;
             MainControl.MaxWidth = From.MaxWidth;
            MainControl.MaxHeight = From.MaxHeight;
            MainControl.MinHeight = From.MinHeight;
                MainControl.Width = From.Width;
               MainControl.Height = From.Height;

            LockEditorUndo();
        }

        internal protected static void HideNavigationPanelButtons(Grid ExceptButtonsGrid, Grid ExceptPreviewLayout)
        {
            foreach (Grid PreviewLayoutChild in MainControl.PreviewLayouts.Children)
            {
                if (!PreviewLayoutChild.Equals(ExceptPreviewLayout)) PreviewLayoutChild.Visibility = Visibility.Collapsed;
                else PreviewLayoutChild.Visibility = Visibility.Visible;
            }

            foreach(Grid SwitchButtonsChild in MainControl.SwitchButtons_MainGrid.Children)
            {
                if (!SwitchButtonsChild.Equals(ExceptButtonsGrid)) SwitchButtonsChild.Visibility = Visibility.Collapsed;
                else SwitchButtonsChild.Visibility = Visibility.Visible;
            }
        }
    }
}
