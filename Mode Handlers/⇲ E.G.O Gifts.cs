﻿using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using Newtonsoft.Json;
using RichText;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_EGOGifts;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    internal abstract class Mode_EGOGifts
    {
        internal protected static dynamic FormalTaskCompleted = null;

        internal protected static int CurrentEGOGiftID = -1;

        internal protected static EGOGifts DeserializedInfo;
        internal protected static Dictionary<string, int> EGOGifts_NameIDs = [];

        internal protected static string TargetSite_StringLine = "Main Description";

        internal abstract class OrganizedData
        {
            internal protected static A1_AttributesDisplayInfo DisplayInfo_Attributes_JSON = new A1_AttributesDisplayInfo();

            internal protected static Dictionary<int, AttributeDisplayInfo> DisplayInfo_Attributes = new Dictionary<int, AttributeDisplayInfo> { };
            internal protected static Dictionary<int, BitmapImage> DisplayInfo_Icons = new Dictionary<int, BitmapImage> { };

            internal protected static Dictionary<int, List<int>> UpgradeLevelsAssociativeIDs = new Dictionary<int, List<int>> { };

            internal protected class A1_AttributesDisplayInfo
            {
                public string Readme { get; set; }

                [JsonProperty("E.G.O Gifts Info")]
                public List<AttributeDisplayInfo> EGOGiftsInfo { get; set; }
            }
            internal protected class AttributeDisplayInfo
            {
                public int ID { get; set; }
                public string Image { get; set; }
                public string Affinity { get; set; }
                public string Keyword { get; set; }
                public string Tier { get; set; }

                [JsonIgnore]
                public BitmapImage LoadedImage { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    if (Image.IsNull())
                    {
                        rin($"Null name for {ID}");
                    }

                    if (File.Exists(@$"⇲ Assets Directory\[⇲] Limbus Images\E.G.O Gifts\{Image}"))
                    {
                        LoadedImage = GenerateBitmapFromFile(@$"⇲ Assets Directory\[⇲] Limbus Images\E.G.O Gifts\{Image}");
                    }
                    else
                    {
                        rin($"Not found file called \"{Image}\" for id {ID}");
                    }
                }
            }

            internal protected class A1_IconsDisplayInfo
            {
                [JsonProperty("ID Matches")]
                public List<IconDisplayInfo> IDMatches { get; set; }
            }
            internal protected class IconDisplayInfo
            {

                [JsonProperty("Base ID")]
                public int BaseID { get; set; }

                [JsonProperty("File")]
                public string Filename { get; set; }

                public BitmapImage Image { get; set; }


                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    if (File.Exists(@$"⇲ Assets Directory\[⇲] Limbus Images\E.G.O Gifts\{Filename}"))
                    {
                        Image = GenerateBitmapFromFile(@$"⇲ Assets Directory\[⇲] Limbus Images\E.G.O Gifts\{Filename}");
                    }
                }
            }

            internal protected static void UpdateOrganizedInfo()
            {
                DisplayInfo_Attributes_JSON = JsonConvert.DeserializeObject<A1_AttributesDisplayInfo>(File.ReadAllText(@"⇲ Assets Directory\[⇲] Limbus Images\E.G.O Gifts\⇲ Display Info.json"));

                foreach (AttributeDisplayInfo EGOGiftViewData in DisplayInfo_Attributes_JSON.EGOGiftsInfo)
                {
                    DisplayInfo_Icons[EGOGiftViewData.ID] = EGOGiftViewData.LoadedImage;
                    DisplayInfo_Icons[int.Parse($"1{EGOGiftViewData.ID}")] = EGOGiftViewData.LoadedImage;
                    DisplayInfo_Icons[int.Parse($"2{EGOGiftViewData.ID}")] = EGOGiftViewData.LoadedImage;

                    DisplayInfo_Attributes[EGOGiftViewData.ID] = EGOGiftViewData;
                    DisplayInfo_Attributes[int.Parse($"1{EGOGiftViewData.ID}")] = EGOGiftViewData;
                    DisplayInfo_Attributes[int.Parse($"2{EGOGiftViewData.ID}")] = EGOGiftViewData;

                    KeywordsInterrogate.EGOGiftInlineImages[$"{EGOGiftViewData.ID}"] = EGOGiftViewData.LoadedImage;
                }
            }

            internal protected static void UpdateUpgradeLevelsAssociativeIDs()
            {
                List<int> notfound = new();
                foreach (var EGOGiftID in DelegateEGOGifts_IDList)
                {
                    int ExceptedID_UpgradeLevel2 = int.Parse($"1{EGOGiftID}");
                    int ExceptedID_UpgradeLevel3 = int.Parse($"2{EGOGiftID}");

                    if (DelegateEGOGifts_IDList.Contains(ExceptedID_UpgradeLevel2))
                    {
                        DelegateEGOGifts[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                        DelegateEGOGifts[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(ExceptedID_UpgradeLevel2);

                        DelegateEGOGifts[ExceptedID_UpgradeLevel2].UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                        DelegateEGOGifts[ExceptedID_UpgradeLevel2].UpgradeLevelsAssociativeIDs.Add(ExceptedID_UpgradeLevel2);

                        DelegateEGOGifts[ExceptedID_UpgradeLevel2].UpgradeLevel = "2";
                    }

                    if (DelegateEGOGifts_IDList.Contains(ExceptedID_UpgradeLevel3))
                    {
                        DelegateEGOGifts[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(ExceptedID_UpgradeLevel3);

                        DelegateEGOGifts[ExceptedID_UpgradeLevel2].UpgradeLevelsAssociativeIDs.Add(ExceptedID_UpgradeLevel3);

                        DelegateEGOGifts[ExceptedID_UpgradeLevel3].UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                        DelegateEGOGifts[ExceptedID_UpgradeLevel3].UpgradeLevelsAssociativeIDs.Add(ExceptedID_UpgradeLevel2);
                        DelegateEGOGifts[ExceptedID_UpgradeLevel3].UpgradeLevelsAssociativeIDs.Add(ExceptedID_UpgradeLevel3);

                        DelegateEGOGifts[ExceptedID_UpgradeLevel3].UpgradeLevel = "3";
                    }

                    if (!DisplayInfo_Attributes.ContainsKey(EGOGiftID))
                    {
                        notfound.Add(EGOGiftID);
                    }
                }

                if (notfound.Count > 0)
                {
                    MessageBox.Show($"Following ID's not found in Display Info:{string.Join("\n", notfound)}");
                }
            }
        }

        internal protected static SwitchedInterfaceProperties SwitchedInterfaceProperties = new()
        {
            Key = "E.G.O Gifts",
            DefaultValues = new()
            {
                Height = 635,
                Width = 812,
                MinHeight = 461,
                MinWidth = 520.3,
                MaxHeight = 10000,
                MaxWidth = 812,
            },
        };

        internal protected static void TriggerSwitch()
        {
            MainControl.NavigationPanel_Skills_UptieLevelSelectorGrid.Visibility = Visibility.Collapsed;
            MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Visibility.Collapsed;
            MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 114, 4, 4);

            MainControl.PreviewLayouts.Height = 420;
            MainControl.NavigationPanel_HeightControlScrollViewer.MaxHeight = 370;
            MainControl.EditorWidthControl.Width = new GridLength(518);

            PreviewUpdate_TargetSite = MainControl.PreviewLayout_EGOGifts;

            Upstairs.ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.DefaultValues);

            HideNavigationPanelButtons(
                  ExceptButtonsGrid: MainControl.SwitchButtons_EGOGifts,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_EGOGifts
            );
        }

        internal protected static Task LoadStructure(FileInfo JsonFile)
        {
            DeserializedInfo = JsonFile.Deserealize<EGOGifts>() as EGOGifts;
            InitializeEGOGiftsDelegateFrom(DeserializedInfo);

            if (DelegateEGOGifts_IDList.Count > 0)
            {
                Mode_Handlers.Mode_EGOGifts.TriggerSwitch();
                OrganizedData.UpdateUpgradeLevelsAssociativeIDs();

                TransformToEGOGift(DelegateEGOGifts_IDList[0]);
            }

            return FormalTaskCompleted;
        }

        internal protected static Task TransformToEGOGift(int EGOGiftID)
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentEGOGiftID = EGOGiftID;

            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Current ID Copy Button"))
            {
                MainControl.STE_NavigationPanel_ObjectID_Display
                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Current ID Copy Button"]
                    .Extern(CurrentEGOGiftID));
            }

            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();

            MainControl.NavigationPanel_ObjectName_Display.Text = DelegateEGOGifts[CurrentEGOGiftID].Name;
            MainControl.SWBT_EGOGifts_EGOGiftName.Text = DelegateEGOGifts[CurrentEGOGiftID].Name.Replace("\n", "\\n");

            SwitchToMainDesc();

            ReCheckEGOGiftInfo();
            
            {
                ManualTextLoadEvent = false;
            }

            return FormalTaskCompleted;
        }

        internal protected static void ReCheckEGOGiftInfo()
        {
            MainControl.STE_DisableCover_Passives_SummaryDescription.Visibility = Visible;

            if (OrganizedData.DisplayInfo_Icons.ContainsKey(CurrentEGOGiftID))
            {
                MainControl.EGOGiftDisplay_MainIcon.Source = OrganizedData.DisplayInfo_Icons[CurrentEGOGiftID];
            }
            else
            {
                MainControl.EGOGiftDisplay_MainIcon.Source = KeywordsInterrogate.KeywordImages["Unknown"];

            }


            /////////////////////////////////////////////////
            var FullLink = DelegateEGOGifts[CurrentEGOGiftID];
            /////////////////////////////////////////////////

            for (int i = 1; i <= 6; i++) ((MainControl.FindName($"STE_DisableCover_EGOGift_SimpleDescription{i}")) as Border).Visibility = Visibility.Visible;

            if (!FullLink.SimpleDescriptions.IsNull())
            {
                for (int i = 1; i <= FullLink.SimpleDescriptions.Count; i++)
                {
                    if (!FullLink.SimpleDescriptions[i-1].Description.Equals(FullLink.SimpleDescriptions[i - 1].EditorDescription))
                    {
                        (MainControl.FindName($"STE_EGOGift_SimpleDescription{i}") as RichTextBox)
                            .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                            .Extern(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — E.G.O Gift Simple Desc {i}"]));
                    }
                    else
                    {
                        (MainControl.FindName($"STE_EGOGift_SimpleDescription{i}") as RichTextBox)
                            .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — E.G.O Gift Simple Desc {i}"]);
                    }
                    ((MainControl.FindName($"STE_DisableCover_EGOGift_SimpleDescription{i}")) as Border).Visibility = Visibility.Collapsed;
                }
            }

            MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel2.Visibility = Visibility.Collapsed;
            MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel3.Visibility = Visibility.Collapsed;

            MainControl.EGOGiftDisplay_UpgradeLevel2Border.Visibility = Visibility.Collapsed;
            MainControl.EGOGiftDisplay_UpgradeLevel3Border.Visibility = Visibility.Collapsed;

            MainControl.EGOGiftDisplay_UpgradeLevel2_OnIcon.Visibility = Visibility.Collapsed;
            MainControl.EGOGiftDisplay_UpgradeLevel3_OnIcon.Visibility = Visibility.Collapsed;

            MainControl.EGOGiftDisplay_Keyword.Visibility = Visibility.Collapsed;


            switch (DelegateEGOGifts[CurrentEGOGiftID].UpgradeLevelsAssociativeIDs.Count)
            {
                case 1:
                    break;

                case 2:
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel2.Visibility = Visibility.Visible;
                    break;

                case 3:
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel2.Visibility = Visibility.Visible;
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel3.Visibility = Visibility.Visible;
                    break;
            }

            switch (FullLink.UpgradeLevel)
            {
                case "1":
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel1.Children[3].Opacity = 1;
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel2.Children[3].Opacity = 0;
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel3.Children[3].Opacity = 0;
                    break;

                case "2":
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel1.Children[3].Opacity = 0;
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel2.Children[3].Opacity = 1;
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel3.Children[3].Opacity = 0;

                    MainControl.EGOGiftDisplay_UpgradeLevel2Border.Visibility = Visibility.Visible;
                    MainControl.EGOGiftDisplay_UpgradeLevel2_OnIcon.Visibility = Visibility.Visible;
                    break;

                case "3":
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel1.Children[3].Opacity = 0;
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel2.Children[3].Opacity = 0;
                    MainControl.EGOGiftDisplay_HotSwitchToUpgradeLevel3.Children[3].Opacity = 1;

                    MainControl.EGOGiftDisplay_UpgradeLevel2Border.Visibility = Visibility.Visible;
                    MainControl.EGOGiftDisplay_UpgradeLevel3Border.Visibility = Visibility.Visible;
                    MainControl.EGOGiftDisplay_UpgradeLevel2_OnIcon.Visibility = Visibility.Visible;
                    MainControl.EGOGiftDisplay_UpgradeLevel3_OnIcon.Visibility = Visibility.Visible;
                    break;
            }

            if (OrganizedData.DisplayInfo_Attributes.ContainsKey(CurrentEGOGiftID))
            {
                MainControl.EGOGiftDisplay_Tier.Text = OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Tier switch
                {
                    "1" => "I",
                    "2" => "II",
                    "3" => "III",
                    "4" => "IV",
                    _ => ""
                };

                if (OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Tier == "EX")
                {
                    MainControl.EGOGiftDisplay_Tier.Visibility = Collapsed;
                    MainControl.EGOGiftDisplay_EXTier.Visibility = Visible;
                }
                else
                {
                    MainControl.EGOGiftDisplay_Tier.Visibility = Visible;
                    MainControl.EGOGiftDisplay_EXTier.Visibility = Collapsed;
                }

                if (!OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword.Equals("-"))
                {
                    MainControl.EGOGiftDisplay_Keyword.Visibility = Visibility.Visible;

                    MainControl.EGOGiftDisplay_Keyword.Source = OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword switch
                    {
                        "Burn"    => KeywordsInterrogate.KeywordImages["Combustion"],
                        "Bleed"   => KeywordsInterrogate.KeywordImages["Laceration"],
                        "Tremor"  => KeywordsInterrogate.KeywordImages["Vibration"],
                        "Poise"   => KeywordsInterrogate.KeywordImages["Breath"],
                        "Charge"  => KeywordsInterrogate.KeywordImages["Charge"],
                        "Rupture" => KeywordsInterrogate.KeywordImages["Burst"],
                        "Sinking" => KeywordsInterrogate.KeywordImages["Sinking"],
                        "Blunt"   => KeywordsInterrogate.KeywordImages["LCLocaliazationInterface_Blunt"],
                        "Pierce"  => KeywordsInterrogate.KeywordImages["LCLocaliazationInterface_Pierce"],
                        "Slash"   => KeywordsInterrogate.KeywordImages["LCLocaliazationInterface_Slash"],
                        _ => KeywordsInterrogate.KeywordImages.ContainsKey(OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword)
                                ? KeywordsInterrogate.KeywordImages[OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword]
                                : KeywordsInterrogate.KeywordImages["Unknown"],
                    };
                }

                MainControl.EGOGiftAffinityType_NameBackground.Background = ToColor(
                OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Affinity switch
                    {
                        "Wrath"    => "#fe0101",
                        "Lust"     => "#fe6f01",
                        "Sloth"    => "#fed435",
                        "Gluttony" => "#a7fe01",
                        "Gloom"    => "#1cc7f1",
                        "Pride"    => "#014fd6",
                        "Envy"     => "#9800df",
                        _ => "#9f6a3a"
                    }
                );
            }
            else
            {
                //MessageBox.Show($"Not found id '{CurrentEGOGiftID}' in Display Info");
                MainControl.EGOGiftAffinityType_NameBackground.Background = ToColor("#9f6a3a");
                MainControl.EGOGiftDisplay_Tier.Text = "";
            }
        }

        internal protected static void SwitchToMainDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            TargetSite_StringLine = "Main Description";

            /////////////////////////////////////////////////
            var FullLink = DelegateEGOGifts[CurrentEGOGiftID];
            /////////////////////////////////////////////////

            if (!FullLink.Description.Equals(FullLink.EditorDescription))
            {
                MainControl.Editor.Text = FullLink.EditorDescription;
            }
            else
            {
                MainControl.Editor.Text = FullLink.Description;
            }

            LockEditorUndo();

            {
                ManualTextLoadEvent = true;
            }
        }

        internal protected static void SwitchToSimpleDesc(string SimpleDescNumber)
        {
            {
                ManualTextLoadEvent = true;
            }

            /////////////////////////////////////////////////
            var FullLink = DelegateEGOGifts[CurrentEGOGiftID];
            /////////////////////////////////////////////////

            TargetSite_StringLine = $"Simple Description №{SimpleDescNumber}";

            int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;
            if (!FullLink.SimpleDescriptions[TargetSimpleDescIndex].Description.Equals(FullLink.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription))
            {
                MainControl.Editor.Text = FullLink.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription;
            }
            else
            {
                MainControl.Editor.Text = FullLink.SimpleDescriptions[TargetSimpleDescIndex].Description;
            }

            LockEditorUndo();

            {
                ManualTextLoadEvent = true;
            }
        }
    }
}
