using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;

namespace Limbus_Json_Preview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Dictionary<string, BitmapImage> FormatSpriteBytes(Dictionary<string, byte[]> Origin)
        {
            Dictionary<string, BitmapImage> exp = new();

            foreach (var Sprite in Origin)
            {
                Console.WriteLine(Sprite.Key);
                using (MemoryStream stream = new MemoryStream(Sprite.Key.EndsWith(".png")? Sprite.Value : ConvertWebPToPng(Sprite.Value)))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    exp[Sprite.Key] = bitmapImage;
                }
            }
            return exp;
        }

        private static byte[] ConvertWebPToPng(byte[] webpData)
        {
            using (var inputStream = new MemoryStream(webpData))
            using (var image = SixLabors.ImageSharp.Image.Load(inputStream))
            {
                using (var outputStream = new MemoryStream())
                {
                    image.SaveAsPng(outputStream);
                    return outputStream.ToArray();
                }
            }
        }

        private static Dictionary<string, byte[]> GetSprites()
        {
            if (!Directory.Exists("Спрайты"))
            {
                Window_NoSpritesFolder();
            }

            Dictionary<string, byte[]> exp = new();
            foreach (string image in Directory.EnumerateFiles("Спрайты", "*.*", SearchOption.AllDirectories))
            {
                //Console.WriteLine(objectCurrent);
                if (!image.EndsWith("txt")) exp[image[8..]] = File.ReadAllBytes(image);
            }

            return exp;
        }
        readonly static Dictionary<string, byte[]> _Sprites_byte = GetSprites();
        readonly static Dictionary<string, BitmapImage> _Sprites_ = FormatSpriteBytes(_Sprites_byte);


        public MainWindow()
        {
            InitializeComponent();

            /*/ 
             *  Добавляя спрайты надо их добавлять так же в 'switch ("<" + parts[i] + ">")' который ниже в UpdatePreview()
             * 
             *  Добавлять новые hex цвета из <color> надо сначала в лист Colors и так же в этот switch по образцу, всё так же в UpdatePreview()
            /*/

            void SetInterfaceImage(System.Windows.Controls.Image XamlImageLink, string ResDir_Source)
            {
                MemoryStream byteStream = new(_Sprites_byte[ResDir_Source]);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = byteStream;
                image.EndInit();
                XamlImageLink.Source = image;
            }
            SetInterfaceImage(TextBG, @"$Другое\Фон.png");

            PreviewLayout.Document.Blocks.Clear();
            string Def = "Статусные эффекты, <style=\\\"upgradeHighlight\\\">Подсветка улучшения</style>\r\n<sprite name=\\\"Breath\\\"><color=#f8c200>Дыхание</color>\r\n<sprite name=\\\"Charge\\\"><color=#f8c200>Заряд</color>\r\n<sprite name=\\\"Laceration\\\"><color=#e30000>Кровотечение</color>\r\n<sprite name=\\\"Combustion\\\"><color=#e30000>Огонь</color>\r\n<sprite name=\\\"Burst\\\"><color=#e30000>Разрыв</color>\r\n<sprite name=\\\"Sinking\\\"><color=#e30000>Утопание</color>\r\n<sprite name=\\\"Vibration\\\"><color=#e30000>Тремор</color>\r\n - <sprite name=\\\"VibrationExplosion\\\"><color=#e30000>Провоцирование тремора</color>\r\n\r\n<sprite name=\\\"Ктокто\\\"><color=#e30000>Неправильный спрайт</color>\r\n<color=#e30000>Неправильный поломанный текст (цвет используется без спрайта, для подсветки улучшения ЭГО Дара должен использоваться вышеуказанный <style=\\\"upgradeHighlight\\\">upgradeHighlight</style>)\r\n\r\n< Поломанный текст после незакрытой острой скобки, всегда закрывайте их\r\n<sprite name=\\\"Burst\\\"><color=#e30000>Разрыв</color>";

            Def = "Конец хода: даёт 1 <sprite name=\\\"Agility\\\"><color=#f8c200><u><link=\\\"Agility\\\">Спешку</link></u></color>, 1 <sprite name=\\\"AttackUp\\\"><color=#f8c200><u><link=\\\"AttackUp\\\">Повышение уровня атаки</link></u></color></style> <style=\\\"upgradeHighlight\\\">и 1</style> <sprite name=\\\"AttackDmgUp\\\"><color=#f8c200><u><link=\\\"AttackDmgUp\\\">Повышение урона</link></u></color> на следующий ход 1 грешнику с наибольшим потенциалом <sprite name=\\\"Breath\\\"><color=#f8c200><u><link=\\\"Breath\\\">Дыхания</link></u></color> и ещё 1 грешнику с наибольшим счётчиком <sprite name=\\\"Breath\\\"><color=#f8c200><u><link=\\\"Breath\\\">Дыхания</link></u></color>. (Оба эффекта могу быть применены к одной идентичности)\n\nЕсли же грешник имеет Атакующий навык с Грехом похоти, тогда даёт 2 <sprite name=\\\"Agility\\\"><color=#f8c200><u><link=\\\"Agility\\\">Спешки</link></u></color>, 2 <sprite name=\\\"AttackUp\\\"><color=#f8c200><u><link=\\\"AttackUp\\\">Повышения уровня атаки</link></u></color></style> <style=\\\"upgradeHighlight\\\">и 2</style> <sprite name=\\\"AttackDmgUp\\\"><color=#f8c200><u><link=\\\"AttackDmgUp\\\">Повышения урона</link></u></color>. (ЭГО навыки не учитываются)";

            
            PreviewLayout.SetValue(Paragraph.LineHeightProperty, 30.0);

            InputJson.Text = Def;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            PreviewLayout.Document.Blocks.Clear();
            UpdatePreview(InputJson.Text);
        }


        public bool WordWrap_WithSprites = true;
        /*/
         * Попытка заставить спрайты переноситься на новую строку вместе с названием статусного эффекта
         * 
         * Выполнено.!!!!!!
         * 
        /*/

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
                    if (WordWrap_WithSprites)
                    {
                        string Tht = parts[i];
                        if (parts[i].Length > 20)
                        {

                            lastParagraph.Inlines.Add(new Run(Tht));
                        }
                        else lastParagraph.Inlines.Add(new Run(Tht));// AddSprite(IsSTPSpace: true);
                    }
                    else
                    {
                        lastParagraph.Inlines.Add(new Run(parts[i]));
                    }
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

        private void AddSprite(string Sprite = "Unknown", bool IsOneWord = true, string NextWord = "Кто это?", string NextColor = "#fac400", bool IsSTPSpace = false)
        {
            if (IsOneWord & WordWrap_WithSprites) Is_OneWord_Queued = true;

            var document = PreviewLayout.Document;
            var lastParagraph = document.Blocks.LastBlock as Paragraph;
            if (lastParagraph == null)
            {
                lastParagraph = new Paragraph();
                document.Blocks.Add(lastParagraph);
            }

            InlineUIContainer SpriteImageContainer = new();

            if (IsOneWord & WordWrap_WithSprites || (Is_MultWords_Queued & WordWrap_WithSprites))
            {
                System.Windows.Controls.Image image = new System.Windows.Controls.Image
                {
                    Source = _Sprites_[Sprite],
                    Width = 23,
                    Height = 23,
                    Margin = new Thickness(-2, -1, -2, 0),
                };
                SpriteImageContainer = new(image);

                StackPanel SpritePlusEffectname = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    //Background = new SolidColorBrush(Colors.Gray), // Border
                };
                SpritePlusEffectname.Children.Add(new TextBlock(SpriteImageContainer));
                Run EffectName = new Run(NextWord)
                {
                    TextDecorations = TextDecorations.Underline,
                };

                if (System.Windows.Media.ColorConverter.ConvertFromString(NextColor) is System.Windows.Media.Color color)
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
                System.Windows.Controls.Image image = new System.Windows.Controls.Image
                {
                    Source = _Sprites_[Sprite],
                    Width = 21,
                    Height = 21,
                    Margin = new Thickness(-2),
                };
                SpriteImageContainer = new(image);

                StackPanel SpritePlusEffectname = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Background = new SolidColorBrush(Colors.Gray), // Border
                };
                SpritePlusEffectname.Children.Add(new TextBlock(SpriteImageContainer));

                lastParagraph.Inlines.Add(SpriteImageContainer);
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
                               .Replace("</u>", "");

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
                        string NextWord = "def";
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
                            if (_Sprites_byte.Keys.Contains($"{spritename}.png")) spritename = $"{spritename}.png";
                            else if (_Sprites_byte.Keys.Contains($"{spritename}.webp")) spritename = $"{spritename}.webp";
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

        private static void Window_NoSpritesFolder()
        {
            MessageBoxResult result = MessageBox.Show("Папка \"Спрайты\" потерялась, она должна быть прямо тут", "Что-то не так", MessageBoxButton.OK, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK) Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы завершили редактирование?", "Закрытие приложения", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) e.Cancel = true;
        }
    }
}
