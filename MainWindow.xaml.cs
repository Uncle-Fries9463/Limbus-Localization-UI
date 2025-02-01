using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Limbus_Localization_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly static Dictionary<string, BitmapImage> SpriteBitmaps = РазноеДругое.GetSpritesBitmaps();

        static string Json_Filepath_str;
        static Dictionary<int, Dictionary<string, object>> Json_Dictionary;
        static Dictionary<int, Dictionary<string, object>> EditBuffer = new();
        static int Json_Dictionary_CurrentID = -1;
        static List<int> JsonKeys;
        string CurrentDesc = "Desc";
        string JsonFilepath_PATH = "";

        public MainWindow()
        {
            InitializeComponent();

            TextBG.Source = SpriteBitmaps[@"$Другое\Фон.png"];
            JsonFile_SelectFile_Button.Content     = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-text-document-32329.png"]};
            JsonFilepath_ReadFile.Content          = new Image{Source = SpriteBitmaps[@"$Другое\free_icon_book_14095035.png"   ], Margin = new Thickness(-1, 0, 0, 0)};
            EgoGift_ID_SwitchPrev.Content          = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-growth-4534634_1.png"], Margin = new Thickness(-1, 0, 0, 0)};
            EgoGift_ID_SwitchNext.Content          = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-growth-4534634_2.png"], Margin = new Thickness( 1, 0, 0, 0)};
            EgoGift_JumpToID_Button.Content        = new Image{Source = SpriteBitmaps[@"$Другое\free_icon_book_14095035.png"   ], Margin = new Thickness(-2, 0, 0, 0)};
            EgoGift_Name_ChangeName_Button.Content = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png" ]};
            
            EgoGift_Desc_ChangeDesc_Button.Content = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
              EgoGift_Desc_SimpleDesc1_Button.Content = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
              EgoGift_Desc_SimpleDesc2_Button.Content = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
              EgoGift_Desc_SimpleDesc3_Button.Content = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
              EgoGift_Desc_SimpleDesc4_Button.Content = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };
              EgoGift_Desc_SimpleDesc5_Button.Content = new Image{Source = SpriteBitmaps[@"$Другое\free-icon-checked_4736822.png"] };

            PreviewLayout.Document.Blocks.Clear();
            string Def = "Статусные эффекты, <style=\\\"upgradeHighlight\\\">Подсветка улучшения</style>\r\n<sprite name=\\\"Breath\\\"><color=#f8c200>Дыхание</color>\r\n<sprite name=\\\"Charge\\\"><color=#f8c200>Заряд</color>\r\n<sprite name=\\\"Laceration\\\"><color=#e30000>Кровотечение</color>\r\n<sprite name=\\\"Combustion\\\"><color=#e30000>Огонь</color>\r\n<sprite name=\\\"Burst\\\"><color=#e30000>Разрыв</color>\r\n<sprite name=\\\"Sinking\\\"><color=#e30000>Утопание</color>\r\n<sprite name=\\\"Vibration\\\"><color=#e30000>Тремор</color>\r\n - <sprite name=\\\"VibrationExplosion\\\"><color=#e30000>Провоцирование тремора</color>\r\n\r\n<sprite name=\\\"Ктокто\\\"><color=#e30000>Неправильный спрайт</color>\r\n<color=#e30000>Неправильный поломанный текст (цвет используется без спрайта, для подсветки улучшения ЭГО Дара должен использоваться вышеуказанный <style=\\\"upgradeHighlight\\\">upgradeHighlight</style>)\r\n\r\n< Поломанный текст после незакрытой острой скобки, всегда закрывайте их\r\n<sprite name=\\\"Burst\\\"><color=#e30000>Разрыв</color>";

            //Def = "Конец хода: даёт 1 <sprite name=\\\"Agility\\\"><color=#f8c200><u><link=\\\"Agility\\\">Спешку</link></u></color>, 1 <sprite name=\\\"AttackUp\\\"><color=#f8c200><u><link=\\\"AttackUp\\\">Повышение уровня атаки</link></u></color></style> <style=\\\"upgradeHighlight\\\">и 1</style> <sprite name=\\\"AttackDmgUp\\\"><color=#f8c200><u><link=\\\"AttackDmgUp\\\">Повышение урона</link></u></color> на следующий ход 1 грешнику с наибольшим потенциалом <sprite name=\\\"Breath\\\"><color=#f8c200><u><link=\\\"Breath\\\">Дыхания</link></u></color> и ещё 1 грешнику с наибольшим счётчиком <sprite name=\\\"Breath\\\"><color=#f8c200><u><link=\\\"Breath\\\">Дыхания</link></u></color>. (Оба эффекта могу быть применены к одной идентичности)\n\nЕсли же грешник имеет Атакующий навык с Грехом похоти, тогда даёт 2 <sprite name=\\\"Agility\\\"><color=#f8c200><u><link=\\\"Agility\\\">Спешки</link></u></color>, 2 <sprite name=\\\"AttackUp\\\"><color=#f8c200><u><link=\\\"AttackUp\\\">Повышения уровня атаки</link></u></color></style> <style=\\\"upgradeHighlight\\\">и 2</style> <sprite name=\\\"AttackDmgUp\\\"><color=#f8c200><u><link=\\\"AttackDmgUp\\\">Повышения урона</link></u></color>. (ЭГО навыки не учитываются)";

            PreviewLayout.SetValue(Paragraph.LineHeightProperty, 30.0);

            InputJson.Text = Def;
        }

        private async void TextChanged(object sender, TextChangedEventArgs e)
        {
            PreviewLayout.Document.Blocks.Clear();
            UpdatePreview(InputJson.Text);

            await Task.Delay(10);
            if (Json_Dictionary_CurrentID != -1)
            {
                switch (CurrentDesc)
                {
                    case "Desc":
                        if (InputJson.Text.Equals(Json_Dictionary[Json_Dictionary_CurrentID]["Desc"]))
                        {
                            EgoGift_SwitchEditorTo_Desc.Content = "Описание";
                            EditBuffer[Json_Dictionary_CurrentID]["Desc"] = "{unedited}";
                        }
                        else
                        {
                            EgoGift_SwitchEditorTo_Desc.Content = "Описание*";
                            EditBuffer[Json_Dictionary_CurrentID]["Desc"] = InputJson.Text;
                        }
                        break;

                    case "SimpleDesc1":
                        if (InputJson.Text.Equals(Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc1"]))
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc1.Content = "Простое описание 1";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc1"] = "{unedited}";
                        }
                        else
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc1.Content = "Простое описание 1*";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc1"] = InputJson.Text;
                        }
                        break;

                    case "SimpleDesc2":
                        if (InputJson.Text.Equals(Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc2"]))
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc2.Content = "Простое описание 2";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc2"] = "{unedited}";
                        }
                        else
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc2.Content = "Простое описание 2*";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc2"] = InputJson.Text;
                        }
                        break;

                    case "SimpleDesc3":
                        if (InputJson.Text.Equals(Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc3"]))
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc3.Content = "Простое описание 3";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc3"] = "{unedited}";
                        }
                        else
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc3.Content = "Простое описание 3*";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc3"] = InputJson.Text;
                        }
                        break;

                    case "SimpleDesc4":
                        if (InputJson.Text.Equals(Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc4"]))
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc4.Content = "Простое описание 4";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc4"] = "{unedited}";
                        }
                        else
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc4.Content = "Простое описание 4*";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc4"] = InputJson.Text;
                        }
                        break;

                    case "SimpleDesc5":
                        if (InputJson.Text.Equals(Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc5"]))
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc5.Content = "Простое описание 5";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc5"] = "{unedited}";
                        }
                        else
                        {
                            EgoGift_SwitchEditorTo_SimpleDesc5.Content = "Простое описание 5*";
                            EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc5"] = InputJson.Text;
                        }
                        break;

                }
            }
        }


        public bool WordWrap_WithSprites = true;

        private void AddText(string text)
        {
            var document = PreviewLayout.Document;
            var lastParagraph = document.Blocks.LastBlock as Paragraph;
            if (lastParagraph == null)
            {
                lastParagraph = new Paragraph();
                document.Blocks.Add(lastParagraph);
            }

            string[] parts = Regex.Split(text, @"<color=(#[0-9a-fA-F]{6})>(.*?)</color>");

            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 3 == 0 & parts[i] != "" & parts[i] != "\0") // обычный текст
                {
                    lastParagraph.Inlines.Add(new Run(parts[i]));
                }
                else if (i % 3 == 1) // цвет "#hex"
                {
                    if (System.Windows.Media.ColorConverter.ConvertFromString(parts[i]) is System.Windows.Media.Color color)
                    {
                        Run coloredRun = new Run(parts[i + 1])
                        {
                            Foreground = new SolidColorBrush(color),
                        };

                        if (parts[i] == "#e30000" | parts[i] == "#fac400") coloredRun.TextDecorations = TextDecorations.Underline;
                        //coloredRun.ToolTip = "Статусный эффект аааа"; //здесь должна быть подсказка по статусному эффекту при наведении мышью, но он упорно отказывается её показывать

                        lastParagraph.Inlines.Add(coloredRun);
                    }
                }
            }
        }

        public bool Is_OneWord_Queued = false;
        public bool Is_MultWords_Queued = false;

        private void AddSprite(string SpriteName = "Unknown.png", bool IsOneWord = true, string NextWord = "Кто это?", string NextColor = "#fac400")
        {
            if (IsOneWord & WordWrap_WithSprites) Is_OneWord_Queued = true;

            var document = PreviewLayout.Document;
            var lastParagraph = document.Blocks.LastBlock as Paragraph;
            if (lastParagraph == null)
            {
                lastParagraph = new Paragraph();
                document.Blocks.Add(lastParagraph);
            }

            if (IsOneWord & WordWrap_WithSprites || (Is_MultWords_Queued & WordWrap_WithSprites))
            {
                InlineUIContainer SpriteImageContainer = new();
                Image SpriteImage = new()
                {
                    Source = SpriteBitmaps[SpriteName],
                    Width = 23,
                    Height = 23,
                    Margin = new Thickness(-2, -1, -2, 0)
                };

                StackPanel SpritePlusEffectname = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    //Background = new SolidColorBrush(Colors.Gray), // Border
                };
                SpritePlusEffectname.Children.Add(new TextBlock(new InlineUIContainer(SpriteImage)));
                Run EffectName = new Run(NextWord) {TextDecorations = TextDecorations.Underline};

                if (ColorConverter.ConvertFromString(NextColor) is System.Windows.Media.Color color)
                {
                    EffectName.Foreground = new SolidColorBrush(color);
                }

                SpritePlusEffectname.Children.Add(new TextBlock(EffectName));
                InlineUIContainer SpritePlusEffectname_Container = new(SpritePlusEffectname);

                // 24 line height = |4.5| 'y' value
                // 30 line height = |10|  'y' value
                /* ///////// ВЫРАВНИВАНИЕ СТРОК И СПРАЙТОВ С НАЗВАНИЯМИ ЭФФЕКТОВ ///////// */
                SpritePlusEffectname.Margin = new Thickness(0, -11, 0, 0);
                SpritePlusEffectname.RenderTransform = new TranslateTransform(0, 10.5); // 'y' value-0.5, может быть, наверное
                /* ///////// ВЫРАВНИВАНИЕ СТРОК И СПРАЙТОВ С НАЗВАНИЯМИ ЭФФЕКТОВ ///////// */

                SpritePlusEffectname.VerticalAlignment = VerticalAlignment.Bottom;

                lastParagraph.Inlines.Add(SpritePlusEffectname_Container);
            }
            else
            {
                Image SpriteImage = new Image
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
            JsonDesc = JsonDesc.Replace("><", ">\0<")
                               .Replace("<style=\\\"upgradeHighlight\\\">", "<color=#f8c200>")
                               .Replace("</style>", "</color>")
                               .Replace("</link>", "")
                               .Replace("<u>", "")
                               .Replace("</u>", "")
                               .Replace("<>", "<s>");

            JsonDesc = Regex.Replace(JsonDesc, @"(?<=<\/color>)([а-яА-Яa-zA-Z])", " $1");
            JsonDesc = Regex.Replace(JsonDesc, @"<link=\\\"".*?\\\"">", "");
            JsonDesc = JsonDesc.Replace("\">\0<color=#f8c200>", "\">\0<color=#fac400>"); // для позитивных статусных эффектов

            char[] splitby = { '<', '>' };
            string[] parts = $"<s>\0{JsonDesc.Replace("\\n", "\n")}".Split(splitby, StringSplitOptions.RemoveEmptyEntries);

            string[] Colors = {
                "color=#e30000", // Красный текст, Негативные статусные эффекты, подчёркивается
                "color=#fac400", // Жёлтый текст, Позитивные статусные эффекты (Спешка, Повышение уровня атаки, ..) + Заряд и Дыхание, подчёркивается

                "color=#f8c200", // Подсветка изменений в ЭГО Даре при улучшении (<style=\"upgradeHighlight\">XXX</style> в англ файле локализации (вполне работает), <color=#f8c200>XXX</color> в существующем переводе)
            };

            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    try
                    {
                        if (!Colors.Contains(parts[i - 1])) // ДОБАВЛЕНИЕ региона ОБЫНОГО текста (без HEX цвета) 
                                                            // Добавление цветного текста с HEX кодом см. ниже в 'else if (parts[i].StartsWith("color"))'
                        {
                            AddText(parts[i]);
                        }
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        string NextWord  = "def";
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
                                AddSprite(spritename, NextWord: NextWord, NextColor: NextColor, IsOneWord: IsOneWord);
                            }
                            catch
                            {
                                AddSprite(@"Unknown.png");
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
                                    /// Добавление ЦВЕТНОГО Текста с HEX кодом(Цвет сюда вставляется вручную)
                                    case "<color=#e30000>":
                                        AddText($"<color=#e30000>{AddT}</color>");
                                        break;

                                    case "<color=#f8c200>":
                                        AddText($"<color=#f8c200>{AddT}</color>");
                                        break;

                                    case "<color=#fac400>":
                                        AddText($"<color=#fac400>{AddT}</color>");
                                        break;
                                }
                            }
                            else Is_OneWord_Queued = false;
                        }
                    }
                    catch
                    {
                        AddSprite(@"Unknown.png");
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
            System.Windows.Application.Current.Shutdown();
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
                (string, string) ExitData = JsonLoader.GetUnsavedContent(EditBuffer);

                if (Convert.ToInt32(ExitData.Item2) != 0)
                {
                    OverrideCover1.Margin = new Thickness(0);
                    OverrideCover2.Margin = new Thickness(0);
                    ExitDialog.Margin = new Thickness(0);

                    e.Cancel = true;
                    UnsavedChangesTooltip_Text.Text = ExitData.Item1.Trim();
                   // UnsavedChangesTooltip.Height = 20 * ExitData.Item1.Trim().Split('\n').Length;
                    UnsavedChangesCount.Text = ExitData.Item2;
                }

            }
        }

        private void Limbus_Json_to_Text_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InputJson.SetValue(HeightProperty, this.ActualHeight - 320);
            if (this.Height == 320)
            {
                this.Height = 320.1;
                this.MaxWidth = 600;
            }
            else this.MaxWidth = 900;
        }

        private static void Check_bgtext(TextBox tb, System.Windows.Controls.Label lb, string lb_text)
        {
            if(tb.Text != "") lb.Content = "";
            else lb.Content = lb_text;
        }

        private void Check_JsonFilepath_bgtext()     => Check_bgtext(JsonFilepath, JsonFilepath_bgtext, "Путь к Json файлу");
        //private void Check_EgoGift_Name_bgtext()     => Check_bgtext(EgoGift_Name, EgoGift_Name_bgtext, "Название ЭГО Дара");
        private void Check_EgoGift_JumpToID_bgtext() => Check_bgtext(EgoGift_JumpToID_Input, EgoGift_JumpToID_bgtext, "ID");


        private void JsonPath_TextChanged(object sender, TextChangedEventArgs e) => Check_bgtext(JsonFilepath, JsonFilepath_bgtext, "Путь к Json файлу");
        private void EgoGift_Name_TextChanged(object sender, TextChangedEventArgs e) => Check_bgtext(EgoGift_Name, EgoGift_Name_bgtext, "Название ЭГО Дара");
        private void EgoGift_JumpToID_Input_TextChanged(object sender, TextChangedEventArgs e) => Check_bgtext(EgoGift_JumpToID_Input, EgoGift_JumpToID_bgtext, "ID");



        

        private void IDSwitch_CheckEditBufferDescs()
        {
            if (!EditBuffer[Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
                EgoGift_SwitchEditorTo_Desc.Content = "Описание*";
            else
                EgoGift_SwitchEditorTo_Desc.Content = "Описание";


            if (!EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc1"].Equals("{unedited}"))
                EgoGift_SwitchEditorTo_SimpleDesc1.Content = "Простое описание 1*";
            else
                EgoGift_SwitchEditorTo_SimpleDesc1.Content = "Простое описание 1";


            if (!EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc2"].Equals("{unedited}"))
                EgoGift_SwitchEditorTo_SimpleDesc2.Content = "Простое описание 2*";
            else
                EgoGift_SwitchEditorTo_SimpleDesc2.Content = "Простое описание 2";


            if (!EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc3"].Equals("{unedited}"))
                EgoGift_SwitchEditorTo_SimpleDesc3.Content = "Простое описание 3*";
            else
                EgoGift_SwitchEditorTo_SimpleDesc3.Content = "Простое описание 3";


            if (!EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc4"].Equals("{unedited}"))
                EgoGift_SwitchEditorTo_SimpleDesc4.Content = "Простое описание 4*";
            else
                EgoGift_SwitchEditorTo_SimpleDesc4.Content = "Простое описание 4";


            if (!EditBuffer[Json_Dictionary_CurrentID]["SimpleDesc5"].Equals("{unedited}"))
                EgoGift_SwitchEditorTo_SimpleDesc5.Content = "Простое описание 5*";
            else
                EgoGift_SwitchEditorTo_SimpleDesc5.Content = "Простое описание 5";
        }

        private void EgoGiftMenu_SwitchToID(int ID)
        {
            Json_Dictionary_CurrentID = ID;
            IDSwitch_CheckEditBufferDescs();
            EgoGift_ID_Switch_CheckButtons();
            InputJson.IsUndoEnabled = false;
            InputJson.IsUndoEnabled = true;

            EgoGift_Name_Show.Text = Convert.ToString(Json_Dictionary[Json_Dictionary_CurrentID]["Name"]);
            EgoGift_Name.Text = Convert.ToString(Json_Dictionary[Json_Dictionary_CurrentID]["Name"]);
            if (Json_Dictionary[ID]["SimpleDesc2"] != "{none}")
            {
                EgoGift_SimpleDesc2_Cover.Height = 0;
                SimpleDesc2_Change_Cover.Height = 0;
            }
            else 
            {
                EgoGift_SimpleDesc2_Cover.Height = 30;
                SimpleDesc2_Change_Cover.Height = 30;
            }

            if (Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc3"] != "{none}")
            {
                EgoGift_SimpleDesc3_Cover.Height = 0;
                SimpleDesc3_Change_Cover.Height = 0;
            }
            else
            {
                EgoGift_SimpleDesc3_Cover.Height = 30;
                SimpleDesc3_Change_Cover.Height = 30;
            }
            
            if (Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc4"] != "{none}")
            {
                EgoGift_SimpleDesc4_Cover.Height = 0;
                SimpleDesc4_Change_Cover.Height = 0;
            }
            else
            {
                EgoGift_SimpleDesc4_Cover.Height = 30;
                SimpleDesc4_Change_Cover.Height = 30;
            }
            
            if (Json_Dictionary[Json_Dictionary_CurrentID]["SimpleDesc5"] != "{none}")
            {
                EgoGift_SimpleDesc5_Cover.Height = 0;
                SimpleDesc5_Change_Cover.Height = 0;
            }
            else
            {
                EgoGift_SimpleDesc5_Cover.Height = 30;
                SimpleDesc5_Change_Cover.Height = 30;
            }
            EgoGift_ID_CopyID_Button.Content = Json_Dictionary_CurrentID;

            CurrentDesc = "Desc";
            if (EditBuffer[Json_Dictionary_CurrentID]["Desc"].Equals("{unedited}"))
            {
                InputJson.Text = Convert.ToString(Json_Dictionary[Json_Dictionary_CurrentID]["Desc"]);
            }
            else
            {
                InputJson.Text = Convert.ToString(EditBuffer[Json_Dictionary_CurrentID]["Desc"]);
            }
        }

        private async void TextBoxFlashWarning(System.Windows.Controls.TextBox TB,
                                               System.Windows.Controls.Label LB,
                                               string WarningText,
                                               string LBContentAfter,
                                               string WhatsNext,
                                               int rounds = 2,
                                               int AfterAwait = 400,
                                               int TimerAwait = 100) {
            TB.Focusable = false;
            TB.Foreground = РазноеДругое.GetColorFromHexa("#FF191919"); ;

            LB.Content = WarningText;

            for (int i = 1; i <= rounds; i++)
            {
                LB.Foreground = РазноеДругое.GetColorFromHexa("#FFFFA4A4");
                await Task.Delay(TimerAwait);
                LB.Foreground = РазноеДругое.GetColorFromHexa("#FFF43D3D");
                await Task.Delay(TimerAwait);
            }
            await Task.Delay(AfterAwait);

            LB.Foreground = РазноеДругое.GetColorFromHexa("#FF514C46");
            LB.Content = LBContentAfter;
            TB.Focusable = true;
            TB.Foreground = РазноеДругое.GetColorFromHexa("#FFA69885");

            if (WhatsNext == "Check_EgoGift_JumpToID_bgtext") Check_EgoGift_JumpToID_bgtext();
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
            JsonFilepath.Foreground = РазноеДругое.GetColorFromHexa("#FF191919");
            JsonFilepath_bgtext.Content = text;
            JsonFilepath_bgtext.Foreground = РазноеДругое.GetColorFromHexa("#FF39CF2B");
            await Task.Delay(1150);
            JsonFilepath_bgtext.Content = "";
            JsonFilepath.Focusable = true;
            JsonFilepath.Foreground = РазноеДругое.GetColorFromHexa("#FFA69885");
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
                    Json_Filepath_str = JsonFilepath.Text;
                    Json_Dictionary = JsonLoader.Json_GetAdressBook(Json_Filepath_str);
                    JsonFilepath_PATH = path;

                    JsonKeys = Json_Dictionary.Keys.ToList();
                    foreach(var ID in JsonKeys)
                    {
                        EditBuffer[ID] = new Dictionary<string, object>
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

                    EgoGift_Desc_Cover.Height = 0;
                    Desc_Change_Cover.Height = 0;

                    EgoGift_SimpleDesc1_Cover.Height = 0;
                    SimpleDesc1_Change_Cover.Height = 0;


                    Json_Dictionary_CurrentID = JsonKeys[0];
                    EgoGiftMenu_SwitchToID(Json_Dictionary_CurrentID);

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



        private void EgoGift_ID_Switch_CheckButtons()
        {
            EgoGift_ID_SwitchPrev_Cover.Height = 0;
            EgoGift_ID_SwitchNext_Cover.Height = 0;
            try { var PrevCheck = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) - 1]; }
            catch { EgoGift_ID_SwitchPrev_Cover.Height = 16; }
            try { var NextCheck = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) + 1]; }
            catch { EgoGift_ID_SwitchNext_Cover.Height = 16; }
        }
        private void EgoGift_ID_SwitchPrev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Json_Dictionary_CurrentID = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) - 1];
                EgoGiftMenu_SwitchToID(Json_Dictionary_CurrentID);

                EgoGift_ID_Switch_CheckButtons();
            }
            catch { }
        }

        
        private void EgoGift_ID_SwitchNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Json_Dictionary_CurrentID = JsonKeys[JsonKeys.IndexOf(Json_Dictionary_CurrentID) + 1];
                EgoGiftMenu_SwitchToID(Json_Dictionary_CurrentID);

                EgoGift_ID_Switch_CheckButtons();
            }
            catch { }
        }


        private void CheckEditBuffer(string CurrentDesc)
        {
            try
            {
                if (EditBuffer[Json_Dictionary_CurrentID][CurrentDesc].Equals("{unedited}"))
                {
                    InputJson.Text = Convert.ToString(Json_Dictionary[Json_Dictionary_CurrentID][CurrentDesc]);
                }
                else
                {
                    InputJson.Text = Convert.ToString(EditBuffer[Json_Dictionary_CurrentID][CurrentDesc]);
                }
            }
            catch { }
        }


        
        private void EgoGift_SwitchEditorTo_Desc_Button(object sender, RoutedEventArgs e)       
        {
            try{ 
                CurrentDesc = "Desc";
                CheckEditBuffer("Desc");
            }catch{}
        }
        private void EgoGift_SwitchEditorTo_SimpleDesc1_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc1";
                CheckEditBuffer("SimpleDesc1");
            }catch {}
        }
        private void EgoGift_SwitchEditorTo_SimpleDesc2_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentDesc = "SimpleDesc2";
                CheckEditBuffer("SimpleDesc2");
            }catch{}
        }
        private void EgoGift_SwitchEditorTo_SimpleDesc3_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc3";
                CheckEditBuffer("SimpleDesc3");
            }catch{}
        }
        private void EgoGift_SwitchEditorTo_SimpleDesc4_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc4";
                CheckEditBuffer("SimpleDesc4");
            }catch{}
        }
        private void EgoGift_SwitchEditorTo_SimpleDesc5_Button(object sender, RoutedEventArgs e)
        {
            try{
                CurrentDesc = "SimpleDesc5";
                CheckEditBuffer("SimpleDesc5");
            }catch{}
        }

        private void EgoGift_JumpToID_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EgoGiftMenu_SwitchToID(Convert.ToInt32(EgoGift_JumpToID_Input.Text));
                EgoGift_JumpToID_Input.Text = "";
                Check_JsonFilepath_bgtext();
                EgoGift_ID_Switch_CheckButtons();
            }
            catch
            {
                TextBoxFlashWarning(EgoGift_JumpToID_Input, EgoGift_JumpToID_bgtext, "ID Не найден", "Перейти к ID..", "Check_EgoGift_JumpToID_bgtext");
            }
        }

        private async void EgoGift_ID_CopyID(object sender, RoutedEventArgs e)
        {
            if (Json_Dictionary_CurrentID != -1)
            {
                Clipboard.SetText(Convert.ToString(EgoGift_ID_CopyID_Button.Content));
                EgoGift_ID_CopyID_Button.Content = "ID Скопирован";

                await Task.Delay(920);
                EgoGift_ID_CopyID_Button.Content = Json_Dictionary_CurrentID;
            }
        }









        private void EgoGift_Name_ChangeName(object sender, RoutedEventArgs e)
        {
            try{
                РазноеДругое.SetRW(JsonFilepath_PATH);
                РазноеДругое.RewriteFileLine($"      \"name\": \"{EgoGift_Name.Text}\",",
                                Json_Filepath_str,
                                Convert.ToInt32(Json_Dictionary[Json_Dictionary_CurrentID]["LineIndex_Name"]));
                РазноеДругое.SetRO(JsonFilepath_PATH);
                Notify("Файл обновлён");
            } catch { TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка сохранения", "Путь к Json файлу", "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600); }
        }

        private void EgoGift_Desc_ChangeOver(string ThisDesc)
        {
            try
            {
                if (!EditBuffer[Json_Dictionary_CurrentID][ThisDesc].Equals("{unedited}"))
                {
                    РазноеДругое.SetRW(JsonFilepath_PATH);
                    РазноеДругое.RewriteFileLine($"{(ThisDesc.StartsWith("SimpleDesc") ? "          \"simpleDesc\": \"" : "      \"desc\": \"")}" +
                                                 $"{Convert.ToString(EditBuffer[Json_Dictionary_CurrentID][ThisDesc]).Replace("\n", "\\n").Replace("\r", "\\n")}" +
                                                 $"{(ThisDesc.StartsWith("SimpleDesc") ? "\"" : "\",")}",
                                                 Json_Filepath_str,
                                                 Convert.ToInt32(Json_Dictionary[Json_Dictionary_CurrentID][$"LineIndex_{ThisDesc}"]));

                    Json_Dictionary[Json_Dictionary_CurrentID][ThisDesc] = EditBuffer[Json_Dictionary_CurrentID][ThisDesc];
                    EditBuffer[Json_Dictionary_CurrentID][ThisDesc] = "{unedited}";
                    IDSwitch_CheckEditBufferDescs();
                    Notify("Файл обновлён");
                    РазноеДругое.SetRO(JsonFilepath_PATH);
                }
            }
            catch { TextBoxFlashWarning(JsonFilepath, JsonFilepath_bgtext, "Ошибка сохранения", "Путь к Json файлу", "Check_JsonFilepath_bgtext", rounds: 3, AfterAwait: 600); }
        }


        private void EgoGift_Desc_ChangeDesc(object sender, RoutedEventArgs e) => EgoGift_Desc_ChangeOver("Desc");
        private void EgoGift_Desc_ChangeSimpleDesc1(object sender, RoutedEventArgs e) => EgoGift_Desc_ChangeOver("SimpleDesc1");
        private void EgoGift_Desc_ChangeSimpleDesc2(object sender, RoutedEventArgs e) => EgoGift_Desc_ChangeOver("SimpleDesc2");
        private void EgoGift_Desc_ChangeSimpleDesc3(object sender, RoutedEventArgs e) => EgoGift_Desc_ChangeOver("SimpleDesc3");
        private void EgoGift_Desc_ChangeSimpleDesc4(object sender, RoutedEventArgs e) => EgoGift_Desc_ChangeOver("SimpleDesc4");
        private void EgoGift_Desc_ChangeSimpleDesc5(object sender, RoutedEventArgs e) => EgoGift_Desc_ChangeOver("SimpleDesc5");
    }
}