using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static AdressBook.AdressBookTools;

namespace Limbus_Json_Preview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Загрузить ресурсы из .AdressBook (Просто архивный файл с спрайтами, весь алгоритм и функционал в 'AdressBook ResourceLoader.cs', да это очень упорото)
        readonly Dictionary<string, byte[]> AdressBook_Resources = AdressBook_LoadPages("Internal Assets.AdressBook");
        public MainWindow()
        {
            InitializeComponent();

            // Обновить файл со спрайтами
            //                    (папка с ними)                                                (Где сохранить)
            // AdressBook_Create(@"Adress Book", @"InternalAssets", @"Internal Assets", @"bin\Debug\net9.0-windows");
            
            /*/ 
             *  В .AdressBook архиве все используемые спрайты
             *  Добавляя спрайты надо их добавлять так же в 'switch ("<" + parts[i] + ">")' который ниже в TextBuilder()
             * 
             *  Добавлять новые hex цвета из <color> надо сначала в лист Colors и так же в этот switch по образцу, всё так же в TextBuilder()
            /*/

            void SetInterfaceImage(System.Windows.Controls.Image XamlImageLink, string ResDir_Source)
            {
                MemoryStream byteStream = new(AdressBook_Resources[ResDir_Source]);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = byteStream;
                image.EndInit();
                XamlImageLink.Source = image;
            }
            SetInterfaceImage(TextBG, @"Internal Assets\$Другое\Фон.png");

            Export.Document.Blocks.Clear();
            string Def = "Статусные эффекты, <style=\\\"upgradeHighlight\\\">Подсветка улучшения</style>\n" +
                "<sprite name=\\\"Breath\\\"><color=#f8c200>Дыхание</color>\n" +
                "<sprite name=\\\"Charge\\\"><color=#f8c200>Заряд</color>\n" +
                "<sprite name=\\\"Laceration\\\"><color=#e30000>Кровотечение</color>\n" +
                "<sprite name=\\\"Combustion\\\"><color=#e30000>Огонь</color>\n" +
                "<sprite name=\\\"Burst\\\"><color=#e30000>Разрыв</color>\n" +
                "<sprite name=\\\"Sinking\\\"><color=#e30000>Утопание</color>\n" +
                "<sprite name=\\\"Vibration\\\"><color=#e30000>Тремор</color>\n" +
                " - <sprite name=\\\"VibrationExplosion\\\"><color=#e30000>Провоцирование тремора</color>\n";

            Export.SetValue(Paragraph.LineHeightProperty, 30.0);
            InputJson.Text = Def;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Export.Document.Blocks.Clear();
            TextBuilder(InputJson.Text);
        }



        public void AddText(string text)
        {
            var document = Export.Document;
            var lastBlock = document.Blocks.LastBlock as Paragraph;
            if (lastBlock == null)
            {
                lastBlock = new Paragraph();
                document.Blocks.Add(lastBlock);
            }

            string pattern = @"<color=(#[0-9a-fA-F]{6})>(.*?)</color>";
            string[] parts = Regex.Split(text, pattern);
            

            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 3 == 0) // обычный текст
                {
                    lastBlock.Inlines.Add(new Run(parts[i]));
                }
                else if (i % 3 == 1) // "#e30000"
                {
                    if (System.Windows.Media.ColorConverter.ConvertFromString(parts[i]) is System.Windows.Media.Color color)
                    {
                        Run coloredRun = new Run(parts[i + 1])
                        {
                            Foreground = new SolidColorBrush(color),
                        };

                        if (parts[i] == "#e30000" | parts[i] == "#fac400") coloredRun.TextDecorations = TextDecorations.Underline;
                        // coloredRun.ToolTip = "Статусный эффект аааа"; здесь должна быть подсказка по статусному эффекту при наведении мышью, но он упорно отказывается её показывать
                        lastBlock.Inlines.Add(coloredRun);
                    }
                }
            }
        }

        private void AddSprite(byte[] Sprite)
        {
            using (MemoryStream stream = new MemoryStream(Sprite))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                Image image = new Image
                {
                    Source = bitmapImage,
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(-2),
                };

                InlineUIContainer container = new(image);

                var document = Export.Document;
                if (document.Blocks.Count == 0 || document.Blocks.LastBlock is not Paragraph lastParagraph)
                {
                    lastParagraph = new Paragraph();
                    document.Blocks.Add(lastParagraph);
                }

                lastParagraph.Inlines.Add(container);
            }
        }




        /// <summary>
        /// Обработать вводимый Json desc текст и вывести в окно
        /// </summary>
        /// <param name="input"></param>
        private void TextBuilder(string input)
        {
            input = input.Replace("><", ">\0<")
                         .Replace("<style=\\\"upgradeHighlight\\\">", "<color=#f8c200>")
                         .Replace("</style>", "</color>")
                         .Replace("</link>", "")
                         .Replace("<u>", "")
                         .Replace("</u>", "");

            input = Regex.Replace(input, @"(?<=<\/color>)([а-яА-Яa-zA-Z])", " $1");
            input = Regex.Replace(input, @"<link=\\\"".*?\\\"">", "");
            input = input.Replace("<sprite name=\\\"Breath\\\">\0<color=#f8c200>", "<sprite name=\\\"Breath\\\">\0<color=#fac400>")
                         .Replace("<sprite name=\\\"Charge\\\">\0<color=#f8c200>", "<sprite name=\\\"Breath\\\">\0<color=#fac400>");

            string[] parts = $"<s>\0{input.Replace("\\n", "\n")}".Split(['<', '>'], StringSplitOptions.RemoveEmptyEntries);

            string[] Colors = [
                "color=#e30000", // Красный текст, (Негативные) 5 статусных эффектов, подчёркивается
                "color=#fac400", // Жёлтый текст, Особые статусные эффекты (Спешка, Повышение уровня атаки, ..) + Заряд и Дыхание, подчёркивается

                "color=#f8c200", // Подсветка изменений в ЭГО Даре при улучшении (<style=\"upgradeHighlight\">XXX</style> в англ файле локализации (вполне работает), <color=#f8c200>XXX</color> в существующем переводе)
            ];

            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    try   { if (!Colors.Contains(parts[i - 1])) AddText(parts[i]); } 
                    catch { }
                }
                else
                {
                    // лучше всё обернуть в foreach
                    try
                    {
                        switch ("<" + parts[i] + ">")
                        {
                            case "<sprite name=\\\"Breath\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Дыхание.png"]);
                                break;

                            case "<sprite name=\\\"Laceration\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Кровотечение.png"]);
                                break;

                            case "<sprite name=\\\"Combustion\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Огонь.png"]);
                                break;

                            case "<sprite name=\\\"Burst\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Разрыв.png"]);
                                break;

                            case "<sprite name=\\\"VibrationExplosion\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Тремор — Провокация.png"]);
                                break;

                            case "<sprite name=\\\"Vibration\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Тремор.png"]);
                                break;

                            case "<sprite name=\\\"Charge\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Заряд.png"]);
                                break;

                            case "<sprite name=\\\"Sinking\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Утопание.png"]);
                                break;



                            case "<sprite name=\\\"AttackDmgUp\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Особые\AttackDmgUp.png"]);
                                break;

                            case "<sprite name=\\\"AttackUp\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Особые\AttackUp.png"]);
                                break;



                            case "<sprite name=\\\"Agility\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Вторичные\Спешка.png"]);
                                break;

                            case "<sprite name=\\\"Binding\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Вторичные\Связывание.png"]);
                                break;

                            case "<sprite name=\\\"Vulnerable\\\">":
                                AddSprite(AdressBook_Resources[@"Internal Assets\Вторичные\Уязвимость.png"]);
                                break;



                            case "<color=#e30000>":
                                AddText($"<color=#e30000>{parts[i + 1]}</color>");
                                break;
                            
                            case "<color=#f8c200>":
                                AddText($"<color=#f8c200>{parts[i + 1]}</color>");
                                break;

                            case "<color=#fac400>":
                                AddText($"<color=#fac400>{parts[i + 1]}</color>");
                                break;

                            default:
                                break;
                        }
                    }
                    catch
                    {
                        AddSprite(AdressBook_Resources[@"Internal Assets\$Другое\Unknown.png"]);
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы завершили редактирование?", "Закрытие приложения", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) e.Cancel = true;
        }
    }
}
