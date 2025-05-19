using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute
{
    internal abstract class NullableControl
    {
        internal abstract class Settings
        {
            internal protected static bool WriteInfomationExternal = false;
            internal protected static bool PlaceStringNullMarker = true;
            internal protected static string StringNullMarker = "<Null>";
        }

        // All null strings to "<Null>" strings
        internal protected static void NullExterminate(object Target, bool WriteInfo = false)
        {
            string NullValueInformation = "\x1b[0m\x1b[38;5;197mNull\x1b[0m";
            string PropertyItemInformation = "\x1b[38;5;245m[$]\x1b[0m";

            PropertyInfo[] TargetPropertiesInfo = Target.GetType().GetProperties();
            if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.WriteLine($"Properties of '\x1b[38;5;248m{Target.GetType().Name}\x1b[0m'");
            foreach (PropertyInfo ClassProperty in TargetPropertiesInfo)
            {
                string PropertyName = ClassProperty.Name;
                object PropertyValue = ClassProperty.GetValue(Target);

                if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.Write($"  [\x1b[38;5;63mProperty Name\x1b[0m] '\x1b[38;5;248m{PropertyName}\x1b[0m' ::\x1b[0m [\x1b[38;5;63mValue\x1b[0m]\x1b[38;5;62m<\x1b[38;5;203m{ClassProperty.PropertyType.ToString().Replace(".", "\x1b[38;5;62m.\x1b[38;5;203m")}\x1b[38;5;62m>\x1b[0m {(PropertyValue.IsNull() ? NullValueInformation : PropertyItemInformation.Extern(PropertyValue))}");

                if (PropertyValue.IsNull())
                {
                    switch (ClassProperty.PropertyType.ToString())
                    {
                        case "System.String": ClassProperty.SetValue(Target, NullableControl.Settings.StringNullMarker);
                            if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.Write($"   \x1b[38;5;62m{{\x1b[38;5;203mReset\x1b[38;5;62m}}\x1b[0m -> \"\x1b[38;5;245m{NullableControl.Settings.StringNullMarker}\x1b[0m\"");
                            break;

                        default: break;
                    }
                }
                if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.WriteLine();
            }
            if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.WriteLine();
        }
    }

    internal static class Requirements
    {
        internal static int LinesCount(this string check) => check.Count(f => f == '\n');

        internal static Encoding GetFileEncoding(this FileInfo TargetFile)
        {
            using (var reader = new StreamReader(TargetFile.FullName, Encoding.Default, true))
            {
                reader.Peek(); // you need this!
                return reader.CurrentEncoding;
            }
        }

        internal static Dictionary<string, string> RemoveItemWithValue(this Dictionary<string, string> TargetDictionary, string RemoveValue)
        {
            foreach (KeyValuePair<string, string> StringItem in TargetDictionary.Where(KeyValuePair => KeyValuePair.Value == RemoveValue).ToList())
            {
                TargetDictionary.Remove(StringItem.Key);
            }

            return TargetDictionary;
        }

        internal static string RemovePrefix(this string Target, params string[] Prefixes)
        {
            if (Target.StartsWithOneOf(Prefixes))
            {
                foreach (string SinglePrefix in Prefixes)
                {
                    if (Target.StartsWith(SinglePrefix))
                    {
                        return Target[SinglePrefix.Length..];
                    }
                }
            }

            return Target;
        }

        internal static string RegexRemove(string TargetString, Regex PartPattern)
        {
            TargetString = PartPattern.Replace(TargetString, Match =>
            {
                return "";
            });
            return TargetString;
        }

        internal static void RegexRemove(this string TargetString, Regex PartPattern, bool s = false)
        {
            TargetString = PartPattern.Replace(TargetString, Match =>
            {
                return "";
            });
        }
        internal static string RemoveMany(this string TargetString, params string[] RemoveItems)
        {
            foreach(string RemoveItem in RemoveItems)
            {
                TargetString = TargetString.Replace(RemoveItem, "");
            }
            return TargetString;
        }

        internal static bool EqualsOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.Equals(Check)) return true;
            }

            return false;
        }
        internal static bool StartsWithOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.StartsWith(Check)) return true;
            }

            return false;
        }
        internal static bool EndsWithOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.EndsWith(Check)) return true;
            }

            return false;
        }
        internal static bool ContainsOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.Contains(Check)) return true;
            }

            return false;
        }
        internal static List<string> ItemsThatContain(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { };
            foreach (var Check in CheckSource)
            {
                if (Check.Contains(CheckString))
                {
                    Export.Add(Check);
                }
            }

            return Export;
        }
        internal static List<string> ItemsThatStartsWith(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { "" };
            foreach (var Check in CheckSource)
            {
                if (Check.StartsWith(CheckString))
                {
                    if (Export[0].Equals("")) Export.RemoveAt(0);
                    Export.Add(Check);
                }
            }

            return Export;
        }
        internal static List<string> ItemsThatEndsWith(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { "" };
            foreach (var Check in CheckSource)
            {
                if (Check.EndsWith(CheckString))
                {
                    if (Export[0].Equals("")) Export.RemoveAt(0);
                    Export.Add(Check);
                }
            }

            return Export;
        }
        internal static bool ContainsFileInfoWithName(this IEnumerable<FileInfo> CheckSource, string CheckString)
        {
            foreach (var file in CheckSource)
            {
                if (file.Name.Equals(CheckString)) return true;
            }

            return false;
        }
        internal static FileInfo? SelectWithName(this IEnumerable<FileInfo> Source, string Name)
        {
            Source = Source.Where(file => file.Name.Equals(Name));
            if (Source.Count() > 0)
            {
                return Source.ToList()[0];
            }
            else
            {
                return null;
            }
        }
        internal static string GetText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }
        internal static string[] GetLines(this FileInfo file)
        {
            return File.ReadAllLines(file.FullName);
        }
        internal static byte[] GetBytes(this FileInfo file)
        {
            return File.ReadAllBytes(file.FullName);
        }

        internal static List<string> RemoveAtIndex(this List<string> Source, int Index)
        {
            Source.RemoveAt(Index);
            return Source;
        }

        /// <summary>
        /// Formatter for [$] insertions
        /// </summary>
        internal static string Extern(this string TargetString, object Replacement)
        {
            return TargetString.Replace("[$]", $"{Replacement}");
        }
        /// <summary>
        /// Formatter for enumerated [$n] insertions
        /// </summary>
        internal static string Exform(this string TargetString, params object[] Replacements)
        {
            Dictionary<string, string> IndexReplacements = new();
            int ReplacementsIndexer = 1;
            foreach (object Replacement in Replacements)
            {
                IndexReplacements[$"[${ReplacementsIndexer}]"] = $"{Replacement}";
                ReplacementsIndexer++;
            }

            foreach (KeyValuePair<string, string> Replacement in IndexReplacements)
            {
                TargetString = TargetString.Replace(Replacement.Key, Replacement.Value);
            }

            return TargetString;
        }


        internal static bool IsNull(this object? item)
        {
            if (item == null)
            {
                return true;
            }
            else if (item.Equals("<Null>"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static void rin(params object[] s) => Console.WriteLine(String.Join(' ', s));
        internal static void rinx(params object[] s) { Console.WriteLine(String.Join(' ', s)); rinx(); }
        internal static void rinx() => Console.ReadKey();

        internal static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(
                    Path.Combine(
                        dirPath,
                        Path.GetRandomFileName()
                    ),
                    1,
                    FileOptions.DeleteOnClose)
                )
                { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

        /// <summary>
        /// Accepts RGB or ARGB hex sequence (#abcdef / #ffabcdef)<br/>
        /// Returns transperent if HexString is null or equals "", White if HexString is not color
        /// </summary>
        internal static SolidColorBrush ToColor(string HexString)
        {
            if (HexString.IsNull()) return System.Windows.Media.Brushes.White;

            if (HexString.Length == 7)
            {
                return new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Convert.ToByte(HexString.Substring(1, 2), 16),
                        Convert.ToByte(HexString.Substring(3, 2), 16),
                        Convert.ToByte(HexString.Substring(5, 2), 16)

                    )
                );
            }
            else if (HexString.Length == 9)
            {
                return new SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(
                        Convert.ToByte(HexString.Substring(1, 2), 16),
                        Convert.ToByte(HexString.Substring(3, 2), 16),
                        Convert.ToByte(HexString.Substring(5, 2), 16),
                        Convert.ToByte(HexString.Substring(7, 2), 16)
                    )
                );
            }
            else
            {
                return System.Windows.Media.Brushes.White;
            }
        }


        internal static System.Windows.Media.FontFamily FileToFontFamily(string FontPath, string OverrideFontInternalName = "")
        {
            if (File.Exists(FontPath))
            {
                string FontFullPath = new FileInfo(FontPath).FullName;
                Uri FontUri = new Uri(FontFullPath, UriKind.Absolute);
                //rin($"  Successful font loading from file \"{FontPath}\"");
                string FontInternalName = OverrideFontInternalName.Equals("") ? GetFontName(FontFullPath) : OverrideFontInternalName;
                return new System.Windows.Media.FontFamily(FontUri, $"./#{FontInternalName}");
            }
            else
            {
                //rin($"  Font file \"{FontPath}\" not found, returning Arial");
                return new System.Windows.Media.FontFamily("Arial");
            }
        }

        internal static string GetFontName(string FontPath)
        {
            using (System.Drawing.Text.PrivateFontCollection PrivateFonts = new())
            {
                PrivateFonts.AddFontFile(FontPath);
                string FontName = PrivateFonts.Families[0].Name;

                return FontName;
            }
        }



        internal static List<string> GetFilesWithExtensions(string path, params string[] Extensions)
        {
            return Directory.GetFiles(path, "*.*")
                            .Where(file => file.EndsWithOneOf(Extensions))
                            .ToList();
        }



        internal static string GetEscapeSequence(char c)
        {
            return ((int)c).ToString("X4");
        }
        internal static string ToUnicodeSequence(this string TargetString, string MaskString = "")
        {
            string Export = "";

            if (MaskString.Equals(""))
            {
                foreach (char c in TargetString)
                {
                    Export += GetEscapeSequence(c) + " ";
                }
            }
            else if (TargetString.Length == TargetString.Length)
            {
                int Indexer = 0;
                foreach (char c in TargetString)
                {
                    Export += GetEscapeSequence(c) + $"[{MaskString[Indexer]}] ";
                    Indexer++;
                }
            }

            return Export;
        }


        internal static BitmapImage GenerateBitmapFromFile(string ImageFilepath)
        {
            bool IsWebp = ImageFilepath.EndsWith(".webp");
            byte[] ImageData = File.ReadAllBytes(ImageFilepath);
            using (MemoryStream stream = new MemoryStream(IsWebp ? ConvertWebpToPng(ImageData) : ImageData))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        internal static byte[] ConvertWebpToPng(byte[] WebpData)
        {
            using (var InputStream = new MemoryStream(WebpData))
            using (var image = SixLabors.ImageSharp.Image.Load(InputStream))
            {
                using (var OutputStream = new MemoryStream())
                {
                    image.SaveAsPng(OutputStream);
                    return OutputStream.ToArray();
                }
            }
        }

        internal static string GetText(this System.Windows.Controls.RichTextBox From)
        {
            return new TextRange(From.Document.ContentStart, From.Document.ContentEnd).Text.Trim().Replace("\0", "");
        }

        internal static FontWeight WeightFrom(string StringVariant)
        {
            return StringVariant switch
            {
                     "Black" => FontWeights.Black,
                      "Bold" => FontWeights.Bold,
                  "DemiBold" => FontWeights.DemiBold,
                "ExtraBlack" => FontWeights.ExtraBlack,
                 "ExtraBold" => FontWeights.ExtraBold,
                "ExtraLight" => FontWeights.ExtraLight,
                     "Heavy" => FontWeights.Heavy,
                     "Light" => FontWeights.Light,
                    "Medium" => FontWeights.Medium,
                    "Normal" => FontWeights.Normal,
                   "Regular" => FontWeights.Regular,
                  "Semibold" => FontWeights.SemiBold,
                      "Thin" => FontWeights.Thin,
                "UltraBlack" => FontWeights.UltraBlack,
                 "UltraBold" => FontWeights.UltraBold,
                "UltraLight" => FontWeights.UltraLight,
                           _ => FontWeights.Regular,
            };
        }
    }
}
