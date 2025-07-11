using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Mode_Handlers;
using RichText;
using static System.Windows.Visibility;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.UIThemesLoader;

namespace LC_Localization_Task_Absolute
{
    public partial class SettingsWindow : Window
    {
        internal protected static SettingsWindow SettingsControl;

        private static Dictionary<string, int> FontWeights = new Dictionary<string, int>();
        public SettingsWindow()
        {
            InitializeComponent();

            SettingsControl = this;
        }

        internal protected static void UpdateSettingsMenu_Regular()
        {
            SettingsControl.UpdateSettingsMenu_Inner();
        }

        internal protected static void UpdateSettingsMenu_CustomLang()
        {
            SettingsControl.UpdateSelectedCustomLanguageSettingsView();
        }

        private void UpdateSettingsMenu_Inner()
        {
            Configurazione.ConfigDelta Settings = Configurazione.DeltaConfig;

            ToggleStyleHighlightion_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle ? Visible : Collapsed;
            ToggleCoinDescHighlightion_OnClick_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick ? Visible : Collapsed;
            ToggleCoinDescHighlightion_OnSwitch_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch ? Visible : Collapsed;
            InputPreviewUpdateDelay.Text = Settings.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay.ToString();
            ToggleTopmostState_I.Visibility = Settings.Internal.AlwaysOnTop ? Visible : Collapsed;

            rin("\n\n----------------------------------------------------");

            Dictionary<string, int> ThemeIndexes = new Dictionary<string, int>();
            int Index_Themes = 0;
            ThemeSelector.Items.Clear();
            foreach (var ThemeDir in new DirectoryInfo(@"⇲ Assets Directory\[+] Themes").GetDirectories())
            {
                ThemeSelector.Items.Add(new TextBlock { Text = ThemeDir.Name });
                ThemeIndexes[ThemeDir.Name] = Index_Themes;
                Index_Themes++;
            }
            if (Directory.Exists(Settings.Internal.UITheme))
            {
                string RelativeThemePathName = Settings.Internal.UITheme.Split("\\")[^1].Split("/")[^1];
                if (ThemeIndexes.ContainsKey(RelativeThemePathName))
                {
                    ThemeSelector.SelectedIndex = ThemeIndexes[RelativeThemePathName];
                }
            }




            int Index_Languages = 0;
            Dictionary<string, int> LanguageIndexes = new Dictionary<string, int>();
            LanguageSelector.Items.Clear();
            foreach (var LanguageFile in new DirectoryInfo(@"⇲ Assets Directory\[+] Languages").GetFiles("*.json"))
            {
                LanguageSelector.Items.Add(new TextBlock { Text = LanguageFile.Name.Replace(".json", "") });
                LanguageIndexes[LanguageFile.Name.Replace(".json", "")] = Index_Languages;
                Index_Languages++;
            }
            if (File.Exists(Settings.Internal.UILanguage))
            {
                string RelativeLangFileName = Settings.Internal.UILanguage.Split("\\")[^1].Split("/")[^1].Replace(".json", "");
                if (LanguageIndexes.ContainsKey(RelativeLangFileName))
                {
                    LanguageSelector.SelectedIndex = LanguageIndexes[RelativeLangFileName];
                }
            }

            int Index_CustomLanguageProperties = 0;
            Dictionary<string, int> CustomLanguagePropIndexes = new Dictionary<string, int>();
            CustomLanguagePropertiesSelector.Items.Clear();
            foreach (var CustomLanguageProperty in Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List)
            {
                if (!CustomLanguageProperty.HideInList)
                {
                    CustomLanguagePropertiesSelector.Items.Add(new TextBlock { Text = CustomLanguageProperty.PropertyName });
                    CustomLanguagePropIndexes[CustomLanguageProperty.PropertyName] = Index_CustomLanguageProperties;
                    Index_CustomLanguageProperties++;

                    if (Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected.Equals(CustomLanguageProperty.PropertyName))
                    {
                        CustomLanguagePropertiesSelector.SelectedIndex = CustomLanguagePropIndexes[CustomLanguageProperty.PropertyName];
                    }
                }
            }

        }

        private void UpdateSelectedCustomLanguageSettingsView()
        {
            Configurazione.CustomLanguageAssociativePropertyValues Settings = Configurazione.SelectedAssociativePropery_Shared.Properties;
            CustomLang_KeywordsDir.Text = Settings.KeywordsDirectory;
            CustomLang_TitleFont.Text = Settings.TitleFont;
            CustomLang_ContextFont.Text = Settings.ContextFont;
        }

        private void Settings_Minimize(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void Settings_Close(object sender, MouseButtonEventArgs e) => DoClose();
        private void DoClose()
        {
            MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(2);
            ScansManager.ToggleScanAreaView();
            this.Hide();
        }

        private void Settings_ReloadConfig(object sender, MouseButtonEventArgs e) => MainWindow.ReloadConfig_Direct();
        private void Window_DragMove(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void AntiComboBoxScroll(object sender, MouseWheelEventArgs e)
        {
            if (!(sender as ComboBox).IsDropDownOpen)
            {
                e.Handled = true;
            }
        }

        // idontwanttoserializeidontwanttoserializeidontwanttoserialize
        private void OptionToggle(object sender, MouseButtonEventArgs e)
        {
            if (!Configurazione.SettingsLoadingEvent)
            {
                string Sender = (sender as FrameworkElement).Name;
                string TempConfigFile = File.ReadAllText(@"⇲ Assets Directory\Configurazione^.json");
                string SelectedPropertiesName = Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected;


                string Input_Simplified = "";
                string Current_Simplified = "";

                switch (Sender)
                {
                    case "ToggleScansPreview":
                        ScansManager.ToggleScanAreaView();
                        break;



                    case "ToggleStyleHighlightion":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle;
                        ToggleStyleHighlightion_I.Visibility = ToggleStyleHighlightion_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };
                        TempConfigFile = Regex.Replace(TempConfigFile, @"""Highlight <style>"": (true|false)(?<Afterward>(,)?(\r)?\n)", Match =>
                        {
                            return @$"""Highlight <style>"": {Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle.ToString().ToLower()}{Match.Groups["Afterward"].Value}";
                        });
                        File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                        RichTextBoxApplicator.SetLimbusRichText(RichText.RichTextBoxApplicator.LastUpdateTarget, RichText.RichTextBoxApplicator.LastUpdateText);
                        break;



                    case "ToggleCoinDescHighlightion_OnClick":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick;
                        ToggleCoinDescHighlightion_OnClick_I.Visibility = ToggleCoinDescHighlightion_OnClick_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };
                        TempConfigFile = Regex.Replace(TempConfigFile, @"""Highlight Coin Descs on right click"": (true|false)(?<Afterward>(,)?(\r)?\n)", Match =>
                        {
                            return @$"""Highlight Coin Descs on right click"": {Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick.ToString().ToLower()}{Match.Groups["Afterward"].Value}";
                        });
                        File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                        RichTextBoxApplicator.SetLimbusRichText(RichText.RichTextBoxApplicator.LastUpdateTarget, RichText.RichTextBoxApplicator.LastUpdateText);
                        break;



                    case "ToggleCoinDescHighlightion_OnSwitch":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch;
                        ToggleCoinDescHighlightion_OnSwitch_I.Visibility = ToggleCoinDescHighlightion_OnSwitch_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };
                        TempConfigFile = Regex.Replace(TempConfigFile, @"""Highlight Coin Descs on manual switch"": (true|false)(?<Afterward>(,)?(\r)?\n)", Match =>
                        {
                            return @$"""Highlight Coin Descs on manual switch"": {Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch.ToString().ToLower()}{Match.Groups["Afterward"].Value}";
                        });
                        File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                        RichTextBoxApplicator.SetLimbusRichText(RichText.RichTextBoxApplicator.LastUpdateTarget, RichText.RichTextBoxApplicator.LastUpdateText);
                        break;



                    case "ToggleTopmostState":
                        Configurazione.DeltaConfig.Internal.AlwaysOnTop = !Configurazione.DeltaConfig.Internal.AlwaysOnTop;
                        MainControl.Topmost = Configurazione.DeltaConfig.Internal.AlwaysOnTop;
                        ToggleTopmostState_I.Visibility = ToggleTopmostState_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };
                        TempConfigFile = Regex.Replace(TempConfigFile, @"""Topmost Window"": (true|false)(?<Afterward>(,)?(\r)?\n)", Match =>
                        {
                            return @$"""Topmost Window"": {Configurazione.DeltaConfig.Internal.AlwaysOnTop.ToString().ToLower()}{Match.Groups["Afterward"].Value}";
                        });
                        File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);
                        break;



                    case "Recheck_PreviewUpdateDelay":
                        try
                        {
                            double InputDelay = double.Parse(InputPreviewUpdateDelay.Text.Replace(".", ","));

                            Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay = InputDelay;
                            string StringedDelay = InputDelay.ToString();
                            InputPreviewUpdateDelay.Text = StringedDelay;
                            TempConfigFile = Regex.Replace(TempConfigFile, @"""Preview Update Delay \(Seconds\)"": (\d+)(\.(\d+))?(?<Afterward>(,)?(\r)?\n)", Match =>
                            {
                                return @$"""Preview Update Delay (Seconds)"": {StringedDelay.Replace(",", ".")}{Match.Groups["Afterward"].Value}";
                            });
                            File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);
                        }
                        catch
                        {
                            MessageBox.Show("Not a number!");
                        }
                        break;



                    case "Recheck_SkillsPanelWidth":
                        try
                        {
                            double NewSkillsWidth = double.Parse(SettingsControl.InputSkillsPanelWidth.Text.Replace(".", ","));

                            SettingsControl.InputSkillsPanelWidth.Text = NewSkillsWidth.ToString();

                            Configurazione.DeltaConfig.ScanParameters.AreaWidth = NewSkillsWidth;
                            if (MainControl.ScanAreaView_Skills.BorderThickness.Top != 0)
                            {
                                if (NewSkillsWidth != 0)
                                {
                                    MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = NewSkillsWidth;
                                }
                                else
                                {
                                    MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
                                }
                            }

                            string StringWidthSkills = NewSkillsWidth.ToString();
                            InputSkillsPanelWidth.Text = StringWidthSkills;
                            TempConfigFile = Regex.Replace(TempConfigFile, @"""Skills Area Width"": (\d+)(\.(\d+))?(?<Afterward>(,)?(\r)?\n)", Match =>
                            {
                                return @$"""Skills Area Width"": {StringWidthSkills.Replace(",", ".")}{Match.Groups["Afterward"].Value}";
                            });
                            File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);
                        }
                        catch (Exception ex) { rin(ex.ToString()); }
                        break;





                    case "Recheck_KeywordsDir":
                        if (Directory.Exists(CustomLang_KeywordsDir.Text))
                        {
                            Input_Simplified = CustomLang_KeywordsDir.Text.Replace("\\", "/").ToLower();
                            Current_Simplified = Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory.Replace("\\", "/").ToLower();

                            if (!Input_Simplified.Equals(Current_Simplified))
                            {
                                string Pattern_KeywordDir = new Regex(@"""Custom Language Associative Settings"": {(?<Between1>.*?)""Name"": ""<NAMEHERE>"",(?<Between2>.*?)""Keywords Directory"": ""(?<KeywordsDir>.*?)""(?<Afterward>(,)?(\r)?\n)").ToString();
                                Pattern_KeywordDir = Pattern_KeywordDir.Replace(
                                    "<NAMEHERE>",
                                    SelectedPropertiesName.ToEscapeRegexString()
                                );

                                string FormattedPath = CustomLang_KeywordsDir.Text.Replace("\\", "/");
                                TempConfigFile = Regex.Replace(TempConfigFile, Pattern_KeywordDir, Match =>
                                {
                                    return @$"""Custom Language Associative Settings"": {{{Match.Groups["Between1"].Value}""Name"": ""{SelectedPropertiesName}"",{Match.Groups["Between2"].Value}""Keywords Directory"": ""{FormattedPath}""{Match.Groups["Afterward"].Value}";
                                }, RegexOptions.Singleline);
                                CustomLang_KeywordsDir.Text = FormattedPath;
                                File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                                Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory = FormattedPath;
                                Configurazione.UpdateCustomLanguagePart(Configurazione.SelectedAssociativePropery_Shared);
                            }
                        }
                        else
                        {
                            MessageBox.Show("This directory does not exists!");
                        }

                        break;


                    case "Recheck_TitleFont":
                        if (File.Exists(CustomLang_TitleFont.Text))
                        {
                            Input_Simplified = CustomLang_TitleFont.Text.Replace("\\", "/").ToLower();
                            Current_Simplified = Configurazione.SelectedAssociativePropery_Shared.Properties.TitleFont.Replace("\\", "/").ToLower();

                            if (!Input_Simplified.Equals(Current_Simplified))
                            {
                                string TitleFontPath = Configurazione.SelectedAssociativePropery_Shared.Properties.TitleFont;
                                string Pattern_TitleFont = new Regex(@"""Custom Language Associative Settings"": {(?<Between1>.*?)""Name"": ""<NAMEHERE>"",(?<Between2>.*?)""Title Font"": ""(?<TitleFontPath>.*?)""(?<Afterward>(,)?(\r)?\n)").ToString();
                                Pattern_TitleFont = Pattern_TitleFont.Replace(
                                    "<NAMEHERE>",
                                    SelectedPropertiesName.ToEscapeRegexString()
                                );

                                string FormattedPath = CustomLang_TitleFont.Text.Replace("\\", "/");
                                TempConfigFile = Regex.Replace(TempConfigFile, Pattern_TitleFont, Match =>
                                {
                                    return @$"""Custom Language Associative Settings"": {{{Match.Groups["Between1"].Value}""Name"": ""{SelectedPropertiesName}"",{Match.Groups["Between2"].Value}""Title Font"": ""{FormattedPath}""{Match.Groups["Afterward"].Value}";
                                }, RegexOptions.Singleline);
                                CustomLang_TitleFont.Text = FormattedPath;
                                File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                                Configurazione.SelectedAssociativePropery_Shared.Properties.TitleFont = FormattedPath;
                                Configurazione.UpdatePreviewLayoutsFont(Configurazione.SelectedAssociativePropery_Shared.Properties);
                            }
                        }
                        else
                        {
                            MessageBox.Show("This font file not found!");
                        }
                        break;


                    case "Recheck_ContextFont":
                        if (File.Exists(CustomLang_ContextFont.Text))
                        {
                            Input_Simplified = CustomLang_ContextFont.Text.Replace("\\", "/").ToLower();
                            Current_Simplified = Configurazione.SelectedAssociativePropery_Shared.Properties.ContextFont.Replace("\\", "/").ToLower();

                            if (!Input_Simplified.Equals(Current_Simplified))
                            {
                                string ContextFontPath = Configurazione.SelectedAssociativePropery_Shared.Properties.ContextFont;
                                string Pattern_ContextFont = new Regex(@"""Custom Language Associative Settings"": {(?<Between1>.*?)""Name"": ""<NAMEHERE>"",(?<Between2>.*?)""Context Font"": ""(?<ContextFontPath>.*?)""(?<Afterward>(,)?(\r)?\n)").ToString();
                                Pattern_ContextFont = Pattern_ContextFont.Replace(
                                    "<NAMEHERE>",
                                    SelectedPropertiesName.ToEscapeRegexString()
                                );

                                string FormattedPath = CustomLang_ContextFont.Text.Replace("\\", "/");
                                TempConfigFile = Regex.Replace(TempConfigFile, Pattern_ContextFont, Match =>
                                {
                                    return @$"""Custom Language Associative Settings"": {{{Match.Groups["Between1"].Value}""Name"": ""{SelectedPropertiesName}"",{Match.Groups["Between2"].Value}""Context Font"": ""{FormattedPath}""{Match.Groups["Afterward"].Value}";
                                }, RegexOptions.Singleline);
                                CustomLang_ContextFont.Text = FormattedPath;
                                File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                                Configurazione.SelectedAssociativePropery_Shared.Properties.ContextFont = FormattedPath;
                                Configurazione.UpdatePreviewLayoutsFont(Configurazione.SelectedAssociativePropery_Shared.Properties);
                            }
                        }
                        else
                        {
                            MessageBox.Show("This font file not found!");
                        }
                        break;
                }
            }
        }

        //""Custom Language Associative Settings"": {(?<Between1>.*?)""Name"": ""Russian \(MTL\)"",(.*?)""Keywords Directory"": ""(?<Between2>.*?)"",    flag single line gms
        private void SelectionToggle(object sender, SelectionChangedEventArgs e)
        {
            if (!Configurazione.SettingsLoadingEvent)
            {
                string Sender = (sender as FrameworkElement).Name;
                string NewSelectionName = "";
                string TempConfigFile = File.ReadAllText(@"⇲ Assets Directory\Configurazione^.json");
                switch (Sender)
                {
                    case "CustomLanguagePropertiesSelector":

                        NewSelectionName = (CustomLanguagePropertiesSelector.SelectedItem as TextBlock).Text;
                        var NewSelectionFound = Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List.Where(x => x.PropertyName.Equals(NewSelectionName)).ToList();
                        if (NewSelectionFound.Count() > 0)
                        {

                            var NewSelection = NewSelectionFound[0];

                            Configurazione.SelectedAssociativePropery_Shared = NewSelection;

                            Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected = NewSelection.PropertyName;

                            Configurazione.UpdateCustomLanguagePart(NewSelection);
                            UpdateSelectedCustomLanguageSettingsView();




                            TempConfigFile = File.ReadAllText(@"⇲ Assets Directory\Configurazione^.json");
                            TempConfigFile = Regex.Replace(TempConfigFile, @"""Associative Properties Selected"": ""(.*?)""(?<Afterward>(,)?(\r)?\n)", Match =>
                            {
                                return @$"""Associative Properties Selected"": ""{NewSelection.PropertyName}""{Match.Groups["Afterward"].Value}";
                            });
                            File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);


                            RichTextBoxApplicator.SetLimbusRichText(RichText.RichTextBoxApplicator.LastUpdateTarget, RichText.RichTextBoxApplicator.LastUpdateText);
                        }


                        break;


                    case "LanguageSelector":
                        NewSelectionName = (LanguageSelector.SelectedItem as TextBlock).Text;
                        UILanguageLoader.InitializeUILanguage(@$"⇲ Assets Directory\[+] Languages\{NewSelectionName}.json");
                        TempConfigFile = Regex.Replace(TempConfigFile, @"""UI Language"": ""(.*?)""(?<Afterward>(,)?(\r)?\n)", Match =>
                        {
                            return @$"""UI Language"": ""⇲ Assets Directory/[+] Languages/{NewSelectionName}.json""{Match.Groups["Afterward"].Value}";
                        });
                        File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                        break;


                    case "ThemeSelector":
                        NewSelectionName = (ThemeSelector.SelectedItem as TextBlock).Text;
                        UIThemesLoader.InitializeUITheme(@$"⇲ Assets Directory\[+] Themes\{NewSelectionName}");
                        TempConfigFile = Regex.Replace(TempConfigFile, @"""UI Theme"": ""(.*?)""(?<Afterward>(,)?(\r)?\n)", Match =>
                        {
                            return @$"""UI Theme"": ""⇲ Assets Directory/[+] Themes/{NewSelectionName}""{Match.Groups["Afterward"].Value}";
                        });
                        File.WriteAllText(@"⇲ Assets Directory\Configurazione^.json", TempConfigFile);

                        break;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            DoClose();
        }
    }
}
