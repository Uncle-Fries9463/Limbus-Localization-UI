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

            T["Left Menu Buttons Box"].Height = Double.NaN;

            T["Name EditBox Shadow"].Content = "Название ЭГО Дара";
            for (int i = 1; i <= 5; i++)
            {
                T[$"EditorSwitch SubDesc {i}"].Content = $"Простое описание {i}";
            }
        }
    }
}
