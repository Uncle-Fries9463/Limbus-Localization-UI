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
using System.Windows.Controls.Primitives;
using static LC_Localization_Task_Absolute.UILanguageLoader;
using RichText;

namespace LC_Localization_Task_Absolute
{
    internal static class ConfigRegexSaver
    {
        // Because serealizing current config with all default values causes a lot of mess string values that hard to remove
        internal static void ChangeJsonConfigViaRegex(string PropertyName, dynamic NewValue, bool IsInsideCurrentCustomLangProperties = false)
        {
            string CurrentConfigurationJsonContent = File.ReadAllText(@"⇲ Assets Directory\Configurazione^.json");
            string OldConfig = CurrentConfigurationJsonContent;

            #region Change
            string MatchAppend = "";
            if (IsInsideCurrentCustomLangProperties)
            {
                /*lang=regex*/
                MatchAppend = @"(?<PatternInsideThisProperty>""Custom Language Associative Settings"": {(.*?)""Name"": ""<Current \0 Name>"",(.*?))"
                    .Replace(
                        @"<Current \0 Name>",
                        Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected.ToEscapeRegexString()
                    );
            }

            string ValueTypePattern = NewValue.GetType().ToString() switch
            {
                /*lang=regex*/ "System.Double"   => @"(\d+)(\.(\d+))?",
                /*lang=regex*/ "System.Boolean" => @"(true|false)",
                /*lang=regex*/ "System.String"    => @"""(.*?)""",
                /*lang=regex*/ _ => @"""(.*?)"""
            };
            rin(@$"{MatchAppend}""{PropertyName.ToEscapeRegexString()}"": {ValueTypePattern}(?<Afterward>(,)?(\r)?\n)");
            CurrentConfigurationJsonContent = Regex.Replace(CurrentConfigurationJsonContent, @$"{MatchAppend}""{PropertyName.ToEscapeRegexString()}"": {ValueTypePattern}(?<Afterward>(,)?(\r)?\n)", Match =>
            {
                string ValueReplacementString = ValueTypePattern switch
                    {
                        /*lang=regex*/ @"(\d+)(\.(\d+))?" => $"{NewValue.ToString().Replace(",", ".")}",
                        /*lang=regex*/ @"(true|false)"        => $"{NewValue.ToString().ToLower()}",
                        /*lang=regex*/ @"""(.*?)"""            => $"\"{NewValue}\"",
                        /*lang=regex*/ _                            => $"\"{NewValue}\"",
                    };
                return @$"{Match.Groups["PatternInsideThisProperty"].Value}""{PropertyName}"": {ValueReplacementString}{Match.Groups["Afterward"].Value}";
            }, MatchAppend.Equals("") ? RegexOptions.None : RegexOptions.Singleline);
            #endregion

            if (!OldConfig.Equals(CurrentConfigurationJsonContent))
            {
                File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", CurrentConfigurationJsonContent);
            }
        }
    }

    internal abstract class Configurazione
    {
        #region loads
        internal protected static ConfigDelta DeltaConfig = new ConfigDelta();

        internal protected static Regex ShorthandsPattern = new Regex("NOTHING THERE");

        internal protected static ShorthandInsertionProperty ShorthandsInsertionShape = new ShorthandInsertionProperty()
        {
            InsertionShape = "[<KeywordID>]",
            InsertionShape_Color = "",
        };

        internal protected static double KeywordSpriteHorizontalOffset = 0;
        internal protected static double KeywordSpriteVerticalOffset = -4;
        internal protected static double KeywordSpriteKeywordContainerHorizontalOffset = 0;

        internal protected static dynamic FormalTaskCompleted = null;

        internal protected static CustomLanguageAssociativePropertyMain SelectedAssociativePropery_Shared = null;

        internal protected static bool SettingsLoadingEvent = false;
        internal protected static void PullLoad()
        {
            if (File.Exists(@"⇲ Assets Directory\Configurazione^.json"))
            {
                try
                {
                    SettingsLoadingEvent = true;
                    rin($"\n\n\n[ Settings load pull initialized ]\n");

                    Configurazione.DeltaConfig = JsonConvert.DeserializeObject<Configurazione.ConfigDelta>(File.ReadAllText(@"⇲ Assets Directory\Configurazione^.json"));
                    rin($" Configuration file readed");

                    SettingsWindow.UpdateSettingsMenu_Regular();


                    KeywordsInterrogate.InitializeGlossaryFrom(
                        KeywordsDirectory: DeltaConfig.PreviewSettings.CustomLanguageProperties.KeywordsFallback.FallbackKeywordsDirectory
                    );

                    string SelectedAssociativePropertyName = DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected;
                    rin($"\n Custom language properties: {SelectedAssociativePropertyName}");


                    var SelectedAssociativePropery_Found = DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List
                        .Where(x => x.PropertyName.Equals(SelectedAssociativePropertyName)).ToList();

                    if (SelectedAssociativePropery_Found.Count() > 0)
                    {
                        SelectedAssociativePropery_Shared = SelectedAssociativePropery_Found[0];

                        UpdateCustomLanguagePart(SelectedAssociativePropery_Shared);

                        KeywordSpriteHorizontalOffset = SelectedAssociativePropery_Shared.Properties.KeywordsSpriteHorizontalOffset;
                        KeywordSpriteVerticalOffset = SelectedAssociativePropery_Shared.Properties.KeywordsSpriteVerticalOffset;

                        SettingsWindow.UpdateSettingsMenu_CustomLang();
                    }


                    SettingsLoadingEvent = false;
                }
                catch
                {
                    SettingsLoadingEvent = false;
                }
            }
        }

        internal protected static void UpdateCustomLanguagePart(CustomLanguageAssociativePropertyMain SelectedAssociativePropery)
        {
            KeywordsInterrogate.InitializeGlossaryFrom
            (
                KeywordsDirectory: SelectedAssociativePropery.Properties.KeywordsDirectory,
                WriteOverFallback: true
            );

            KeywordsInterrogate.ReadKeywordsMultipleMeanings(SelectedAssociativePropery.Properties.KeywordsMultipleMeaningsDictionary);


            LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection = SelectedAssociativePropery.Properties.Keywords_AutodetectionRegex;
            rin($"  Keywords Autodetection Regex Pattern: {LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection}");
            Configurazione.ShorthandsPattern = new Regex(SelectedAssociativePropery.Properties.Keywords_ShorthandsRegex);
            rin($"  Keywords Shorthands Regex Pattern: {Configurazione.ShorthandsPattern}");


            if (SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape != null)
            {
                ShorthandsInsertionShape.InsertionShape = SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape;
            }
            if (SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape_HexColor != null)
            {
                ShorthandsInsertionShape.InsertionShape_Color = SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape_HexColor;
            }

            rin($"   Loading fonts:");
            UpdatePreviewLayoutsFont(SelectedAssociativePropery.Properties);
        }



        internal protected static Task UpdatePreviewLayoutsFont(CustomLanguageAssociativePropertyValues Properties = null)
        {
            if (Properties != null)
            {
                if (File.Exists(Properties.ContextFont))
                {
                    rin($"    - Context font file: \"{Properties.ContextFont}\"");

                    FontFamily ContextFontFamily = FileToFontFamily(Properties.ContextFont, Properties.ContextFont_OverrideReadName, WriteInfo: true);
                    
                    foreach (RichTextBox PreviewLayoutItem in PreviewLayoutsList)
                    {
                        try
                        {
                            PreviewLayoutItem.FontFamily = ContextFontFamily;
                            PreviewLayoutItem.FontSize  *= Properties.ContextFont_FontSizeMultipler;
                            PreviewLayoutItem.FontWeight = WeightFrom(Properties.ContextFont_FontWeight);
                        }
                        catch { }
                    }
                    rin($"      Applied context font");
                }
                else
                {
                    rin($"    - [!] Context font file NOT FOUND (\"{Properties.ContextFont}\")");
                }


                if (File.Exists(Properties.TitleFont))
                {
                    rin($"    - Title font file: \"{Properties.TitleFont}\"");
                    FontFamily TitleFontFamily = FileToFontFamily(Properties.TitleFont, Properties.TitleFont_OverrideReadName, WriteInfo: true);

                    MainControl.NavigationPanel_ObjectName_Display.FontFamily = TitleFontFamily;
                    rin($"      Applied title font");
                    MainControl.NavigationPanel_ObjectName_Display.FontWeight = WeightFrom(Properties.TitleFont_FontWeight);

                    MainControl.NavigationPanel_ObjectName_Display.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.PreviewLayout_Keywords_Bufs_Name.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.EGOGiftName_PreviewLayout.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.STE_EGOGifts_LivePreview_ViewDescButtons.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.PreviewLayout_Keywords_BattleKeywords_Name.FontSize *= Properties.TitleFont_FontSizeMultipler;
                }
                else
                {
                    rin($"    - [!] Title font file NOT FOUND (\"{Properties.TitleFont}\")");
                }


                if (File.Exists(@"⇲ Assets Directory\[⇲] Limbus Images\UI\BattleKeywords Background.png"))
                {
                    MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords_BackgroundImage.Source = GenerateBitmapFromFile(@"⇲ Assets Directory\[⇲] Limbus Images\UI\BattleKeywords Background.png");
                }

                string StringWidthSkills = Configurazione.DeltaConfig.ScanParameters.AreaWidth.ToString();
                SettingsControl.InputSkillsPanelWidth.Text = StringWidthSkills;
            }

            return FormalTaskCompleted;
        }
        #endregion

        internal protected class ShorthandInsertionProperty
        {
            [JsonProperty("(Context Menu) Insertion Shape")]
            public string InsertionShape { get; set; } = "[<KeywordID>:`<KeywordName>`]<KeywordColor>";

            [JsonProperty("(Context Menu) Insertion Shape (Color)")]
            public string InsertionShape_Color { get; set; } = "<HexColor>";
        }

        internal protected class ConfigDelta
        {
            [JsonProperty("Internal")]
            public Internal Internal { get; set; } = new Internal();

            [JsonProperty("Preview Settings")]
            public PreviewSettings PreviewSettings { get; set; } = new PreviewSettings();

            [JsonProperty("Scan Parameters")]
            public ScanParameters ScanParameters { get; set; } = new ScanParameters();

            [JsonProperty("Technical Actions")]
            public TechnicalActions TechnicalActions { get; set; } = new TechnicalActions();
        }
        internal protected class Internal
        {
            [JsonProperty("UI Language")]
            public string UILanguage { get; set; } = "";

            [JsonProperty("UI Theme")]
            public string UITheme { get; set; } = "";

            [JsonProperty("Topmost Window")]
            public bool AlwaysOnTop { get; set; } = true;

            [OnDeserialized]
            internal void OnDeserialized(StreamingContext context)
            {
                UILanguageLoader.InitializeUILanguage(UILanguage);
                rin($" UI Language loaded from \"{UILanguage}\"");
                UIThemesLoader.InitializeUITheme(UITheme);
                rin($" UI Theme loaded from \"{UITheme}\"");

                MainControl.Topmost = AlwaysOnTop;
                rin($" Always on top: {AlwaysOnTop}");
            }
        }
        internal protected class PreviewSettings
        {
            [JsonProperty("Base")]
            public PreviewSettingsBaseSettings PreviewSettingsBaseSettings { get; set; } = new PreviewSettingsBaseSettings();

            [JsonProperty("Custom Language Properties")]
            public CustomLanguageProperties CustomLanguageProperties { get; set; } = new CustomLanguageProperties();
        }
        internal protected class PreviewSettingsBaseSettings
        {
            [JsonProperty("Preview Update Delay (Seconds)")]
            public double PreviewUpdateDelay { get; set; } = 0.00;

            [JsonProperty("Highlight <style>")]
            public bool HighlightStyle { get; set; } = true;

            [JsonProperty("Highlight Coin Descs on right click")]
            public bool HighlightCoinDescsOnRightClick { get; set; } = true;

            [JsonProperty("Highlight Coin Descs on manual switch")]
            public bool HighlightCoinDescsOnManualSwitch { get; set; } = false;
        }
        internal protected class CustomLanguageProperties
        {
            [JsonProperty("Keywords Ignore")]
            public List<string> KeywordsIgnore { get; set; } = new List<string>();

            [JsonProperty("Keywords Fallback")]
            public FallbackKeywords KeywordsFallback { get; set; } = new FallbackKeywords();

            [JsonProperty("Custom Language Associative Settings")]
            public CustomLanguageAssociativeSettings AssociativeSettings { get; set; }
        }
        internal protected class FallbackKeywords
        {
            [JsonProperty("Directory")]
            public string FallbackKeywordsDirectory { get; set; } = "";
        }
        internal protected class CustomLanguageAssociativeSettings
        {
            [JsonProperty("Associative Properties Selected")]
            public string Selected { get; set; } = "";

            [JsonProperty("Associative Properties List")]
            public List<CustomLanguageAssociativePropertyMain> List { get; set; } = new List<CustomLanguageAssociativePropertyMain>();
        }
        internal protected class CustomLanguageAssociativePropertyMain
        {
            [JsonProperty("Name")]
            public string PropertyName { get; set; } = "<none>";

            [JsonProperty("Hide in list")]
            public bool HideInList { get; set; } = false;

            [JsonProperty("Properties")]
            public CustomLanguageAssociativePropertyValues Properties { get; set; } = new CustomLanguageAssociativePropertyValues();
        }
        internal protected class CustomLanguageAssociativePropertyValues
        {
            [JsonProperty("Keywords Directory")]
            public string KeywordsDirectory { get; set; } = "";

            [JsonProperty("Keywords Autodetection Regex Pattern")]
            public string Keywords_AutodetectionRegex { get; set; } = new Regex(@"(KeywordNameWillBeHere)(?![\p{L}\[\]\-<'"":+])").ToString();

            [JsonProperty("Keywords Shorthands Regex Pattern")]
            public string Keywords_ShorthandsRegex { get; set; } = new Regex(@"NOTHING THERE").ToString();

            [JsonProperty("Keywords Shorthands Contextmenu Insertion Shape")]
            public string Keywords_ShorthandsContextMenuInsertionShape { get; set; }

            [JsonProperty("Keywords Shorthands Contextmenu Insertion Shape <KeywordColor>")]
            public string Keywords_ShorthandsContextMenuInsertionShape_HexColor { get; set; }

            [JsonProperty("Keywords Multiple Meanings Dictionary")]
            public string KeywordsMultipleMeaningsDictionary { get; set; } = "";

            [JsonProperty("Keywords Sprite Horizontal Offset")]
            public double KeywordsSpriteHorizontalOffset { get; set; } = 0;

            [JsonProperty("Keywords Sprite Vertical Offset")]
            public double KeywordsSpriteVerticalOffset { get; set; } = -4;



            [JsonProperty("Title Font")]
            public string TitleFont { get; set; } = "";

            [JsonProperty("Title Font (Override Read Name)")]
            public string TitleFont_OverrideReadName { get; set; } = "";

            [JsonProperty("Title Font (Font Weight)")]
            public string TitleFont_FontWeight { get; set; } = "";

            [JsonProperty("Title Font (Font Size Multipler)")]
            public double TitleFont_FontSizeMultipler { get; set; } = 1.0;

            
            [JsonProperty("Context Font")]
            public string ContextFont { get; set; } = "";
            [JsonProperty("Context Font (Override Read Name)")]
            public string ContextFont_OverrideReadName { get; set; } = "";
            [JsonProperty("Context Font (Font Weight)")]
            public string ContextFont_FontWeight { get; set; } = "";
            [JsonProperty("Context Font (Font Size Multipler)")]
            public double ContextFont_FontSizeMultipler { get; set; } = 1.0;
        }
        internal protected class ScanParameters
        {
            [JsonProperty("Skills Area Width")]
            public double AreaWidth { get; set; } = 0;
        }
        internal protected class TechnicalActions
        {
            [JsonProperty("Keywords Multiple Meanings Dictionary")]
            public TA_KeywordsDictionary KeywordsDictionary { get; set; } = new TA_KeywordsDictionary();
        }
        internal protected class TA_KeywordsDictionary
        {
            [JsonProperty("Generate On Startup")]
            public bool Generate { get; set; } = false;

            [JsonProperty("Source Path")]
            public string Path { get; set; } = "";

            [JsonProperty("Comment")]
            public string Comment { get; set; } = "";
        }
    }
}
