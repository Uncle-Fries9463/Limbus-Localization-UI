using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Limbus_Localization_UI.Mode_Handlers
{
    internal class Mode_EGO_Gifts
    {
        static Dictionary<string, dynamic> T;
        public static void InitTDictionaryHere(Dictionary<string, dynamic> FromExternal) => T=FromExternal;

        public static void AdjustUI()
        {
            T["PreviewLayout Background"].Source = new BitmapImage(new Uri("pack://application:,,,/Images/Фон ЭГО даров.png"));

            T["Window"].MinHeight = 320;
            T["Window"].MinWidth = 600;

            T["Window"].MaxWidth = 895;

            T["Window"].Width = 895;
            T["Window"].Height = 443;

            T["JsonIO Column"].Width = new GridLength(590);
            T["Json EditBox"].Width = 580;

            T["PreviewLayout @ EGO Gift"].Margin = new Thickness(53, 0, 0, 60);
            T["PreviewLayout @ Skill"   ].Margin = new Thickness(2000, 0, 0, 40);

            T["Left Menu Buttons box"].Margin = new Thickness(0, 0, 0, 0);
            T["Skill UptieLevel Selection Box"].Height = 0;

            T["Coin Desc Selection Box"].Height = 0;
            T["Coin Desc Selection Box sub"].Height = 0;


            T["SaveChanges Desc"].Margin = new Thickness(0, 4, 0, 0);
            T["SaveChanges Desc [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
            T["EditorSwitch Desc"].Width = 232.5;
            T["EditorSwitch Desc [UnavalibleCover]"].Width = 232.5;

            T["SaveChanges SubDesc 1"].Margin = new Thickness(0, 4, 0, 0);
            T["SaveChanges SubDesc 1 [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
            T["EditorSwitch SubDesc 1"].Width = 232.5;
            T["EditorSwitch SubDesc 1 [UnavalibleCover]"].Width = 232.5;

            T["SaveChanges SubDesc 2"].Margin = new Thickness(0, 4, 0, 0);
            T["SaveChanges SubDesc 2 [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
            T["EditorSwitch SubDesc 2"].Width = 232.5;
            T["EditorSwitch SubDesc 2 [UnavalibleCover]"].Width = 232.5;

            T["SaveChanges SubDesc 3"].Margin = new Thickness(0, 4, 0, 0);
            T["SaveChanges SubDesc 3 [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
            T["EditorSwitch SubDesc 3"].Width = 232.5;
            T["EditorSwitch SubDesc 3 [UnavalibleCover]"].Width = 232.5;

            T["SaveChanges SubDesc 4"].Margin = new Thickness(0, 4, 0, 0);
            T["SaveChanges SubDesc 4 [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
            T["EditorSwitch SubDesc 4"].Width = 232.5;
            T["EditorSwitch SubDesc 4 [UnavalibleCover]"].Width = 232.5;

            T["SaveChanges SubDesc 5"].Margin = new Thickness(0, 4, 0, 0);
            T["SaveChanges SubDesc 5 [UnavalibleCover]"].Margin = new Thickness(0, -30, 0, 0);
            T["EditorSwitch SubDesc 5"].Width = 232.5;
            T["EditorSwitch SubDesc 5 [UnavalibleCover]"].Width = 232.5;

            T["Unsaved Changes Tooltip"].Width = 185;

            T["Left Menu Buttons Box"].Height = Double.NaN;

            T["Name EditBox Shadow"].Content = "Название ЭГО Дара";
            for (int i = 1; i <= 5; i++)
            {
                T[$"EditorSwitch SubDesc {i}"].Content = $"Простое описание {i}";
            }
        }
    }
}
