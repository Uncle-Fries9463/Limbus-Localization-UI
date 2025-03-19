using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Limbus_Localization_UI.Json;
using Limbus_Localization_UI.Additions;
using Limbus_Localization_UI.Mode_Handlers;
using static Limbus_Localization_UI.Additions.Consola;
using static Limbus_Localization_UI.TagDefier;

namespace Limbus_Localization_UI
{
    public partial class MainWindow : Window
    {
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

        static string Mainfile_Filename = "";
        
        static Dictionary<string, BitmapImage> SpriteBitmaps = РазноеДругое.GetSpritesBitmaps();


        public static Dictionary<string, string> Keywords = new();
        public static Dictionary<string, string> KeywordIDName = new();
        

        static Dictionary<string, string> Replacements = РазноеДругое.GetAddtReplacements();
        static Dictionary<string, string> ColorPairs = РазноеДругое.GetColorPairs();

        static string LastPreviewUpdateText = "";
        static RichTextBox LastPreviewUpdateTarget = new();

        public static bool JsonEditor_EnableHighlight = true;
        public static bool EnableDynamicKeywords = false;
        public static string BattleKeywords_Type = "RU";
        public static string Shorthand_Type = "Knightey";
        public static FontFamily JsonEditor_FontFamily = new FontFamily("Lucida Sans Unicode");
        public static SolidColorBrush JsonEditor_TextColor = РазноеДругое.GetColorFromAHEX("#FFA69885");

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

        public Dictionary<string, dynamic> T = new();
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


                ["Coin Descs 1 Button"] = CoinDescs_1_Button,
                ["Coin Descs 2 Button"] = CoinDescs_2_Button,
                ["Coin Descs 3 Button"] = CoinDescs_3_Button,
                ["Coin Descs 4 Button"] = CoinDescs_4_Button,
                ["Coin Descs 5 Button"] = CoinDescs_5_Button,
                ["Coin Descs 6 Button"] = CoinDescs_6_Button,

                // Ужас
                ["Skill PreviewLayout Desc"] = MainSkillDesc,
                ["Skill PreviewLayout Coin 1 Desc 1"] = Skill_Coin1_Desc1,
                ["Skill PreviewLayout Coin 1 Desc 2"] = Skill_Coin1_Desc2,
                ["Skill PreviewLayout Coin 1 Desc 3"] = Skill_Coin1_Desc3,
                ["Skill PreviewLayout Coin 1 Desc 4"] = Skill_Coin1_Desc4,
                ["Skill PreviewLayout Coin 1 Desc 5"] = Skill_Coin1_Desc5,
                ["Skill PreviewLayout Coin 1 Desc 6"] = Skill_Coin1_Desc6,

                ["Skill PreviewLayout Coin 2 Desc 1"] = Skill_Coin2_Desc1,
                ["Skill PreviewLayout Coin 2 Desc 2"] = Skill_Coin2_Desc2,
                ["Skill PreviewLayout Coin 2 Desc 3"] = Skill_Coin2_Desc3,
                ["Skill PreviewLayout Coin 2 Desc 4"] = Skill_Coin2_Desc4,
                ["Skill PreviewLayout Coin 2 Desc 5"] = Skill_Coin2_Desc5,
                ["Skill PreviewLayout Coin 2 Desc 6"] = Skill_Coin2_Desc6,

                ["Skill PreviewLayout Coin 3 Desc 1"] = Skill_Coin3_Desc1,
                ["Skill PreviewLayout Coin 3 Desc 2"] = Skill_Coin3_Desc2,
                ["Skill PreviewLayout Coin 3 Desc 3"] = Skill_Coin3_Desc3,
                ["Skill PreviewLayout Coin 3 Desc 4"] = Skill_Coin3_Desc4,
                ["Skill PreviewLayout Coin 3 Desc 5"] = Skill_Coin3_Desc5,
                ["Skill PreviewLayout Coin 3 Desc 6"] = Skill_Coin3_Desc6,

                ["Skill PreviewLayout Coin 4 Desc 1"] = Skill_Coin4_Desc1,
                ["Skill PreviewLayout Coin 4 Desc 2"] = Skill_Coin4_Desc2,
                ["Skill PreviewLayout Coin 4 Desc 3"] = Skill_Coin4_Desc3,
                ["Skill PreviewLayout Coin 4 Desc 4"] = Skill_Coin4_Desc4,
                ["Skill PreviewLayout Coin 4 Desc 5"] = Skill_Coin4_Desc5,
                ["Skill PreviewLayout Coin 4 Desc 6"] = Skill_Coin4_Desc6,

                ["Skill PreviewLayout Coin 5 Desc 1"] = Skill_Coin5_Desc1,
                ["Skill PreviewLayout Coin 5 Desc 2"] = Skill_Coin5_Desc2,
                ["Skill PreviewLayout Coin 5 Desc 3"] = Skill_Coin5_Desc3,
                ["Skill PreviewLayout Coin 5 Desc 4"] = Skill_Coin5_Desc4,
                ["Skill PreviewLayout Coin 5 Desc 5"] = Skill_Coin5_Desc5,
                ["Skill PreviewLayout Coin 5 Desc 6"] = Skill_Coin5_Desc6,

                ["Coin Desc Selection Box"] = CoinDescsSelectionBox,
                ["Coin Desc Selection Box sub"] = CoinDescsSelectionBox_Sub,



                ["Skill UptieLevel Selection Box"] = Skill_UptieLevel_Selection_Box,
                ["Uptie Level Icons"] = UptieLevelIcons,

                ["Uptie Level 1"] = UptieLevel1_Button, ["Uptie Level 1 [UnavalibleCover]"] = UptieLevel1_Cover, ["Uptie Level 1 [UnavalibleSubCover]"] = UptieLevel1_SubCover, 
                ["Uptie Level 2"] = UptieLevel2_Button, ["Uptie Level 2 [UnavalibleCover]"] = UptieLevel2_Cover, ["Uptie Level 2 [UnavalibleSubCover]"] = UptieLevel2_SubCover, 
                ["Uptie Level 3"] = UptieLevel3_Button, ["Uptie Level 3 [UnavalibleCover]"] = UptieLevel3_Cover, ["Uptie Level 3 [UnavalibleSubCover]"] = UptieLevel3_SubCover, 
                ["Uptie Level 4"] = UptieLevel4_Button, ["Uptie Level 4 [UnavalibleCover]"] = UptieLevel4_Cover, ["Uptie Level 4 [UnavalibleSubCover]"] = UptieLevel4_SubCover, 
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

            StartInits();
        }

        private void StartInits()
        {
            try { Console.OutputEncoding = Encoding.UTF8; }
            catch { }
            MSettings.InitTDictionaryHere(T);
            MSettings.LoadSettings();

            PreviewLayout_Skills.PreviewMouseLeftButtonDown += SurfaceScroll_MouseLeftButtonDown;
            PreviewLayout_Skills.PreviewMouseMove += SurfaceScroll_MouseMove;
            PreviewLayout_Skills.PreviewMouseLeftButtonUp += SurfaceScroll_MouseLeftButtonUp;
            string Def;
            try
            {
                Def = File.ReadAllText(@"[Ресурсы]\& Stringtypes\DefaultText.txt");
            }
            catch
            {
                Def = "<font=\"Consolas\">「Limbus Company <sprite name=\"KnowledgeExplored\"><u><color=#ffab57>TextMeshPro WPF-Mimicry</u></color> ⇄ 2.0」</font>\r\n\r\n[ <sprite name=\"Breath\"> <sprite name=\"Charge\"> <sprite name=\"ParryingResultUp\"> ] Вставка спрайтов через тег <color=#ed5558><sprite name=\"...\"></color> (Перенос строки вместе с ними целостный)\r\n\r\n[ <color=#c57609>[KeywordID]</color> ]\r\n- [AaCePcBb]\r\n- [TimeKillerWatch]\r\n- [VibrationNesting]\r\n\r\n[ Все теги вместе (Развёртка ссылок) ]\r\n- <sprite name=\"AaCePcBc\"><color=#e30000><u><link=\"AaCePcBc\">Слепая одержимость</link></u></color>\r\n- <sprite name=\"ParryingResultUp\"><color=#f8c200><u><link=\"ParryingResultUp\">Повышение силы в столкновении</link></u></color>\r\n\r\n[ <color=#abcdef>Цвет</color> ] Цветной текст через тег <color=#hexrgb>\r\n[ <i>Крусив</i> ] <i>Курсивный текст через тег</i> i\r\n[ <u>Подчёркивание</u> ] <u>Нижнее подчёркивание текста через тег</u> u\r\n[ <strikethrough>Зачёркнутый</strikethrough> ] <strikethrough>Зачёркнутый</strikethrough> текст через тег strikethrough\r\n[ <uppercase>верхний</uppercase> и <lowercase>нижний</lowercase> регистры ]\r\n[ <b>Толстый текст</b> ] <b>Утолщённый текст через тег</b> b\r\n[ - ] <b>И <i>их <u>комби<color=#d24020>ниров</color>ание</i></u></b>\r\n\r\n\r\n<font=\"Consolas\">(i)</color> Этот текст можно изменить в файле <font=\"Consolas\">\"[Ресурсы]\\& Stringtypes\\DefaultText.txt\"</font>";
            }



            PreviewLayout_EGOgift.SetValue(Paragraph.LineHeightProperty, 30.0);

            // Скрыть все описания каждой монеты
            T[$"Skill PreviewLayout Desc"].SetValue(Paragraph.LineHeightProperty, 30.0);
            for (int i = 1; i <= 5; i++)
            {
                for (int e = 1; e <= 6; e++)
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
                        PreviewLayout_EGOgift.Document.Blocks.Clear();
                        UpdatePreview(JsonEditor.Text, PreviewLayout_EGOgift);
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
                                T["EditorSwitch Desc"].Content = "Описание*";
                            }
                            else
                            {
                                Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"] = "{unedited}";
                                T["EditorSwitch Desc"].Content = "Описание";
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
                    UpdatePreview(UpdatePreview_Text, UpdatePreview_Target);
                }

                else if (EditorMode.Equals("Passives"))
                {
                    UpdatePreview(JsonEditor.Text, MainSkillDesc);
                    MainSkillDesc.Height = double.NaN;
                    //rin(MainSkillDesc.Height);
                    switch (Passives_CurrentEditingField)
                    {
                        case "Desc":
                            if (!JsonEditor.Text.Equals(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Desc"]))
                            {
                                T["EditorSwitch Desc"].Content = "Описание*";
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"] = JsonEditor.Text.Replace("\r", "");
                            }
                            else
                            {
                                T["EditorSwitch Desc"].Content = "Описание";
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                            }
                            break;

                        case "Summary":
                            if (!JsonEditor.Text.Equals(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Summary"]))
                            {
                                T["EditorSwitch SubDesc 1"].Content = "Суммарно*";
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"] = JsonEditor.Text.Replace("\r", "");
                            }
                            else
                            {
                                T["EditorSwitch SubDesc 1"].Content = "Суммарно";
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
                                SwitchEditorTo_Desc.Content = "Описание";
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                            }
                            else
                            {
                                SwitchEditorTo_Desc.Content = "Описание*";
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"] = JsonEditor.Text.Replace("\r", "");
                            }

                            break;


                        default:

                            char DescNumber = EGOgift_CurrentEditingField[^1];

                            if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{DescNumber}"]))
                            {
                                T[$"EditorSwitch SubDesc {DescNumber}"].Content = $"Простое описание {DescNumber}";
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{DescNumber}"] = "{unedited}";
                            }
                            else
                            {
                                T[$"EditorSwitch SubDesc {DescNumber}"].Content = $"Простое описание {DescNumber}*";
                                EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{DescNumber}"] = JsonEditor.Text.Replace("\r", "");
                            }

                            break;
                    }
                }
            }
            catch //(Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }
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
                    Margin = new Thickness(-2, -0.5, 0, 0)
                };

                StackPanel SpritePlusEffectname = new() { Orientation = Orientation.Horizontal };
                SpritePlusEffectname.Children.Add(new TextBlock(new InlineUIContainer(SpriteImage)));

                Run KeywordName = new Run(SpriteData.TextBase.Text);
                ApplyTags(ref KeywordName, SpriteData.TextBase.InnerTags);

                SpritePlusEffectname.Children.Add(new TextBlock(KeywordName));

                if (EditorMode.Equals("EGOgift"))
                {
                    SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 10);
                    SpritePlusEffectname.Margin = new Thickness(0, -10, 0, 0);
                }
                else if (EditorMode.Equals("Skills") | EditorMode.Equals("Passives"))
                {
                    SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 11);
                    SpritePlusEffectname.Margin = new Thickness(0, -11, 0, 0);
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
            Target.Document.Blocks.Clear();

            LastPreviewUpdateText = JsonDesc;
            LastPreviewUpdateTarget = Target;
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

            // Выделение вставок через .Format в определённых файлах // Форматирование для других файлов
            if (Mainfile_Filename.StartsWith("Bufs") | Mainfile_Filename.StartsWith("BattleKeywords"))
            {
                JsonDesc = JsonDesc.Replace("{", "<color=#f95e00>{").Replace("}", "}</color>");
            }
            else
            {
                // Замена обычных слов на вствку ключевых с цветом и спрайтом, если они совпадают (Свойства [TabExplain] сохраняются)
                if (EnableDynamicKeywords)
                {
                    foreach (var KeywordName in KeywordIDName.Reverse())
                    {
                        JsonDesc = Regex.Replace(JsonDesc, @$"(?<![a-zA-Zа-яА-Я<>\[\]\'""*])({KeywordName.Key})(?![a-zA-Zа-яА-Я<[""*])", match =>
                        {
                            return $"<sprite name=\"{KeywordName.Value}\"><color={(ColorPairs.ContainsKey(KeywordName.Value) ? ColorPairs[KeywordName.Value] : "#f8c200")}><u>{KeywordName.Key}⟦InnerTag/LocalTabExplain⟧</u></color>";
                        });
                    }
                }

                // Заменить квадратные скобки ссылок на <sprite><color>...</color>, если текст из них есть в списке id из всех Keywords файлов
                JsonDesc = Regex.Replace(JsonDesc, @"\[(\w+)\]", match =>
                {
                    string MaybeKeyword = match.Groups[1].Value;
                    try
                    {
                        return Keywords.ContainsKey(MaybeKeyword) ? $"<sprite name=\"{MaybeKeyword}\"><color={ColorPairs[MaybeKeyword]}><u>{Keywords[MaybeKeyword]}</u></color>" : $"[{MaybeKeyword}]";
                    }
                    catch
                    {
                        return $"[{MaybeKeyword}]";
                    }
                });
            
                // Обработка особых вставок эффектов [Sinking:'Утопания'] [Combustion:'Огня'] без полной развёртки в теги
                JsonDesc = Regex.Replace(JsonDesc, Shorthand_Type.Equals("kimght") ? @"\[(\w+)\:'(.*?)'\]" : @"\{(\w+)\: \*(.*?)\*\}", match =>
                {
                    string MaybeKeyword = match.Groups[1].Value;
                    string MaybeName = match.Groups[2].Value;

                    if (Keywords.ContainsKey(MaybeKeyword))
                    {
                        return $"<sprite name=\"{MaybeKeyword}\"><color={(ColorPairs.ContainsKey(MaybeKeyword) ? ColorPairs[MaybeKeyword] : "#f8c200")}><u>{MaybeName}</u></color>";
                    }
                    else
                    {
                        return match.Groups[0].Value;
                    }
                });
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
                "color",
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
                "lowercase",
                "/lowercase",
                "uppercase",
                "/uppercase",
                "strikethrough",
                "/strikethrough",
                "/font",

                "\0",

                "EMPTY¤",
            };
            foreach(var Tag in TagList)
            {
                JsonDesc = JsonDesc.Replace($">{Tag}<", $">\0{Tag}<");
            }

            bool ICm(string Range_TextItem)
            {
                return !TagList.Contains(Range_TextItem) & !Range_TextItem.StartsWith("color=#") & !Range_TextItem.StartsWith("sprite name=\"") & !Range_TextItem.StartsWith("font=\"");
            }

            #region Базовое форматирование текста
            JsonDesc = JsonDesc.Replace("color=#None", "color=#ffffff")
                               .Replace("<style=\"highlight\">", "<style=\"upgradeHighlight\">") // Подсветка улучшения (Без разницы как)
                               .Replace("</link>", "") // Ссылки вырезать (тултипы не работают (Возможно))
                               .Replace("[TabExplain]", "");
            
            JsonDesc = Regex.Replace(JsonDesc, @"<link=""\w+"">", ""); // убрать все link (Тултип не рабоатет)
            
            // Сепарированые обычных '<' '>' от тегов
            JsonDesc = Regex.Replace(JsonDesc, @"<color=#([0-9a-fA-F]{6})>", @"⇱color=#$1⇲");
            JsonDesc = Regex.Replace(JsonDesc, @"<sprite name=""(\w+?)"">", @"⇱sprite name=""$1""⇲");
            JsonDesc = Regex.Replace(JsonDesc, @"<style=""(\w+?)"">", @"⇱style=""$1""⇲");
            JsonDesc = Regex.Replace(JsonDesc, @"<font=""(.*?)"">", @"⇱font=""$1""⇲");
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
                if (TextItem.Equals("style=\"upgradeHighlight\""))
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
                                    !__TextSegmented__[RangeIndex - 3].StartsWith("sprite name=\"") &
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


                #region ⟦OuterFormat/TextStyle@LowerCase⟧
                if (TextItem.Equals("lowercase"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/lowercase")) break;
                        if (ICm(Range_TextItem))
                        {
                            __TextSegmented__[RangeIndex] = __TextSegmented__[RangeIndex].ToLower();
                        }
                    }
                }
                #endregion

                #region ⟦OuterFormat/TextStyle@UpperCase⟧
                if (TextItem.Equals("uppercase"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/uppercase")) break;
                        if (ICm(Range_TextItem))
                        {
                            __TextSegmented__[RangeIndex] = __TextSegmented__[RangeIndex].ToUpper();
                        }
                    }
                }
                #endregion
            }
            #endregion

            // Очистить сегментированный список текстовых фрагметов от тегов
            __TextSegmented__.RemoveAll(TextItem => (TagList.Contains(TextItem) | TextItem.StartsWith("color=#") | TextItem.StartsWith("font=\"")) );

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

                                    SpriteKeyword = $":«{SpriteKeywordAppend + NextTextItem_InnerTags}»";
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
            Application.Current.Shutdown();
        }
        private void Exit_No(object sender, RoutedEventArgs e)
        {
            OverrideCover1.Margin = new Thickness(1000);
            OverrideCover2.Margin = new Thickness(1000);
            ExitDialog.Margin = new Thickness(1000);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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

        private static void BackgroundShadowTextCheck(TextBox TextBox, Label Label, string Label_DefaultText)
        {
            if(TextBox.Text != "") Label.Content = "";
            else Label.Content = Label_DefaultText;
        }

        private void Check_JsonFilepath_bgtext() => BackgroundShadowTextCheck(JsonFilepath,   JsonFilepath_bgtext, "Путь к Json файлу");
        private void Check_JumpToID_bgtext()     => BackgroundShadowTextCheck(JumpToID_Input, JumpToID_bgtext,     "Перейти к ID.."   );

        private void JsonPath_TextChanged(object sender, TextChangedEventArgs e) => BackgroundShadowTextCheck(JsonFilepath, JsonFilepath_bgtext, "Путь к Json файлу");
        private void Name_EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string BGText = EditorMode switch
            {
                "EGOgift"  => "Название ЭГО Дара",
                "Skills"   => "Название навыка",
                "Passives" => "Название",

                _ => "Название",
            };
            BackgroundShadowTextCheck(Name_EditBox, Name_Label_bgtext, BGText);
        }
        private void ABName_EditBox_TextChanged(object sender, TextChangedEventArgs e) => BackgroundShadowTextCheck(ABName_EditBox, ABName_Label_bgtext, "Фоновое название ЭГО");
        private void JumpToID_Input_TextChanged(object sender, TextChangedEventArgs e) => BackgroundShadowTextCheck(JumpToID_Input, JumpToID_bgtext, "Перейти к ID..");




        private void IDSwitch_CheckEditBufferDescs()
        {
            if (!EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                SwitchEditorTo_Desc.Content = "Описание*";
            else
                SwitchEditorTo_Desc.Content = "Описание";

            for (int i = 1; i <= 5; i++)
            {
                if (!EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][$"SimpleDesc{i}"].Equals("{unedited}"))
                {
                    T[$"EditorSwitch SubDesc {i}"].Content = $"Простое описание {i}*";
                }
                else
                {
                    T[$"EditorSwitch SubDesc {i}"].Content = $"Простое описание {i}";
                }

            }
        }



        private void SwitchToID(int ID)
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


        private async void TextBoxFlashWarning(TextBox TB,
                                               Label LB,
                                               string WarningText,
                                               string LabelTextAfter,
                                               string WhatsNext,
                                               int rounds = 2,
                                               int AfterAwait = 400,
                                               int TimerAwait = 100) {
            TB.Focusable = false;
            TB.Foreground = РазноеДругое.GetColorFromAHEX("#FF191919"); ;

            LB.Content = WarningText;

            for (int i = 1; i <= rounds; i++)
            {
                LB.Foreground = РазноеДругое.GetColorFromAHEX("#FFFFA4A4");
                await Task.Delay(TimerAwait);
                LB.Foreground = РазноеДругое.GetColorFromAHEX("#FFF43D3D");
                await Task.Delay(TimerAwait);
            }
            await Task.Delay(AfterAwait);

            LB.Foreground = РазноеДругое.GetColorFromAHEX("#FF514C46");
            LB.Content = LabelTextAfter;
            TB.Focusable = true;
            TB.Foreground = РазноеДругое.GetColorFromAHEX("#FFA69885");

            if (WhatsNext == "Check_JumpToID_bgtext") Check_JumpToID_bgtext();
            else Check_JsonFilepath_bgtext();
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
                TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка при чтении файла", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
            }
        }

        private async void Notify(string text)
        {
            JsonFilepath.Focusable = false;
            JsonFilepath.Foreground = РазноеДругое.GetColorFromAHEX("#FF191919");
            JsonFilepath_bgtext.Content = text;
            JsonFilepath_bgtext.Foreground = РазноеДругое.GetColorFromAHEX("#FFCCCCCC");
            await Task.Delay(1150);
            JsonFilepath_bgtext.Content = "";
            JsonFilepath.Focusable = true;
            JsonFilepath.Foreground = РазноеДругое.GetColorFromAHEX("#FFA69885");
        }




        /////////////////////////////////////////////////////////////
        ///  * * * Загрузка Json файлов и адаптация режима * * *  ///
        /////////////////////////////////////////////////////////////
        private void LoadJsonFile(string path)
        {
            if (!File.Exists(path))
            {
                TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Файл не найден", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
            }
            else
            {
                try
                {
                    CurrentHighlight_YOffset = 0;
                    Json_Filepath = path;
                    Mainfile_Filename = Json_Filepath.Split('\\')[^1];

                    bool IsSupportedFileType = true;
                    foreach(var type in NotSupportedFileTypes)
                    {
                        if (Mainfile_Filename.StartsWith(type)) IsSupportedFileType = false;
                    }

                    if (Mainfile_Filename.StartsWith("EGOgift_"))
                    {
                        EditorMode = "EGOgift";
                        Mode_Handlers.Mode_EGO_Gifts.AdjustUI();
                        EGOgift_Json_Dictionary = JsonLoader_EGOgifts.GetJsonDictionary(Json_Filepath);
                        EGOgift_JsonKeys = EGOgift_Json_Dictionary.Keys.ToList();

                        foreach(var ID in EGOgift_JsonKeys) // Получние буфера не сохранённых изменений ЭГО даров
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

                    else if(Mainfile_Filename.StartsWith("Skills"))
                    {
                        EditorMode = "Skills";
                        // Основной словарь с текстом из JsonData.dataList и Буфер не сохранённых изменений
                        (Skills_Json_Dictionary, Skills_EditBuffer) = JsonLoader_Skills.GetJsonDictionary(Json_Filepath);
                        Skills_JsonKeys = Skills_Json_Dictionary.Keys.ToList();

                        // По умолчанию переключиться на самый первый ID в файле
                        Skills_Json_Dictionary_CurrentID = Skills_JsonKeys[0];

                        foreach(var MinUptieLevel in Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID])
                        {
                            Skills_Json_Dictionary_CurrentUptieLevel = MinUptieLevel.Key;
                            break;
                        }

                        // Адаптировать интерфейс под навыки
                        T["Name EditBox [UnavalibleCover]"]     .Height = 0; // Разблокировать кнопку имени
                        T["Name SaveChanges [UnavalibleCover]"] .Height = 0;
                        T["EditorSwitch Desc [UnavalibleCover]"].Height = 0; // Разблокировать кнопку описания

                        // Навыки грешников
                        if (Mainfile_Filename.StartsWith("Skills_Ego_Personality-"))
                        {
                            SaveChangesButtons.Height = 270;
                            SaveChangesButtons.Margin = new Thickness(236, -270, 0, 0);
                            ABName_Change_StackPanel.Height = 33;
                            ABName_Input_StackPanel.Height = 33;
                            CurrentHighlight_YOffset = 33;
                            Mode_Handlers.Mode_Skills.AdjustUI(IsEGO: true);
                            Name_Label_bgtext.Content = "Название ЭГО";
                        }
                        else if(Mainfile_Filename.StartsWith("Skills_personality-") | Mainfile_Filename.Equals("Skills.json"))
                        {
                            SaveChangesButtons.Height = 237;
                            SaveChangesButtons.Margin = new Thickness(236, -237, 0, 0);
                            ABName_Change_StackPanel.Height = 0;
                            ABName_Input_StackPanel.Height = 0;
                            CurrentHighlight_YOffset = 0;
                            Mode_Handlers.Mode_Skills.AdjustUI(IsEGO: false);
                            Name_Label_bgtext.Content = "Название навыка";
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
                            Name_Label_bgtext.Content = "Название навыка";
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
                        TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Неподдерживаемый формат", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
                    }

                    else
                    {
                        TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Неподдерживаемый формат", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
                    }
                }
                catch (Exception ex)
                {
                    JsonFilepath.Text = "";
                    TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка при чтении файла", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.Message);
                }
            }
            CurrentHighlight.RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
        }


        /// <summary>
        /// Проверка доступности следующего ID из списка. Если ID последний в списке и не удалось найти следующий, отключить кнопку переключения по порядку
        /// </summary>
        private void ID_Switch_CheckButtons()
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
            catch{}
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
            }
        }

        private async void ID_SwitchPrev_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ID_SwitchPrev.BorderBrush = РазноеДругое.GetColorFromAHEX("#FFFFFFFF");
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
            catch{}

            await Task.Delay(100);
            ID_SwitchPrev.BorderBrush = РазноеДругое.GetColorFromAHEX("#FF333333");
        }

        private async void ID_SwitchNext_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ID_SwitchNext.BorderBrush = РазноеДругое.GetColorFromAHEX("#FFFFFFFF");
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
            catch{}

            await Task.Delay(100);
            ID_SwitchNext.BorderBrush = РазноеДругое.GetColorFromAHEX("#FF333333");
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




        private void SwitchEditorTo_SubDesc1_Button(object sender, RoutedEventArgs e)
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

                    Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount);
                    Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount);

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
            catch {}
            ResetUndo();
        }
        private void SwitchEditorTo_SubDesc2_Button(object sender, RoutedEventArgs e)
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

                    Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount);
                    Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount);

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
        private void SwitchEditorTo_SubDesc3_Button(object sender, RoutedEventArgs e)
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

                    Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount);
                    Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount);

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
        private void SwitchEditorTo_SubDesc4_Button(object sender, RoutedEventArgs e)
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

                    Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount);
                    Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount);

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
        private void SwitchEditorTo_SubDesc5_Button(object sender, RoutedEventArgs e)
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

                    Mode_Handlers.Mode_Skills.ReEnableAvalibleCoinDescs(DescsCount);
                    Mode_Handlers.Mode_Skills.SetCurrentCoinDescHighlight(0, DescsCount);

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















        private void JumpToID_Button_Click(object sender, RoutedEventArgs e)
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
                TextBoxFlashWarning(JumpToID_Input, JumpToID_bgtext, "ID Не найден", "Перейти к ID..", "Check_JumpToID_bgtext");
            }
        }

        private async void ID_Copy(object sender, RoutedEventArgs e)
        {
            if (EGOgift_Json_Dictionary_CurrentID != -1 | Skills_Json_Dictionary_CurrentID != -1 | !Convert.ToString(Passives_Json_Dictionary_CurrentID).Equals("-1"))
            {
                string MemID = $"{ID_Copy_Button.Content}";
                Clipboard.SetText(Convert.ToString(ID_Copy_Button.Content));
                ID_Copy_Button.Foreground = РазноеДругое.GetColorFromAHEX("#00FFFFFF");
                IDCopiedNotify.Foreground = РазноеДругое.GetColorFromAHEX("#FF7C746B");

                await Task.Delay(810);
                ID_Copy_Button.Foreground = РазноеДругое.GetColorFromAHEX("#FF7C746B");
                IDCopiedNotify.Foreground = РазноеДругое.GetColorFromAHEX("#00FFFFFF");
            }
        }


        public static Dictionary<int, Image> UptieLevelIcons = new()
        {
            [0] = new Image(),

            [1] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/1 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
            [2] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/2 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
            [3] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/3 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
            [4] = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Images/UptieLevels/4 Уровень связи.png")), Stretch = Stretch.Fill, Width = 40, Height = 40 },
        };

        private void UptieLevel1_Button_MouseEnter(object sender, MouseEventArgs e) => UptieLevel1_Button.Content = UptieLevelIcons[1];
        private void UptieLevel1_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Skills_Json_Dictionary_CurrentUptieLevel != 1) UptieLevel1_Button.Content = UptieLevelIcons[0];
        }

        private void UptieLevel2_Button_MouseEnter(object sender, MouseEventArgs e) => UptieLevel2_Button.Content = UptieLevelIcons[2];
        private void UptieLevel2_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Skills_Json_Dictionary_CurrentUptieLevel != 2) UptieLevel2_Button.Content = UptieLevelIcons[0];
        }

        private void UptieLevel3_Button_MouseEnter(object sender, MouseEventArgs e) => UptieLevel3_Button.Content = UptieLevelIcons[3];
        private void UptieLevel3_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Skills_Json_Dictionary_CurrentUptieLevel != 3) UptieLevel3_Button.Content = UptieLevelIcons[0];
        }

        private void UptieLevel4_Button_MouseEnter(object sender, MouseEventArgs e) => UptieLevel4_Button.Content = UptieLevelIcons[4];
        private void UptieLevel4_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Skills_Json_Dictionary_CurrentUptieLevel != 4) UptieLevel4_Button.Content = UptieLevelIcons[0];
        }

        private void UptieLevel1_Button_Click(object sender, RoutedEventArgs e) => Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 1);
        private void UptieLevel2_Button_Click(object sender, RoutedEventArgs e) => Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 2);
        private void UptieLevel3_Button_Click(object sender, RoutedEventArgs e) => Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 3);
        private void UptieLevel4_Button_Click(object sender, RoutedEventArgs e) => Mode_Handlers.Mode_Skills.UpdateMenuInfo(Skills_Json_Dictionary_CurrentID, 4);

        private void CoinDescs_1(object sender, RoutedEventArgs e)
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
        private void CoinDescs_2(object sender, RoutedEventArgs e)
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
        private void CoinDescs_3(object sender, RoutedEventArgs e)
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
        private void CoinDescs_4(object sender, RoutedEventArgs e)
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
        private void CoinDescs_5(object sender, RoutedEventArgs e)
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
        private void CoinDescs_6(object sender, RoutedEventArgs e)
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


        private void ABName_ChangeName(object sender, RoutedEventArgs e)
        {
            int JSON_IndexOf_ID = JsonLoader_Skills.ID_AND_INDEX[Skills_Json_Dictionary_CurrentID];
            int JSON_IndexOf_UptieLevel = JsonLoader_Skills.UPTIELEVEL_AND_INDEX[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel];

            РазноеДругое.SetRW(Json_Filepath);
            JsonLoader_Skills.JSON.dataList[JSON_IndexOf_ID].levelList[JSON_IndexOf_UptieLevel].abName = ABName_EditBox.Text.Replace("\r", "");
            РазноеДругое.SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
            РазноеДругое.SetRO(Json_Filepath);
            Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["ABName"] = ABName_EditBox.Text;

            Notify("Фоновое имя обновлено");
        }
        private void Name_ChangeName(object sender, RoutedEventArgs e)
        {
            try{
                if (EditorMode.Equals("EGOgift"))
                {
                    РазноеДругое.SetRW(Json_Filepath);
                    РазноеДругое.RewriteFileLine($"\"name\": \"{Name_EditBox.Text.Replace("\r", "").Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",",
                                    Json_Filepath,
                                    Convert.ToInt32(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["LineIndex_Name"]));
                    РазноеДругое.SetRO(Json_Filepath);
                    Name_Label.Text = Name_EditBox.Text;
                    EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID]["Name"] = Name_EditBox.Text.Replace("\r", "");
                }
                else if (EditorMode.Equals("Skills"))
                {
                    int JSON_IndexOf_ID = JsonLoader_Skills.ID_AND_INDEX[Skills_Json_Dictionary_CurrentID];
                    int JSON_IndexOf_UptieLevel = JsonLoader_Skills.UPTIELEVEL_AND_INDEX[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel];
                    
                    РазноеДругое.SetRW(Json_Filepath);
                    JsonLoader_Skills.JSON.dataList[JSON_IndexOf_ID].levelList[JSON_IndexOf_UptieLevel].name = Name_EditBox.Text;
                    РазноеДругое.SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
                    РазноеДругое.SetRO(Json_Filepath);
                    Name_Label.Text = Name_EditBox.Text;
                    Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Name"] = Name_EditBox.Text;
                }
                else if (EditorMode.Equals("Passives"))
                {
                    int LineToRewrite = Convert.ToInt32(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["LineIndex_Name"]);
                    string NewLine = $"\"name\": \"{Name_EditBox.Text.Replace("\r", "").Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",";
                    //rin($"Rewriting line {LineToRewrite} with:\n{NewLine}\n");
                    РазноеДругое.SetRW(Json_Filepath);
                    РазноеДругое.RewriteFileLine(NewLine, Json_Filepath, LineToRewrite);
                    Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Name"] = Name_EditBox.Text;
                    Name_Label.Text = Name_EditBox.Text;
                    РазноеДругое.SetRO(Json_Filepath);
                }

            } catch(Exception ex) {
                TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка сохранения", "Путь к Json файлу", "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
            }

            Notify("Имя обновлено");
        }

        private void Desc_ChangeOver(string ThisDesc)
        {
            try
            {
                if (EditorMode.Equals("EGOgift"))
                {
                    if (!EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc].Equals("{unedited}"))
                    {
                        //rin($"Saving: \"{EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc]}\"");
                        РазноеДругое.SetRW(Json_Filepath);
                        РазноеДругое.RewriteFileLine($"{(ThisDesc.StartsWith("SimpleDesc") ? "\"simpleDesc\": \"" : "\"desc\": \"")}" +
                                                     $"{Convert.ToString(EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc]).Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}" +
                                                     $"{(ThisDesc.StartsWith("SimpleDesc") ? "\"" : "\",")}",
                                                     Json_Filepath,
                                                     Convert.ToInt32(EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][$"LineIndex_{ThisDesc}"]));

                        EGOgift_Json_Dictionary[EGOgift_Json_Dictionary_CurrentID][ThisDesc] = EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc];
                        EGOgift_EditBuffer[EGOgift_Json_Dictionary_CurrentID][ThisDesc] = "{unedited}";
                        IDSwitch_CheckEditBufferDescs();
                        
                        РазноеДругое.SetRO(Json_Filepath);
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
                                

                                РазноеДругое.SetRW(Json_Filepath);
                                try
                                {
                                    JsonLoader_Skills.JSON.dataList[JSON_IndexOf_ID].levelList[JSON_IndexOf_UptieLevel].desc = Skills_Json_Dictionary[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Desc"].Replace("\\n", "\n");
                                    РазноеДругое.SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
                                    T["EditorSwitch Desc"].Content = "Описание";

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine(ex.StackTrace);
                                    Console.WriteLine(ex.Source);
                                }
                                РазноеДругое.SetRO(Json_Filepath);
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


                                    РазноеДругое.SetRW(Json_Filepath);
                                    РазноеДругое.SaveJson(JsonLoader_Skills.JSON, Json_Filepath);
                                    T[$"Coin Descs {CoinDescIndex + 1} Button"].Content = $"№{CoinDescIndex + 1}";
                                    РазноеДругое.SetRO(Json_Filepath);
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
                    РазноеДругое.SetRW(Json_Filepath);
                    switch (Passives_CurrentEditingField)
                    {
                        case "Desc":
                            if (!Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                            {
                                LineToRewrite = Convert.ToInt32(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["LineIndex_Desc"]);
                                NewLine = $"\"desc\": \"{Convert.ToString(Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"]).Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",";
                                //rin($"Rewriting line {LineToRewrite} with:\n{NewLine}\n");
                                РазноеДругое.RewriteFileLine(NewLine, Json_Filepath, LineToRewrite);

                                Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Desc"] = Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"];
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                                T["EditorSwitch Desc"].Content = "Описание";
                            }
                            break;

                        case "Summary":
                            if (!Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"].Equals("{unedited}"))
                            {
                                LineToRewrite = Convert.ToInt32(Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["LineIndex_Summary"]);
                                NewLine = $"\"summary\": \"{Convert.ToString(Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"]).Replace("\\\"", "\"").Replace("\\n", "\n").Replace(@"\", @"\\").Replace("\"", "\\\"")}\",";
                                //rin($"Rewriting line {LineToRewrite} with:\n{NewLine}\n");
                                РазноеДругое.RewriteFileLine(NewLine, Json_Filepath, LineToRewrite);

                                Passives_Json_Dictionary[Passives_Json_Dictionary_CurrentID]["Summary"] = Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"];
                                Passives_EditBuffer[Passives_Json_Dictionary_CurrentID]["Summary"] = "{unedited}";
                                T["EditorSwitch SubDesc 1"].Content = "Суммарно";
                            }
                            break;
                    }
                    
                    РазноеДругое.SetRO(Json_Filepath);
                }
            }
            catch(Exception ex) {
                TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка сохранения", "Путь к Json файлу", "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
            }
        }


        private void Desc_ChangeDesc(object sender, RoutedEventArgs e)     => Desc_ChangeOver("Desc");
        private void Desc_ChangeSubDesc1(object sender, RoutedEventArgs e) => Desc_ChangeOver("SimpleDesc1");
        private void Desc_ChangeSubDesc2(object sender, RoutedEventArgs e) => Desc_ChangeOver("SimpleDesc2");
        private void Desc_ChangeSubDesc3(object sender, RoutedEventArgs e) => Desc_ChangeOver("SimpleDesc3");
        private void Desc_ChangeSubDesc4(object sender, RoutedEventArgs e) => Desc_ChangeOver("SimpleDesc4");
        private void Desc_ChangeSubDesc5(object sender, RoutedEventArgs e) => Desc_ChangeOver("SimpleDesc5");








        // Сохранение на CTRL + S
        private bool isCtrlSPressed = false;

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
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
                if (!ABName_EditBox.IsFocused & !Name_EditBox.IsFocused & !JsonEditor.IsFocused & !JumpToID_Input.IsFocused)
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

        private void UnfocusTB(TextBox tb)
        {
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(tb), null);
            Keyboard.ClearFocus();
            this.Focus();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S) isCtrlSPressed = false;
        }



        // Прокрутка предпросмотра навыка на перетягивание через ЛКМ господи наконец-то
        private bool isDragging = false;
        private Point lastMousePosition;
        private void SurfaceScroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            lastMousePosition = e.GetPosition(PreviewLayout_Skills);
            PreviewLayout_Skills.CaptureMouse();
        }

        private void SurfaceScroll_MouseMove(object sender, MouseEventArgs e)
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

        private void SurfaceScroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            PreviewLayout_Skills.ReleaseMouseCapture();
        }



        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void WindowMode_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }

        
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var cancelEventArgs = new System.ComponentModel.CancelEventArgs();
            Window_Closing(this, cancelEventArgs);

            if (!cancelEventArgs.Cancel) this.Close();
        }


        private void Refractor_Click(object sender, RoutedEventArgs e)
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
        private void Refractor1_Click_LMB(object sender, RoutedEventArgs e)
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
        private void Refractor1_Click_RMB(object sender, RoutedEventArgs e)
        {
            string ReplaceSquareLinks = Regex.Replace(JsonEditor.Text, @"\[(\w+)\]", match =>
            {
                string KeywordID = match.Groups[1].Value;

                if (Shorthand_Type.Equals("Knightey"))
                {
                    return Keywords.ContainsKey(KeywordID) ? $"{{{KeywordID}: *{Keywords[KeywordID]}*}}" : $"[{KeywordID}]";
                }
                else
                {
                return Keywords.ContainsKey(KeywordID) ? $"[{KeywordID}:'{Keywords[KeywordID]}']" : $"[{KeywordID}]";
                }
            });

            JsonEditor.Text = ReplaceSquareLinks;
            Refractor1.Width = 0;
            Refractor2.Width = 0;
        }

        private void Refractor2_Click_RMB(object sender, RoutedEventArgs e)
        {
            if (!JsonEditor.SelectedText.Equals(""))
            {
                JsonEditor.SelectedText = $"<style=\"upgradeHighlight\">{JsonEditor.SelectedText}</style>";
            }
            else
            {
                JsonEditor.Text = JsonEditor.Text.Insert(JsonEditor.CaretIndex, "<style=\"upgradeHighlight\"></style>");
            }
        }
        private void Refractor2_Click_LMB(object sender, RoutedEventArgs e)
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
                    //JsonEditor.SelectedText = $"<style=\"upgradeHighlight\">{JsonEditor.SelectedText}</style>";
                    JsonEditor.Text = JsonEditor.Text.Insert(JsonEditor.CaretIndex, "<style=\"upgradeHighlight\"></style>");
                }
                else if (EditorMode.Equals("Skills") | EditorMode.Equals("Passives"))
                {
                    //JsonEditor.SelectedText = $"<style=\"highlight\">{JsonEditor.SelectedText}</style>";
                    JsonEditor.Text = JsonEditor.Text.Insert(JsonEditor.CaretIndex, "<style=\"highlight\"></style>");
                }


                
            }
        }
        private void Refractor1_Shorthand_Changetype(object sender, RoutedEventArgs e)
        {
            if (EnableDynamicKeywords)
            {
                EnableDynamicKeywords = false;
                EnableDynamicKeywords_Display.Text = "Отключено";
            }
            else
            {
                EnableDynamicKeywords = true;
                EnableDynamicKeywords_Display.Text = "Включено";
            }
            MSettings.SaveSetting("Enable Dynamic Keywords", EnableDynamicKeywords ? "Yes" : "No");
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }
        private void BattleKeywords_Change_Type(object sender, RoutedEventArgs e)
        {
            if (BattleKeywords_Type.Equals("RU"))
            {
                (Keywords, KeywordIDName) = РазноеДругое.GetKeywords(from: "EN");
                Replacements = РазноеДругое.GetAddtReplacements(from: "EN");
                BattleKeywords_Type = "EN";
                BattleKeywords_TypeDisplay.Text = BattleKeywords_Type;
            }
            else if (BattleKeywords_Type.Equals("EN"))
            {
                (Keywords, KeywordIDName) = РазноеДругое.GetKeywords(from: "CN");
                Replacements = РазноеДругое.GetAddtReplacements(from: "CN");
                BattleKeywords_Type = "CN";
                BattleKeywords_TypeDisplay.Text = BattleKeywords_Type;
            }
            else if (BattleKeywords_Type.Equals("CN"))
            {
                (Keywords, KeywordIDName) = РазноеДругое.GetKeywords(from: "RU");
                Replacements = РазноеДругое.GetAddtReplacements(from: "RU");
                BattleKeywords_Type = "RU";
                BattleKeywords_TypeDisplay.Text = BattleKeywords_Type;
            }
            MSettings.SaveSetting("Keywords Type", BattleKeywords_Type);
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }

        private void Refractor_MouseEnter(object sender, MouseEventArgs e) => Refractor.Background = РазноеДругое.GetColorFromAHEX("#FF282828");
        private void Refractor_MouseLeave(object sender, MouseEventArgs e) => Refractor.Background = РазноеДругое.GetColorFromAHEX("#FF191919");
        private void Minimize_MouseEnter(object sender, MouseEventArgs e) => Minimize.Background = РазноеДругое.GetColorFromAHEX("#FF282828");
        private void Minimize_MouseLeave(object sender, MouseEventArgs e) => Minimize.Background = РазноеДругое.GetColorFromAHEX("#FF191919");
        private void WindowMode_MouseEnter(object sender, MouseEventArgs e) => WindowMode.Background = РазноеДругое.GetColorFromAHEX("#FF282828");
        private void WindowMode_MouseLeave(object sender, MouseEventArgs e) => WindowMode.Background = РазноеДругое.GetColorFromAHEX("#FF191919");
        private void Settings_MouseEnter(object sender, MouseEventArgs e) => Settings.Background = РазноеДругое.GetColorFromAHEX("#FF282828");
        private void Settings_MouseLeave(object sender, MouseEventArgs e) => Settings.Background = РазноеДругое.GetColorFromAHEX("#FF191919");
        private void Close_MouseEnter(object sender, MouseEventArgs e)
        {
            Close.Foreground = new SolidColorBrush(Colors.Black);
            Close.Background = РазноеДругое.GetColorFromAHEX("#FFf55442");
        }
        private void Close_MouseLeave(object sender, MouseEventArgs e)
        {
            Close.Background = РазноеДругое.GetColorFromAHEX("#FF191919");
            Close.Foreground = РазноеДругое.GetColorFromAHEX("#FFA69885");
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
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

        private void ToggleHighlight_Click(object sender, RoutedEventArgs e)
        {
            if (JsonEditor_EnableHighlight)
            {
                JsonEditor_EnableHighlight = false;
                ToggleHighlight_Text.Text = "Нет";
            }
            else
            {
                JsonEditor_EnableHighlight = true;
                ToggleHighlight_Text.Text = "Да";
            }
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);

            MSettings.SaveSetting("Enable <style> as color", ToggleHighlight_Text.Text.Equals("Да")? "Yes" : "No");
        }

        private void FontSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFont = FontSelector.SelectedItem.ToString().Split("{ Text = ")[1].Split(", FontFamily")[0];
            FontLabel.Content = selectedFont;
            JsonEditor_FontFamily = new FontFamily(selectedFont);
            FontLabel.FontFamily = JsonEditor_FontFamily;
            JsonEditor.FontFamily = JsonEditor_FontFamily;

            MSettings.SaveSetting("JsonEditor Font", selectedFont);
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
            OverrideCover1.Margin = new Thickness(1000);
            OverrideCover2.Margin = new Thickness(1000);
            SettingsDialog.Margin = new Thickness(1000);
        }

        private void Reload_Sprites(object sender, RoutedEventArgs e)
        {
            SpriteBitmaps = РазноеДругое.GetSpritesBitmaps();
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }

        private void Reload_Keywords(object sender, RoutedEventArgs e)
        {
            (Keywords, KeywordIDName) = РазноеДругое.GetKeywords();
            Replacements = РазноеДругое.GetAddtReplacements();
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }
        
        private void Reload_Colors(object sender, RoutedEventArgs e)
        {
            ColorPairs = РазноеДругое.GetColorPairs();
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }

        private void OverrideCover2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OverrideCover1.Margin = new Thickness(1000);
            OverrideCover2.Margin = new Thickness(1000);
            SettingsDialog.Margin = new Thickness(1000);
        }

        private void OverrideCover1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OverrideCover1.Margin = new Thickness(1000);
            OverrideCover2.Margin = new Thickness(1000);
            SettingsDialog.Margin = new Thickness(1000);
        }

        private void Shorthand_TypeChange(object sender, RoutedEventArgs e)
        {
            if (Shorthand_Type == "Knightey")
            {
                Shorthand_Type = "kimght";
            }
            else
            {
                Shorthand_Type = "Knightey";
            }
            MSettings.SaveSetting("Shorthand Type", Shorthand_Type);
            Shorthand_TypeDisplay.Text = Shorthand_Type;
            UpdatePreview(LastPreviewUpdateText, LastPreviewUpdateTarget);
        }





        private void DiffFile_SelectFile_Input_TextChanged(object sender, TextChangedEventArgs e)
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

        private void DiffFile_SelectFile_Click(object sender, RoutedEventArgs e)
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
                TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка при чтении файла", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
