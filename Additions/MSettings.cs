using System.IO;
using System.Windows.Media;

using static Limbus_Localization_UI.Additions.Consola;

namespace Limbus_Localization_UI.Additions
{
    class MSettings
    {
        static Dictionary<string, dynamic> T;
        public static void InitTDictionaryHere(Dictionary<string, dynamic> FromExternal) => T = FromExternal;

        public static void LoadSettings()
        {
            string[] Settings = File.ReadAllLines(@"[Ресурсы]\& Stringtypes\Settings.txt");

            try
            {
                MainWindow.JsonEditor_EnableHighlight = Settings[0].Split(" ¤ ")[^1].Trim().Equals("Yes") ? true : false;
                T["Settings ToggleHighlight"].Text = MainWindow.JsonEditor_EnableHighlight ? "Да" : "Нет";
            }
            catch { }
            try
            {
                string font = Settings[1].Split(" ¤ ")[^1].Trim();
                T["Json EditBox"].FontFamily = new FontFamily(font);
                T["Settings EditorFont"].FontFamily = new FontFamily(font);
                T["Settings EditorFont"].Content = font;
            }
            catch
            {
                T["Settings EditorFont"].Content = "Lucida Sans Unicode";
            }
            try
            {
                T["Json EditBox"].Foreground = РазноеДругое.GetColorFromAHEX("#FF" + Settings[2].Split(" ¤ ")[^1].Trim()[1..]);
                T["Settings EditorColor"].Text = "#" + Settings[2].Split(" ¤ ")[^1].Trim()[1..].ToUpper();
            }
            catch { }
            try
            {
                MainWindow.EnableDynamicKeywords = Settings[3].Split(" ¤ ")[^1].Trim().Equals("Yes") ? true : false;
                T["Enable Dynamic Keywords"].Text = MainWindow.EnableDynamicKeywords ? "Включено" : "Отключено";
            }
            catch { }
            try
            {
                MainWindow.BattleKeywords_Type = Settings[4].Split(" ¤ ")[^1].Trim();
                (MainWindow.Keywords, MainWindow.KeywordIDName) = РазноеДругое.GetKeywords(from: MainWindow.DefinedKeywords[MainWindow.BattleKeywords_Type]);
                MainWindow.Replacements = РазноеДругое.GetAddtReplacements(from: MainWindow.DefinedKeywords[MainWindow.BattleKeywords_Type]);
                T["Keywords Type Display"].Text = MainWindow.BattleKeywords_Type;
            }
            catch { }
            try
            {
                MainWindow.Shorthand_Type = Settings[5].Split(" ¤ ")[^1].Trim();
                T["Shorthand Type Display"].Text = MainWindow.Shorthand_Type;
            }
            catch { }
            try
            {
                MainWindow.ApplyLanguage(from: Settings[6].Split(" ¤ ")[^1].Trim());
            }
            catch { }

            if (MainWindow.BattleKeywords_Type == "KR")
            {
                РазноеДругое.SwitchToSDream();
            }
            else if (MainWindow.BattleKeywords_Type == "CN")
            {
                РазноеДругое.SwitchToSourceHanSansSC();
            }
        }

        public static void SaveSetting(string SettingType, string Setting)
        {
            try
            {
                string[] Settings = File.ReadAllLines(@"[Ресурсы]\& Stringtypes\Settings.txt");

                switch (SettingType)
                {
                    case "Enable <style> as color":
                        Settings[0] = $"Enable <style> as color ¤ {Setting}";
                        break;

                    case "JsonEditor Font":
                        Settings[1] = $"JsonEditor Font         ¤ {Setting}";
                        break;

                    case "JsonEditor Font Color":
                        Settings[2] = $"JsonEditor Font Color   ¤ {Setting}";
                        break;

                    case "Enable Dynamic Keywords":
                        Settings[3] = $"Dynamic Keywords        ¤ {Setting}";
                        break;

                    case "Keywords Type":
                        Settings[4] = $"Keywords Type           ¤ {Setting}";
                        break;

                    case "Shorthand Type":
                        Settings[5] = $"Shorthand Type          ¤ {Setting}";
                        break;

                    case "UI Language":
                        Settings[6] = $"UI Language             ¤ {Setting}";
                        break;
                }

                File.WriteAllLines(@"[Ресурсы]\& Stringtypes\Settings.txt", Settings);
            }
            catch { }
        }
    }
}