using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using static Limbus_Localization_UI.MainWindow;

namespace Limbus_Localization_UI.Mode_Handlers
{
    internal class Mode_EGO_Gifts
    {
        static Dictionary<string, dynamic> T;
        public static void InitTDictionaryHere(Dictionary<string, dynamic> FromExternal) => T=FromExternal;

        public static void AdjustUI()
        {
            try
            {
                for (int i = 1; i <= 5; i++)
                {
                    T[$"EditorSwitch SubDesc {i} [UnavalibleCover]"].Height = 30;
                    T[$"EditorSwitch SubDesc {i}"].Height = 30;

                    T[$"SaveChanges SubDesc {i}"].Height = 30;
                    T[$"SaveChanges SubDesc {i} [UnavalibleCover]"].Height = 30;
                }
                T["EditorSwitch SubDesc 1"].Content = "Простое описание 1";
                T["Save Changes Buttons"].Margin = new Thickness(236, -237, 0, 0);
                T["Save Changes Buttons"].Height = 237;
                T["Save Menu Buttons Box SubBox"].Height = Double.NaN;

                T["Left Menu Buttons box"].Margin = new Thickness(0, 0, 0, 0);
                T["Skill UptieLevel Selection Box"].Height = 0;

                T["PreviewLayout Background"].Source = new BitmapImage(new Uri("pack://application:,,,/Images/Фон ЭГО даров.png"));

                T["Splitter"].Width = new GridLength(1);

                T["Window"].MinHeight = 320;
                T["Window"].MinWidth = 600;

                T["Window"].MaxWidth = 895;

                T["Window"].Width = 895;
                T["Window"].Height = 440;

                T["JsonIO Column"].Width = new GridLength(590);
                T["Json EditBox"].Width = 574;

                T["PreviewLayout @ EGO Gift"].Margin = new Thickness(53, 0, 0, 60);
                T["PreviewLayout @ Skill"   ].Margin = new Thickness(2000, 0, 0, 40);

                T["Left Menu Buttons box"].Margin = new Thickness(0, 0, 0, 0);
                T["Skill UptieLevel Selection Box"].Height = 0;

                T["Coin Desc Selection Box"].Height = 0;
                T["Coin Desc Selection Box sub"].Height = 0;

                T["SaveChanges Desc"].Margin = new Thickness(0, 0, 0, 0);
                T["SaveChanges Desc [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
                T["EditorSwitch Desc"].Width = 232.5;
                T["EditorSwitch Desc [UnavalibleCover]"].Width = 232.5;

                for (int i = 1; i <= 5; i++)
                {
                    T[$"SaveChanges SubDesc {i}"].Margin = new Thickness(0, 4, 0, 0);
                    T[$"SaveChanges SubDesc {i} [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
                    T[$"EditorSwitch SubDesc {i}"].Width = 232.5;
                    T[$"EditorSwitch SubDesc {i} [UnavalibleCover]"].Width = 232.5;
                }

                T["Unsaved Changes Tooltip"].Width = 185;

                T["Left Menu Buttons Box"].Height = Double.NaN;

                string s = InterfaceTextContent["[Left Menu] EGO Gift Description № Button"];
                for (int i = 1; i <= 5; i++)
                {
                    T[$"EditorSwitch SubDesc {i}"].Content = s.Exform(i);// $"Простое описание {i}";
                }
            }
            catch { }
        }
    }
}
