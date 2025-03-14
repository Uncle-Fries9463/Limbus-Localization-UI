using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System.IO;
using System.Text;
using System.Windows.Media;
using Limbus_Localization_UI.Json;
using System.Windows.Media.Imaging;
using static Limbus_Localization_UI.Additions.Consola;

namespace Limbus_Localization_UI.Additions
{
    internal class РазноеДругое
    {
        public static Encoding UTF8_BOM = new UTF8Encoding(true);

        public static void SetRO(string path)
        {
            var attr = File.GetAttributes(path);
            attr = attr | FileAttributes.ReadOnly;
            File.SetAttributes(path, attr);
        }

        public static void SetRW(string path)
        {
            var attr = File.GetAttributes(path);
            attr = attr & ~FileAttributes.ReadOnly;
            File.SetAttributes(path, attr);
        }


        public static void RewriteFileLine(string NewLineText, string Filepath, int LineNumber)
        {
            string[] LineArray = File.ReadAllLines(Filepath);
            LineArray[LineNumber - 1] = NewLineText;

            // Образцовое форматирование Json с LF переносом строк и адекватным количеством пробелов
            var ParsedJson = JToken.Parse(String.Join('\n', LineArray));
            var FormattedJson = ParsedJson.ToString(Formatting.Indented).Replace("\r", "");
            File.WriteAllText(Filepath, FormattedJson, encoding: UTF8_BOM);
        }

        public static void SaveJson(JsonData JSON, string Path)
        {                                                                                                              // Что бы не втыкало Name и Desc из ЭГО даров
            File.WriteAllText(Path, JsonConvert.SerializeObject(JSON, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }).Replace("\r", "").Replace("\\r", ""), encoding: UTF8_BOM);
        }

        public static (Dictionary<string, string>, Dictionary<string, string>) GetKeywords()
        {
            Dictionary<string, string> SpriteNames = new();
            Dictionary<string, string> KeywordTexts = new();

            int counter = 0;
            try
            {
                foreach (string KeywordFile in Directory.EnumerateFiles(@"[Ресурсы]\$Другое\BattleKeywords", "*.*", SearchOption.AllDirectories))
                {
                    string[] Lines = File.ReadAllLines(KeywordFile);

                    for (int i = 0; i <= Lines.Count() - 1; i++)
                    {
                        if (Lines[i].Trim().StartsWith("\"id\": "))
                        {
                            string SpriteId = Lines[i].Split("\"id\": \"")[1].Split("\",")[0];
                            string SpriteName = Lines[i + 1].Split("\"name\": \"")[1].Split("\",")[0];
                            SpriteNames[SpriteId] = SpriteName;
                            KeywordTexts[SpriteName] = SpriteId;
                            counter++;
                        }
                    }
                }
            }
            catch{}

            rin($"Загружено ключевых слов: {counter}");
            return (SpriteNames, KeywordTexts);
        }


        public static Dictionary<string, string> GetColorPairs()
        {
            Dictionary<string, string> ColorPairs = new();

            int counter = 0;
            try
            {
                foreach (var Line in File.ReadAllLines(@"[Ресурсы]\$Другое\BattleKeywords\ColorPairs.txt"))
                {
                    string ID = Line.Split(" ¤ ")[0];
                    string Color = Line.Split(" ¤ ")[1];
                    ColorPairs[ID] = Color;
                    counter++;
                }

                rin($"Загружено цветовых соответствий: {counter}");
            }
            catch{}
            return ColorPairs;
        }


        public static Dictionary<string, string> GetAddtReplacements()
        {
            Dictionary<string, string> Replacements = new();

            int counter = 0;
            try
            {
                foreach(var Line in File.ReadAllLines(@"[Ресурсы]\$Другое\Доп замены.txt").ToList())
                {
                    if (Line.StartsWith("------------------------------------------------")) break;

                    if (Line.StartsWith("\""))
                    {
                        try
                        {
                            string Keyword = Line.Split("\": \"")[0][1..];
                            string Replace = Line.Split("\": \"")[1][0..^1];
                            Replacements[Keyword] = Replace;
                            //rin($"{Keyword}: {Replace}");
                            counter++;
                        }
                        catch{}
                    }
                }
                rin($"Загружено доп. замен: {counter}");
            }
            catch{}

            return Replacements;
        }

        private static byte[] ConvertWebPToPng(byte[] webpData)
        {
            using (var inputStream = new MemoryStream(webpData))
            using (var image = Image.Load(inputStream))
            {
                using (var outputStream = new MemoryStream())
                {
                    image.SaveAsPng(outputStream);
                    return outputStream.ToArray();
                }
            }
        }

        public static SolidColorBrush GetColorFromAHEX(string hexaColor)
        {
            return new SolidColorBrush(
                System.Windows.Media.Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }


        private static Dictionary<string, byte[]> GetSpriteFiles()
        {
            Dictionary<string, byte[]> SpriteFiles = new();
            foreach (string image in Directory.EnumerateFiles(@"[Ресурсы]\Спрайты", "*.*", SearchOption.TopDirectoryOnly))
            {
                if (image.EndsWith(".png") | image.EndsWith(".webp"))
                {
                    SpriteFiles[image.Split("\\")[^1]] = File.ReadAllBytes(image);
                }
            }
            rin($"Загружено спрайтов: {SpriteFiles.Keys.Count} ");
            return SpriteFiles;
        }

        public static Dictionary<string, BitmapImage> GetSpritesBitmaps()
        {
            Dictionary<string, BitmapImage> SpriteBitmaps = new();

            foreach (var Sprite in GetSpriteFiles())
            {
                using (MemoryStream stream = new MemoryStream(Sprite.Key.EndsWith(".png") ? Sprite.Value : ConvertWebPToPng(Sprite.Value)))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    SpriteBitmaps[Sprite.Key] = bitmapImage;
                }
            }

            return SpriteBitmaps;
        }
    }
}
