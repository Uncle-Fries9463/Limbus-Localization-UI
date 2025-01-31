using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Limbus_Json_Preview
{
    internal class РазноеДругое
    {
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

        public static void RewriteFileLine(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }




        public static SolidColorBrush GetColorFromHexa(string hexaColor)
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

        private static Dictionary<string, byte[]> GetSpriteFiles()
        {
            if (!Directory.Exists("Спрайты")) MainWindow.Window_NoSpritesFolder();

            Dictionary<string, byte[]> exp = new();
            foreach (string image in Directory.EnumerateFiles("Спрайты", "*.*", SearchOption.AllDirectories))
            {
                if (image.EndsWith(".png") | image.EndsWith(".webp"))
                {
                    exp[image[8..]] = File.ReadAllBytes(image);
                    Console.WriteLine($"Спрайт загружен: {image}");
                }
            }
            return exp;
        }

        public static Dictionary<string, BitmapImage> GetSpritesBitmaps()
        {
            Dictionary<string, BitmapImage> exp = new();

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
                    exp[Sprite.Key] = bitmapImage;
                }
            }
            return exp;
        }
    }
}
