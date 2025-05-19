using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using LC_Localization_Task_Absolute.Json;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.MainWindow;
using LC_Localization_Task_Absolute.Limbus_Integration;

namespace LC_Localization_Task_Absolute
{
    internal static class S
    {
        internal static void Save(this Configurazione.SettingsValue Target)
        {
            Target.MarkSerialize(@"⇲ Assets Directory\Configurazione.json");
        }
    }
    internal abstract class Configurazione
    {
        internal protected static SettingsValue Settings = new();
        internal protected static FontFamily MainText_FontFamily;

        internal protected static Regex ShorthandsPattern = new Regex("");

        internal protected static string KeywordsFolder;

        internal protected static AssociativeKeywordsSettings SelectedLanguageProperties;
        internal protected static ShorthandsProperty SelectedShorthands = new()
        {
            Regex = @"\[(?<ID>\w+):`(?<Name>.*?)`\](?<Color>\(#[a-fA-F0-9]{6}\))?",
            InsertionShape = "[<KeywordID>:`<KeywordName>`][<KeywordColor>]",
            InsertionShape_Color = "([$])",
        };

        internal protected static double KeywordSpriteHorizontalOffset = 0;
        internal protected static double KeywordSpriteVerticalOffset = 0;
        internal protected static double KeywordSpriteKeywordContainerHorizontalOffset = 0;

        internal protected static dynamic FormalTaskCompleted = null;

        internal protected static void PullLoad()
        {
            rin($"[ Settings load pull initialized ]\n");

            Settings = JsonConvert.DeserializeObject<Configurazione.SettingsValue>(File.ReadAllText(@"⇲ Assets Directory\Configurazione.json"));

            rin("※ Keywords settings:");
            rin($"  Fallback Keywords: \"{Settings.Preview.FallbackKeywordsFolder}\"");

            List<ShorthandsProperty> SelectedShorthandsProperties_Found = Settings.Preview.Shorthands.Where(x => x.Name.Equals(Settings.Preview.SelectedShorthands)).ToList();
            if (SelectedShorthandsProperties_Found.Count > 0)
            {
                Configurazione.SelectedShorthands = SelectedShorthandsProperties_Found[0];
                Configurazione.ShorthandsPattern = new Regex(SelectedShorthands.Regex);
            }

            List<AssociativeKeywordsSettings> SelectedKeywordsPropertyFound = Settings.Preview.KeywordsAssociativeSettings.Where(x => x.FolderName.Equals(Settings.Preview.KeywordsFolder)).ToList();
            if (SelectedKeywordsPropertyFound.Count > 0)
            {
                Configurazione.SelectedLanguageProperties = SelectedKeywordsPropertyFound[0];
                rin($"  Keywords: \"{SelectedLanguageProperties.FolderName}\"");

                if (!Configurazione.SelectedLanguageProperties.KeywordSpriteHorizontalOffset.IsNull())
                {
                    Configurazione.KeywordSpriteHorizontalOffset = (double)Configurazione.SelectedLanguageProperties.KeywordSpriteHorizontalOffset;
                    rin($"  Sprites Horizontal Offset: {Configurazione.KeywordSpriteHorizontalOffset}");
                }

                if (!Configurazione.SelectedLanguageProperties.KeywordSpriteVerticalOffset.IsNull())
                {
                    Configurazione.KeywordSpriteVerticalOffset = (double)Configurazione.SelectedLanguageProperties.KeywordSpriteVerticalOffset;
                    rin($"  Sprites Vertical Offset: {Configurazione.KeywordSpriteVerticalOffset}");
                }

                if (!Configurazione.SelectedLanguageProperties.AutoKeywordsDetectionRegex.IsNull())
                {
                    LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection = Configurazione.SelectedLanguageProperties.AutoKeywordsDetectionRegex;
                    rin($"  Keywords Auto Keywords Detection: {LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection}");
                }

                if (!Configurazione.SelectedLanguageProperties.MainText_FontWeigt.IsNull())
                {
                    FontWeight FontWeight = WeightFrom(Configurazione.SelectedLanguageProperties.MainText_FontWeigt);
                    rin($"  Main Text Font Weight: {FontWeight}");
                    foreach (RichTextBox PreviewLayoutItem in MainWindow.PreviewLayoutsList)
                    {
                        PreviewLayoutItem.FontWeight = FontWeight;
                    }
                }

                if (!Configurazione.SelectedLanguageProperties.TitlesAndNames_FontWeight.IsNull())
                {
                    MainControl.NavigationPanel_ObjectName_Display.FontWeight = WeightFrom(Configurazione.SelectedLanguageProperties.TitlesAndNames_FontWeight);
                }
                if (Configurazione.SelectedLanguageProperties.TitlesAndNames_FontSize != 0)
                {
                    MainControl.NavigationPanel_ObjectName_Display.FontSize = Configurazione.SelectedLanguageProperties.TitlesAndNames_FontSize;
                }
            }


            KeywordsInterrogate.InitializeGlossaryFrom(@$"⇲ Assets Directory\[+] Keywords\Text\{Settings.Preview.FallbackKeywordsFolder}");

            UpdatePreviewLayoutsFont();
            if (!Settings.Preview.KeywordsCustomFolder.Enabled & !Configurazione.SelectedLanguageProperties.FolderName.IsNull())
            {
                Configurazione.KeywordsFolder = Configurazione.SelectedLanguageProperties.FolderName;
                KeywordsInterrogate.InitializeGlossaryFrom(@$"⇲ Assets Directory\[+] Keywords\Text\{Settings.Preview.KeywordsFolder}", WriteOverFallback: true);
            }
            else
            {
                rin($"  Custom keywords folder: \"{Settings.Preview.KeywordsCustomFolder.Path}\"");
                Configurazione.KeywordsFolder = Settings.Preview.KeywordsCustomFolder.Path;
                KeywordsInterrogate.InitializeGlossaryFrom(Settings.Preview.KeywordsCustomFolder.Path, WriteOverFallback: true);
            }

            if (!Configurazione.SelectedLanguageProperties.KeywordsMultipleMeaningsFile.Equals(""))
            {
                string FinalPath = "";

                if (File.Exists(Configurazione.SelectedLanguageProperties.KeywordsMultipleMeaningsFile))
                {
                    FinalPath = Configurazione.SelectedLanguageProperties.KeywordsMultipleMeaningsFile;
                }
                else if (File.Exists(@$"⇲ Assets Directory\[+] Keywords\Text\{Settings.Preview.KeywordsFolder}\{Configurazione.SelectedLanguageProperties.KeywordsMultipleMeaningsFile}"))
                {
                    FinalPath = @$"⇲ Assets Directory\[+] Keywords\Text\{Settings.Preview.KeywordsFolder}\{Configurazione.SelectedLanguageProperties.KeywordsMultipleMeaningsFile}";
                }

                if (!FinalPath.Equals(""))
                {
                    rin($"  Keywords Multiple Meanings file: {FinalPath}");
                    KeywordsInterrogate.ReadKeywordsMultipleMeanings(FinalPath);
                }
            }
        }



        internal protected static Task UpdatePreviewLayoutsFont()
        {
            string MainText_FontPath = @$"⇲ Assets Directory\[+] Keywords\Associative Fonts\{SelectedLanguageProperties.MainText_FontFile}";
            string TitlesAndNames_FontPath = @$"⇲ Assets Directory\[+] Keywords\Associative Fonts\{SelectedLanguageProperties.TitlesAndNames_FontFile}";

            if (File.Exists(MainText_FontPath))
            {
                string OverrideInternalFontName = "";
                if (!Configurazione.SelectedLanguageProperties.MainText_OverrideFontInternalName.IsNull()) OverrideInternalFontName = SelectedLanguageProperties.MainText_OverrideFontInternalName;

                MainText_FontFamily = FileToFontFamily(MainText_FontPath, OverrideInternalFontName);

                foreach (RichTextBox PreviewLayoutItem in PreviewLayoutsList)
                {
                    PreviewLayoutItem.FontFamily = MainText_FontFamily;
                }
            }


            if (File.Exists(TitlesAndNames_FontPath) & !SelectedLanguageProperties.TitlesAndNames_FontFile.IsNull())
            {
                string OverrideObjectNameInternalFontName = "";
                if (!Configurazione.SelectedLanguageProperties.TitlesAndNames_OverrideFontInternalName.IsNull()) OverrideObjectNameInternalFontName = SelectedLanguageProperties.TitlesAndNames_OverrideFontInternalName;

                FontFamily NameFontFamily = FileToFontFamily(TitlesAndNames_FontPath, OverrideObjectNameInternalFontName);

                // All other title and names fonts binded to this element fontfamily
                MainControl.NavigationPanel_ObjectName_Display.FontFamily = NameFontFamily;
            }
            
            if (File.Exists(@"⇲ Assets Directory\[⇲] Limbus Images\UI\BattleKeywords Background.png"))
            {
                MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords_BackgroundImage.Source = GenerateBitmapFromFile(@"⇲ Assets Directory\[⇲] Limbus Images\UI\BattleKeywords Background.png");
            }

            return FormalTaskCompleted;
        }

        internal protected static Task ChangeBackgroundImageVisibility(bool ShowBackgroundImage)
        {
            MainControl.BackgroundImage.Visibility = ShowBackgroundImage switch
            {
                true => Visibility.Visible,
                false => Visibility.Collapsed,
            };

            return FormalTaskCompleted;
        }

        internal protected static Task ChangeBackgroundImageShadowTransperacy(string BackgroundImageTransperacy)
        {
            MainControl.BackgroundImageShadowingColor.Background = ToColor(BackgroundImageTransperacy);

            return FormalTaskCompleted;
        }

        internal protected class SettingsValue
        {
            public Internal Internal { get; set; }
            public Preview Preview { get; set; }

            [JsonProperty("Technical Actions")]
            public TechnicalActions TechnicalActions { get; set; }
        }
        internal protected class Internal
        {
            [JsonProperty("UI Language (⇲ Assets Directory/[+] Languages/)")]
            public string UILanguage { get; set; }

            [JsonProperty("UI Theme (⇲ Assets Directory/[+] Themes/)")]
            public string UITheme { get; set; }

            [JsonProperty("Topmost Window")]
            public bool? AlwaysOnTop { get; set; }

            [OnDeserialized]
            internal void OnInit(StreamingContext context)
            {
                UILanguageLoader.InitializeUILanguage(UILanguage);
                UIThemesLoader.InitializeUITheme(UITheme);

                if (!AlwaysOnTop.IsNull())
                {
                    MainControl.Topmost = (bool)AlwaysOnTop;
                }
            }
        }
        
        internal protected class Preview
        {
            [JsonProperty("Information Board [README]")]
            public List<string> Readme { get; set; }

            [JsonProperty("Preview Update Delay (Seconds)")]
            public float PreviewUpdateDelay { get; set; }

            [JsonProperty("Highlight <style>")]
            public bool EnableStyleHighlight { get; set; }

            [JsonProperty("Highlight Coin Descs on click")]
            public bool HighlightCoinDescsOnClick { get; set; }

            [JsonProperty("Highlight Coin Descs on manual switch")]
            public bool HighlightCoinDescsOnManualSwitch { get; set; }

            [JsonProperty("Fallback Keywords Folder (⇲ Assets Directory/[+] Keywords/Text/)")]
            public string FallbackKeywordsFolder { get; set; }
            
            [JsonProperty("Keywords Folder (⇲ Assets Directory/[+] Keywords/Text/)")]
            public string KeywordsFolder { get; set; }

            [JsonProperty("Keywords Associative Settings (⇲ Assets Directory/[+] Keywords/Associative Fonts)")]
            public List<AssociativeKeywordsSettings> KeywordsAssociativeSettings { get; set; }

            [JsonProperty("Keywords Custom Folder")]
            public KeywordsCustomFolder KeywordsCustomFolder { get; set; }

            [JsonProperty("Selected Shorthands")]
            public string SelectedShorthands { get; set; }

            public List<ShorthandsProperty> Shorthands { get; set; }

            [JsonProperty("Keywords Ignore")]
            public List<string> KeywordsIgnore { get; set; }

            [OnDeserialized]
            internal void OnInit(StreamingContext context)
            {
                Configurazione.KeywordsFolder = KeywordsFolder;
            }
        }
        internal protected class AssociativeKeywordsSettings
        {
            [JsonProperty("Folder Name")]
            public string FolderName { get; set; }

            [JsonProperty("[Main Text] Font File")]
            public string MainText_FontFile { get; set; }
            
            [JsonProperty("[Main Text] Font Weight")]
            public string MainText_FontWeigt { get; set; }

            [JsonProperty("[Main Text] Font Override Internal Name")]
            public string MainText_OverrideFontInternalName { get; set; }

            [JsonProperty("[Titles and Names] Font File")]
            public string TitlesAndNames_FontFile { get; set; }

            [JsonProperty("[Titles and Names] Font Weight")]
            public string TitlesAndNames_FontWeight { get; set; }

            [JsonProperty("[Titles and Names] Font Size")]
            public double TitlesAndNames_FontSize { get; set; }

            [JsonProperty("[Titles and Names] Font Override Internal Name")]
            public string TitlesAndNames_OverrideFontInternalName { get; set; }

            [JsonProperty("Keyword Sprite Horizontal Offset")]
            public double? KeywordSpriteHorizontalOffset { get; set; }
            
            [JsonProperty("Keyword Sprite Vertical Offset")]
            public double? KeywordSpriteVerticalOffset { get; set; }

            [JsonProperty("Keywords Auto Detection Regex Pattern")]
            public string AutoKeywordsDetectionRegex { get; set; }

            [JsonProperty("Keywords Multiple Meanings File")]
            public string KeywordsMultipleMeaningsFile { get; set; }

            [OnDeserialized]
            internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }
        internal protected class KeywordsCustomFolder
        {
            public bool Enabled { get; set; }
            public string Path { get; set; }
        }
        internal protected class ShorthandsProperty
        {
            public string Name { get; set; }
            public string Regex { get; set; }
            public string Comment { get; set; }

            [JsonProperty("(Context Menu) Insertion Shape")]
            public string InsertionShape { get; set; }

            [JsonProperty("(Context Menu) Insertion Shape (Color)")]
            public string InsertionShape_Color { get; set; }
        }
        internal protected class TechnicalActions
        {
            [JsonProperty("Keywords Multiple Meanings Dictionary")]
            public TechnicalActions_KeywordsMultipleMeanings KeywordsMultipleMeanings { get; set; }
        }
        internal protected class TechnicalActions_KeywordsMultipleMeanings
        {
            [JsonProperty("Generate On Startup")]
            public bool GenerateOnStartup { get; set; }

            [JsonProperty("Source Path")]
            public string SourcePath { get; set; }

            [OnDeserialized] internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }
    }
}
