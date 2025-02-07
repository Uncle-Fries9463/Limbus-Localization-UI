using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Limbus_Localization_UI.Additions;

namespace Limbus_Localization_UI
{
    public partial class MainWindow : Window
    {
        readonly static Dictionary<string, BitmapImage> SpriteBitmaps = РазноеДругое.GetSpritesBitmaps();
        public static Dictionary<string, Tuple<Button, StackPanel, RichTextBox, TextBox, Label, Border, Image>> StaticLinks = new();

        static string Json_Filepath;
        static Dictionary<int, Dictionary<string, object>> EGOgift_Json_Dictionary; // В случае редактирования ЭГО даров они редактируются через удобный словарь, а не Json элемент
        static Dictionary<int, Dictionary<string, object>> EGOgift_EditBuffer = new();      // Буфер не сохранённых изменений (копия верхнего, но с "{unedited}" вместо строк)
        static int Json_Dictionary_CurrentID = -1; // ID редактруемого ЭГО дара
        static List<int> JsonKeys; // Список всех ID 
        string CurrentDesc = "Desc"; // Текущее редактруемое поле
        string JsonFilepath_FullPath = "";

        // Список ссылок на все объекты интерфейса для обращения к ним в других классах без мороки с MVVM
        public static void InitializeStaticLinks()
        {
            StaticLinks = new()
            {

            };
        }

        public MainWindow()
        {
            InitializeComponent();


            InitializeStaticLinks();
            StartInits();
        }

        private void StartInits()
        {
            EGOgift_Background.Source = SpriteBitmaps[@"$Другое\Фон.png"];
            JsonFile_SelectFile_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-text-document-32329.png"] };
            JsonFile_ReadFile.Content = new Image { Source = SpriteBitmaps[@"$Другое\free_icon_book_14095035.png"], Margin = new Thickness(-1, 0, 0, 0) };
            ID_SwitchPrev.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-growth-4534634_1.png"], Margin = new Thickness(-1, 0, 0, 0) };
            ID_SwitchNext.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-growth-4534634_2.png"], Margin = new Thickness(1, 0, 0, 0) };
            ID_JumpTo.Content = new Image { Source = SpriteBitmaps[@"$Другое\free_icon_book_14095035.png"], Margin = new Thickness(-2, 0, 0, 0) };

            Name_Change_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };

            Desc_Change_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
            SubDesc1_Change_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
            SubDesc2_Change_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
            SubDesc3_Change_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
            SubDesc4_Change_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
            SubDesc5_Change_Button.Content = new Image { Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };

            string Def = "Статусные эффекты, <style=\\\"upgradeHighlight\\\">Подсветка улучшения</style>\n<sprite name=\\\"Breath\\\"><color=#f8c200>Дыхание</color>\n<sprite name=\\\"Charge\\\"><color=#f8c200>Заряд</color>\n<sprite name=\\\"Laceration\\\"><color=#e30000>Кровотечение</color>\n<sprite name=\\\"Combustion\\\"><color=#e30000>Огонь</color>\n<sprite name=\\\"Burst\\\"><color=#e30000>Разрыв</color>\n<sprite name=\\\"Sinking\\\"><color=#e30000>Утопание</color>\n<sprite name=\\\"Vibration\\\"><color=#e30000>Тремор</color>\n - <sprite name=\\\"VibrationExplosion\\\"><color=#e30000>Провоцирование тремора</color>\n\n\n<sprite name=\\\"Какой то\\\"><color=#e30000>Неизвестный спрайт</color>\\nПоломанный текст:\\n             < Поломанный текст после незакрытой острой скобки, всегда закрывайте их<sprite name=\\\"Burst\\\"><color=#e30000>Разрыв</color>";

            PreviewLayout_EGOgift.SetValue(Paragraph.LineHeightProperty, 30.0);

            JsonEditor.Text = Def;
        }










        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            PreviewLayout_EGOgift.Document.Blocks.Clear();
            // Обновить предпросмотр
            UpdatePreview(JsonEditor.Text);

            // Проверить буфер не сохранённых изменений для обозначения соответсвующих кнопок звёздочкой
            if (Json_Dictionary_CurrentID != -1)
            {
                switch (CurrentDesc)
                {
                    case "Desc":
                        if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["Desc"]))
                        {
                            SwitchEditorTo_Desc.Content = "Описание";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                        }
                        else
                        {
                            SwitchEditorTo_Desc.Content = "Описание*";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["Desc"] = JsonEditor.Text;
                        }
                        break;

                    case "SimpleDesc1":
                        if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc1"]))
                        {
                            SwitchEditorTo_SubDesc1.Content = "Простое описание 1";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc1"] = "{unedited}";
                        }
                        else
                        {
                            SwitchEditorTo_SubDesc1.Content = "Простое описание 1*";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc1"] = JsonEditor.Text;
                        }
                        break;

                    case "SimpleDesc2":
                        if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc2"]))
                        {
                            SwitchEditorTo_SubDesc2.Content = "Простое описание 2";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc2"] = "{unedited}";
                        }
                        else
                        {
                            SwitchEditorTo_SubDesc2.Content = "Простое описание 2*";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc2"] = JsonEditor.Text;
                        }
                        break;

                    case "SimpleDesc3":
                        if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc3"]))
                        {
                            SwitchEditorTo_SubDesc3.Content = "Простое описание 3";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc3"] = "{unedited}";
                        }
                        else
                        {
                            SwitchEditorTo_SubDesc3.Content = "Простое описание 3*";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc3"] = JsonEditor.Text;
                        }
                        break;

                    case "SimpleDesc4":
                        if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc4"]))
                        {
                            SwitchEditorTo_SubDesc4.Content = "Простое описание 4";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc4"] = "{unedited}";
                        }
                        else
                        {
                            SwitchEditorTo_SubDesc4.Content = "Простое описание 4*";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc4"] = JsonEditor.Text;
                        }
                        break;

                    case "SimpleDesc5":
                        if (JsonEditor.Text.Equals(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc5"]))
                        {
                            SwitchEditorTo_SubDesc5.Content = "Простое описание 5";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc5"] = "{unedited}";
                        }
                        else
                        {
                            SwitchEditorTo_SubDesc5.Content = "Простое описание 5*";
                            EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc5"] = JsonEditor.Text;
                        }
                        break;

                }
            }
        }


        public bool WordWrap_WithSprites = true;

        private void AddText(string text, RichTextBox Target)
        {
            var document = Target.Document;
            if (document.Blocks.LastBlock is not Paragraph lastParagraph)
            {
                lastParagraph = new Paragraph();
                document.Blocks.Add(lastParagraph);
            }

            string[] TextParts = Regex.Split(text, @"<color=(#[0-9a-fA-F]{6})>(.*?)</color>");

            for (int i = 0; i < TextParts.Length; i++)
            {
                if (i % 3 == 0 & TextParts[i] != "" & TextParts[i] != "\0") // Цветной текст
                {
                    lastParagraph.Inlines.Add(new Run(TextParts[i]));
                }
                else if (i % 3 == 1) // Цветной текст
                {
                    if (ColorConverter.ConvertFromString(TextParts[i]) is Color color)
                    {
                        Run coloredRun = new(TextParts[i + 1])
                        {
                            Foreground = new SolidColorBrush(color),
                        };

                        // Если цвет соответствует статусному эффекту
                        if (TextParts[i] == "#e30000" | TextParts[i] == "#fac400" | TextParts[i] == "#ffffff") coloredRun.TextDecorations = TextDecorations.Underline;
                        lastParagraph.Inlines.Add(coloredRun);
                    }
                }
            }
        }

        public bool Is_OneWord_Queued = false;
        public bool Is_MultWords_Queued = false;

        private void AddSprite(string SpriteName, RichTextBox Target, bool IsOneWord = true, string NextWord = "Кто это?", string NextColor = "#fac400")
        {
            if (IsOneWord & WordWrap_WithSprites) Is_OneWord_Queued = true;

            var document = Target.Document;
            var lastParagraph = document.Blocks.LastBlock as Paragraph;
            if (lastParagraph == null)
            {
                lastParagraph = new Paragraph();
                document.Blocks.Add(lastParagraph);
            }

            if (IsOneWord & WordWrap_WithSprites || (Is_MultWords_Queued & WordWrap_WithSprites))
            {
                BitmapImage source;
                if (SpriteBitmaps.ContainsKey(SpriteName)) source = SpriteBitmaps[SpriteName];
                else source = SpriteBitmaps[@"Unknown.png"];
                Image SpriteImage = new()
                {
                    Source = source,
                    Width = 23,
                    Height = 23,
                    Margin = new Thickness(-2, -1, -2, 0)
                };

                StackPanel SpritePlusEffectname = new()
                {
                    Orientation = Orientation.Horizontal,
                    //Background = new SolidColorBrush(Colors.Gray), // Граница контейнера спрайта и названия статусного эффекта (Для цельного переноса на новую строку)
                };
                SpritePlusEffectname.Children.Add(new TextBlock(new InlineUIContainer(SpriteImage)));
                Run EffectName = new(NextWord) {TextDecorations = TextDecorations.Underline};

                if (ColorConverter.ConvertFromString(NextColor) is Color color)
                {
                    EffectName.Foreground = new SolidColorBrush(color);
                }

                SpritePlusEffectname.Children.Add(new TextBlock(EffectName));
                InlineUIContainer SpritePlusEffectname_Container = new(SpritePlusEffectname);

                // 24 line height = |4.5| 'y' value
                // 30 line height = |10|  'y' value
                SpritePlusEffectname.Margin = new Thickness(0, -11, 0, 0);
                SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 10.6); // 'y' value- (~0.5), может быть, наверное

                SpritePlusEffectname.VerticalAlignment = VerticalAlignment.Bottom;

                lastParagraph.Inlines.Add(SpritePlusEffectname_Container);
            }
            else
            {
                Image SpriteImage = new()
                {
                    Source = SpriteBitmaps[SpriteName],
                    Width = 23,
                    Height = 23,
                    Margin = new Thickness(-2, -1, -2, 0),
                };

                lastParagraph.Inlines.Add(new InlineUIContainer(SpriteImage));
            }
        }


        /// <summary>
        /// Обработать вводимый Json desc текст и вывести в окно
        /// </summary>
        private void UpdatePreview(string JsonDesc)
        {
            JsonDesc = JsonDesc.Replace("color=#None", "color=#ffffff")
                               .Replace("<style=\\\"highlight\\\">", "<style=\\\"upgradeHighlight\\\">") // Подсветка изменений подобно upgradeHighlight, но для навыков
                               .Replace("<style=\\\"upgradeHighlight\\\">", "<color=#f8c200>")
                               .Replace("</style>", "</color>")
                               .Replace("</link>", "")
                               .Replace("<u>", "")
                               .Replace("</u>", "")
                               .Replace("<>", "<s>")
                               .Replace("><", ">\0<");

            JsonDesc = Regex.Replace(JsonDesc, @"(?<=<\/color>)([а-яА-Яa-zA-Z])", " $1");
            JsonDesc = Regex.Replace(JsonDesc, @"<link=\\\"".*?\\\"">", "");
            JsonDesc = JsonDesc.Replace("\">\0<color=#f8c200>", "\">\0<color=#fac400>");

            char[] splitby = { '<', '>' };
            string[] parts = $"<s>\0{JsonDesc.Replace("\\n", "\n")}".Split(splitby, StringSplitOptions.RemoveEmptyEntries);

            string[] Colors = {
                "color=#e30000", // Красный текст, Негативные статусные эффекты, подчёркивается
                "color=#fac400", // Жёлтый текст, Позитивные статусные эффекты (Спешка, Повышение уровня атаки, ..) + Заряд и Дыхание, подчёркивается
                "color=#ffffff",  // Белый текст (У Дон Кихот в идентичности Менеджера Ла-Манчалаенда эффект Кровавая броня) 

                "color=#f8c200", // Подсветка изменений в ЭГО Даре при улучшении (<style=\"upgradeHighlight\">XXX</style> в англ файле локализации (вполне работает), <color=#f8c200>XXX</color> в существующем переводе)
            };

            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    try
                    {
                        if (!Colors.Contains(parts[i - 1]))
                        {
                            AddText(parts[i], PreviewLayout_EGOgift);
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        string NextWord  = "?";
                        string NextColor = "f8c200";

                        string[] NextWordCheck;
                        int NextWordCheck_Count;

                        bool IsOneWord = false;

                        if (WordWrap_WithSprites)
                        {
                            try
                            {
                                NextWord = parts[i + 3];
                            }
                            catch { }

                            try
                            {
                                NextColor = parts[i + 2].Split("=")[^1];
                            }
                            catch { }
                        }


                        if (parts[i].StartsWith("sprite name"))
                        {
                            NextWordCheck = NextWord.Split(' ');
                            NextWordCheck_Count = NextWordCheck.Count();
                            if (NextWordCheck_Count == 1)
                            {
                                IsOneWord = true;
                            }
                            else
                            {
                                Is_MultWords_Queued = true;
                                NextWord = NextWordCheck[0];
                            }

                            string spritename = parts[i].Split("\\\"")[1];
                            if (SpriteBitmaps.ContainsKey($"{spritename}.png")) spritename = $"{spritename}.png";
                            else if (SpriteBitmaps.ContainsKey($"{spritename}.webp")) spritename = $"{spritename}.webp";
                            try
                            {
                                AddSprite(spritename, PreviewLayout_EGOgift, NextWord: NextWord, NextColor: NextColor, IsOneWord: IsOneWord);
                            }
                            catch
                            {
                                //AddSprite(@"Unknown.png", PreviewLayout_EGOgift);
                            }
                        }


                        else if (parts[i].StartsWith("color"))
                        {
                            if (!Is_OneWord_Queued)
                            {
                                string AddT = parts[i + 1];
                                if (Is_MultWords_Queued)
                                {
                                    AddT = " " + String.Join(' ', parts[i + 1].Split(' ')[1..]);
                                    Is_MultWords_Queued = false;
                                }
                                switch ("<" + parts[i] + ">")
                                {
                                    case "<color=#e30000>":
                                        AddText($"<color=#e30000>{AddT}</color>", PreviewLayout_EGOgift);
                                        break;

                                    case "<color=#f8c200>":
                                        AddText($"<color=#f8c200>{AddT}</color>", PreviewLayout_EGOgift);
                                        break;

                                    case "<color=#fac400>":
                                        AddText($"<color=#fac400>{AddT}</color>", PreviewLayout_EGOgift);
                                        break;
                                    
                                    case "<color=#ffffff>":
                                        AddText($"<color=#ffffff>{AddT}</color>", PreviewLayout_EGOgift);
                                        break;

                                    default: break;
                                }
                            }
                            else Is_OneWord_Queued = false;
                        }
                    }
                    catch
                    {
                        //AddSprite(@"Unknown.png", PreviewLayout_EGOgift);
                    }
                }
            }
        }







        public static void Window_NoSpritesFolder()
        {
            MessageBoxResult result = MessageBox.Show("Папка \"Спрайты\" потерялась, она должна быть прямо тут", "Что-то не так", MessageBoxButton.OK, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK) Environment.Exit(0);
        }
        

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
            if (Json_Dictionary_CurrentID != -1)
            {
                (string, string) ExitData = JsonLoader_EGOgifts.GetUnsavedChanges(EGOgift_EditBuffer);

                if (Convert.ToInt32(ExitData.Item2) != 0)
                {
                    OverrideCover1.Margin = new Thickness(0);
                    OverrideCover2.Margin = new Thickness(0);
                    ExitDialog.Margin = new Thickness(0);

                    e.Cancel = true;
                    UnsavedChangesTooltip_Text.Text = ExitData.Item1.Trim();
                    UnsavedChangesCount.Text = ExitData.Item2;
                }

            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            JsonEditor.SetValue(HeightProperty, this.ActualHeight - 320);
            if (this.Height == 320)
            {
                this.Height = 320.1;
                this.MaxWidth = 600;
            }
            else this.MaxWidth = 900;
        }
















        private static void BackgroundShadowTextCheck(TextBox TextBox, Label Label, string Label_DefaultText)
        {
            if(TextBox.Text != "") Label.Content = "";
            else Label.Content = Label_DefaultText;
        }

        private void Check_JsonFilepath_bgtext()     => BackgroundShadowTextCheck(JsonFilepath,   JsonFilepath_bgtext, "Путь к Json файлу");
        //private void Check_Name_Label_bgtext()       => BackgroundShadowTextCheck(Name_EditBox,   Name_Label_bgtext,   "Название ЭГО Дара");
        private void Check_JumpToID_bgtext() => BackgroundShadowTextCheck(JumpToID_Input, JumpToID_bgtext,     "Перейти к ID..");


        private void JsonPath_TextChanged(object sender, TextChangedEventArgs e) => BackgroundShadowTextCheck(JsonFilepath, JsonFilepath_bgtext, "Путь к Json файлу");
        private void Name_EditBox_TextChanged(object sender, TextChangedEventArgs e) => BackgroundShadowTextCheck(Name_EditBox, Name_Label_bgtext, "Название ЭГО Дара");
        private void JumpToID_Input_TextChanged(object sender, TextChangedEventArgs e) => BackgroundShadowTextCheck(JumpToID_Input, JumpToID_bgtext, "Перейти к ID..");




        private void IDSwitch_CheckEditBufferDescs()
        {
            if (!EGOgift_EditBuffer[Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                SwitchEditorTo_Desc.Content = "Описание*";
            else
                SwitchEditorTo_Desc.Content = "Описание";


            if (!EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc1"].Equals("{unedited}"))
                SwitchEditorTo_SubDesc1.Content = "Простое описание 1*";
            else
                SwitchEditorTo_SubDesc1.Content = "Простое описание 1";


            if (!EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc2"].Equals("{unedited}"))
                SwitchEditorTo_SubDesc2.Content = "Простое описание 2*";
            else
                SwitchEditorTo_SubDesc2.Content = "Простое описание 2";


            if (!EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc3"].Equals("{unedited}"))
                SwitchEditorTo_SubDesc3.Content = "Простое описание 3*";
            else
                SwitchEditorTo_SubDesc3.Content = "Простое описание 3";


            if (!EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc4"].Equals("{unedited}"))
                SwitchEditorTo_SubDesc4.Content = "Простое описание 4*";
            else
                SwitchEditorTo_SubDesc4.Content = "Простое описание 4";


            if (!EGOgift_EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc5"].Equals("{unedited}"))
                SwitchEditorTo_SubDesc5.Content = "Простое описание 5*";
            else
                SwitchEditorTo_SubDesc5.Content = "Простое описание 5";
        }



        private void SwitchToID(int ID)
        {
            Json_Dictionary_CurrentID = ID;
            IDSwitch_CheckEditBufferDescs();
            ID_Switch_CheckButtons();
           

            Name_Label.Text = Convert.ToString(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["Name"]);
            Name_EditBox.Text = Convert.ToString(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["Name"]);
            if (!EGOgift_Json_Dictionary[ID]["SimpleDesc2"].Equals("{none}"))
            {
                SubDesc2_Cover.Height = 0;
                SubDesc2_Change_Cover.Height = 0;
            }
            else 
            {
                SubDesc2_Cover.Height = 30;
                SubDesc2_Change_Cover.Height = 30;
            }

            if (!EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc3"].Equals("{none}"))
            {
                SubDesc3_Cover.Height = 0;
                SubDesc3_Change_Cover.Height = 0;
            }
            else
            {
                SubDesc3_Cover.Height = 30;
                SubDesc3_Change_Cover.Height = 30;
            }
            
            if (!EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc4"].Equals("{none}"))
            {
                SubDesc4_Cover.Height = 0;
                SubDesc4_Change_Cover.Height = 0;
            }
            else
            {
                SubDesc4_Cover.Height = 30;
                SubDesc4_Change_Cover.Height = 30;
            }
            
            if (!EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc5"].Equals("{none}"))
            {
                SubDesc5_Cover.Height = 0;
                SubDesc5_Change_Cover.Height = 0;
            }
            else
            {
                SubDesc5_Cover.Height = 30;
                SubDesc5_Change_Cover.Height = 30;
            }
            ID_Copy_Button.Content = Json_Dictionary_CurrentID;

            CurrentDesc = "Desc";
            if (EGOgift_EditBuffer[Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
            {
                JsonEditor.Text = Convert.ToString(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["Desc"]);
            }
            else
            {
                JsonEditor.Text = Convert.ToString(EGOgift_EditBuffer[Json_Dictionary_CurrentID]["Desc"]);
            }
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
            TB.Foreground = РазноеДругое.GetColorFromHEXA("#FF191919"); ;

            LB.Content = WarningText;

            for (int i = 1; i <= rounds; i++)
            {
                LB.Foreground = РазноеДругое.GetColorFromHEXA("#FFFFA4A4");
                await Task.Delay(TimerAwait);
                LB.Foreground = РазноеДругое.GetColorFromHEXA("#FFF43D3D");
                await Task.Delay(TimerAwait);
            }
            await Task.Delay(AfterAwait);

            LB.Foreground = РазноеДругое.GetColorFromHEXA("#FF514C46");
            LB.Content = LabelTextAfter;
            TB.Focusable = true;
            TB.Foreground = РазноеДругое.GetColorFromHEXA("#FFA69885");

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
                    JsonFilepath.Text = filename;
                    LoadJsonFile(filename);
                }
            }
            catch
            {
                TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка при чтении файла", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
            }
        }

        private async void Notify(string text)
        {
            JsonFilepath.Focusable = false;
            JsonFilepath.Foreground = РазноеДругое.GetColorFromHEXA("#FF191919");
            JsonFilepath_bgtext.Content = text;
            JsonFilepath_bgtext.Foreground = РазноеДругое.GetColorFromHEXA("#FF39CF2B");
            await Task.Delay(1150);
            JsonFilepath_bgtext.Content = "";
            JsonFilepath.Focusable = true;
            JsonFilepath.Foreground = РазноеДругое.GetColorFromHEXA("#FFA69885");
        }

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
                    //EGOgift mode
                    Json_Filepath = JsonFilepath.Text;
                    JsonFilepath_FullPath = path; 
                    EGOgift_Json_Dictionary = JsonLoader_EGOgifts.Json_GetAdressBook(Json_Filepath);

                    JsonKeys = EGOgift_Json_Dictionary.Keys.ToList();
                    foreach(var ID in JsonKeys)
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
                    Name_ChangeNameInput_Cover.Height = 0;
                    Name_ChangeName_Cover.Height = 0;

                    Desc_Cover.Height = 0;
                    Desc_Change_Cover.Height = 0;

                    SubDesc1_Cover.Height = 0;
                    SubDesc1_Change_Cover.Height = 0;


                    Json_Dictionary_CurrentID = JsonKeys[0];
                    SwitchToID(Json_Dictionary_CurrentID);

                    РазноеДругое.SetRO(path);

                    Notify("Json загружен");
                }
                catch
                {
                    JsonFilepath.Text = "";
                    TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Неверный формат", "Путь к Json файлу", "Check_JsonFilepath_bgtext");
                }
            }
        }

        private void LoadJsonFile_Button(object sender, RoutedEventArgs e)
        {
            string JsonFilepath_PATH = JsonFilepath.Text.Trim();
            LoadJsonFile(JsonFilepath_PATH);
        }



        private void ID_Switch_CheckButtons()
        {
            ID_SwitchPrev_Cover.Height = 0;
            ID_SwitchNext_Cover.Height = 0;
            try { var PrevCheck = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) - 1]; }
            catch { ID_SwitchPrev_Cover.Height = 16; }
            try { var NextCheck = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) + 1]; }
            catch { ID_SwitchNext_Cover.Height = 16; }
        }
        private void ID_SwitchPrev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Json_Dictionary_CurrentID = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) - 1];
                SwitchToID(Json_Dictionary_CurrentID);

                ID_Switch_CheckButtons();
            }
            catch { }
        }

        
        private void ID_SwitchNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Json_Dictionary_CurrentID = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) + 1];
                SwitchToID(Json_Dictionary_CurrentID);

                ID_Switch_CheckButtons();
            }
            catch { }
        }


        private void CheckEditBuffer(string CurrentDesc)
        {
            try
            {
                if (EGOgift_EditBuffer[Json_Dictionary_CurrentID][CurrentDesc].Equals("{unedited}"))
                {
                    JsonEditor.Text = Convert.ToString(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID][CurrentDesc]);
                }
                else
                {
                    JsonEditor.Text = Convert.ToString(EGOgift_EditBuffer[Json_Dictionary_CurrentID][CurrentDesc]);
                }
            }
            catch { }
        }


        private void ResetUndo()
        {
            JsonEditor.IsUndoEnabled = false;
            JsonEditor.IsUndoEnabled = true;
        }
        private void SwitchEditorTo_Desc_Button(object sender, RoutedEventArgs e)       
        {
            try{
                CurrentDesc = "Desc";
                CheckEditBuffer("Desc");
                ResetUndo();
            }catch{}
        }
        private void SwitchEditorTo_SubDesc1_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc1";
                CheckEditBuffer("SimpleDesc1");
                ResetUndo();
            }catch {}
        }
        private void SwitchEditorTo_SubDesc2_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc2";
                CheckEditBuffer("SimpleDesc2");
                ResetUndo();
            }catch{}
        }
        private void SwitchEditorTo_SubDesc3_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc3";
                CheckEditBuffer("SimpleDesc3");
                ResetUndo();
            }catch{}
        }
        private void SwitchEditorTo_SubDesc4_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc4";
                CheckEditBuffer("SimpleDesc4");
                ResetUndo();
            }catch{}
        }
        private void SwitchEditorTo_SubDesc5_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc5";
                CheckEditBuffer("SimpleDesc5");
                ResetUndo();
            }catch{}
        }

        private void JumpToID_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SwitchToID(Convert.ToInt32(JumpToID_Input.Text));
                JumpToID_Input.Text = "";
                Check_JsonFilepath_bgtext();
                ID_Switch_CheckButtons();
            }
            catch
            {
                TextBoxFlashWarning(JumpToID_Input, JumpToID_bgtext, "ID Не найден", "Перейти к ID..", "Check_JumpToID_bgtext");
            }
        }

        private async void ID_Copy(object sender, RoutedEventArgs e)
        {
            if (Json_Dictionary_CurrentID != -1)
            {
                Clipboard.SetText(Convert.ToString(ID_Copy_Button.Content));
                ID_Copy_Button.Content = "ID Скопирован";

                await Task.Delay(890);
                ID_Copy_Button.Content = Json_Dictionary_CurrentID;
            }
        }









        private void Name_ChangeName(object sender, RoutedEventArgs e)
        {
            try{
                РазноеДругое.SetRW(JsonFilepath_FullPath);
                РазноеДругое.RewriteFileLine($"      \"name\": \"{Name_EditBox.Text}\",",
                                Json_Filepath,
                                Convert.ToInt32(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["LineIndex_Name"]));
                РазноеДругое.SetRO(JsonFilepath_FullPath);
                Name_Label.Text = Name_EditBox.Text;
                EGOgift_Json_Dictionary[Json_Dictionary_CurrentID]["Name"] = Name_EditBox.Text;
            } catch { TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка сохранения", "Путь к Json файлу", "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600); }
        }

        private void Desc_ChangeOver(string ThisDesc)
        {
            try
            {
                if (!EGOgift_EditBuffer[Json_Dictionary_CurrentID][ThisDesc].Equals("{unedited}"))
                {
                    РазноеДругое.SetRW(JsonFilepath_FullPath);
                    РазноеДругое.RewriteFileLine($"{(ThisDesc.StartsWith("SimpleDesc") ? "\"simpleDesc\": \"" : "\"desc\": \"")}" +
                                                 $"{Convert.ToString(EGOgift_EditBuffer[Json_Dictionary_CurrentID][ThisDesc]).Replace("\n", "\\n").Replace("\r", "\\n")}" +
                                                 $"{(ThisDesc.StartsWith("SimpleDesc") ? "\"" : "\",")}",
                                                 Json_Filepath,
                                                 Convert.ToInt32(EGOgift_Json_Dictionary[Json_Dictionary_CurrentID][$"LineIndex_{ThisDesc}"]));

                    EGOgift_Json_Dictionary[Json_Dictionary_CurrentID][ThisDesc] = EGOgift_EditBuffer[Json_Dictionary_CurrentID][ThisDesc];
                    EGOgift_EditBuffer[Json_Dictionary_CurrentID][ThisDesc] = "{unedited}";
                    IDSwitch_CheckEditBufferDescs();
                    //Notify("Файл обновлён");
                    РазноеДругое.SetRO(JsonFilepath_FullPath);
                }
            }
            catch { TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка сохранения", "Путь к Json файлу", "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600); }
        }


        private void Desc_ChangeDesc(object sender, RoutedEventArgs e) => Desc_ChangeOver("Desc");
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
                    Desc_ChangeOver(CurrentDesc);
                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S) isCtrlSPressed = false;
        }
    }
}
