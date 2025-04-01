using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Limbus_Localization_UI.Additions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using static Limbus_Localization_UI.MainWindow;
using static Limbus_Localization_UI.Additions.Consola;

namespace Limbus_Localization_UI.Mode_Handlers
{
    class Mode_Passives
    {
        static Dictionary<string, dynamic> T;
        public static void InitTDictionaryHere(Dictionary<string, dynamic> FromExternal) => T = FromExternal;

        public static void AdjustUI()
        {
            try
            {
                T["PreviewLayout Background"].Source = new BitmapImage(new Uri("pack://application:,,,/Images/Фон Навыков.png"));

                T["Left Menu Buttons box"].Margin = new Thickness(0, 0, 0, 0);
                T["Skill UptieLevel Selection Box"].Height = 0;

                T["Splitter"].Width = new GridLength(3);

                T["Window"].MinHeight = 421; // Минимальная высота и ширина под фон предпросмтора навыка
                T["Window"].MinWidth = 717;

                T["Window"].MaxWidth = 1005; // Максимальная ширина с боковым меню

                T["Window"].Width = 1005;    // Ширина и высота по умолчанию
                T["Window"].Height = 545;

                T["JsonIO Column"].Width = new GridLength(705); // Ширина поля предпросмотра и редактора json элемента
                T["Json EditBox"].Width = 693.5;

                T["PreviewLayout @ EGO Gift"].Margin = new Thickness(5300, 0, 0, 60);  // Скрыть предпросмотр ЭГО дара
                T["PreviewLayout @ Skill"].Margin = new Thickness(11, 0, 0, 40);       // Показать предпросмотр Навыка

                T["Unsaved Changes Tooltip"].Width = 100;

                
                T["Name EditBox [UnavalibleCover]"].Height = 0;
                T["Name SaveChanges [UnavalibleCover]"].Height = 0;
                T["EditorSwitch Desc [UnavalibleCover]"].Height = 0;

                T["SaveChanges Desc [UnavalibleCover]"].Height = 0;

                T["EditorSwitch SubDesc 1"].Content = InterfaceTextContent["[Left Menu] Passive Summary Description"];

                for (int i = 2; i <= 5; i++)
                {
                    T[$"EditorSwitch SubDesc {i} [UnavalibleCover]"].Height = 0;
                    T[$"EditorSwitch SubDesc {i}"].Height = 0;

                    T[$"SaveChanges SubDesc {i}"].Height = 0;
                    T[$"SaveChanges SubDesc {i} [UnavalibleCover]"].Height = 0;
                }
                T["Save Changes Buttons"].Margin = new Thickness(236, -117, 0, 0);
                T["Save Changes Buttons"].Height = 97;
                T["Save Menu Buttons Box SubBox"].Height = 101;
                for (int i = 1; i <= 5; i++)
                {
                    T[$"Skill PreviewLayout Coin {i} Panel"].MinHeight = 0;
                    T[$"Skill PreviewLayout Coin {i} Panel"].Height = 0;
                }
            }
            catch { }
        }
        public static void UpdateMenuInfo(dynamic PassiveID)
        {
            try
            {
                Passives_CurrentEditingField = "Desc";
                T["SaveChanges SubDesc 1 [UnavalibleCover]"].Height = 30;
                T["EditorSwitch SubDesc 1 [UnavalibleCover]"].Height = 30;
                T["ID Label"].Content = $"{PassiveID}";
                T["Name EditBox"].Text = $"{Passives_Json_Dictionary[PassiveID]["Name"]}";
                if (Passives_EditBuffer[PassiveID]["Desc"].Equals("{unedited}"))
                {
                    T["Json EditBox"].Text = $"{Passives_Json_Dictionary[PassiveID]["Desc"]}";
                }
                else
                {
                    T["Json EditBox"].Text = $"{Passives_EditBuffer[PassiveID]["Desc"]}";
                }
                if (!Passives_EditBuffer[PassiveID]["Summary"].Equals("{unedited}"))
                {
                    T["EditorSwitch SubDesc 1"].Content = InterfaceTextContent["[Left Menu] Passive Summary Description"] + "*";
                }
                else T["EditorSwitch SubDesc 1"].Content = InterfaceTextContent["[Left Menu] Passive Summary Description"];

                if (!Passives_Json_Dictionary[PassiveID]["Summary"].Equals("{none}"))
                {
                    T["SaveChanges SubDesc 1 [UnavalibleCover]"].Height = 0;
                    T["EditorSwitch SubDesc 1 [UnavalibleCover]"].Height = 0;
                }
                T["Name Label"].Text = $"{Passives_Json_Dictionary[PassiveID]["Name"]}";
            }
            catch { }
        }
    }
}
