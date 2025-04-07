using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

using Limbus_Localization_UI.Json;
using Limbus_Localization_UI.Additions;
using Limbus_Localization_UI.Mode_Handlers;
using static Limbus_Localization_UI.TextBases;
using static Limbus_Localization_UI.TagManager;
using static Limbus_Localization_UI.Additions.Consola;
using static Limbus_Localization_UI.Additions.РазноеДругое;

namespace Limbus_Localization_UI
{
    public partial class MainWindow : Window
    {
        public static Dictionary<string, dynamic> InterfaceTextElements = new();
        public static Dictionary<string, dynamic> InterfaceTextContent = new();
        bool IsOnlyStartedNow = true;
        public void InitializeUILanguage()
        {
            InterfaceTextElements = new()
            {
                ["Language Display"] = LangLabel,
                ["Language Hiths"] = EnableTranslationHints,

                ["Program Title"] = ProgramTitleName,
                ["(Refractor Button at Title) Open links"] = LinksToStyle,
                ["(Refractor Button at Title) Insert <style>"] = InsertStyle,
                ["(Refractor Button at Title) Tooltip"] = TitleRefractor_Tooltip,
                ["(Settings Button at Title) Tooltip"] = TitleSettings_Tooltip,
                ["[Settings] Title"] = Settings_Title,
                ["[Settings] Enable <style> highlight (Header)"] = Settings_EnableStyleHighlight_Title,
                ["[Settings] Enable <style> highlight (Description)"] = Settings_EnableStyleHighlight_Desc,
                ["[Settings] Enable <style> highlight (Display text)"] = ToggleHighlight_Text,
                ["[Settings] Json Editor Font (Header)"] = Settings_SelectedFont_Title,
                ["[Settings] Json Editor Font (Description)"] = Settings_SelectedFont_Desc,
                ["[Settings] Json Editor Foreground (Title)"] = Settings_EditorTextColor_Title,
                ["[Settings] Json Editor Foreground (Description)"] = Settings_EditorTextColor_Desc,

                ["[Settings] Enable Dynamic Keywords"] = Settings_EnableDynamicKeywords,
                ["[Settings] Enable Dynamic Keywords (Display text)"] = EnableDynamicKeywords_Display,

                ["[Settings] Keywords Type"] = Settings_KeywordsType, // Keywords - RU/EN/KR/CN

                ["[Settings] [Sub Buttons] Reload Sprites"] = SubButtons_ReloadSprites, // α
                ["[Settings] [Sub Buttons] Reload Keywords and Replacements+"] = SubButtons_ReloadKeywordsAndReplacements, // β
                ["[Settings] [Sub Buttons] Reload Color Pairs"] = SubButtons_ReloadColorPairs, // δ

                ["[Exit Dialog] Title"] = CloseEditor_Header,
                ["[Exit Dialog] Unsaved Changes Count"] = CloseEditor_UnsavedChangesCount,
                ["[Exit Dialog] Yes"] = CloseEditor_Yes,
                ["[Exit Dialog] No"] = CloseEditor_No,


                ["[Json Editor - Context Menu] Use <style>"] = ContextMenu_UseStyle,
                ["[Json Editor - Context Menu] Open Links to Tags"] = ContextMenu_LinkOpenTags,
                ["[Json Editor - Context Menu] Open Links to Shorthands"] = ContextMenu_LinkOpenShorthand,
                ["[Json Editor - Context Menu] Collapse Tags to Links"] = ContextMenu_TagsCollapse,
                ["[Json Editor - Context Menu] Recognize keyword to Tags"] = ContextMenu_RawKeywordConvertToTags,
                ["[Json Editor - Context Menu] Recognize keyword to Shorthand"] = ContextMenu_RawKeywordConvertToShorthand,

                ["[Json Editor - Context Menu] Use Italic"] = ContextMenu_UseItalic,
                ["[Json Editor - Context Menu] Use Bold"] = ContextMenu_UseBold,
                ["[Json Editor - Context Menu] Use Underline"] = ContextMenu_UseUnderline,
                ["[Json Editor - Context Menu] Use Strikethrough"] = ContextMenu_UseStrikethrough,
                ["[Json Editor - Context Menu] Use Size"] = ContextMenu_UseSize,
                ["[Json Editor - Context Menu] Use Font"] = ContextMenu_UseFont,
                ["[Json Editor - Context Menu] Use Color"] = ContextMenu_UseColor,

                ["[Json Editor - Context Menu] Select line"] = ContextMenu_SelectLine,
                ["[Json Editor - Context Menu] Clear Text"] = ContextMenu_ClearAllText,

                ["[Json Editor - Context Menu] Enable Translation Tips"] = ContextMenu_EnableTranslationTips_Display,

                ["[Left Menu] Json Filepath (Background text)"] = JsonFilepath_bgtext,
                ["[Left Menu] Select File tooltip"] = SelectFile_Tooltip,
                ["[Left Menu] ID Item Name"] = Name_Label,
                ["[Left Menu] ID Item Name (Background text)"] = Name_Label_bgtext,
                ["[Left Menu] Jump to ID (Background text)"] = JumpToID_bgtext,
                ["[Left Menu] Jump to ID Button (Tooltip)"] = lng001,
                ["[Left Menu] Copy ID (Tooltip)"] = lng002,
                ["[Left Menu] ID Copied Notify"] = IDCopiedNotify,

                ["[Left Menu] Background Name of EGO"] = ABName_Label_bgtext,

                ["[Left Menu] Main Description Button"] = SwitchEditorTo_Desc,
                ["[Left Menu] Sub Description 1 Button"] = SwitchEditorTo_SubDesc1,
                ["[Left Menu] Sub Description 2 Button"] = SwitchEditorTo_SubDesc2,
                ["[Left Menu] Sub Description 3 Button"] = SwitchEditorTo_SubDesc3,
                ["[Left Menu] Sub Description 4 Button"] = SwitchEditorTo_SubDesc4,
                ["[Left Menu] Sub Description 5 Button"] = SwitchEditorTo_SubDesc5,

                ["[Left Menu] Coin Descriptions"] = CoinDescsList,
                ["[Settings] Shorthand type Button"] = Shorthand_Type_Display,

                ["[Left Menu] Insertions (Header)"] = FormatInsertions_Title,
            };
            InterfaceTextContent = new()
            {
                ["[Settings] Enable <style> highlight (Display text - Yes)"] = "Да",
                ["[Settings] Enable <style> highlight (Display text - No)"] = "Нет",
                ["[Settings] Enable Dynamic Keywords (Display text - Enabled)"] = "Включено",
                ["[Settings] Enable Dynamic Keywords (Display text - Disabled)"] = "Отключено",
                ["[Json Editor - Context Menu] Enable Translation Tips - Enabled"] = "Подсказки перевода: Вкл",
                ["[Json Editor - Context Menu] Enable Translation Tips - Disabled"] = "Подсказки перевода: Выкл",
                ["[Left Menu] Description button (Duplicate)"] = "Описание",
                ["[Left Menu] Passive Summary Description"] = "Суммарно",
                ["[Left Menu] EGO Gift Description № Button"] = "Простое описание [№]",
                ["[Left Menu] Skill Coin № Button"] = "Монета [№]",
                ["[Left Menu] Insertions (Background text)"] = "Вставка [№]",

                ["Unsopported file warning"] = "Не поддерживается",
                ["File reading warning"] = "Ошибка чтения файла",
                ["ID not found warning"] = "ID не найден",
                ["Path to json file (After warning)"] = "Путь к Json файлу",
                ["Saving error warning"] = "Ошибка сохранения",

                ["[Exit Dialog] Unsaved changes tooltip (Desc)"] = "Описание",
                ["[Exit Dialog] Unsaved changes tooltip (EGO gift - Simple desc)"] = "Простое описание",
                ["[Exit Dialog] Unsaved changes tooltip (Summary)"] = "Суммарное описание",
                ["[Exit Dialog] Unsaved changes tooltip (Skills - Uptie level)"] = "Уровень связи",
            };
        }

        public static Dictionary<string, string> DefinedKeywords = new();
        public void ReadDefinedKeywords()
        {
            string[] Config = File.ReadAllLines(@"[Ресурсы]\& Stringtypes\BattleKeywords\$ Config.txt");
            int LineIndex = 0;
            foreach (var Line in Config)
            {
                if (Line.StartsWith("{ <--Keywords--> }"))
                {
                    string KeywordsTitle = Config[LineIndex + 1][9..];
                    string KeywordsFolder = Config[LineIndex + 2][10..];
                    DefinedKeywords[KeywordsTitle] = KeywordsFolder;

                    KeywordsSelector.Items.Add(new { Text = KeywordsTitle, FontSize = 16, Margin = new Thickness(-5,0,0,0), HorizontalAlignment = HorizontalAlignment.Center, Foreground = РазноеДругое.GetColorFromAHEX(@"#FFB4B4B4") });
                }

                LineIndex++;
            }
        }

        public Dictionary<string, string> DefinedLanguages = new();
        public void ReadDefinedLanguages()
        {
            string[] Config = File.ReadAllLines(@"[Ресурсы]\& Stringtypes\Languages\$ Config.txt");

            int LineIndex = 0;
            foreach(var Line in Config)
            {
                if (Line.StartsWith("{ <--Language--> }"))
                {
                    string LangTitle = Config[LineIndex + 1][9..];
                    string LangFile = Config[LineIndex + 2][8..];
                    DefinedLanguages[LangTitle] = LangFile;

                    LanguageSelector.Items.Add(new { Text = LangTitle, FontSize = 17, HorizontalAlignment=HorizontalAlignment.Center, Foreground = РазноеДругое.GetColorFromAHEX(@"#FFB4B4B4") }); 
                }

                LineIndex++;
            }
        }

        public static void ApplyLanguage(string from = "RU")
        {
            List<string> Language = File.ReadLines(@$"[Ресурсы]\& Stringtypes\Languages\{from}.llang").ToList();

            for (int LineIndex = 0; LineIndex < Language.Count; LineIndex++)
            {
                if (Language[LineIndex].Equals("{ <--¤--> }"))
                {
                    string Descriptor = Regex.Match(Language[LineIndex + 1], @"  Descriptor: '(.*?)'").Groups[1].ToString();
                    string TextData = Regex.Match(Language[LineIndex + 3], @"  Content: '(.*?)'").Groups[1].ToString();
                    try
                    {
                        TextData = TextData.Replace("\\n", "\n");
                        try
                        {
                            if (!InterfaceTextElements[Descriptor].Text.Equals("")) InterfaceTextElements[Descriptor].Text = TextData;
                        }
                        catch
                        {
                            if (!InterfaceTextElements[Descriptor].Content.Equals("")) InterfaceTextElements[Descriptor].Content = TextData;
                        }
                        InterfaceTextContent[Descriptor] = TextData.Replace("\\n", "\n");
                    }
                    catch (KeyNotFoundException) { }
                }
                else if (Language[LineIndex].Equals("{ <--l--> }"))
                {
                    string Descriptor = Regex.Match(Language[LineIndex + 1], @"  Descriptor: '(.*?)'").Groups[1].ToString();
                    string TextData = Regex.Match(Language[LineIndex + 3], @"  Content: '(.*?)'").Groups[1].ToString();

                    if (Descriptor.Equals("[Exit Dialog] Unsaved changes tooltip (Desc)")) rin(TextData);

                    InterfaceTextContent[Descriptor] = TextData;
                }
            }

            #region Дополнительные исключения
            if (JsonEditor_EnableHighlight)
            {
                T["Settings ToggleHighlight"].Text = InterfaceTextContent["[Settings] Enable <style> highlight (Display text - Yes)"];
            }
            else
            {
                T["Settings ToggleHighlight"].Text = InterfaceTextContent["[Settings] Enable <style> highlight (Display text - No)"];
            }


            if (EditorMode.Equals("Passives"))
            {
                T["EditorSwitch SubDesc 1"].Content = InterfaceTextContent["[Left Menu] Passive Summary Description"];
            }

            for(int i = 1; i <= 6; i++)
            {
                if (!T[$"Insertions {i} bgtext"].Equals(""))
                {
                    string s = InterfaceTextContent["[Left Menu] Insertions (Background text)"];
                    if (!T[$"Insertions {i} bgtext"].Text.Equals("")) T[$"Insertions {i} bgtext"].Text = s.Exform(i-1);
                }
            }

            
            if (EditorMode.Equals("EGOgift") & !OnlyStartedNow)
            {
                string s = InterfaceTextContent["[Left Menu] EGO Gift Description № Button"];
                for (int i = 1; i <= 5; i++)
                {
                    rin($"SimpleDesc{i}");
                    if (!EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{i}"].Equals("{unedited}"))
                    {
                        T[$"EditorSwitch SubDesc {i}"].Content = s.Exform(i) + "*";
                    }
                    else
                    {
                        T[$"EditorSwitch SubDesc {i}"].Content = s.Exform(i);
                    }
                }
            }

            if (EditorMode.Equals("Skills"))
            {
                string s = InterfaceTextContent["[Left Menu] Skill Coin № Button"];
                for (int i = 1; i <= 5; i++)
                {
                    T[$"EditorSwitch SubDesc {i}"].Content = s.Exform(i);
                }
            }

            if (!EnableTranslationHints)
            {
                InterfaceTextElements["[Json Editor - Context Menu] Enable Translation Tips"].Text = InterfaceTextContent["[Json Editor - Context Menu] Enable Translation Tips - Disabled"];
            }
            else
            {
                InterfaceTextElements["[Json Editor - Context Menu] Enable Translation Tips"].Text = InterfaceTextContent["[Json Editor - Context Menu] Enable Translation Tips - Enabled"];
            }


            if (!EnableDynamicKeywords)
            {
                T["Enable Dynamic Keywords"].Text = InterfaceTextContent["[Settings] Enable Dynamic Keywords (Display text - Disabled)"];
            }
            else
            {
                T["Enable Dynamic Keywords"].Text = InterfaceTextContent["[Settings] Enable Dynamic Keywords (Display text - Enabled)"];
            }


            #endregion

            InterfaceTextElements["Language Display"].Text = from;
            OnlyStartedNow = false;
        }
        private void MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.XButton1://Back button
                    ID_SwitchPrev_Click(null, new RoutedEventArgs());
                    break;
                case MouseButton.XButton2://forward button
                    ID_SwitchNext_Click(null, new RoutedEventArgs());
                    break;
                default:
                    break;
            }
        }

        #region Системные штуки

        #region Переменные
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        // На будущее..
        List<string> NotSupportedFileTypes = new()
        {
            "AbDlg_",
            "AbEvents",
            "AbnormalityGuides",
            "ActionEvents",
            "Announcer",
            "Assist",
            "AssociationName",
            "AttendanceRewardsText",
            "AttributeText",
            "BattleSpeechBubbleDlg",
            "BuffAbilities",
            "Characters",
            "DanteAbility",
            "DungeonNode",
            "DungeonStartBuffs",
            "EgoGiftCategory",
            "FileDownloadDesc",
            "IAP",
            "IntroduceCharacter",
            "Items",
            "MentalCondition",
            "MirrorDungeon",
            "NickName",
            "PanicInfo",
            "Personalities",
            "RailwayDungeon",
            "StageChapterText",
            "StageNode",
            "StagePartText",
            "StoryTheater",
            "UnlockCode",
            "Announcer_",
            "Voice_",

            "Skills_Ego_Personality-nextupdate",
            "Skills_Ego-a1c5p2-nextupdate",
        };

        private static bool OnlyStartedNow = true;
        public static bool IsUpdatePreviewDelayEnabled = false;
        public static float UpdatePreviewDelay = 0.08F;

        static string Mainfile_Filename = "";
        
        static Dictionary<string, BitmapImage> SpriteBitmaps = GetSpritesBitmaps();

        static bool EnableTranslationHints = false;
        static Dictionary<string, string> TranslationHints = GetTranslationHints();

        public static Dictionary<string, string> Keywords = new();
        public static Dictionary<string, string> KeywordIDName = new();

        public static Dictionary<string, string> K = new();

        Dictionary<string, string> KeywordsRuSet = GetKeywordsSet();
        public static Dictionary<string, string> Replacements = GetAddtReplacements();
        public static Dictionary<string, string> ColorPairs = GetColorPairs();

        static string LastPreviewUpdateText = "";
        static RichTextBox LastPreviewUpdateTarget = new();

        public static bool JsonEditor_EnableHighlight = true;
        public static bool EnableDynamicKeywords = false;
        public static string BattleKeywords_Type = "RU";
        public static string Shorthand_Type = "Crescent Corp.";
        public static FontFamily JsonEditor_FontFamily = new FontFamily("Lucida Sans Unicode");
        public static SolidColorBrush JsonEditor_TextColor = GetColorFromAHEX("#FFA69885");

        static string Json_Filepath;
        public static Dictionary<int, Dictionary<string, object>> EGOgift_Json_Dictionary = new();
        public static Dictionary<int, Dictionary<string, object>> EGOgift_EditBuffer = new(); // Буфер не сохранённых изменений (копия верхнего, но с "{unedited}" вместо строк)
        public static Dictionary<dynamic, Dictionary<string, object>> Passives_Json_Dictionary = new();
        public static Dictionary<dynamic, Dictionary<string, object>> Passives_EditBuffer = new();
        public static Dictionary<int, dynamic> Skills_Json_Dictionary = new();
        public static Dictionary<int, dynamic> Skills_EditBuffer = new();
        public static int Skills_CurrentCoinNumber = 1;
        public static void Update_CurrentEditingField(string New) => Skills_CurrentEditingField=New;

        static List<int> EGOgift_JsonKeys; // Список всех ID 
        static List<int> Skills_JsonKeys; // Список всех ID 
        static List<dynamic> Passives_JsonKeys; // Список всех ID 

        public static int EGOgift_Json_Dictionary_CurrentID = -1; // ID редактруемого ЭГО дара
        public static int Skills_Json_Dictionary_CurrentID = -1; // ID редактруемого Навыка
        public static dynamic Passives_Json_Dictionary_CurrentID = -1; // ID редактруемого Навыка

        public static int Skills_Json_Dictionary_CurrentUptieLevel = 0; // ID редактруемого Навыка

        public static string Skills_CurrentEditingField = "Desc"; // Текущее редактируемое поле Навыка
        public static string EGOgift_CurrentEditingField = "Desc"; // Текущее редактруемое поле ЭГО дара
        public static string Passives_CurrentEditingField = "Desc"; // Текущее редактруемое поле ЭГО дара

        static string EditorMode = "EGOgift";
        public static int CurrentHighlight_YOffset = 0;
        #endregion

        public static Dictionary<string, dynamic> T = new();
        public void InitializeStaticLinks()
        {
            T = new()
            {
                ["Window"] = this,
                ["JsonIO Column"] = JsonIO_Column,
                ["Left Menu Buttons box"] = LeftMenu_Box,

                ["Settings ToggleHighlight"] = ToggleHighlight_Text,
                ["Settings EditorFont"] = FontLabel,
                ["Settings EditorColor"] = JsonEditor_ColorSelector,
                ["Splitter"] = Splitter,

                ["Json EditBox"] = JsonEditor, ["ABName EditBox Shadow"] = ABName_Label_bgtext,
                ["PreviewLayout Background"] = PreviewLayout_BackgroundImage,
                ["PreviewLayout @ EGO Gift"] = PreviewLayout_EGOgift,
                ["PreviewLayout @ Skill"   ] = PreviewLayout_Skills,
                ["PreviewLayout @ Skills [Panel]"] = CurrentSkill_Panel,

                ["Unsaved Changes Tooltip"] = UnsavedChangesTooltip,
                ["Enable Dynamic Keywords"] = EnableDynamicKeywords_Display,
                ["Keywords Type Display"  ] = BattleKeywords_TypeDisplay,
                ["Shorthand Type Display" ] = Shorthand_TypeDisplay,

                ["ABName EditBox StackPanel"] = ABName_Input_StackPanel,     // 33
                ["ABName SaveChanges StackPanel"] = ABName_Input_StackPanel, // 33
                ["ABName EditBox"] = ABName_EditBox,
                ["ABName EditBox [UnavalibleCover]"] = ABName_ChangeNameInput_Cover,
                ["ABName SaveChanges [UnavalibleCover]"] = ABName_ChangeName_Cover,

                ["Name EditBox"] = Name_EditBox, ["Name EditBox Shadow"] = Name_Label_bgtext, 
                ["Name EditBox [UnavalibleCover]"    ] = Name_ChangeNameInput_Cover,
                ["Name SaveChanges [UnavalibleCover]"] = Name_ChangeName_Cover,
                ["ID Input"] = JumpToID_Input,

                ["ID Label"] = ID_Copy_Button, ["Name Label"] = Name_Label,
                ["Current Highlight"] = CurrentHighlight,

                ["Left Menu Buttons Box"] = LeftMenu_Box,
                ["Save Menu Buttons Box SubBox"] = LefMenuSubBox,

                ["Save Changes Buttons"] = SaveChangesButtons,

                ["EditorSwitch Desc"     ] = SwitchEditorTo_Desc    ,  ["EditorSwitch Desc [UnavalibleCover]"     ] = Desc_Cover    ,
                ["EditorSwitch SubDesc 1"] = SwitchEditorTo_SubDesc1,  ["EditorSwitch SubDesc 1 [UnavalibleCover]"] = SubDesc1_Cover,
                ["EditorSwitch SubDesc 2"] = SwitchEditorTo_SubDesc2,  ["EditorSwitch SubDesc 2 [UnavalibleCover]"] = SubDesc2_Cover,
                ["EditorSwitch SubDesc 3"] = SwitchEditorTo_SubDesc3,  ["EditorSwitch SubDesc 3 [UnavalibleCover]"] = SubDesc3_Cover,
                ["EditorSwitch SubDesc 4"] = SwitchEditorTo_SubDesc4,  ["EditorSwitch SubDesc 4 [UnavalibleCover]"] = SubDesc4_Cover,
                ["EditorSwitch SubDesc 5"] = SwitchEditorTo_SubDesc5,  ["EditorSwitch SubDesc 5 [UnavalibleCover]"] = SubDesc5_Cover,

                ["SaveChanges Desc"      ] = Desc_Change_Button     ,  ["SaveChanges Desc [UnavalibleCover]"      ] = Desc_Change_Cover    ,
                ["SaveChanges SubDesc 1" ] = SubDesc1_Change_Button ,  ["SaveChanges SubDesc 1 [UnavalibleCover]" ] = SubDesc1_Change_Cover,
                ["SaveChanges SubDesc 2" ] = SubDesc2_Change_Button ,  ["SaveChanges SubDesc 2 [UnavalibleCover]" ] = SubDesc2_Change_Cover,
                ["SaveChanges SubDesc 3" ] = SubDesc3_Change_Button ,  ["SaveChanges SubDesc 3 [UnavalibleCover]" ] = SubDesc3_Change_Cover,
                ["SaveChanges SubDesc 4" ] = SubDesc4_Change_Button ,  ["SaveChanges SubDesc 4 [UnavalibleCover]" ] = SubDesc4_Change_Cover,
                ["SaveChanges SubDesc 5" ] = SubDesc5_Change_Button ,  ["SaveChanges SubDesc 5 [UnavalibleCover]" ] = SubDesc5_Change_Cover,


                ["Skill PreviewLayout Coin 1 Panel"] = Skill_Coin1,
                ["Skill PreviewLayout Coin 2 Panel"] = Skill_Coin2,
                ["Skill PreviewLayout Coin 3 Panel"] = Skill_Coin3,
                ["Skill PreviewLayout Coin 4 Panel"] = Skill_Coin4,
                ["Skill PreviewLayout Coin 5 Panel"] = Skill_Coin5,


                ["Coin Descs 1 Button" ] = CoinDescs_1_Button,
                ["Coin Descs 2 Button" ] = CoinDescs_2_Button,
                ["Coin Descs 3 Button" ] = CoinDescs_3_Button,
                ["Coin Descs 4 Button" ] = CoinDescs_4_Button,
                ["Coin Descs 5 Button" ] = CoinDescs_5_Button,
                ["Coin Descs 6 Button" ] = CoinDescs_6_Button,
                ["Coin Descs 7 Button" ] = CoinDescs_7_Button,
                ["Coin Descs 8 Button" ] = CoinDescs_8_Button,
                ["Coin Descs 9 Button" ] = CoinDescs_9_Button,
                ["Coin Descs 10 Button"] = CoinDescs_10_Button,
                ["Coin Descs 11 Button"] = CoinDescs_11_Button,
                ["Coin Descs 12 Button"] = CoinDescs_12_Button,

                // Гэ
                ["Skill PreviewLayout Desc"] = MainSkillDesc,
                ["Skill PreviewLayout Coin 1 Desc 1" ] = Skill_Coin1_Desc1,
                ["Skill PreviewLayout Coin 1 Desc 2" ] = Skill_Coin1_Desc2,
                ["Skill PreviewLayout Coin 1 Desc 3" ] = Skill_Coin1_Desc3,
                ["Skill PreviewLayout Coin 1 Desc 4" ] = Skill_Coin1_Desc4,
                ["Skill PreviewLayout Coin 1 Desc 5" ] = Skill_Coin1_Desc5,
                ["Skill PreviewLayout Coin 1 Desc 6" ] = Skill_Coin1_Desc6,
                ["Skill PreviewLayout Coin 1 Desc 7" ] = Skill_Coin1_Desc7,
                ["Skill PreviewLayout Coin 1 Desc 8" ] = Skill_Coin1_Desc8,
                ["Skill PreviewLayout Coin 1 Desc 9" ] = Skill_Coin1_Desc9,
                ["Skill PreviewLayout Coin 1 Desc 10"] = Skill_Coin1_Desc10,
                ["Skill PreviewLayout Coin 1 Desc 11"] = Skill_Coin1_Desc11,
                ["Skill PreviewLayout Coin 1 Desc 12"] = Skill_Coin1_Desc12,

                ["Skill PreviewLayout Coin 2 Desc 1" ] = Skill_Coin2_Desc1,
                ["Skill PreviewLayout Coin 2 Desc 2" ] = Skill_Coin2_Desc2,
                ["Skill PreviewLayout Coin 2 Desc 3" ] = Skill_Coin2_Desc3,
                ["Skill PreviewLayout Coin 2 Desc 4" ] = Skill_Coin2_Desc4,
                ["Skill PreviewLayout Coin 2 Desc 5" ] = Skill_Coin2_Desc5,
                ["Skill PreviewLayout Coin 2 Desc 6" ] = Skill_Coin2_Desc6,
                ["Skill PreviewLayout Coin 2 Desc 7" ] = Skill_Coin2_Desc7,
                ["Skill PreviewLayout Coin 2 Desc 8" ] = Skill_Coin2_Desc8,
                ["Skill PreviewLayout Coin 2 Desc 9" ] = Skill_Coin2_Desc9,
                ["Skill PreviewLayout Coin 2 Desc 10"] = Skill_Coin2_Desc10,
                ["Skill PreviewLayout Coin 2 Desc 11"] = Skill_Coin2_Desc11,
                ["Skill PreviewLayout Coin 2 Desc 12"] = Skill_Coin2_Desc12,

                ["Skill PreviewLayout Coin 3 Desc 1" ] = Skill_Coin3_Desc1,
                ["Skill PreviewLayout Coin 3 Desc 2" ] = Skill_Coin3_Desc2,
                ["Skill PreviewLayout Coin 3 Desc 3" ] = Skill_Coin3_Desc3,
                ["Skill PreviewLayout Coin 3 Desc 4" ] = Skill_Coin3_Desc4,
                ["Skill PreviewLayout Coin 3 Desc 5" ] = Skill_Coin3_Desc5,
                ["Skill PreviewLayout Coin 3 Desc 6" ] = Skill_Coin3_Desc6,
                ["Skill PreviewLayout Coin 3 Desc 7" ] = Skill_Coin3_Desc7,
                ["Skill PreviewLayout Coin 3 Desc 8" ] = Skill_Coin3_Desc8,
                ["Skill PreviewLayout Coin 3 Desc 9" ] = Skill_Coin3_Desc9,
                ["Skill PreviewLayout Coin 3 Desc 10"] = Skill_Coin3_Desc10,
                ["Skill PreviewLayout Coin 3 Desc 11"] = Skill_Coin3_Desc11,
                ["Skill PreviewLayout Coin 3 Desc 12"] = Skill_Coin3_Desc12,

                ["Skill PreviewLayout Coin 4 Desc 1" ] = Skill_Coin4_Desc1,
                ["Skill PreviewLayout Coin 4 Desc 2" ] = Skill_Coin4_Desc2,
                ["Skill PreviewLayout Coin 4 Desc 3" ] = Skill_Coin4_Desc3,
                ["Skill PreviewLayout Coin 4 Desc 4" ] = Skill_Coin4_Desc4,
                ["Skill PreviewLayout Coin 4 Desc 5" ] = Skill_Coin4_Desc5,
                ["Skill PreviewLayout Coin 4 Desc 6" ] = Skill_Coin4_Desc6,
                ["Skill PreviewLayout Coin 4 Desc 7" ] = Skill_Coin4_Desc7,
                ["Skill PreviewLayout Coin 4 Desc 8" ] = Skill_Coin4_Desc8,
                ["Skill PreviewLayout Coin 4 Desc 9" ] = Skill_Coin4_Desc9,
                ["Skill PreviewLayout Coin 4 Desc 10"] = Skill_Coin4_Desc10,
                ["Skill PreviewLayout Coin 4 Desc 11"] = Skill_Coin4_Desc11,
                ["Skill PreviewLayout Coin 4 Desc 12"] = Skill_Coin4_Desc12,

                ["Skill PreviewLayout Coin 5 Desc 1" ] = Skill_Coin5_Desc1,
                ["Skill PreviewLayout Coin 5 Desc 2" ] = Skill_Coin5_Desc2,
                ["Skill PreviewLayout Coin 5 Desc 3" ] = Skill_Coin5_Desc3,
                ["Skill PreviewLayout Coin 5 Desc 4" ] = Skill_Coin5_Desc4,
                ["Skill PreviewLayout Coin 5 Desc 5" ] = Skill_Coin5_Desc5,
                ["Skill PreviewLayout Coin 5 Desc 6" ] = Skill_Coin5_Desc6,
                ["Skill PreviewLayout Coin 5 Desc 7" ] = Skill_Coin5_Desc7,
                ["Skill PreviewLayout Coin 5 Desc 8" ] = Skill_Coin5_Desc8,
                ["Skill PreviewLayout Coin 5 Desc 9" ] = Skill_Coin5_Desc9,
                ["Skill PreviewLayout Coin 5 Desc 10"] = Skill_Coin5_Desc10,
                ["Skill PreviewLayout Coin 5 Desc 11"] = Skill_Coin5_Desc11,
                ["Skill PreviewLayout Coin 5 Desc 12"] = Skill_Coin5_Desc12,

                ["Coin Desc Selection Box"] = CoinDescsSelectionBox,
                ["Coin Desc Selection Box sub"] = CoinDescsSelectionBox_Sub,



                ["Skill UptieLevel Selection Box"] = Skill_UptieLevel_Selection_Box,
                ["Uptie Level Icons"] = UptieLevelIcons,

                ["Uptie Level 1"] = UptieLevel1_Button, ["Uptie Level 1 [UnavalibleCover]"] = UptieLevel1_Cover, ["Uptie Level 1 [UnavalibleSubCover]"] = UptieLevel1_SubCover, 
                ["Uptie Level 2"] = UptieLevel2_Button, ["Uptie Level 2 [UnavalibleCover]"] = UptieLevel2_Cover, ["Uptie Level 2 [UnavalibleSubCover]"] = UptieLevel2_SubCover, 
                ["Uptie Level 3"] = UptieLevel3_Button, ["Uptie Level 3 [UnavalibleCover]"] = UptieLevel3_Cover, ["Uptie Level 3 [UnavalibleSubCover]"] = UptieLevel3_SubCover, 
                ["Uptie Level 4"] = UptieLevel4_Button, ["Uptie Level 4 [UnavalibleCover]"] = UptieLevel4_Cover, ["Uptie Level 4 [UnavalibleSubCover]"] = UptieLevel4_SubCover,

                ["Insertions 1 bgtext"] = FormatInsertion_1_bgtext,
                ["Insertions 2 bgtext"] = FormatInsertion_2_bgtext,
                ["Insertions 3 bgtext"] = FormatInsertion_3_bgtext,
                ["Insertions 4 bgtext"] = FormatInsertion_4_bgtext,
                ["Insertions 5 bgtext"] = FormatInsertion_5_bgtext,
                ["Insertions 6 bgtext"] = FormatInsertion_6_bgtext,
            };
        }
        private void LoadFonts()
        {
            // Все шрифты для настроек
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                FontSelector.Items.Add(new { Text = fontFamily.Source, FontFamily = new FontFamily(fontFamily.Source) });
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeStaticLinks();
            LoadFonts();


            Mode_Handlers.Mode_Skills   .InitTDictionaryHere(T);
            Mode_Handlers.Mode_EGO_Gifts.InitTDictionaryHere(T);
            Mode_Handlers.Mode_Passives .InitTDictionaryHere(T);
            РазноеДругое.InitTDictionaryHere(T);

            StartInits();
        }

        private void StartInits()
        {
            try { Console.OutputEncoding = Encoding.UTF8; }
            catch { }

            ReadDefinedLanguages();
            ReadDefinedKeywords();
            InitializeUILanguage();
            MSettings.InitTDictionaryHere(T);
            MSettings.LoadSettings();

            PreviewLayout_Skills.PreviewMouseLeftButtonDown += SurfaceScroll_MouseLeftButtonDown;
            PreviewLayout_Skills.PreviewMouseMove += SurfaceScroll_MouseMove;
            PreviewLayout_Skills.PreviewMouseLeftButtonUp += SurfaceScroll_MouseLeftButtonUp;
            PreviewMouseDown += MouseDownEvent;
            string Def;
            try
            {
                Def = File.ReadAllText(@"[Ресурсы]\& Stringtypes\DefaultText.txt");
            }
            catch
            {
                Def = "<font=\"Consolas\">「Limbus Company <sprite name=\"KnowledgeExplored\"><u><color=#ffab57>TextMeshPro WPF-Mimicry</u></color> ⇄ 2.0」</font>\n\n[ <sprite name=\"Breath\"> <sprite name=\"Charge\"> <sprite name=\"ParryingResultUp\"> ] Вставка спрайтов через тег <color=#ed5558><sprite name=\"...\"></color> (Перенос строки вместе с ними целостный)\n\n[ <color=#c57609>[KeywordID]</color> ]\n- [AaCePcBb]\n- [TimeKillerWatch]\n- [VibrationNesting]\n\n[ Все теги вместе (Развёртка ссылок) ]\n- <sprite name=\"AaCePcBc\"><color=#e30000><u><link=\"AaCePcBc\">Слепая одержимость</link></u></color>\n- <sprite name=\"ParryingResultUp\"><color=#f8c200><u><link=\"ParryingResultUp\">Повышение силы в столкновении</link></u></color>\n\n[ <color=#abcdef>Цвет</color> ] Цветной текст через тег <color=#hexrgb>\n[ <i>Крусив</i> ] <i>Курсивный текст через тег</i> i\n[ <u>Подчёркивание</u> ] <u>Нижнее подчёркивание текста через тег</u> u\n[ <strikethrough>Зачёркнутый</strikethrough> ] <strikethrough>Зачёркнутый</strikethrough> текст через тег strikethrough\n[ <b>Толстый текст</b> ] <b>Утолщённый текст через тег</b> b\n[ - ] <b>И <i>их <u>комби<color=#d24020>ниров</color>ание</i></u></b>\n\n\n<font=\"Consolas\">(i)</color> Этот текст можно изменить в файле <font=\"Consolas\">\"[Ресурсы]\\& Stringtypes\\DefaultText.txt\"</font>";
            }



            PreviewLayout_EGOgift.SetValue(Paragraph.LineHeightProperty, 30.0);

            // Скрыть все описания каждой монеты
            T[$"Skill PreviewLayout Desc"].SetValue(Paragraph.LineHeightProperty, 30.0);
            for (int i = 1; i <= 5; i++)
            {
                for (int e = 1; e <= 12; e++)
                {
                    T[$"Skill PreviewLayout Coin {i} Desc {e}"].SetValue(Paragraph.LineHeightProperty, 30.0);
                }
            }

            EditorMode = "Passives";
            Mode_Passives.AdjustUI();
            T["Json EditBox"].Text = Def;
        }
        #endregion


        #region Предпросмотр
        static bool IsPendingUpdate = false;
        public static void Call_UpdatePreview(string JsonDesc, RichTextBox Target, bool UpdatingFromLoad = false)
        {
            if (!OnlyStartedNow & IsUpdatePreviewDelayEnabled) // Задержка вывода на предпросмотр и анти-лаг
            {
                if (!IsPendingUpdate)
                {
                    IsPendingUpdate = true;
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(UpdatePreviewDelay) };
                    timer.Start();

                    timer.Tick += (sender, args) =>
                    {
                        timer.Stop();
                        UpdatePreview(UpdatingFromLoad ? JsonDesc : T["Json EditBox"].Text, Target);
                        IsPendingUpdate = false;
                    };
                }
            }
            else
            {
                UpdatePreview(JsonDesc, Target);
            }
            OnlyStartedNow = false;
        }

        /// <summary>
        /// При редактировании Json элемента обновлять предпросмотр и добавлять к кнопкам звёздочку при наличии несохранённых изменений
        /// </summary>
        private void Json_EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (EditorMode == "EGOgift")
                {
                    try
                    {
                        Call_UpdatePreview(JsonEditor.Text, PreviewLayout_EGOgift);
                    }
                    catch { }
                }

                else if (EditorMode == "Skills")
                {
                    var UpdatePreview_Target = MainSkillDesc;
                    var UpdatePreview_Text = "";
                    switch (Skills_CurrentEditingField[0..4])
                    {
                        case "Desc":
                            if (!JsonEditor.Text.Equals(Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"]))
                            {
                                Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"] = JsonEditor.Text;
                                T["EditorSwitch Desc"].Content = InterfaceTextContent["[Left Menu] Main Description Button"] + "*";
                            }
                            else
                            {
                                Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"] = "{unedited}";
                                T["EditorSwitch Desc"].Content = InterfaceTextContent["[Left Menu] Main Description Button"];
                            }


                            if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"].Equals("{unedited}"))
                            {
                                UpdatePreview_Text = JsonEditor.Text;
                            }
                            else
                            {
                                UpdatePreview_Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"];
                            }

                            if (!JsonEditor.Text.Equals(""))
                            {
                                T["Skill PreviewLayout Desc"].Height = Double.NaN; // Height="Auto"
                                T["Skill PreviewLayout Desc"].MinHeight = 30;
                            }
                            else
                            {
                                T["Skill PreviewLayout Desc"].Height = 0; // Height="Auto"
                                T["Skill PreviewLayout Desc"].MinHeight = 0;
                            }

                            UpdatePreview_Target = MainSkillDesc;
                            break;


                        default:
                            // Обновить предпросмотр описания монеты
                            int CoinDescIndex = int.Parse($"{Skills_CurrentEditingField[^1]}");
                            if (!JsonEditor.Text.Equals(Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex]))
                            {
                                Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex] = JsonEditor.Text;
                                T[$"Coin Descs {CoinDescIndex + 1} Button"].Content = $"№{CoinDescIndex + 1}*";
                            }
                            else
                            {
                                Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex] = "{unedited}";
                                T[$"Coin Descs {CoinDescIndex + 1} Button"].Content = $"№{CoinDescIndex + 1}";
                            }

                            if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex].Equals("{unedited}"))
                            {
                                UpdatePreview_Text = JsonEditor.Text.Replace("\r", "");
                            }
                            else
                            {
                                UpdatePreview_Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex];
                            }

                            UpdatePreview_Target = T[$"Skill PreviewLayout Coin {Skills_CurrentCoinNumber} Desc {CoinDescIndex + 1}"];

                            break;
                    }
                    Call_UpdatePreview(UpdatePreview_Text, UpdatePreview_Target);
                }

                else if (EditorMode.Equals("Passives"))
                {
                    Call_UpdatePreview(JsonEditor.Text, MainSkillDesc);
                    MainSkillDesc.Height = double.NaN;
                    switch (Passives_CurrentEditingField)
                    {
                        case "Desc":
                            if (!JsonEditor.Text.Equals(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Desc"]))
                            {
                                T["EditorSwitch Desc"].Content = InterfaceTextContent["[Left Menu] Main Description Button"] + "*";
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"] = JsonEditor.Text.Replace("\r", "");
                            }
                            else
                            {
                                T["EditorSwitch Desc"].Content = InterfaceTextContent["[Left Menu] Main Description Button"];
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                            }
                            break;

                        case "Summary":
                            if (!JsonEditor.Text.Equals(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Summary"]))
                            {
                                T["EditorSwitch SubDesc 1"].Content = InterfaceTextContent["[Left Menu] Passive Summary Description"] + "*";
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"] = JsonEditor.Text.Replace("\r", "");
                            }
                            else
                            {
                                T["EditorSwitch SubDesc 1"].Content = InterfaceTextContent["[Left Menu] Passive Summary Description"];
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"] = "{unedited}";
                            }
                            break;
                    }
                }


                // Проверить буфер не сохранённых изменений для обозначения соответсвующих кнопок звёздочкой
                if (EGOgift_Json_Dictionary_CurrentID != -1 & EditorMode == "EGOgift")
                {
                    switch (EGOgift_CurrentEditingField)
                    {
                        case "Desc":

                            if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["Desc"]))
                            {
                                SwitchEditorTo_Desc.Content = InterfaceTextContent["[Left Menu] Main Description Button"];
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                            }
                            else
                            {
                                SwitchEditorTo_Desc.Content = InterfaceTextContent["[Left Menu] Main Description Button"] + "*";
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"] = JsonEditor.Text.Replace("\r", "");
                            }

                            break;


                        default:

                            char DescNumber = EGOgift_CurrentEditingField[^1];
                            string s = InterfaceTextContent["[Left Menu] EGO Gift Description № Button"];

                            if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{DescNumber}"]))
                            {
                                T[$"EditorSwitch SubDesc {DescNumber}"].Content = s.Exform(DescNumber);
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{DescNumber}"] = "{unedited}";
                            }
                            else
                            {
                                T[$"EditorSwitch SubDesc {DescNumber}"].Content = s.Exform(DescNumber) + "*";
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{DescNumber}"] = JsonEditor.Text.Replace("\r", "");
                            }

                            break;
                    }
                }
            }
            catch { }
        }


        #region Методы добавления текста и спрайтов на предпросмотр
        private static void PreviewLayout_AppendText(TextConstructor TextData, RichTextBox Target)
        {
            try
            {
                var document = Target.Document;
                if (document.Blocks.LastBlock is not Paragraph lastParagraph)
                {
                    lastParagraph = new Paragraph();
                    document.Blocks.Add(lastParagraph);
                }

                Run PreviewLayout_AppendRun = new Run(TextData.Text);

                ApplyTags(ref PreviewLayout_AppendRun, TextData.InnerTags);

                #region Sub/Sup
                if (TextData.InnerTags.Contains("TextStyle@Subscript") | TextData.InnerTags.Contains("TextStyle@Superscript"))
                {
                    PreviewLayout_AppendRun.FontSize = 12;
                    StackPanel AlterHeight = new()
                    {
                        Height = 16,
                        Children = { new TextBlock(PreviewLayout_AppendRun) }
                    };

                    if (TextData.InnerTags.Contains("TextStyle@Subscript"))
                    {
                        AlterHeight.Margin = new Thickness(0, 0, 0, 0);
                    }
                    else if(TextData.InnerTags.Contains("TextStyle@Superscript"))
                    {
                        AlterHeight.Margin = new Thickness(0, -40, 0, 0);
                    }
                    AlterHeight.RenderTransform = new TranslateTransform(0, 5);

                    lastParagraph.Inlines.Add(new InlineUIContainer(AlterHeight));
                }
                #endregion
                
                else
                {
                    lastParagraph.Inlines.Add(PreviewLayout_AppendRun);
                }
            }
            catch { }
        }


        private static void PreviewLayout_AppendSprite(SpriteConstructor SpriteData, RichTextBox Target)
        {
            try
            {
                var document = Target.Document;
                if (document.Blocks.LastBlock is not Paragraph lastParagraph)
                {
                    lastParagraph = new Paragraph();
                    document.Blocks.Add(lastParagraph);
                }
                BitmapImage source;
                if (SpriteBitmaps.ContainsKey(SpriteData.SpriteLink)) source = SpriteBitmaps[SpriteData.SpriteLink];
                else source = SpriteBitmaps[@"$Unknown.png"];
                Image SpriteImage = new()
                {
                    Source = source,
                    Width = 21.6, Height = 21,
                    Margin = new Thickness(BattleKeywords_Type.Equals("CN") ? 0: -2, BattleKeywords_Type.Equals("CN") ? 5 : -0.5, 0, 0)
                };

                StackPanel SpritePlusEffectname = new() { Orientation = Orientation.Horizontal };
                SpritePlusEffectname.Children.Add(new TextBlock(new InlineUIContainer(SpriteImage)));

                Run KeywordName = new Run(SpriteData.TextBase.Text);
                ApplyTags(ref KeywordName, SpriteData.TextBase.InnerTags);

                SpritePlusEffectname.Children.Add(new TextBlock(KeywordName));

                if (EditorMode.Equals("EGOgift"))
                {
                    if (BattleKeywords_Type.Equals("KR") & KeywordName.FontFamily.ToString().Equals("Segoe UI"))
                    {
                        SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 6.845);
                        SpritePlusEffectname.Margin = new Thickness(0, -6.845 , 0, 0);
                    }
                    else if (BattleKeywords_Type.Equals("CN") & KeywordName.FontFamily.ToString().Equals("Segoe UI"))
                    {
                        SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 7);
                        SpritePlusEffectname.Margin = new Thickness(0, -7, 0, 0);
                    }
                    else
                    {
                        SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 10);
                        SpritePlusEffectname.Margin = new Thickness(0, -10, 0, 0);
                    }
                }
                else if (EditorMode.Equals("Skills") | EditorMode.Equals("Passives"))
                {
                    if (BattleKeywords_Type.Equals("KR") & KeywordName.FontFamily.ToString().Equals("Segoe UI"))
                    {
                        SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 8);
                        SpritePlusEffectname.Margin = new Thickness(0, -8, 0, 0);
                    }

                    else if (BattleKeywords_Type.Equals("CN") & KeywordName.FontFamily.ToString().Equals("Segoe UI"))
                    {
                        SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 7.35);
                        SpritePlusEffectname.Margin = new Thickness(0, -7.35, 0, 0);
                    }
                    else
                    {
                        SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 11);
                        SpritePlusEffectname.Margin = new Thickness(0, -11, 0, 0);
                    }

                }

                SpritePlusEffectname.VerticalAlignment = VerticalAlignment.Bottom;
                lastParagraph.Inlines.Add(new InlineUIContainer(SpritePlusEffectname));
            }
            catch {}
        }
        #endregion


        /// <summary>
        /// Обработать вводимый Json desc текст и вывести в окно предпросмотра
        /// </summary>
        public static void UpdatePreview(string JsonDesc, RichTextBox Target)
        {
            LastPreviewUpdateText = JsonDesc;
            LastPreviewUpdateTarget = Target;

            int index = 0;
            foreach (var Insertion in FormatInsertions)
            {
                if (Insertion != "")
                {
                    JsonDesc = JsonDesc.Replace($"{{{index}}}", Insertion);
                }
                index++;
            }
            // Выделение вставок через .Format в определённых файлах // Форматирование для других файлов
            if (Mainfile_Filename.StartsWith("Bufs") | Mainfile_Filename.StartsWith("BattleKeywords"))
            {
                JsonDesc = Regex.Replace(JsonDesc, @"{(\d+)}", Match => { return $"<color=#f95e00>{{{Match.Groups[1].Value}}}</color>"; });
                JsonDesc = JsonDesc.Replace("<style=\"highlight\">", "").Replace("<style=\"upgradeHighlight\">", "").Replace("</style>", "");
            }
            else
            {
                if (!JsonEditor_EnableHighlight)
                {
                    JsonDesc = JsonDesc.Replace("<style=\"highlight\">", "").Replace("<style=\"upgradeHighlight\">", "").Replace("</style>", "");
                }
                else
                {
                    if (EditorMode.Equals("EGOgift"))
                    {
                        JsonDesc = JsonDesc.Replace("<style=\"highlight\">", "");
                    }
                    else if (EditorMode.Equals("Skills") | EditorMode.Equals("Passives"))
                    {
                        JsonDesc = JsonDesc.Replace("<style=\"upgradeHighlight\">", "");
                    }
                }

                // Замена обычных слов на вствку ключевых с цветом и спрайтом, если они совпадают (Свойства [TabExplain] сохраняются)
                if (EnableDynamicKeywords)
                {
                    try
                    {
                        foreach (var KeywordName in KeywordIDName.Reverse())
                        {
                            JsonDesc = Regex.Replace(JsonDesc, @"(?<![\uAC00-\uD7A3a-zA-Zа-яА-Я<>\[\]\'`""*])" + KeywordName.Key + @"(?![\uAC00-\uC773\uC775-\uD7A3a-zA-Zа-яА-Я<[""*)('`])", match =>
                            {
                                return $"[{KeywordName.Value}]";
                            });
                        }
                    }
                    catch { }
                }
                // Заменить квадратные скобки ссылок на <sprite><color><u>...</u></color>, если текст из них есть в списке id из всех Keywords файлов
                JsonDesc = Regex.Replace(JsonDesc, @"\[(\w+)\]", match =>
                {
                    string MaybeKeyword = match.Groups[1].Value;
                    try
                    {
                        return Keywords.ContainsKey(MaybeKeyword) ? $"<sprite name=\"{MaybeKeyword}\"><color={(ColorPairs.ContainsKey(MaybeKeyword) ? ColorPairs[MaybeKeyword] : "#f8c200")}><u>{Keywords[MaybeKeyword]}</u></color>" : $"[{MaybeKeyword}]";
                    }
                    catch
                    {
                        return $"[{MaybeKeyword}]";
                    }
                });
            }


            // Обработка особых вставок эффектов [Sinking:'Утопания'] [Combustion:'Огня'] без полной развёртки в теги
            JsonDesc = Regex.Replace(JsonDesc, Shorthand_Type.Equals("kimght") ? @"\[(\w+)\:`(.*?)`\](\(#[a-fA-F0-9]{6}\))?" : @"\[(\w+)\:\*(.*?)\*\]", match =>
            {
                string KeywordID = match.Groups[1].Value;
                string KeywordName = match.Groups[2].Value;
                string KeywordColor = match.Groups[3].Value;

                string InsertColor = ColorPairs.ContainsKey(KeywordID) ? ColorPairs[KeywordID] : "#f8c200";

                if (!KeywordColor.Equals(""))
                {
                    InsertColor = Regex.Match(KeywordColor, @"[a-fA-F0-9]{6}").Groups[1].Value;
                }

                if (Keywords.ContainsKey(KeywordID))
                {
                    return $"<sprite name=\"{KeywordID}\"><color={InsertColor}><u>{KeywordName}</u></color>";
                }
                else
                {
                    return match.Groups[0].Value;
                }
            });


            if (TranslationHints.Count > 2 & EnableTranslationHints)
            {
                try
                {
                    string Size = TranslationHints["Size"];
                    string Color = TranslationHints["Color"];

                    foreach (var Replacement in TranslationHints)
                    {
                        JsonDesc = Regex.Replace(JsonDesc, @$"(?<![\uAC00-\uD7A3]){Replacement.Key}(?![\uAC00-\uC773\uC775-\uD7A3])", Match =>
                        {
                            return $"{Replacement.Key}<size={Size}><color={Color}>{Replacement.Value}</color></size>";
                        });
                    }
                }
                catch { }
            }
            

            // Доп замены
            if (Replacements.Count > 0)
            {
                foreach(var Replacement in Replacements)
                {
                    JsonDesc = JsonDesc.Replace(Replacement.Key, Replacement.Value);
                }
            }

            List<string> TagList = new()
            {
                "/color",
                "sub",
                "sup",
                "/sub",
                "/sup",
                "i",
                "/i",
                "u",
                "/u",
                "b",
                "/b",
                "/",
                "style=\"upgradeHighlight\"",
                "/style",
                "strikethrough",
                "/strikethrough",
                "/font",
                "/size",

                "\0",

                "EMPTY¤",
            };
            foreach(var Tag in TagList)
            {
                JsonDesc = JsonDesc.Replace($">{Tag}<", $">\0{Tag}<");
            }

            bool ICm(string Range_TextItem)
            {
                return !TagList.Contains(Range_TextItem) & !Range_TextItem.StartsWith("color=#") & !Range_TextItem.StartsWith("sprite name=\"") & !Range_TextItem.StartsWith("font=\"") & !Range_TextItem.StartsWith("size=");
            }

            #region Базовое форматирование текста
            JsonDesc = JsonDesc.Replace("color=#None", "color=#ffffff")
                               .Replace("<style=\"highlight\">", "<style=\"upgradeHighlight\">") // Подсветка улучшения (Без разницы как)
                               .Replace("</link>", "") // Ссылки вырезать (тултипы не работают (Возможно))
                               .Replace("[TabExplain]", "");
            
            JsonDesc = Regex.Replace(JsonDesc, @"<link=""\w+"">", ""); // убрать все link (Тултип не рабоатет)
            
            // Сепарированые обычных '<' '>' от тегов
            JsonDesc = Regex.Replace(JsonDesc, @"<color=#([0-9a-fA-F]{6})>", @"⇱color=#$1⇲");
            JsonDesc = Regex.Replace(JsonDesc, @"<sprite name=""(\w+)"">", @"⇱sprite name=""$1""⇲");
            JsonDesc = Regex.Replace(JsonDesc, @"<style=""(\w+)"">", @"⇱style=""$1""⇲");
            JsonDesc = Regex.Replace(JsonDesc, @"<font=""(.*?)"">", @"⇱font=""$1""⇲");
            JsonDesc = Regex.Replace(JsonDesc, @"<size=(\d+)%>", @"⇱size=$1%⇲");
            foreach (var Tag in TagList) JsonDesc = JsonDesc.Replace($"<{Tag}>", $"⇱{Tag}⇲");
            #endregion

            // Главное разбивание текста на список с обычным текстом и тегами
            string[] TextSegmented = $"⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0{JsonDesc.Replace("\\n", "\n")}\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0".Split(new char[] { '⇱', '⇲' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> __TextSegmented__ = TextSegmented.ToList();
            __TextSegmented__.RemoveAll(TextItem => TextItem.Equals("\0"));

          #region ¤ Форматирование тегов ¤
            int TextSegmented_Count = __TextSegmented__.Count();

            #region Обычный текст
            for (int TextItem_Index = 0; TextItem_Index < __TextSegmented__.Count; TextItem_Index++)
            {
                string TextItem = __TextSegmented__[TextItem_Index];

                #region ⟦InnerTag/UptieHighlight⟧
                if (TextItem.Equals("style=\"upgradeHighlight\"") & JsonEditor_EnableHighlight)
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/style")) break;
                        else if (!TagList.Contains(Range_TextItem))
                        {
                            if (Range_TextItem.Equals("/color"))
                            {
                                __TextSegmented__[RangeIndex] = "";
                            }
                            try
                            {
                                if (!__TextSegmented__[RangeIndex - 1].StartsWith("sprite name=\"") &
                                    !__TextSegmented__[RangeIndex - 2].StartsWith("sprite name=\"") &
                                    !__TextSegmented__[RangeIndex].StartsWith("size=") &
                                    !__TextSegmented__[RangeIndex - 1].StartsWith("size=") &
                                    !__TextSegmented__[RangeIndex].StartsWith("sprite name=\"")) // Сохранять цвета для статусных эффектов
                                    
                                {
                                    if (__TextSegmented__[RangeIndex].StartsWith("color=#"))
                                    {
                                        __TextSegmented__[RangeIndex] = "color=#f8c200";
                                    }
                                    else if (!Range_TextItem.Contains("⟦InnerTag/UptieHighlight⟧"))
                                    {
                                        __TextSegmented__[RangeIndex] += "⟦InnerTag/UptieHighlight⟧";
                                    }
                                }
                            }
                            catch { }
                        }

                    }
                }
                #endregion

                #region ⟦InnerTag/FontFamily@FontFamilyName⟧
                if (TextItem.StartsWith("font=\""))
                {
                    string FontFamily = Regex.Match(TextItem, @"font=""(.*)""").Groups[1].ToString();

                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/font") | Range_TextItem.StartsWith("font=\"")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/FontFamily@"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/FontFamily@{FontFamily}⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/FontSize@Percent⟧
                if (TextItem.StartsWith("size=") & TextItem.EndsWith('%'))
                {
                    string FontSize = Regex.Match(TextItem, @"size=(\d+)%").Groups[1].ToString();

                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/size") | (Range_TextItem.StartsWith("size=") & Range_TextItem.EndsWith('%'))) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/FontSize@"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/FontSize@{FontSize}%⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextColor@HexRGB⟧
                if (TextItem.StartsWith("color=#") & TextItem.Length == 13)
                {
                    string ColorCode = Regex.Match(TextItem, @"([0-9a-fA-F]{6})").Groups[1].ToString();
                    if (РазноеДругое.IsColor(ColorCode))
                    {
                        for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                        {
                            string Range_TextItem = __TextSegmented__[RangeIndex];

                            if (Range_TextItem.Equals("/color") | Range_TextItem.StartsWith("color=#")) break;

                            else
                            {
                                if (ICm(Range_TextItem))
                                {
                                    __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextColor@{ColorCode}⟧";
                                }
                            }
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Underline⟧
                if (TextItem.Equals("u"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/u")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Underline⟧") & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Strikethrough⟧"))
                        {
                            __TextSegmented__[RangeIndex] += "⟦InnerTag/TextStyle@Underline⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Italic⟧
                if (TextItem.Equals("i"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/i")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Italic⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Italic⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Bold⟧
                if (TextItem.Equals("b"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/b")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Bold⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Bold⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Strikethrough⟧
                if (TextItem.Equals("strikethrough"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/strikethrough")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Strikethrough⟧") & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Underline⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Strikethrough⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Subscript⟧
                if (TextItem.Equals("sub"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/sub") | Range_TextItem.Equals("sup")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Subscript⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Subscript⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Superscript⟧
                if (TextItem.Equals("sup"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/sup") | Range_TextItem.Equals("sub")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Superscript⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Superscript⟧";
                        }
                    }
                }
                #endregion
            }
            #endregion

            // Очистить сегментированный список текстовых фрагметов от тегов
            __TextSegmented__.RemoveAll(TextItem => (TagList.Contains(TextItem) | TextItem.StartsWith("color=#") | TextItem.StartsWith("font=\"") | TextItem.StartsWith("size=")) );

            #region Спрайты
            for (int TextItem_Index = 0; TextItem_Index < __TextSegmented__.Count; TextItem_Index++)
            {
                string TextItem = __TextSegmented__[TextItem_Index];

                #region ⟦LevelTag/SpriteLink⟧
                if (TextItem.StartsWith("sprite name=\""))
                {
                    string SpriteLink = TextItem.Split("sprite name=\"")[^1][0..^1];
                    
                    string SpriteKeyword = ":«»";
                    try
                    {
                        if (TextItem_Index + 1 != __TextSegmented__.Count)
                        {
                            string SpriteKeywordAppend = __TextSegmented__[TextItem_Index + 1].Split(' ')[0];
                            if (!__TextSegmented__[TextItem_Index + 1].StartsWith("sprite name=\""))
                            {
                                if (!__TextSegmented__[TextItem_Index + 1][0].Equals(" "))
                                {
                                    bool SpaceAdd = false;
                                    if (__TextSegmented__[TextItem_Index + 1].Split(' ').Count() > 1) SpaceAdd = true;
                                    __TextSegmented__[TextItem_Index + 1] = (SpaceAdd? " ": "") + String.Join(' ', __TextSegmented__[TextItem_Index + 1].Split(' ')[1..]);

                                    string NextTextItem_InnerTags = "";
                                    Regex InnerTags = new Regex(@"⟦InnerTag/(.*?)⟧");

                                    foreach(Match InnerTagMatch in InnerTags.Matches(__TextSegmented__[TextItem_Index + 1]))
                                    {
                                        NextTextItem_InnerTags += InnerTagMatch;
                                    }

                                    SpriteKeyword = $":«{(!SpriteKeywordAppend.Contains("\n")? SpriteKeywordAppend : "") + NextTextItem_InnerTags}»";
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    __TextSegmented__[TextItem_Index] = $"⟦LevelTag/SpriteLink@{SpriteLink}{SpriteKeyword}⟧";
                    
                }
                #endregion
            }
            #endregion

            #endregion

            //rin($"Name: {Target.Name}  —  Font: {Target.FontFamily}");

            #region Debug Preview
            {
                //string ApplySyntax(string s)
                //{
                //    s = s.Replace("⟦", "\x1b[38;5;62m⟦\x1b[0m");
                //    s = s.Replace("⟧", "\x1b[38;5;62m⟧\x1b[0m");
                //    s = s.Replace("@", "\x1b[38;5;62m@\x1b[0m");
                //    s = s.Replace("«", "\x1b[38;5;72m«");
                //    s = s.Replace("»", "\x1b[38;5;72m»\x1b[0m");
                //    s = s.Replace("InnerTag", "\x1b[38;5;203mInnerTag\x1b[0m");
                //    s = s.Replace("LevelTag", "\x1b[38;5;204mLevelTag\x1b[0m");
                //    s = s.Replace("FontFamily", "\x1b[38;5;202mFontFamily\x1b[0m");
                //    s = s.Replace("TextColor", "\x1b[38;5;202mTextColor\x1b[0m");
                //    s = s.Replace("TextStyle", "\x1b[38;5;202mTextStyle\x1b[0m");
                //    s = s.Replace("SpriteLink", "\x1b[38;5;202mSpriteLink\x1b[0m");
                //    return s;
                //}

                //Console.Clear();
                //Console.WriteLine($"Limbus Company TextMeshPro WPF-Mimicry  (\x1b[38;5;62m{Target.Name}\x1b[0m):");
                //int index = 0;
                //List<string> PreviewDebug = new();
                //foreach (var TextItem in __TextSegmented__)
                //{
                //    if (TextItem.StartsWith("⟦LevelTag/SpriteLink"))
                //    {
                //        string Content = Regex.Match(TextItem, @"«(.*?)»").Groups[1].Value;
                //        rin(ApplySyntax($"\n({index}) LevelTag ^\n - Link: «{TextItem.Split("⟦LevelTag/SpriteLink@")[1].Split(":«")[0]}»\n - Content: «`{ClearText(TextItem)}`»\n - Properties: {String.Join("\n               ", InnerTags(TextItem))}").Replace("»", "").Replace("«", ""));
                //    }
                //    else
                //    {
                //        rin(ApplySyntax($"\n({index}) InnerTag ^\n - Content: «`{ClearText(TextItem)}`»\n - Properties: {String.Join("\n               ", InnerTags(TextItem))}").Replace("«", "").Replace("»", ""));
                //    }
                //    index++;
                //}
            }
            #endregion
            Target.Document.Blocks.Clear();
            #region Вывод на предпросмотр
            foreach (string TextItem in __TextSegmented__)
            {
                #region Спрайты
                if (TextItem.StartsWith("⟦LevelTag/SpriteLink@") & TextItem.EndsWith('⟧'))
                {
                    var SpriteSet = TextItem.Split(":«");
                    string This_SpriteKeyword = SpriteSet[0].Split("@")[^1].Replace("⟧", "");

                    string This_StickedWord = "";
                    if (SpriteSet.Count() == 2) This_StickedWord = SpriteSet[1][0..^2];

                    if (Keywords.ContainsKey(This_SpriteKeyword))
                    {
                       This_SpriteKeyword += SpriteBitmaps.ContainsKey(This_SpriteKeyword + ".png")? ".png" : ".webp";
                    }
                    else
                    {
                        This_SpriteKeyword = "Unknown.png";
                    }

                    SpriteConstructor Current_SpriteConstructor = new SpriteConstructor
                    {
                        SpriteLink = This_SpriteKeyword,
                        TextBase = new TextConstructor
                        {
                            InnerTags = InnerTags(This_StickedWord),
                            Text = ClearText(This_StickedWord),
                        }
                    };
                    PreviewLayout_AppendSprite(Current_SpriteConstructor, Target);
                }
                #endregion
                #region Обычный текст
                else
                {
                    TextConstructor Current_TextConstructor = new TextConstructor
                    {
                        InnerTags = InnerTags(TextItem),
                        Text = ClearText(TextItem)
                    };

                    PreviewLayout_AppendText(Current_TextConstructor, Target);
                }
                #endregion
            }
          #endregion
        }
        #endregion


        #region Интерфейс
        private void Exit_Yes(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch { }
        }
        private void Exit_No(object sender, RoutedEventArgs e)
        {
            try
            {
                OverrideCover1.Margin = new Thickness(1000);
                OverrideCover2.Margin = new Thickness(1000);
                ExitDialog.Margin = new Thickness(1000);
            }
            catch { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (EditorMode.Equals("EGOgift"))
                {
                    if (EGOgift_Json_Dictionary_CurrentID != -1)
                    {
                        (string, int) ExitData = JsonLoader_EGOgifts.GetUnsavedChanges(EGOgift_EditBuffer);

                        if (ExitData.Item2 != 0)
                        {
                            OverrideCover1.Margin = new Thickness(0);
                            OverrideCover2.Margin = new Thickness(0);
                            ExitDialog.Margin = new Thickness(0);

                            e.Cancel = true;
                            UnsavedChangesTooltip_Text.Text = ExitData.Item1.Trim();
                            UnsavedChangesCount.Text = $"{ExitData.Item2}";
                        }
                    }
                }
                else if (EditorMode.Equals("Skills"))
                {
                    if (Skills_Json_Dictionary_CurrentID != -1)
                    {
                        (string, int) ExitData = JsonLoader_Skills.GetUnsavedChanges(Skills_EditBuffer);

                        if (ExitData.Item2 != 0)
                        {
                            OverrideCover1.Margin = new Thickness(0);
                            OverrideCover2.Margin = new Thickness(0);
                            ExitDialog.Margin = new Thickness(0);

                            e.Cancel = true;
                            UnsavedChangesTooltip_Text.Text = ExitData.Item1.Trim();
                            UnsavedChangesCount.Text = $"{ExitData.Item2}";
                        }
                    }
                }
                else if (EditorMode.Equals("Passives"))
                {
                    if ($"{Passives_Json_Dictionary_CurrentID}".Equals("-1"))
                    {
                        (string, int) ExitData = JsonLoader_Passives.GetUnsavedChanges(Passives_EditBuffer);

                        if (ExitData.Item2 != 0)
                        {
                            OverrideCover1.Margin = new Thickness(0);
                            OverrideCover2.Margin = new Thickness(0);
                            ExitDialog.Margin = new Thickness(0);

                            e.Cancel = true;
                            UnsavedChangesTooltip_Text.Text = ExitData.Item1.Trim();
                            UnsavedChangesCount.Text = $"{ExitData.Item2}";
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (Mainfile_Filename.StartsWith("BattleKeywords") | Mainfile_Filename.StartsWith("Bufs"))
                {
                    FormatInsertionsControl.Margin = new Thickness(0, LeftMenu_Box.ActualHeight + 12 , 0,0);
                    FormatInsertionsControl.Height = double.NaN;
                }
                else
                {
                    FormatInsertionsControl.Height = 0;
                }

                if (this.ActualWidth < 728)
                {
                    Settings.Width = 0;
                    OverrideCover1.Margin = new Thickness(1000);
                    OverrideCover2.Margin = new Thickness(1000);
                    SettingsDialog.Margin = new Thickness(1000);
                }
                else Settings.Width = 30;

                // Сворачивание бокового меню при высоте окна равной окну предпросмотра (Чистый режим)
                if (EditorMode.Equals("EGOgift"))
                {
                    this.MinWidth = 585;
                    JsonEditor.SetValue(HeightProperty, this.ActualHeight - 320);
                    if (this.Height == 320)
                    {
                        this.Height = 320.1;
                        this.MaxWidth = 588;
                    }
                    else this.MaxWidth = 877;
                }
                else if (EditorMode.Equals("Skills") | EditorMode.Equals("Passives"))
                {
                    this.MinWidth = 705;
                    JsonEditor.SetValue(HeightProperty, this.ActualHeight - 421);
                    if (this.Height == 420.8)
                    {
                        this.Height = 420.9;
                        this.MaxWidth = 702;
                    }
                    else this.MaxWidth = 992;
                }
                //rin(ActualHeight);
                NewWindowSizes.Rect = new Rect(0, 0, Width, Height);
            }
            catch (Exception ex) { rin(ex.ToString()); }
        }

        private static void BackgroundShadowTextCheck(TextBox TextBox, Label Label, string Label_DefaultText)
        {
            try
            {
                if(TextBox.Text != "") Label.Content = "";
                else Label.Content = Label_DefaultText;
            }
            catch { }
        }

        private void Check_JsonFilepath_bgtext() { try { BackgroundShadowTextCheck(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["Path to json file (After warning)"]); } catch { } }
        private void Check_JumpToID_bgtext()     { try { BackgroundShadowTextCheck(JumpToID_Input, JumpToID_bgtext, InterfaceTextContent["[Left Menu] Jump to ID (Background text)"]); } catch { } }

        private void JsonPath_TextChanged(object sender, TextChangedEventArgs e) { try { BackgroundShadowTextCheck(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["Path to json file (After warning)"]); } catch { } }
        private void Name_EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string BGText = EditorMode switch
                {
                    "EGOgift"  => InterfaceTextContent["[Left Menu] ID Item Name"],
                    "Skills"   => InterfaceTextContent["[Left Menu] ID Item Name"],
                    "Passives" => InterfaceTextContent["[Left Menu] ID Item Name"],

                    _ => InterfaceTextContent["[Left Menu] ID Item Name"],
                };
                BackgroundShadowTextCheck(Name_EditBox, Name_Label_bgtext, BGText);
            }
            catch
            {

            }
        }
        private void ABName_EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                BackgroundShadowTextCheck(ABName_EditBox, ABName_Label_bgtext, "Фоновое название ЭГО");
            }
            catch { }
        }
        private void JumpToID_Input_TextChanged(object sender, TextChangedEventArgs e) => BackgroundShadowTextCheck(JumpToID_Input, JumpToID_bgtext, InterfaceTextContent["[Left Menu] Jump to ID (Background text)"]);




        private void IDSwitch_CheckEditBufferDescs()
        {
            try
            {
                string s = InterfaceTextContent["[Left Menu] Main Description Button"];

                if (!EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                    SwitchEditorTo_Desc.Content = s + "*";
                else
                    SwitchEditorTo_Desc.Content = s;


                s = InterfaceTextContent["[Left Menu] EGO Gift Description № Button"];
                for (int i = 1; i <= 5; i++)
                {
                    if (!EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{i}"].Equals("{unedited}"))
                    {
                        T[$"EditorSwitch SubDesc {i}"].Content = s.Exform(i) + "*";
                    }
                    else
                    {
                        T[$"EditorSwitch SubDesc {i}"].Content = s.Exform(i);
                    }

                }
            }
            catch { }
        }



        private void SwitchToID(int ID)
        {
            try
            {
                EGOgift_Json_Dictionary_CurrentID = ID;
                IDSwitch_CheckEditBufferDescs();
                ID_Switch_CheckButtons();
            
                Name_Label.Text = Convert.ToString(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["Name"]);
                Name_EditBox.Text = Convert.ToString(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["Name"]);
                ID_Copy_Button.Content = EGOgift_Json_Dictionary_CurrentID;

                for (int i = 1; i <= 5; i++)
                {
                    if (!EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{i}"].Equals("{none}"))
                    {
                        T[$"EditorSwitch SubDesc {i} [UnavalibleCover]"].Height = 0;
                        T[$"SaveChanges SubDesc {i} [UnavalibleCover]"].Height = 0;
                    }
                    else
                    {
                        T[$"EditorSwitch SubDesc {i} [UnavalibleCover]"].Height = 30;
                        T[$"SaveChanges SubDesc {i} [UnavalibleCover]"].Height = 30;
                    }
                }
            
                EGOgift_CurrentEditingField = "Desc";
                if (EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                {
                    JsonEditor.Text = Convert.ToString(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["Desc"]);
                }
                else
                {
                    JsonEditor.Text = Convert.ToString(EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"]);
                }
                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                ResetUndo();
            }
            catch { }
        }


        private async void TextBoxFlashWarning(TextBox TB,
                                               Label LB,
                                               string WarningText,
                                               string LabelTextAfter,
                                               string WhatsNext,
                                               int rounds = 2,
                                               int AfterAwait = 400,
                                               int TimerAwait = 100) {
            try
            {
                TB.Focusable = false;
                TB.Foreground = GetColorFromAHEX("#FF191919"); ;

                LB.Content = WarningText;

                for (int i = 1; i <= rounds; i++)
                {
                    LB.Foreground = GetColorFromAHEX("#FFFFA4A4");
                    await Task.Delay(TimerAwait);
                    LB.Foreground = GetColorFromAHEX("#FFF43D3D");
                    await Task.Delay(TimerAwait);
                }
                await Task.Delay(AfterAwait);

                LB.Foreground = GetColorFromAHEX("#FF514C46");
                LB.Content = LabelTextAfter;
                TB.Focusable = true;
                TB.Foreground = GetColorFromAHEX("#FFA69885");

                if (WhatsNext == "Check_JumpToID_bgtext") Check_JumpToID_bgtext();
                else Check_JsonFilepath_bgtext();
            }
            catch { }
        }


        private void JsonFile_SelectFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();

                try   { dialog.InitialDirectory = $@"{Directory.GetDirectories(@"C:\Program Files (x86)\Steam\steamapps\common\Limbus Company\BepInEx\plugins")[0]}\Localize\RU"; }
                catch { dialog.InitialDirectory = ""; }
                dialog.DefaultExt = ".json";
                dialog.Filter = "Text documents (.json)|*.json";

                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    string filename = dialog.FileName;
                    LoadJsonFile(filename);
                }
            }
            catch (Exception ex)
            {
                TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["File reading warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
            }
        }

        private async void Notify(string text)
        {
            try
            {
                JsonFilepath.Focusable = false;
                JsonFilepath.Foreground = GetColorFromAHEX("#FF191919");
                JsonFilepath_bgtext.Content = text;
                JsonFilepath_bgtext.Foreground = GetColorFromAHEX("#FFCCCCCC");
                await Task.Delay(1150);
                JsonFilepath_bgtext.Content = "";
                JsonFilepath.Focusable = true;
                JsonFilepath.Foreground = GetColorFromAHEX("#FFA69885");
            }
            catch { }
        }




        /////////////////////////////////////////////////////////////
        ///  * * * Загрузка Json файлов и адаптация режима * * *  ///
        /////////////////////////////////////////////////////////////
        private void LoadJsonFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["File reading warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext");
                }
                else
                {
                    try
                    {
                        CurrentHighlight_YOffset = 0;
                        Json_Filepath = path;
                        Mainfile_Filename = Json_Filepath.Split('\\')[^1];

                        bool IsSupportedFileType = true;
                        foreach (var type in NotSupportedFileTypes)
                        {
                            if (Mainfile_Filename.StartsWith(type)) IsSupportedFileType = false;
                        }

                        if (Mainfile_Filename.StartsWith("EGOgift_"))
                        {
                            EditorMode = "EGOgift";
                            Mode_Handlers.Mode_EGO_Gifts.AdjustUI();
                            EGOgift_Json_Dictionary = JsonLoader_EGOgifts.GetJsonDictionary(Json_Filepath);
                            EGOgift_JsonKeys = EGOgift_Json_Dictionary.Keys.ToList();

                            foreach (var ID in EGOgift_JsonKeys) // Получние буфера не сохранённых изменений ЭГО даров
                            {
                                EGOgift_EditBuffer[ID] = new Dictionary<string, object>
                                {
                                    ["Name"] = "{unedited}",
                                    ["Desc"] = "{unedited}",
                                    ["SimpleDesc1"] = "{unedited}",
                                    ["SimpleDesc2"] = "{unedited}",
                                    ["SimpleDesc3"] = "{unedited}",
                                    ["SimpleDesc4"] = "{unedited}",
                                    ["SimpleDesc5"] = "{unedited}",
                                };
                            }

                            EGOgift_Json_Dictionary_CurrentID = EGOgift_JsonKeys[0];
                            SwitchToID(EGOgift_Json_Dictionary_CurrentID);

                            ABName_Change_StackPanel.Height = 0;
                            ABName_Input_StackPanel.Height = 0;
                            Name_ChangeNameInput_Cover.Height = 0;
                            Name_ChangeName_Cover.Height = 0;

                            Desc_Cover.Height = 0;
                            Desc_Change_Cover.Height = 0;

                            SubDesc1_Cover.Height = 0;
                            SubDesc1_Change_Cover.Height = 0;

                            JsonFilepath.Text = path;
                        }

                        else if (Mainfile_Filename.StartsWith("Skills"))
                        {
                            EditorMode = "Skills";
                            // Основной словарь с текстом из JsonData.dataList и Буфер не сохранённых изменений
                            (Skills_Json_Dictionary, Skills_EditBuffer) = JsonLoader_Skills.GetJsonDictionary(Json_Filepath);
                            Skills_JsonKeys = Skills_Json_Dictionary.Keys.ToList();

                            // По умолчанию переключиться на самый первый ID в файле
                            Skills_Json_Dictionary_CurrentID = Skills_JsonKeys[0];

                            foreach (var MinUptieLevel in Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID])
                            {
                                Skills_Json_Dictionary_CurrentUptieLevel = MinUptieLevel.Key;
                                break;
                            }

                            // Адаптировать интерфейс под навыки
                            T["Name EditBox [UnavalibleCover]"].Height = 0; // Разблокировать кнопку имени
                            T["Name SaveChanges [UnavalibleCover]"].Height = 0;
                            T["EditorSwitch Desc [UnavalibleCover]"].Height = 0; // Разблокировать кнопку описания

                            // Навыки грешников
                            if (Mainfile_Filename.StartsWith("Skills_Ego_Personality-") | Mainfile_Filename.Equals("Skills_Ego.json"))
                            {
                                SaveChangesButtons.Height = 270;
                                SaveChangesButtons.Margin = new Thickness(236, -270, 0, 0);
                                ABName_Change_StackPanel.Height = 33;
                                ABName_Input_StackPanel.Height = 33;
                                CurrentHighlight_YOffset = 33;
                                Mode_Handlers.Mode_Skills.AdjustUI(IsEGO: true);
                                //Name_Label_bgtext.Content = "Название ЭГО";
                            }
                            else if (Mainfile_Filename.StartsWith("Skills_personality-") | Mainfile_Filename.Equals("Skills.json"))
                            {
                                SaveChangesButtons.Height = 237;
                                SaveChangesButtons.Margin = new Thickness(236, -237, 0, 0);
                                ABName_Change_StackPanel.Height = 0;
                                ABName_Input_StackPanel.Height = 0;
                                CurrentHighlight_YOffset = 0;
                                Mode_Handlers.Mode_Skills.AdjustUI(IsEGO: false);
                                //Name_Label_bgtext.Content = "Название навыка";
                            }

                            // Все остальные
                            else
                            {
                                SaveChangesButtons.Height = 237;
                                SaveChangesButtons.Margin = new Thickness(236, -237, 0, 0);
                                ABName_Change_StackPanel.Height = 0;
                                ABName_Input_StackPanel.Height = 0;
                                CurrentHighlight_YOffset = 0;
                                Mode_Handlers.Mode_Skills.AdjustUI(IsEGO: false, IsEnemies: true);
                                //Name_Label_bgtext.Content = "Название навыка";
                            }


                            ID_SwitchNext_Cover.Height = 0;

                            Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID); // Взять самый первый ID из списка

                            JsonFilepath.Text = path;
                        }

                        else if (Mainfile_Filename.StartsWith("Passive") | Mainfile_Filename.StartsWith("Bufs") | Mainfile_Filename.StartsWith("BattleKeywords"))
                        {

                            EditorMode = "Passives";
                            (Passives_Json_Dictionary, Passives_EditBuffer) = JsonLoader_Passives.GetJsonDictionary(Json_Filepath);
                            Mode_Passives.AdjustUI();

                            Passives_JsonKeys = Passives_Json_Dictionary.Keys.ToList();


                            Passives_Json_Dictionary_CurrentID = Passives_JsonKeys[0];

                            ABName_Change_StackPanel.Height = 0;
                            ABName_Input_StackPanel.Height = 0;
                            ID_Copy_Button.Content = $"{Passives_Json_Dictionary_CurrentID}";
                            Name_EditBox.Text = $"{Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Name"]}";
                            JsonEditor.Text = $"{Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Desc"]}";
                            Name_Label.Text = $"{Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Name"]}";
                            if (!Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Summary"].Equals("{none}"))
                            {
                                T["SaveChanges SubDesc 1 [UnavalibleCover]"].Height = 0;
                                T["EditorSwitch SubDesc 1 [UnavalibleCover]"].Height = 0;
                            }
                            T["Skill PreviewLayout Desc"].Height = Double.NaN;
                            ID_SwitchNext_Cover.Height = 0;

                            JsonFilepath.Text = path;
                        }
                        else if (!IsSupportedFileType)
                        {
                            TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["Unsopported file warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext");
                        }

                        else
                        {
                            TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["Unsopported file warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext");
                        }
                    }
                    catch (Exception ex)
                    {
                        JsonFilepath.Text = "";
                        TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["File reading warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext");
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine(ex.Source);
                        Console.WriteLine(ex.Message);
                    }
                }
                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                ID_Switch_CheckButtons();
            }
            catch { }
        }


        /// <summary>
        /// Проверка доступности следующего ID из списка. Если ID последний в списке и не удалось найти следующий, отключить кнопку переключения по порядку
        /// </summary>
        private void ID_Switch_CheckButtons()
        {
            try
            {
                ID_SwitchPrev_Cover.Height = 0;
                ID_SwitchNext_Cover.Height = 0;
                try
                {
                    if (EditorMode == "EGOgift")
                    {
                        var PrevCheck = EGOgift_JsonKeys[EGOgift_JsonKeys.IndexOf(EGOgift_Json_Dictionary_CurrentID) - 1];
                    }
                    else if (EditorMode == "Skills")
                    {
                        var PrevCheck = Skills_JsonKeys[Skills_JsonKeys.IndexOf(Skills_Json_Dictionary_CurrentID) - 1];
                    }
                    else if (EditorMode == "Passives")
                    {
                        var PrevCheck = Passives_JsonKeys[Passives_JsonKeys.IndexOf(Passives_Json_Dictionary_CurrentID) - 1];
                    }
                }
                catch{ID_SwitchPrev_Cover.Height = 16;}

                try
                {
                    if (EditorMode == "EGOgift")
                    {
                        var PrevCheck = EGOgift_JsonKeys[EGOgift_JsonKeys.IndexOf(EGOgift_Json_Dictionary_CurrentID) + 1];
                    }
                    else if (EditorMode == "Skills")
                    {
                        var PrevCheck = Skills_JsonKeys[Skills_JsonKeys.IndexOf(Skills_Json_Dictionary_CurrentID) + 1];
                    }
                    else if (EditorMode == "Passives")
                    {
                        var PrevCheck = Passives_JsonKeys[Passives_JsonKeys.IndexOf(Passives_Json_Dictionary_CurrentID) + 1];
                    }
                }
                catch{ID_SwitchNext_Cover.Height = 16;}
            }
            catch { }
        }
        
        private void ID_SwitchPrev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EditorMode == "EGOgift")
                {
                    EGOgift_Json_Dictionary_CurrentID = EGOgift_JsonKeys[EGOgift_JsonKeys.IndexOf(EGOgift_Json_Dictionary_CurrentID) - 1];
                    SwitchToID(EGOgift_Json_Dictionary_CurrentID);
                }
                else if (EditorMode == "Skills")
                {
                    Skills_Json_Dictionary_CurrentID = Skills_JsonKeys[Skills_JsonKeys.IndexOf(Skills_Json_Dictionary_CurrentID) - 1];
                    Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID);
                    ResetUndo();
                }
                else if (EditorMode == "Passives")
                {
                    Passives_CurrentEditingField = "Desc";
                    Passives_Json_Dictionary_CurrentID = Passives_JsonKeys[Passives_JsonKeys.IndexOf(Passives_Json_Dictionary_CurrentID) - 1];
                    //rin($"Switching to {Passives_Json_Dictionary_CurrentID}");
                    Mode_Handlers.Mode_Passives.UpdateMenuInfo(Passives_Json_Dictionary_CurrentID);
                    ResetUndo();
                }

                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                Mode_Skills.ReEnableAvalibleCoinDescs(Disable: true);
                ID_Switch_CheckButtons();
            }
            catch { }
        }
        private void ID_SwitchNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EditorMode == "EGOgift")
                {
                    EGOgift_Json_Dictionary_CurrentID = EGOgift_JsonKeys[EGOgift_JsonKeys.IndexOf(EGOgift_Json_Dictionary_CurrentID) + 1];
                    SwitchToID(EGOgift_Json_Dictionary_CurrentID);
                }
                else if (EditorMode == "Skills")
                {
                    Skills_Json_Dictionary_CurrentID = Skills_JsonKeys[Skills_JsonKeys.IndexOf(Skills_Json_Dictionary_CurrentID) + 1];
                    Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID);
                    ResetUndo();
                }
                else if (EditorMode == "Passives")
                {
                    Passives_CurrentEditingField = "Desc";
                    Passives_Json_Dictionary_CurrentID = Passives_JsonKeys[Passives_JsonKeys.IndexOf(Passives_Json_Dictionary_CurrentID) + 1];
                    Mode_Handlers.Mode_Passives.UpdateMenuInfo(Passives_Json_Dictionary_CurrentID);
                    ResetUndo();
                }

                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                Mode_Skills.ReEnableAvalibleCoinDescs(Disable: true);
                ID_Switch_CheckButtons();
            }
            catch { }
        }

        private void ID_SwitchPrev_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (EditorMode == "EGOgift")
                {
                    EGOgift_Json_Dictionary_CurrentID = EGOgift_JsonKeys[0];
                    SwitchToID(EGOgift_Json_Dictionary_CurrentID);
                }
                else if (EditorMode == "Skills")
                {
                    Skills_Json_Dictionary_CurrentID = Skills_JsonKeys[0];
                    Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID);
                    ResetUndo();
                }
                else if (EditorMode == "Passives")
                {
                    Passives_Json_Dictionary_CurrentID = Passives_JsonKeys[0];
                    Mode_Handlers.Mode_Passives.UpdateMenuInfo(Passives_Json_Dictionary_CurrentID);
                    ResetUndo();
                }

                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                Mode_Skills.ReEnableAvalibleCoinDescs(Disable: true);
                ID_Switch_CheckButtons();
            }
            catch { }
        }

        private void ID_SwitchNext_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (EditorMode == "EGOgift")
                {
                    EGOgift_Json_Dictionary_CurrentID = EGOgift_JsonKeys[^1];
                    SwitchToID(EGOgift_Json_Dictionary_CurrentID);
                }
                else if (EditorMode == "Skills")
                {
                    Skills_Json_Dictionary_CurrentID = Skills_JsonKeys[^1];
                    Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID);
                    ResetUndo();
                }
                else if (EditorMode == "Passives")
                {
                    Passives_Json_Dictionary_CurrentID = Passives_JsonKeys[^1];
                    Mode_Handlers.Mode_Passives.UpdateMenuInfo(Passives_Json_Dictionary_CurrentID);
                    ResetUndo();
                }

                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                Mode_Skills.ReEnableAvalibleCoinDescs(Disable: true);
                ID_Switch_CheckButtons();
            }
            catch { }
        }



        private void CheckEditBuffer(string CurrentDesc)
        {
            try{
                if (EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][CurrentDesc].Equals("{unedited}"))
                {
                    JsonEditor.Text = Convert.ToString(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][CurrentDesc]);
                }
                else
                {
                    JsonEditor.Text = Convert.ToString(EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][CurrentDesc]);
                }
            }catch{}
        }












        private void ResetUndo()
        {
            JsonEditor.IsUndoEnabled = false;
            JsonEditor.IsUndoEnabled = true;
        }


        private void SwitchEditorTo_Desc_Button(object sender, RoutedEventArgs e)       
        {
            try
            {
                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                try{
                    if (EditorMode.Equals("EGOgift"))
                    {
                        EGOgift_CurrentEditingField = "Desc";
                        CheckEditBuffer("Desc");
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        Skills_CurrentEditingField = "Desc";

                        if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"].Equals("{unedited}"))
                        {
                            JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"];
                        }
                        else
                        {
                            JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"];
                        }

                        Mode_Skills.ReEnableAvalibleCoinDescs(Disable: true);

                    }
                    else if (EditorMode.Equals("Passives"))
                    {
                        Passives_CurrentEditingField = "Desc";
                        if (Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                        {
                            JsonEditor.Text = $"{Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Desc"]}";
                        }
                        else
                        {
                            JsonEditor.Text = $"{Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"]}";
                        }
                    }
                }
                catch{}
                ResetUndo();
            }
            catch { }
        }




        private void SwitchEditorTo_SubDesc1_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 95);
                try
                {
                    if (EditorMode.Equals("EGOgift"))
                    {
                        EGOgift_CurrentEditingField = "SimpleDesc1";
                        CheckEditBuffer("SimpleDesc1");
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        Skills_CurrentCoinNumber = 1;
                        int DescsCount = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][1].Count;

                        List<int> EmptydescExceptions = new();
                        int descCounter = 1;
                        foreach (string i in Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][1])
                        {
                            rin(i);
                            if (i.Equals("{empty}"))
                            {
                                EmptydescExceptions.Add(descCounter);
                            }
                            descCounter++;
                        }

                        Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount, EmptydescExceptions: EmptydescExceptions);
                        Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount, EmptydescExceptions: EmptydescExceptions);

                        Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{0}";

                        if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0].Equals("{unedited}"))
                        {
                            JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                        else
                        {
                            JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                    }
                    else if (EditorMode.Equals("Passives"))
                    {
                        Passives_CurrentEditingField = "Summary";
                        if (Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"].Equals("{unedited}"))
                        {
                            JsonEditor.Text = $"{Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Summary"]}";
                        }
                        else
                        {
                            JsonEditor.Text = $"{Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"]}";
                        }
                    }
                }
                catch { }
                ResetUndo();
            }
            catch { }
        }
        private void SwitchEditorTo_SubDesc2_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 129);
                try{
                    if (EditorMode.Equals("EGOgift"))
                    {
                        EGOgift_CurrentEditingField = "SimpleDesc2";
                        CheckEditBuffer("SimpleDesc2");
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        Skills_CurrentCoinNumber = 2;
                        int DescsCount = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][2].Count;

                        List<int> EmptydescExceptions = new();
                        int descCounter = 1;
                        foreach (string i in Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][2])
                        {
                            if (i.Equals("{empty}"))
                            {
                                EmptydescExceptions.Add(descCounter);
                            }
                            descCounter++;
                        }

                        Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount, EmptydescExceptions: EmptydescExceptions);
                        Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount, EmptydescExceptions: EmptydescExceptions);

                        Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{0}";

                        if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0].Equals("{unedited}"))
                        {
                            JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                        else
                        {
                            JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                    }
                }catch{}
                ResetUndo();
            }
            catch { }
        }
        private void SwitchEditorTo_SubDesc3_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 163);
                try{
                    if (EditorMode.Equals("EGOgift"))
                    {
                        EGOgift_CurrentEditingField = "SimpleDesc3";
                        CheckEditBuffer("SimpleDesc3");
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        Skills_CurrentCoinNumber = 3;
                        int DescsCount = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][3].Count;

                        List<int> EmptydescExceptions = new();
                        int descCounter = 1;
                        foreach (string i in Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][3])
                        {
                            if (i.Equals("{empty}"))
                            {
                                EmptydescExceptions.Add(descCounter);
                            }
                            descCounter++;
                        }

                        Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount, EmptydescExceptions: EmptydescExceptions);
                        Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount, EmptydescExceptions: EmptydescExceptions);

                        Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{0}";

                        if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0].Equals("{unedited}"))
                        {
                            JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                        else
                        {
                            JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                    }
                }catch{}
                ResetUndo();
            }
            catch { }
        }
        private void SwitchEditorTo_SubDesc4_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 197);
                try{
                    if (EditorMode.Equals("EGOgift"))
                    {
                        EGOgift_CurrentEditingField = "SimpleDesc4";
                        CheckEditBuffer("SimpleDesc4");
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        Skills_CurrentCoinNumber = 4;
                        int DescsCount = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][4].Count;

                        List<int> EmptydescExceptions = new();
                        int descCounter = 1;
                        foreach (string i in Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][4])
                        {
                            if (i.Equals("{empty}"))
                            {
                                EmptydescExceptions.Add(descCounter);
                            }
                            descCounter++;
                        }

                        Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount, EmptydescExceptions: EmptydescExceptions);
                        Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount, EmptydescExceptions: EmptydescExceptions);

                        Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{0}";

                        if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0].Equals("{unedited}"))
                        {
                            JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                        else
                        {
                            JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                    }
                }catch{}
                ResetUndo();
            }
            catch { }
        }
        private void SwitchEditorTo_SubDesc5_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                try{
                    if (EditorMode.Equals("EGOgift"))
                    {
                        EGOgift_CurrentEditingField = "SimpleDesc5";
                        CheckEditBuffer("SimpleDesc5");
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        Skills_CurrentCoinNumber = 5;
                        int DescsCount = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][5].Count;

                        List<int> EmptydescExceptions = new();
                        int descCounter = 1;
                        foreach (string i in Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][5])
                        {
                            if (i.Equals("{empty}"))
                            {
                                EmptydescExceptions.Add(descCounter);
                            }
                            descCounter++;
                        }

                        Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount, EmptydescExceptions: EmptydescExceptions);
                        Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount, EmptydescExceptions: EmptydescExceptions);

                        Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{0}";

                        if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0].Equals("{unedited}"))
                        {
                            JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                        else
                        {
                            JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                        }
                    }
                }catch{}
                ResetUndo();
            }
            catch { }
        }















        private void JumpToID_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    if (EditorMode == "EGOgift")
                    {
                        SwitchToID(Convert.ToInt32(JumpToID_Input.Text));
                        Check_JsonFilepath_bgtext();
                    }
                    else if (EditorMode == "Skills")
                    {
                        Skills_Json_Dictionary_CurrentID = Convert.ToInt32(JumpToID_Input.Text);
                        Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID);
                    }
                    else if (EditorMode == "Passives")
                    {                    
                        if (Passives_JsonKeys.Contains(JumpToID_Input.Text))
                        {
                            Passives_CurrentEditingField = "Desc";
                            Passives_Json_Dictionary_CurrentID = JumpToID_Input.Text;
                            Mode_Handlers.Mode_Passives.UpdateMenuInfo(Passives_Json_Dictionary_CurrentID);
                            ResetUndo();
                        }
                        else if (Passives_JsonKeys.Contains(Convert.ToInt32(JumpToID_Input.Text)))
                        {
                            try
                            {
                                Passives_CurrentEditingField = "Desc";
                                Passives_Json_Dictionary_CurrentID = Convert.ToInt32(JumpToID_Input.Text);
                                Mode_Handlers.Mode_Passives.UpdateMenuInfo(Passives_Json_Dictionary_CurrentID);
                                ResetUndo();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                                Console.WriteLine(ex.Source);
                                Console.WriteLine(ex.Message);
                                throw new Exception();
                            }
                        }
                        else throw new Exception();
                    }
                    JumpToID_Input.Text = "";
                    ID_Switch_CheckButtons();
                }
                catch
                {
                    TextBoxFlashWarning(JumpToID_Input, JumpToID_bgtext, InterfaceTextContent["ID not found warning"], InterfaceTextContent["[Left Menu] Jump to ID (Background text)"], "Check_JumpToID_bgtext");
                }
            }
            catch { }
        }

        private async void ID_Copy(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EGOgift_Json_Dictionary_CurrentID != -1 | Skills_Json_Dictionary_CurrentID != -1 | !Convert.ToString(Passives_Json_Dictionary_CurrentID).Equals("-1"))
                {
                    string MemID = $"{ID_Copy_Button.Content}";
                    Clipboard.SetText(Convert.ToString(ID_Copy_Button.Content));
                    ID_Copy_Button.Foreground = GetColorFromAHEX("#00FFFFFF");
                    IDCopiedNotify.Foreground = GetColorFromAHEX("#FF7C746B");

                    await Task.Delay(810);
                    ID_Copy_Button.Foreground = GetColorFromAHEX("#FF7C746B");
                    IDCopiedNotify.Foreground = GetColorFromAHEX("#00FFFFFF");
                }
            }
            catch { }
        }


        public static Dictionary<int, Image> UptieLevelIcons = new()
        {
            [0] = new Image(),

            [1] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/1 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
            [2] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/2 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
            [3] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/3 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
            [4] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/4 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
        };

        private void UptieLevel1_Button_MouseEnter(object sender, MouseEventArgs e) { try { UptieLevel1_Button.Content = UptieLevelIcons[1]; } catch { } }
        private void UptieLevel1_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (Skills_Json_Dictionary_CurrentUptieLevel != 1) UptieLevel1_Button.Content = UptieLevelIcons[0];
            }
            catch { }
        }

        private void UptieLevel2_Button_MouseEnter(object sender, MouseEventArgs e) { try { UptieLevel2_Button.Content = UptieLevelIcons[2]; } catch { } }
        private void UptieLevel2_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (Skills_Json_Dictionary_CurrentUptieLevel != 2) UptieLevel2_Button.Content = UptieLevelIcons[0];
            }
            catch { }
        }

        private void UptieLevel3_Button_MouseEnter(object sender, MouseEventArgs e) { try { UptieLevel3_Button.Content = UptieLevelIcons[3]; } catch { } }
        private void UptieLevel3_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (Skills_Json_Dictionary_CurrentUptieLevel != 3) UptieLevel3_Button.Content = UptieLevelIcons[0];
            }
            catch { }
        }

        private void UptieLevel4_Button_MouseEnter(object sender, MouseEventArgs e) { try { UptieLevel4_Button.Content = UptieLevelIcons[4]; } catch { } }
        private void UptieLevel4_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (Skills_Json_Dictionary_CurrentUptieLevel != 4) UptieLevel4_Button.Content = UptieLevelIcons[0];
            }
            catch { }
        }

        private void UptieLevel1_Button_Click(object sender, RoutedEventArgs e) { try { Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 1); } catch { } }
        private void UptieLevel2_Button_Click(object sender, RoutedEventArgs e) { try { Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 2); } catch { } }
        private void UptieLevel3_Button_Click(object sender, RoutedEventArgs e) { try { Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 3); } catch { } }
        private void UptieLevel4_Button_Click(object sender, RoutedEventArgs e) { try { Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 4); } catch { } }

        private void CoinDescs_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{0}";
                Mode_Skills.SetCurrentCoinDescHighlight(0, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][0];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{1}";
                Mode_Skills.SetCurrentCoinDescHighlight(1, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][1].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][1];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][1];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{2}";
                Mode_Skills.SetCurrentCoinDescHighlight(2, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][2].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][2];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][2];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_4(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{3}";
                Mode_Skills.SetCurrentCoinDescHighlight(3, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][3].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][3];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][3];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_5(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{4}";
                Mode_Skills.SetCurrentCoinDescHighlight(4, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][4].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][4];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][4];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_6(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{5}";
                Mode_Skills.SetCurrentCoinDescHighlight(5, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][5].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][5];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][5];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_7(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{6}";
                Mode_Skills.SetCurrentCoinDescHighlight(6, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][6].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][6];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][6];
                }
                ResetUndo();
            }
            catch (Exception ex) { rin(ex.ToString()); }
        }
        private void CoinDescs_8(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{7}";
                Mode_Skills.SetCurrentCoinDescHighlight(7, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][7].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][7];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][7];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_9(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{8}";
                Mode_Skills.SetCurrentCoinDescHighlight(8, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][8].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][8];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][8];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_10(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{9}";
                Mode_Skills.SetCurrentCoinDescHighlight(9, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][9].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][9];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][9];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_11(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{10}";
                Mode_Skills.SetCurrentCoinDescHighlight(10, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][10].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][10];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][10];
                }
                ResetUndo();
            }
            catch { }
        }
        private void CoinDescs_12(object sender, RoutedEventArgs e)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{11}";
                Mode_Skills.SetCurrentCoinDescHighlight(11, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][11].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][11];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][11];
                }
                ResetUndo();
            }
            catch { }
        }

        private void CoinDescs_SwitchOver(int CoinDescIndex)
        {
            try
            {
                Skills_CurrentEditingField = $"Coin {Skills_CurrentCoinNumber} Decs ind.{CoinDescIndex}";
                Mode_Skills.SetCurrentCoinDescHighlight(CoinDescIndex, Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber].Count);
                if (Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex].Equals("{unedited}"))
                {
                    JsonEditor.Text = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex];
                }
                else
                {
                    JsonEditor.Text = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex];
                }
                ResetUndo();
            }
            catch { }
        }




        private void ABName_ChangeName(object sender, RoutedEventArgs e)
        {
            try
            {
                int JSON_IndexOf_ID = JsonLoader_Skills.ID_AND_INDEX[Skills_Json_Dictionary_CurrentID];
                int JSON_IndexOf_UptieLevel = JsonLoader_Skills.UPTIELEVEL_AND_INDEX[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel];

                SetRW(Json_Filepath);
                JsonLoader_Skills.JSON.dataList[JSON_IndexOf_ID].levelList[JSON_IndexOf_UptieLevel].abName = ABName_EditBox.Text.Replace("\r", "");
                SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
                SetRO(Json_Filepath);
                Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["ABName"] = ABName_EditBox.Text;

                Notify("Фоновое имя обновлено");
            }
            catch { }
        }
        private void Name_ChangeName(object sender, RoutedEventArgs e)
        {
            try
            {
                try{
                    if (EditorMode.Equals("EGOgift"))
                    {
                        SetRW(Json_Filepath);
                        RewriteFileLine($"\"name\": \"{Name_EditBox.Text.Replace("\r", "").Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",",
                                        Json_Filepath,
                                        Convert.ToInt32(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["LineIndex_Name"]));
                        SetRO(Json_Filepath);
                        Name_Label.Text = Name_EditBox.Text;
                        EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["Name"] = Name_EditBox.Text.Replace("\r", "");
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        int JSON_IndexOf_ID = JsonLoader_Skills.ID_AND_INDEX[Skills_Json_Dictionary_CurrentID];
                        int JSON_IndexOf_UptieLevel = JsonLoader_Skills.UPTIELEVEL_AND_INDEX[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel];
                    
                        SetRW(Json_Filepath);
                        JsonLoader_Skills.JSON.dataList[JSON_IndexOf_ID].levelList[JSON_IndexOf_UptieLevel].name = Name_EditBox.Text;
                        SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
                        SetRO(Json_Filepath);
                        Name_Label.Text = Name_EditBox.Text;
                        Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Name"] = Name_EditBox.Text;
                    }
                    else if (EditorMode.Equals("Passives"))
                    {
                        int LineToRewrite = Convert.ToInt32(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["LineIndex_Name"]);
                        string NewLine = $"\"name\": \"{Name_EditBox.Text.Replace("\r", "").Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",";
                        //rin($"Rewriting line {LineToRewrite} with:\n{NewLine}\n");
                        SetRW(Json_Filepath);
                        RewriteFileLine(NewLine, Json_Filepath, LineToRewrite);
                        Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Name"] = Name_EditBox.Text;
                        Name_Label.Text = Name_EditBox.Text;
                        SetRO(Json_Filepath);
                    }
                    Notify("Имя обновлено");
                } catch(Exception ex) {
                    TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["Saving error warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.Message);
                }
            }
            catch { }
        }

        private void Desc_ChangeOver(string ThisDesc)
        {
            try
            {

                try
                {
                    if (EditorMode.Equals("EGOgift"))
                    {
                        if (!EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc].Equals("{unedited}"))
                        {
                            //rin($"Saving: \"{EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc]}\"");
                            SetRW(Json_Filepath);
                            RewriteFileLine($"{(ThisDesc.StartsWith("SimpleDesc") ? "\"simpleDesc\": \"" : "\"desc\": \"")}" +
                                                         $"{Convert.ToString(EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc]).Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}" +
                                                         $"{(ThisDesc.StartsWith("SimpleDesc") ? "\"" : "\",")}",
                                                         Json_Filepath,
                                                         Convert.ToInt32(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][$"LineIndex_{ThisDesc}"]));

                            EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][ThisDesc] = EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc];
                            EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc] = "{unedited}";
                            IDSwitch_CheckEditBufferDescs();
                        
                            SetRO(Json_Filepath);
                        }
                    }
                    else if (EditorMode.Equals("Skills"))
                    {
                        int JSON_IndexOf_ID = JsonLoader_Skills.ID_AND_INDEX[Skills_Json_Dictionary_CurrentID];
                        int JSON_IndexOf_UptieLevel = JsonLoader_Skills.UPTIELEVEL_AND_INDEX[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel];
                    
                        switch (ThisDesc)
                        {
                            case "Desc":
                                if (!Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"].Equals("{unedited}"))
                                {
                                    Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"] = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"];
                                    Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"] = "{unedited}";
                                

                                    SetRW(Json_Filepath);
                                    try
                                    {
                                        JsonLoader_Skills.JSON.dataList[JSON_IndexOf_ID].levelList[JSON_IndexOf_UptieLevel].desc = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"].Replace("\\n", "\n");
                                        SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
                                        T["EditorSwitch Desc"].Content = InterfaceTextContent["[Left Menu] Main Description Button"];

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        Console.WriteLine(ex.StackTrace);
                                        Console.WriteLine(ex.Source);
                                    }
                                    SetRO(Json_Filepath);
                                }

                                break;

                            default:
                                int CoinDescIndex = int.Parse($"{ThisDesc[^1]}");

                                if (!Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex].Equals("{unedited}"))
                                {
                                    try
                                    {
                                        Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex] = Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex];
                                        Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex] = "{unedited}";
                                        int JSON_IndexOf_Coin = JsonLoader_Skills.COIN_AND_INDEX[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel][Skills_CurrentCoinNumber];
                                        JsonLoader_Skills.JSON.dataList[JSON_IndexOf_ID].levelList[JSON_IndexOf_UptieLevel].coinlist[JSON_IndexOf_Coin].coindescs[CoinDescIndex].desc = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][CoinDescIndex];


                                        SetRW(Json_Filepath);
                                        SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
                                        T[$"Coin Descs {CoinDescIndex + 1} Button"].Content = $"№{CoinDescIndex + 1}";
                                        SetRO(Json_Filepath);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        Console.WriteLine(ex.StackTrace);
                                        Console.WriteLine(ex.Source);
                                    }
                                }
                                break;
                        }
                    }
                    else if (EditorMode.Equals("Passives"))
                    {
                        int LineToRewrite = -1;
                        string NewLine = "";
                        SetRW(Json_Filepath);
                        switch (Passives_CurrentEditingField)
                        {
                            case "Desc":
                                if (!Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                                {
                                    LineToRewrite = Convert.ToInt32(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["LineIndex_Desc"]);
                                    NewLine = $"\"desc\": \"{Convert.ToString(Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"]).Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",";
                                    //rin($"Rewriting line {LineToRewrite} with:\n{NewLine}\n");
                                    RewriteFileLine(NewLine, Json_Filepath, LineToRewrite);

                                    Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Desc"] = Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"];
                                    Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                                    T["EditorSwitch Desc"].Content = InterfaceTextContent["[Left Menu] Main Description Button"];
                                }
                                break;

                            case "Summary":
                                if (!Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"].Equals("{unedited}"))
                                {
                                    LineToRewrite = Convert.ToInt32(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["LineIndex_Summary"]);
                                    NewLine = $"\"summary\": \"{Convert.ToString(Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"]).Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",";
                                    //rin($"Rewriting line {LineToRewrite} with:\n{NewLine}\n");
                                    RewriteFileLine(NewLine, Json_Filepath, LineToRewrite);
                                    try
                                    {
                                        rin(InterfaceTextContent["[Left Menu] Passive Summary Description"]);
                                    }
                                    catch (Exception ex)
                                    {
                                        rin(ex.ToString());
                                    }
                                    Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Summary"] = Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"];
                                    Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"] = "{unedited}";
                                    T["EditorSwitch SubDesc 1"].Content = InterfaceTextContent["[Left Menu] Passive Summary Description"];
                                }
                                break;
                        }
                    
                        SetRO(Json_Filepath);
                    }
                }
                catch (KeyNotFoundException)
                {

                }

                catch(Exception ex) {
                    TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["Saving error warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.Message);
                }
            }
            catch { }
        }


        private void Desc_ChangeDesc(object sender, RoutedEventArgs e)     { try { Desc_ChangeOver("Desc");        } catch { } }
        private void Desc_ChangeSubDesc1(object sender, RoutedEventArgs e) { try { Desc_ChangeOver("SimpleDesc1"); } catch { } }
        private void Desc_ChangeSubDesc2(object sender, RoutedEventArgs e) { try { Desc_ChangeOver("SimpleDesc2"); } catch { } }
        private void Desc_ChangeSubDesc3(object sender, RoutedEventArgs e) { try { Desc_ChangeOver("SimpleDesc3"); } catch { } }
        private void Desc_ChangeSubDesc4(object sender, RoutedEventArgs e) { try { Desc_ChangeOver("SimpleDesc4"); } catch { } }
        private void Desc_ChangeSubDesc5(object sender, RoutedEventArgs e) { try { Desc_ChangeOver("SimpleDesc5"); } catch { } }








        // Сохранение на CTRL + S
        private bool isCtrlSPressed = false;

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (e.Key == Key.S && !isCtrlSPressed)
                    {
                        isCtrlSPressed = true;
                        if (EditorMode.Equals("EGOgift"))
                        {
                            Desc_ChangeOver(EGOgift_CurrentEditingField);
                        }
                        else if (EditorMode.Equals("Skills"))
                        {
                            Desc_ChangeOver(Skills_CurrentEditingField);
                        }
                        else if (EditorMode.Equals("Passives"))
                        {
                            Desc_ChangeOver(Passives_CurrentEditingField);
                        
                        }
                    }

                }
                else if (e.Key == Key.Left | e.Key == Key.Right)
                {
                    if (!ABName_EditBox.IsFocused &
                        !Name_EditBox.IsFocused &
                        !JsonEditor.IsFocused &
                        !JumpToID_Input.IsFocused &
                        !FormatInsertion_1_Text.IsFocused & 
                        !FormatInsertion_2_Text.IsFocused & 
                        !FormatInsertion_3_Text.IsFocused & 
                        !FormatInsertion_4_Text.IsFocused & 
                        !FormatInsertion_5_Text.IsFocused & 
                        !FormatInsertion_6_Text.IsFocused
                       )
                    {
                        if (e.Key == Key.Right)
                        {
                            ID_SwitchNext_Click(null, new RoutedEventArgs());
                        }
                        else if (e.Key == Key.Left)
                        {
                            ID_SwitchPrev_Click(null, new RoutedEventArgs());
                        }
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    if (ABName_EditBox.IsFocused) UnfocusTB(ABName_EditBox);
                    if (Name_EditBox.IsFocused  ) UnfocusTB(ABName_EditBox);
                    if (JumpToID_Input.IsFocused) UnfocusTB(ABName_EditBox);
                    if (JsonEditor.IsFocused    ) UnfocusTB(ABName_EditBox);

                    if (SettingsDialog.Margin == new Thickness(0))
                    {
                        OverrideCover1.Margin = new Thickness(1000);
                        OverrideCover2.Margin = new Thickness(1000);
                        SettingsDialog.Margin = new Thickness(1000);
                    }
                }
            }
            catch { }
        }

        private void UnfocusTB(TextBox tb)
        {
            try
            {
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(tb), null);
                Keyboard.ClearFocus();
                this.Focus();
            }
            catch { }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.S) isCtrlSPressed = false;
            }
            catch { }
            
        }



        // Прокрутка предпросмотра навыка на перетягивание через ЛКМ господи наконец-то
        private bool isDragging = false;
        private Point lastMousePosition;
        private void SurfaceScroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                isDragging = true;
                lastMousePosition = e.GetPosition(PreviewLayout_Skills);
                PreviewLayout_Skills.CaptureMouse();
            }
            catch { }
        }

        private void SurfaceScroll_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (isDragging)
                {
                    Point currentPosition = e.GetPosition(PreviewLayout_Skills);
                    Vector diff = lastMousePosition - currentPosition;
                    PreviewLayout_Skills.ScrollToVerticalOffset(PreviewLayout_Skills.VerticalOffset + diff.Y);
                    PreviewLayout_Skills.ScrollToHorizontalOffset(PreviewLayout_Skills.HorizontalOffset + diff.X);
                    lastMousePosition = currentPosition;
                }
            }
            catch { }
        }

        private void SurfaceScroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                isDragging = false;
                PreviewLayout_Skills.ReleaseMouseCapture();
            }
            catch { }
        }



        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowState = WindowState.Minimized;
            }
            catch { }
        }

        private void WindowMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else WindowState = WindowState.Maximized;
            }
            catch { }
        }

        
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cancelEventArgs = new System.ComponentModel.CancelEventArgs();
                Window_Closing(this, cancelEventArgs);

                if (!cancelEventArgs.Cancel) this.Close();
            }
            catch { }
        }


        private void Refractor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Refractor1.Width == 0 & Refractor2.Width == 0)
                {
                    Refractor1.Width = 120;
                    Refractor2.Width = 120;
                }
                else
                {
                    Refractor1.Width = 0;
                    Refractor2.Width = 0;
                }
            }
            catch { }
        }
        private void Refractor1_Click_LMB(object sender, RoutedEventArgs e)
        {
            try
            {
                string ReplaceSquareLinks = Regex.Replace(JsonEditor.Text, @"\[(.*?)\]", match =>
                {
                    string KeywordID = match.Groups[1].Value;

                    return Keywords.ContainsKey(KeywordID) ? $"<sprite name=\"{KeywordID}\"><color={ColorPairs[KeywordID]}><u><link=\"{KeywordID}\">{Keywords[KeywordID]}</link></u></color>" : $"[{KeywordID}]";
                });

                JsonEditor.Text = ReplaceSquareLinks;
                Refractor1.Width = 0;
                Refractor2.Width = 0;
            }
            catch { }
        }
        private void Refractor1_Click_RMB(object sender, RoutedEventArgs e)
        {
            try
            {
                string ReplaceSquareLinks = Regex.Replace(JsonEditor.Text, @"\[(\w+)\]", match =>
                {
                    string KeywordID = match.Groups[1].Value;

                    if (Shorthand_Type.Equals("Crescent Corp."))
                    {
                        return Keywords.ContainsKey(KeywordID) ? $"[{KeywordID}:*{Keywords[KeywordID]}*]" : $"[{KeywordID}]";
                    }
                    else
                    {
                        return Keywords.ContainsKey(KeywordID) ? $"[{KeywordID}:`{Keywords[KeywordID]}`]" : $"[{KeywordID}]";
                    }
                });

                JsonEditor.Text = ReplaceSquareLinks;
                Refractor1.Width = 0;
                Refractor2.Width = 0;
            }
            catch { }
        }

        
        private void Refractor2_Click_LMB(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!JsonEditor.SelectedText.Equals(""))
                {
                    if (EditorMode.Equals("EGOgift"))
                    {
                        JsonEditor.SelectedText = $"<style=\"upgradeHighlight\">{JsonEditor.SelectedText}</style>";
                    }
                    else if (EditorMode.Equals("Skills") | EditorMode.Equals("Passives"))
                    {
                        JsonEditor.SelectedText = $"<style=\"highlight\">{JsonEditor.SelectedText}</style>";
                    }
                }
                else
                {
                    if (EditorMode.Equals("EGOgift"))
                    {
                        JsonEditor.Text = JsonEditor.Text.Insert(JsonEditor.CaretIndex, "<style=\"upgradeHighlight\"></style>");
                    }
                    else if (EditorMode.Equals("Skills") | EditorMode.Equals("Passives"))
                    {
                        JsonEditor.Text = JsonEditor.Text.Insert(JsonEditor.CaretIndex, "<style=\"highlight\"></style>");
                    }
                }
            }
            catch { }
        }
        private void Refractor1_Shorthand_Changetype(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnableDynamicKeywords)
                {
                    EnableDynamicKeywords = false;
                    EnableDynamicKeywords_Display.Text = InterfaceTextContent["[Settings] Enable Dynamic Keywords (Display text - Disabled)"];
                }
                else
                {
                    EnableDynamicKeywords = true;
                    EnableDynamicKeywords_Display.Text = InterfaceTextContent["[Settings] Enable Dynamic Keywords (Display text - Enabled)"];
                }
                MSettings.SaveSetting("Enable Dynamic Keywords", EnableDynamicKeywords ? "Yes" : "No");
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            catch { }
        }
        private void BattleKeywords_Change_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BattleKeywords_Type.Equals("RU"))
                {
                    (Keywords, KeywordIDName) = GetKeywords(from: "EN");
                    Replacements = GetAddtReplacements(from: "EN");
                    BattleKeywords_Type = "EN";
                    BattleKeywords_TypeDisplay.Text = BattleKeywords_Type;
                }
                else if (BattleKeywords_Type.Equals("EN"))
                {
                    (Keywords, KeywordIDName) = GetKeywords(from: "CN");
                    Replacements = GetAddtReplacements(from: "CN");
                    BattleKeywords_Type = "CN";
                    BattleKeywords_TypeDisplay.Text = BattleKeywords_Type;
                }
                else if (BattleKeywords_Type.Equals("CN"))
                {
                    (Keywords, KeywordIDName) = GetKeywords(from: "KR");
                    Replacements = GetAddtReplacements(from: "KR");
                    BattleKeywords_Type = "KR";
                    BattleKeywords_TypeDisplay.Text = BattleKeywords_Type;

                }
                else if (BattleKeywords_Type.Equals("KR"))
                {
                    (Keywords, KeywordIDName) = GetKeywords(from: "RU");
                    Replacements = GetAddtReplacements(from: "RU");
                    BattleKeywords_Type = "RU";
                    BattleKeywords_TypeDisplay.Text = BattleKeywords_Type;
                }
                MSettings.SaveSetting("Keywords Type", BattleKeywords_Type);


                if (BattleKeywords_Type.Equals("KR")) РазноеДругое.SwitchToSDream();
                else if (BattleKeywords_Type.Equals("CN")) РазноеДругое.SwitchToSourceHanSansSC();
                else РазноеДругое.SwitchToPretendard();

                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            catch { }
        }

        private void Refractor_MouseEnter(object sender, MouseEventArgs e)  { try { Refractor.Background = GetColorFromAHEX("#FF282828");  } catch { } }
        private void Refractor_MouseLeave(object sender, MouseEventArgs e)  { try { Refractor.Background = GetColorFromAHEX("#FF191919");  } catch { } }
        private void Minimize_MouseEnter(object sender, MouseEventArgs e)   { try { Minimize.Background = GetColorFromAHEX("#FF282828");   } catch { } }
        private void Minimize_MouseLeave(object sender, MouseEventArgs e)   { try { Minimize.Background = GetColorFromAHEX("#FF191919");   } catch { } }
        private void WindowMode_MouseEnter(object sender, MouseEventArgs e) { try { WindowMode.Background = GetColorFromAHEX("#FF282828"); } catch { } }
        private void WindowMode_MouseLeave(object sender, MouseEventArgs e) { try { WindowMode.Background = GetColorFromAHEX("#FF191919"); } catch { } }
        private void Settings_MouseEnter(object sender, MouseEventArgs e)   { try { Settings.Background = GetColorFromAHEX("#FF282828");   } catch { } }
        private void Settings_MouseLeave(object sender, MouseEventArgs e)   { try { Settings.Background = GetColorFromAHEX("#FF191919");   } catch { } }
        private void Close_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Close.Foreground = new SolidColorBrush(Colors.Black);
                Close.Background = GetColorFromAHEX("#FFf55442");
            }
            catch { }
        }
        private void Close_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Close.Background = GetColorFromAHEX("#FF191919");
                Close.Foreground = GetColorFromAHEX("#FFA69885");
            }
            catch { }
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SettingsDialog.Margin == new Thickness(0))
                {
                    OverrideCover1.Margin = new Thickness(1000);
                    OverrideCover2.Margin = new Thickness(1000);
                    SettingsDialog.Margin = new Thickness(1000);
                }
                else
                {
                    OverrideCover1.Margin = new Thickness(0);
                    OverrideCover2.Margin = new Thickness(0);
                    SettingsDialog.Margin = new Thickness(0);
                }
            }
            catch { }
        }

        private void ToggleHighlight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JsonEditor_EnableHighlight)
                {
                    JsonEditor_EnableHighlight = false;
                    ToggleHighlight_Text.Text = InterfaceTextContent["[Settings] Enable <style> highlight (Display text - No)"];
                }
                else
                {
                    JsonEditor_EnableHighlight = true;
                    ToggleHighlight_Text.Text = InterfaceTextContent["[Settings] Enable <style> highlight (Display text - Yes)"];
                }
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);

                MSettings.SaveSetting("Enable <style> as color", ToggleHighlight_Text.Text.Equals("Да")? "Yes" : "No");
            }
            catch { }
        }

        private void FontSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selectedFont = FontSelector.SelectedItem.ToString().Split("{ Text = ")[1].Split(", FontFamily")[0];
                FontLabel.Content = selectedFont;
                JsonEditor_FontFamily = new FontFamily(selectedFont);
                FontLabel.FontFamily = JsonEditor_FontFamily;
                JsonEditor.FontFamily = JsonEditor_FontFamily;

                MSettings.SaveSetting("JsonEditor Font", selectedFont);
            }
            catch { }
        }

        private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selectedLang = LanguageSelector.SelectedItem.ToString().Split("{ Text = ")[1].Split(", FontSize")[0];
                LangLabel.Text = selectedLang;
                rin(DefinedLanguages[selectedLang]);
                ApplyLanguage(DefinedLanguages[selectedLang].Replace(".llang", ""));
                SelectedLanguage_Display.Text = selectedLang;
                MSettings.SaveSetting("UI Language", DefinedLanguages[selectedLang].Replace(".llang", ""));
            }
            catch { }
        }

        private void KeywordsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string selectedKeywords = KeywordsSelector.SelectedItem.ToString().Split("{ Text = ")[1].Split(", FontSize")[0].Trim();

                BattleKeywords_TypeDisplay.Text = selectedKeywords;
                (Keywords, KeywordIDName) = GetKeywords(from: DefinedKeywords[selectedKeywords]);
                Replacements = GetAddtReplacements(from: DefinedKeywords[selectedKeywords]);
                BattleKeywords_Type = selectedKeywords;

                MSettings.SaveSetting("Keywords Type", BattleKeywords_Type);


                if (BattleKeywords_Type.Equals("KR")) РазноеДругое.SwitchToSDream();
                else if (BattleKeywords_Type.Equals("CN")) РазноеДругое.SwitchToSourceHanSansSC();
                else РазноеДругое.SwitchToPretendard();

                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            catch { }
        }

        private void JsonEditor_ColorSelector_TextChanged(object sender, TextChangedEventArgs e)
        {
            try{
                if (ColorConverter.ConvertFromString(JsonEditor_ColorSelector.Text) is Color color)
                {
                    JsonEditor.Foreground = new SolidColorBrush(color);
                    MSettings.SaveSetting("JsonEditor Font Color", JsonEditor_ColorSelector.Text);
                }
            }
            catch{}
        }

        private void Settings_OK(object sender, RoutedEventArgs e)
        {
            try
            {
                OverrideCover1.Margin = new Thickness(1000);
                OverrideCover2.Margin = new Thickness(1000);
                SettingsDialog.Margin = new Thickness(1000);
            }
            catch { }
        }

        private void Reload_Sprites(object sender, RoutedEventArgs e)
        {
            try
            {
                SpriteBitmaps = GetSpritesBitmaps();
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            catch { }
        }

        private void Reload_Keywords(object sender, RoutedEventArgs e)
        {
            try
            {
                (Keywords, KeywordIDName) = GetKeywords(from: BattleKeywords_Type);
                Replacements = GetAddtReplacements(from: BattleKeywords_Type);
                TranslationHints = GetTranslationHints();
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            catch { }
        }
        
        private void Reload_Colors(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPairs = GetColorPairs();
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            catch { }
        }

        private void OverrideCover2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OverrideCover1.Margin = new Thickness(1000);
                OverrideCover2.Margin = new Thickness(1000);
                ExitDialog.Margin = new Thickness(1000);
                SettingsDialog.Margin = new Thickness(1000);
            }
            catch { }
        }

        private void OverrideCover1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OverrideCover1.Margin = new Thickness(1000);
                OverrideCover2.Margin = new Thickness(1000);
                ExitDialog.Margin = new Thickness(1000);
                SettingsDialog.Margin = new Thickness(1000);
            }
            catch{ }
        }

        private void Shorthand_TypeChange(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Shorthand_Type == "Crescent Corp.")
                {
                    Shorthand_Type = "kimght";
                }
                else
                {
                    Shorthand_Type = "Crescent Corp.";
                }
                MSettings.SaveSetting("Shorthand Type", Shorthand_Type);
                Shorthand_TypeDisplay.Text = Shorthand_Type;
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            catch { }
        }





        private void DiffFile_SelectFile_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (DiffFile_SelectFile_Input.Text.Equals(""))
                {
                    DiffFile_SelectFile_Input_bgtext.Content = "Файл сравнения";
                }
                else
                {
                    DiffFile_SelectFile_Input_bgtext.Content = "";
                }
            }
            catch { }
        }

        private void DiffFile_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    var dialog = new Microsoft.Win32.OpenFileDialog();

                    try { dialog.InitialDirectory = $@"{Directory.GetDirectories(@"C:\Program Files (x86)\Steam\steamapps\common\Limbus Company\BepInEx\plugins")[0]}\Localize\RU"; }
                    catch { dialog.InitialDirectory = ""; }
                    dialog.DefaultExt = ".json";
                    dialog.Filter = "Text documents (.json)|*.json";

                    bool? result = dialog.ShowDialog();

                    if (result == true)
                    {
                        string DiffFile_Filename = dialog.FileName;
                        if (DiffFile_Filename.Split("\\")[^1].Equals(Mainfile_Filename))
                        {
                            DiffFile_SelectFile_Input.Text = DiffFile_Filename;
                        }
                        else
                        {
                            TextBoxFlashWarning(DiffFile_SelectFile_Input, DiffFile_SelectFile_Input_bgtext, "Файл не идентичен", "Файл сравнения", "Check_JsonFilepath_bgtext");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, InterfaceTextContent["File reading warning"], InterfaceTextContent["Path to json file (After warning)"], "Check_JsonFilepath_bgtext");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.Message);
                }
            }
            catch { }
        }
        #endregion

        #region Контекстное меню
        private void MenuChange(Object sender, RoutedEventArgs ags)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || cxm == null) return;

            switch (rb.Name)
            {
                case "rbCustom":
                    JsonEditor.ContextMenu = cxm;
                    break;
                case "rbDefault":
                    JsonEditor.ClearValue(ContextMenuProperty);
                    break;
                case "rbDisabled":
                    JsonEditor.ContextMenu = null;
                    break;
                default:
                    break;
            }
        }

        void ClickClear(Object sender, RoutedEventArgs args) { JsonEditor.Clear(); }
        void ClickSelectLine(Object sender, RoutedEventArgs args)
        {
            int lineIndex = JsonEditor.GetLineIndexFromCharacterIndex(JsonEditor.CaretIndex);
            int lineStartingCharIndex = JsonEditor.GetCharacterIndexFromLineIndex(lineIndex);
            int lineLength = JsonEditor.GetLineLength(lineIndex);
            JsonEditor.Select(lineStartingCharIndex, lineLength);
        }

        private void InsertStyle_ContextMenu(object sender, RoutedEventArgs e)
        {
            Refractor2_Click_LMB(null, new RoutedEventArgs());
        }

        private void SelectedText_TagUnlesh(object sender, RoutedEventArgs e)
        {
            try
            {
                string ReplaceSquareLinks = Regex.Replace(JsonEditor.SelectedText, @"\[(.*?)\]", match =>
                {
                    string KeywordID = match.Groups[1].Value;

                    return Keywords.ContainsKey(KeywordID) ? $"<sprite name=\"{KeywordID}\"><color={ColorPairs[KeywordID]}><u><link=\"{KeywordID}\">{Keywords[KeywordID]}</link></u></color>" : $"[{KeywordID}]";
                });

                JsonEditor.SelectedText = ReplaceSquareLinks;
            }
            catch { }
        }

        private void SelectedText_RawKeywordToShorthand(object sender, RoutedEventArgs e)
        {
            try
            {
                string RawKeyword = JsonEditor.SelectedText.Trim();

                Dictionary<string, string> SearchSource = new();

                if (BattleKeywords_Type.Equals("RU"))
                {
                    SearchSource = KeywordsRuSet;
                }
                else
                {
                    SearchSource = Keywords;
                }

                if (SearchSource.ContainsKey(RawKeyword))
                {
                    if (Shorthand_Type.Equals("Crescent Corp."))
                    {
                        JsonEditor.SelectedText = JsonEditor.SelectedText.Replace(RawKeyword, $"[{SearchSource[RawKeyword]}:*{RawKeyword}*]");
                    }
                    else
                    {
                        JsonEditor.SelectedText = JsonEditor.SelectedText.Replace(RawKeyword, $"[{SearchSource[RawKeyword]}:`{RawKeyword}`]");
                    }
                }
            }
            catch { }
        }

        private void SelectedText_RawKeywordToTags(object sender, RoutedEventArgs e)
        {
            try
            {
                string RawKeyword = JsonEditor.SelectedText.Trim();

                Dictionary<string, string> SearchSource = new();

                if (BattleKeywords_Type.Equals("RU"))
                {
                    SearchSource = KeywordsRuSet;
                }
                else
                {
                    SearchSource = Keywords;
                }

                if (SearchSource.ContainsKey(RawKeyword))
                {
                    string KeywordID = SearchSource[RawKeyword];
                    string KeywordColor = ColorPairs[KeywordID];
                    JsonEditor.SelectedText = JsonEditor.SelectedText.Replace(RawKeyword, $"<sprite name=\"{KeywordID}\"><color={KeywordColor}><u><link=\"{KeywordID}\">{RawKeyword}</link></u></color>");
                }
            }
            catch { }
        }

        private void SelectedText_TagUnlesh_Shorthand(object sender, RoutedEventArgs e)
        {
            try
            {
                string ReplaceSquareLinks = Regex.Replace(JsonEditor.SelectedText, @"\[(\w+)\]", match =>
                {
                    string KeywordID = match.Groups[1].Value;

                    if (Shorthand_Type.Equals("Crescent Corp."))
                    {
                        return Keywords.ContainsKey(KeywordID) ? $"[{KeywordID}:*{Keywords[KeywordID]}*]" : $"[{KeywordID}]";
                    }
                    else
                    {
                        return Keywords.ContainsKey(KeywordID) ? $"[{KeywordID}:`{Keywords[KeywordID]}`]" : $"[{KeywordID}]";
                    }
                });

                JsonEditor.SelectedText = ReplaceSquareLinks;
            }
            catch { }
        }

        private void SelectedText_CollapseTags(object sender, RoutedEventArgs e)
        {
            try
            {
                string ReplaceSquareLinks = Regex.Replace(JsonEditor.SelectedText, @"<sprite name=""(\w+)""><color=#([0-9a-fA-F]{6})><u><link=""(\w+)"">(.*?)</link></u></color>", match =>
                {
                    string KeywordID = match.Groups[1].Value;

                    return $"[{KeywordID}]";
                });

                JsonEditor.SelectedText = ReplaceSquareLinks;
            }
            catch { }
        }

        private void InsertItalic(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<i>") & !JsonEditor.SelectedText.EndsWith("</i>"))
            {
                JsonEditor.SelectedText = $"<i>{JsonEditor.SelectedText}</i>";
            }
            else
            {
                JsonEditor.SelectedText = JsonEditor.SelectedText[3..^4];
            }
        }
        private void InsertBold(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<b>") & !JsonEditor.SelectedText.EndsWith("</b>"))
            {
                JsonEditor.SelectedText = $"<b>{JsonEditor.SelectedText}</b>";
            }
            else
            {
                JsonEditor.SelectedText = JsonEditor.SelectedText[3..^4];
            }
        }
        private void InsertUnderline(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<u>") & !JsonEditor.SelectedText.EndsWith("</u>"))
            {
                JsonEditor.SelectedText = $"<u>{JsonEditor.SelectedText}</u>";
            }
            else
            {
                JsonEditor.SelectedText = JsonEditor.SelectedText[3..^4];
            }
        }
        private void InsertStrikethrough(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<strikethrough>") & !JsonEditor.SelectedText.EndsWith("</strikethrough>"))
            {
                JsonEditor.SelectedText = $"<strikethrough>{JsonEditor.SelectedText}</strikethrough>";
            }
            else
            {
                JsonEditor.SelectedText = JsonEditor.SelectedText[15..^16];
            }
        }
        private void InsertSize(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<size=") & !JsonEditor.SelectedText.EndsWith("</size>"))
            {
                JsonEditor.SelectedText = $"<size=100%>{JsonEditor.SelectedText}</size>";
            }
            else
            {
                JsonEditor.SelectedText = Regex.Replace(JsonEditor.SelectedText, @"<size=(\d+)>", Match => { return ""; }).Replace("</size>", "");
            }
        }
        private void InsertFont(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<font=\"") & !JsonEditor.SelectedText.EndsWith("</font>"))
            {
                JsonEditor.SelectedText = $"<font=\"Arial\">{JsonEditor.SelectedText}</font>";
            }
            else
            {
                JsonEditor.SelectedText = Regex.Replace(JsonEditor.SelectedText, @"<font=""(.*?)"">", Match => { return ""; }).Replace("</font>", "");
            }
        }
        private void InsertSub(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<sub>") & !JsonEditor.SelectedText.EndsWith("</sub>"))
            {
                JsonEditor.SelectedText = $"<sub>{JsonEditor.SelectedText}</sub>";
            }
            else
            {
                JsonEditor.SelectedText = JsonEditor.SelectedText[5..^6];
            }
        }
        private void InsertSup(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<sup>") & !JsonEditor.SelectedText.EndsWith("</sup>"))
            {
                JsonEditor.SelectedText = $"<sup>{JsonEditor.SelectedText}</sup>";
            }
            else
            {
                JsonEditor.SelectedText = JsonEditor.SelectedText[5..^6];
            }
        }
        private void InsertColor(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.StartsWith("<color=#") & !JsonEditor.SelectedText.EndsWith("</color>"))
            {
                JsonEditor.SelectedText = $"<color=#EBCAA2>{JsonEditor.SelectedText}</color>";
            }
            else
            {
                JsonEditor.SelectedText = Regex.Replace(JsonEditor.SelectedText, @"<color=#([0-9a-fA-F]{6})>", Match => { return ""; })[0..^8];
            }
        }

        private void ToggleTranslationHints(object sender, RoutedEventArgs e)
        {
            if (EnableTranslationHints)
            {
                EnableTranslationHints = false;
                ContextMenu_EnableTranslationTips_Display.Text = InterfaceTextContent["[Json Editor - Context Menu] Enable Translation Tips - Disabled"];
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
            else
            {
                EnableTranslationHints = true;
                ContextMenu_EnableTranslationTips_Display.Text = InterfaceTextContent["[Json Editor - Context Menu] Enable Translation Tips - Enabled"];
                Call_UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
            }
        }
        #endregion

        private void SelectedLanguage_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (SelectedLanguage_Display.Text.Equals("RU"))
            //{
            //    ApplyLanguage("CN");
            //    SelectedLanguage_Display.Text = "CN";
            //    MSettings.SaveSetting("UI Language", "CN");
            //}
            if (SelectedLanguage_Display.Text.Equals("RU"))
            {
                ApplyLanguage("EN");
                SelectedLanguage_Display.Text = "EN";
                MSettings.SaveSetting("UI Language", "EN");
            }
            else if (SelectedLanguage_Display.Text.Equals("EN"))
            {
                ApplyLanguage("RU");
                SelectedLanguage_Display.Text = "RU";
                MSettings.SaveSetting("UI Language", "RU");
            }
        }

        private static List<string> FormatInsertions = new()
        {
            "", "", "", "", "", "",
        };
        private void FormatInsertion_1_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FormatInsertion_1_Text.Text != "")
            {
                FormatInsertion_1_bgtext.Text = "";
                FormatInsertions[0] = FormatInsertion_1_Text.Text;
            }
            else
            {
                string s = InterfaceTextContent["[Left Menu] Insertions (Background text)"];
                FormatInsertion_1_bgtext.Text = s.Exform(0);
                FormatInsertions[0] = "";
            }
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }
        private void FormatInsertion_2_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FormatInsertion_2_Text.Text != "")
            {
                FormatInsertion_2_bgtext.Text = "";
                FormatInsertions[1] = FormatInsertion_2_Text.Text;
            }
            else
            {
                string s = InterfaceTextContent["[Left Menu] Insertions (Background text)"];
                FormatInsertion_2_bgtext.Text = s.Exform(1);
                FormatInsertions[1] = "";
            }
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }
        private void FormatInsertion_3_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FormatInsertion_3_Text.Text != "")
            {
                FormatInsertion_3_bgtext.Text = "";
                FormatInsertions[2] = FormatInsertion_3_Text.Text;
            }

            else
            {
                string s = InterfaceTextContent["[Left Menu] Insertions (Background text)"];
                FormatInsertion_3_bgtext.Text = s.Exform(2);
                FormatInsertions[2] = "";
            }
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }
        private void FormatInsertion_4_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FormatInsertion_4_Text.Text != "")
            {
                FormatInsertion_4_bgtext.Text = "";
                FormatInsertions[3] = FormatInsertion_4_Text.Text;
            }
            else
            {
                string s = InterfaceTextContent["[Left Menu] Insertions (Background text)"];
                FormatInsertion_4_bgtext.Text = s.Exform(3);
                FormatInsertions[3] = "";
            }
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }
        private void FormatInsertion_5_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FormatInsertion_5_Text.Text != "")
            {
                FormatInsertion_5_bgtext.Text = "";
                FormatInsertions[4] = FormatInsertion_5_Text.Text;
            }
            else
            {
                string s = InterfaceTextContent["[Left Menu] Insertions (Background text)"];
                FormatInsertion_5_bgtext.Text = s.Exform(4);
                FormatInsertions[4] = "";
            }
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }
        private void FormatInsertion_6_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FormatInsertion_6_Text.Text != "")
            {
                FormatInsertion_6_bgtext.Text = "";
                FormatInsertions[5] = FormatInsertion_6_Text.Text;
            }
            else
            {
                string s = InterfaceTextContent["[Left Menu] Insertions (Background text)"];
                FormatInsertion_6_bgtext.Text = s.Exform(5);
                FormatInsertions[5] = "";
            }
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }

    }
}
