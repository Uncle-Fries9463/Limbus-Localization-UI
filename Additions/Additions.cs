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
        static Dictionary<string, dynamic> T;
        public static void InitTDictionaryHere(Dictionary<string, dynamic> FromExternal) => T = FromExternal;

        public static Encoding UTF8_BOM = new UTF8Encoding(true);
        public static void SetRO(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                attr = attr | FileAttributes.ReadOnly;
                File.SetAttributes(path, attr);
            }
            catch { }
        }

        public static void SetRW(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                attr = attr & ~FileAttributes.ReadOnly;
                File.SetAttributes(path, attr);
            }
            catch { }
        }


        public static void RewriteFileLine(string NewLineText, string Filepath, int LineNumber)
        {
            try
            {
                string[] LineArray = File.ReadAllLines(Filepath);
                LineArray[LineNumber - 1] = NewLineText;

                // Образцовое форматирование Json с LF переносом строк и адекватным количеством пробелов
                var ParsedJson = JToken.Parse(String.Join('\n', LineArray));
                var FormattedJson = ParsedJson.ToString(Formatting.Indented).Replace("\r", "");
                File.WriteAllText(Filepath, FormattedJson, encoding: UTF8_BOM);
            }
            catch { }
        }

        public static void SaveJson(JsonData JSON, string Path)
        {
            try
            {
                File.WriteAllText(Path, JsonConvert.SerializeObject(JSON, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }).Replace("\r", "").Replace("\\r", ""), encoding: UTF8_BOM);
            }
            catch { }
        }

        public static Dictionary<string, string> GetKeywordsSet(string from = "RU")
        {
            Dictionary<string, string> KeywordsSet = new();

            foreach(var line in File.ReadLines(@$"[Ресурсы]\& Stringtypes\BattleKeywords\{from}\Keywords Set.txt"))
            {
                string KeywordID = line.Split(": 【")[0];
                string[] KeywordTexts = line.Split(": 【")[1][0..^1].Split(" ¤ ");
                foreach(var KeywordText in KeywordTexts)
                {
                    KeywordsSet[KeywordText] = KeywordID;
                }
            }

            return KeywordsSet;
        }

        public static (Dictionary<string, string>, Dictionary<string, string>) GetKeywords(string from = "RU")
        {
            Dictionary<string, string> SpriteNames = new();
            Dictionary<string, string> KeywordTexts = new();
            List<string> KeywordTexts_Text = new();
            List<string> KeywordTexts_ID = new();
            try
            {
                int counter = 0;
                try
                {
                    foreach (string KeywordFile in Directory.EnumerateFiles(@$"[Ресурсы]\& Stringtypes\BattleKeywords\{from}", "*.*", SearchOption.TopDirectoryOnly))
                    {
                        string[] Lines = File.ReadAllLines(KeywordFile);

                        for (int i = 0; i <= Lines.Count() - 1; i++)
                        {
                            try
                            {
                                if (Lines[i].Trim().StartsWith("\"id\": "))
                                {
                                    string SpriteId = Lines[i].Split("\"id\": \"")[1].Split("\",")[0];
                                    string SpriteName = Lines[i + 1].Split("\"name\": \"")[1].Split("\",")[0];
                                    SpriteNames[SpriteId] = SpriteName;
                                    KeywordTexts[SpriteName.Trim()] = SpriteId;
                                    counter++;
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch{}
                KeywordTexts = KeywordTexts.OrderBy(obj => obj.Key.Length).ToDictionary(obj => obj.Key, obj => obj.Value);
                rin($"Загружено ключевых слов: {counter}");
            }
            catch { }
           
            return (SpriteNames, KeywordTexts);
        }

        public static Dictionary<string, string> GetColorPairs()
        {
            Dictionary<string, string> ColorPairs = new();
            try
            {
                int counter = 0;
                try
                {
                    foreach (var Line in File.ReadAllLines(@"[Ресурсы]\& Stringtypes\BattleKeywords\ColorPairs.txt"))
                    {
                        string ID = Line.Split(" ¤ ")[0];
                        string Color = Line.Split(" ¤ ")[1];
                        ColorPairs[ID] = Color;
                        counter++;
                    }

                }
                catch{}
                rin($"Загружено цветовых соответствий: {counter}");
            }
            catch { }
            return ColorPairs;
        }


        public static Dictionary<string, string> GetAddtReplacements(string from = "RU")
        {
            Dictionary<string, string> Replacements = new();
            try
            {
                int counter = 0;
                try
                {
                    foreach(var Line in File.ReadAllLines(@$"[Ресурсы]\& Stringtypes\BattleKeywords\{from}\Replacements+.txt").ToList())
                    {
                        if (Line.StartsWith("------------------------------------------------")) break;
                    
                        try
                        {
                            string Keyword = Line.Split("¤")[0].Trim();
                            string Replace = Line.Split("¤")[1].Trim();
                            Replacements[Keyword] = Replace;
                            //rin($"{Keyword}: {Replace}");
                            counter++;
                        }
                        catch{}
                    }
                }
                catch{}
                rin($"Загружено доп. замен: {counter}");
            }
            catch { }

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


        public static bool IsColor(string color)
        {
            try
            {
                GetColorFromAHEX("#ff" + color);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static Dictionary<string, string> GetTranslationHints()
        {
            Dictionary<string, string> KRTranslationTips = new();

            try
            {
                string[] Lines = File.ReadAllLines(@"[Ресурсы]\& Stringtypes\Translation hints.txt");
                KRTranslationTips = new()
                {
                    ["Size"] = Lines[0].Split("Size: ")[1],
                    ["Color"] = Lines[1].Split("Color: ")[1]
                };

                foreach (var Replacement in Lines[3..])
                {
                    if (Replacement.Contains(" ¤ "))
                    {
                        string A = Replacement.Split(" ¤ ")[0];
                        string B = Replacement.Split(" ¤ ")[1];
                        KRTranslationTips[A] = B;
                    }
                }
            }
            catch { }

            return KRTranslationTips;
        }


        private static Dictionary<string, byte[]> GetSpriteFiles()
        {
            Dictionary<string, byte[]> SpriteFiles = new();
            try
            {
                foreach (string image in Directory.EnumerateFiles(@"[Ресурсы]\Sprites", "*.*", SearchOption.TopDirectoryOnly))
                {
                    if (image.EndsWith(".png") | image.EndsWith(".webp"))
                    {
                        SpriteFiles[image.Split("\\")[^1]] = File.ReadAllBytes(image);
                    }
                }
                rin($"Загружено спрайтов: {SpriteFiles.Keys.Count} ");
            }
            catch { }
            return SpriteFiles;
        }

        public static Dictionary<string, BitmapImage> GetSpritesBitmaps()
        {
            Dictionary<string, BitmapImage> SpriteBitmaps = new();

            try
            {
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
            }
            catch { }

            return SpriteBitmaps;
        }

        public static void SwitchToSourceHanSansSC()
        {
            FontFamily Source_Han_Sans_SC = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/SourceHanSansSC/OTF/SimplifiedChinese/#Source Han Sans SC");

            T["Skill PreviewLayout Desc"].FontFamily = Source_Han_Sans_SC;
            T["PreviewLayout @ EGO Gift"].FontFamily = Source_Han_Sans_SC;
            T["PreviewLayout @ Skill"].FontFamily = Source_Han_Sans_SC;
            for (int CoinNumber = 1; CoinNumber <= 5; CoinNumber++)
            {
                for (int CoinDescNumber = 1; CoinDescNumber <= 12; CoinDescNumber++)
                {
                    T[$"Skill PreviewLayout Coin {CoinNumber} Desc {CoinDescNumber}"].FontFamily = Source_Han_Sans_SC;
                }
            }
        }
        public static void SwitchToSDream()
        {
            FontFamily S_Core_Dream_5_Medium = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/S_Core_Dream/OTF/#S-Core Dream 5 Medium");

            T["PreviewLayout @ EGO Gift"].FontFamily = S_Core_Dream_5_Medium;
            T["Skill PreviewLayout Desc"].FontFamily = S_Core_Dream_5_Medium;
            for (int CoinNumber = 1; CoinNumber <= 5; CoinNumber++)
            {
                for (int CoinDescNumber = 1; CoinDescNumber <= 12; CoinDescNumber++)
                {
                    T[$"Skill PreviewLayout Coin {CoinNumber} Desc {CoinDescNumber}"].FontFamily = S_Core_Dream_5_Medium;
                }
            }
        }
        public static void SwitchToPretendard()
        {
            FontFamily Pretendard = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/Pretendard/public/static/#Pretendard Light");

            T["PreviewLayout @ EGO Gift"].FontFamily = Pretendard;
            T["Skill PreviewLayout Desc"].FontFamily = Pretendard;
            for (int CoinNumber = 1; CoinNumber <= 5; CoinNumber++)
            {
                for (int CoinDescNumber = 1; CoinDescNumber <= 12; CoinDescNumber++)
                {
                    T[$"Skill PreviewLayout Coin {CoinNumber} Desc {CoinDescNumber}"].FontFamily = Pretendard;
                }
            }
        }
    }
}
