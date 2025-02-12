using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System.IO;
using System.Text;
using System.Windows.Media;
using Limbus_Localization_UI.Json;
using System.Windows.Media.Imaging;

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
            File.WriteAllText(Path, JsonConvert.SerializeObject(JSON, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }).Replace("\r", ""), encoding: UTF8_BOM);
        }

        public static Dictionary<string, string> GetSpriteNames()
        {
            Dictionary<string, string> SpriteNames = new();

            string[] Lines = File.ReadAllLines(@"Спрайты\$Другое\Bufs.json");

            for (int i = 0; i <= Lines.Count() - 1; i++)
            {
                if (Lines[i].Trim().StartsWith("\"id\": "))
                {
                    string SpriteId = Lines[i].Split("\"id\": \"")[1].Split("\",")[0];
                    string SpriteName = Lines[i+1].Split("\"name\": \"")[1].Split("\",")[0];
                    SpriteNames[SpriteId] = SpriteName;
                }
            }
            
            return SpriteNames;
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
            if (!Directory.Exists("Спрайты")) MainWindow.Window_NoSpritesFolder();

            Dictionary<string, byte[]> SpriteFiles = new();
            foreach (string image in Directory.EnumerateFiles("Спрайты", "*.*", SearchOption.AllDirectories))
            {
                if (image.EndsWith(".png") | image.EndsWith(".webp"))
                {
                    SpriteFiles[image[8..]] = File.ReadAllBytes(image);
                }
            }
            Console.WriteLine($"Загружено спрайтов: {SpriteFiles.Keys.Count} ");
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
