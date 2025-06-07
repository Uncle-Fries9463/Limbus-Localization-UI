using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LC_Localization_Task_Absolute.Json;
using Newtonsoft.Json;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_SkillTag;
using System.Text.Json.Serialization;

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    internal abstract class KeywordsInterrogate
    {
        internal protected record KeywordSingleton
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string StringColor { get; set; }
        }

        internal protected record KeywordImagesIDInfo
        {
            [JsonProperty("ID Matches")]
            public List<KeywordImageIDInfo> GeneralInfo { get; set; }
        }
        internal protected record KeywordImageIDInfo
        {
            [JsonProperty("Base ID")]
            public string BaseID { get; set; }
            public string File { get; set; }
        }

        #region Keywords Multiple Meanings create/read
        internal protected static Dictionary<string, List<string>> KeywordsMultipleMeaningsDictionary = new();
        internal protected record KeywordsMultipleMeaningsDictionaryJson
        {
            public List<KeywordsMultipleMeanings> Info { get; set; }
        }
        internal protected record KeywordsMultipleMeanings
        {
            [JsonProperty("Keyword ID")]
            public string KeywordID { get; set; }

            public List<string> Meanings { get; set; }
        }
        internal protected static void ExportKeywordsMultipleMeaningsDictionary(string LocalizationWithKeywordsPath, bool IgnoreRailAndMirrorKeywords = true)
        {
            KeywordsMultipleMeaningsDictionaryJson Export = new KeywordsMultipleMeaningsDictionaryJson();
            Export.Info = new List<KeywordsMultipleMeanings>();
            int FoundCounter = 0;
            foreach (FileInfo LocalizeFile in new DirectoryInfo(LocalizationWithKeywordsPath)
                .GetFiles(searchPattern: "*.json", searchOption: SearchOption.AllDirectories)
                    .Where(file => file.Name.StartsWithOneOf([
                        "Passive",
                        "EGOgift",
                        "Bufs",
                        "BattleKeywords",
                        "Skills",
                        "PanicInfo"
                    ])
                )
            )
            {
                string TextToAnalyze = File.ReadAllText(LocalizeFile.FullName);
                foreach (Match keywordMatch in Regex.Matches(TextToAnalyze, @"<sprite name=\\""(?<ID>\w+)\\""><color=(?<Color>#[a-fA-F0-9]{6})><u><link=\\""\w+\\"">(?<Name>.*?)</link></u></color>"))
                {
                    string ID = keywordMatch.Groups["ID"].Value;
                    string VariativeName = keywordMatch.Groups["Name"].Value;

                    if (!KeywordsMultipleMeaningsDictionary.ContainsKey(ID)) KeywordsMultipleMeaningsDictionary[ID] = new();

                    if (KeywordsGlossary.ContainsKey(ID))
                    {
                        if (!KeywordsMultipleMeaningsDictionary[ID].Contains(KeywordsGlossary[ID].Name))
                        {
                            KeywordsMultipleMeaningsDictionary[ID].Add(KeywordsGlossary[ID].Name);
                        }
                    }

                    if (!KeywordsMultipleMeaningsDictionary[ID].Contains(VariativeName))
                    {
                        KeywordsMultipleMeaningsDictionary[ID].Add(VariativeName);
                    }

                    FoundCounter++;
                }
            }

            foreach (var i in KeywordsMultipleMeaningsDictionary)
            {
                Export.Info.Add(new KeywordsMultipleMeanings()
                {
                    KeywordID = i.Key,
                    Meanings = i.Value
                });
            }

            if (FoundCounter > 0)
            {
                Export.MarkSerialize("Keywords Multiple Meanings.json");
                MessageBox.Show($"Keywords multiple meanings from \"{LocalizationWithKeywordsPath}\" dir exported as \"Keywords Multiple Meanings.json\" at program folder");
            }
        }
        internal protected static void ReadKeywordsMultipleMeanings(string Filepath)
        {
            if (File.Exists(Filepath))
            {
                KeywordsMultipleMeaningsDictionaryJson Readed = JsonConvert.DeserializeObject<KeywordsMultipleMeaningsDictionaryJson>(File.ReadAllText(Filepath));
                if (!Readed.IsNull())
                {
                    KeywordsMultipleMeaningsDictionary = new();
                    foreach(KeywordsMultipleMeanings KeywordMeaningsInfo in Readed.Info)
                    {
                        if (!KeywordsMultipleMeaningsDictionary.ContainsKey(KeywordMeaningsInfo.KeywordID)) KeywordsMultipleMeaningsDictionary[KeywordMeaningsInfo.KeywordID] = new();
                        foreach(string KeywordMeaning in KeywordMeaningsInfo.Meanings)
                        {
                            KeywordsMultipleMeaningsDictionary[KeywordMeaningsInfo.KeywordID].Add(KeywordMeaning);

                            Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter[KeywordMeaning] = KeywordMeaningsInfo.KeywordID;
                        }
                    }
                }
            }
        }
        #endregion

        internal protected static Dictionary<string, KeywordSingleton> KeywordsGlossary = [];
        internal protected static Dictionary<string, string> Keywords_IDName = [];
        /// <summary>
        /// Contains matches of keyword names and their IDs in descending order of name length (For limbus preview formatter, only base keyword names)
        /// </summary>
        internal protected static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter = [];
        /// <summary>
        /// Extended version with other keyword meanings (E.g. "Charge", "Charges" for same Charge keyword id)
        /// </summary>
        internal protected static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter = [];
        internal protected static Dictionary<string, BitmapImage> KeywordImages = [];
        internal protected static Dictionary<string, BitmapImage> EGOGiftInlineImages = [];
        internal protected static Dictionary<string, string> SkillTags = [];
        internal protected static List<string> KnownID = [];
        
        internal protected static void LoadInlineImages()
        {
            //rin($"\n$ Loading keyword images");
            KeywordImages["Unknown"] = new BitmapImage(new Uri("pack://application:,,,/Default/Images/Unknown.png"));
            int Counter = 0;
            foreach (FileInfo KeywordImage in new DirectoryInfo(@"⇲ Assets Directory\[⇲] Limbus Images\Keywords").GetFiles("*.png"))
            {
                string TargetID = KeywordImage.Name.Replace(KeywordImage.Extension, "");
                KeywordImages[TargetID] = GenerateBitmapFromFile(KeywordImage.FullName);
                Counter++;
            }
            //rin($"  {Counter} images loaded from \"⇲ Assets Directory\\[⇲] Limbus Images\\Keywords\" directory");
        }
        internal protected static void InitializeGlossaryFrom(string KeywordsDirectory, bool WriteOverFallback = false, string FilesPrefix = "")
        {
            if (!WriteOverFallback)
            {
                KeywordsGlossary.Clear();
                SkillTags.Clear();
                KnownID.Clear();
            }

            
            int Counter = 0;

            if (Directory.Exists(KeywordsDirectory))
            {
                rin($"Loading Keywords from \"{KeywordsDirectory}\" with files prefix \"{FilesPrefix}\"");
                Dictionary<string, string> SkillTagColors = new Dictionary<string, string>();
                if (File.Exists(@"⇲ Assets Directory\[+] Keywords\SkillTag Colors.T[-]"))
                {
                    foreach (string Line in File.ReadAllLines(@"⇲ Assets Directory\[+] Keywords\SkillTag Colors.T[-]").Where(Line => Line.Contains(" ¤ ")))
                    {
                        string[] ColorPair = Line.Split(" ¤ ");
                        if (ColorPair.Count() == 2)
                        {
                            string SkillTagID = ColorPair[0].Trim();
                            string SkillTagColor = ColorPair[1].Trim();
                            //rin($"  Load {SkillTagID} -> {SkillTagColor}");
                            SkillTagColors[SkillTagID] = SkillTagColor;
                        }
                    }
                }

                if (File.Exists(@$"{KeywordsDirectory}\{FilesPrefix}SkillTag.json"))
                {
                    BaseTypes.Type_SkillTag.SkillTags SkillTagsJson = JsonConvert.DeserializeObject<SkillTags>(File.ReadAllText(@$"{KeywordsDirectory}\{FilesPrefix}SkillTag.json"));
                    if (!SkillTagsJson.dataList.IsNull())
                    {
                        foreach (SkillTag SkillTag in SkillTagsJson.dataList)
                        {
                            SkillTags[$"[{SkillTag.ID}]"] = $"<color={(SkillTagColors.ContainsKey(SkillTag.ID) ? SkillTagColors[SkillTag.ID] : "#93f03f")}>{SkillTag.Tag}</color>";
                            //rin($"{SkillTag.ID} -> {SkillTags[$"[{SkillTag.ID}]"]}");
                        }
                    }
                }

                //rin($"  Skill tags loaded: {Counter}");

                //rin($"\n$ Loading keyword colors");
                Counter = 0;
                Dictionary<string, string> KeywordColors = [];
                try
                {
                    foreach(string ColorPair in File.ReadAllLines(@"⇲ Assets Directory\[+] Keywords\Keyword Colors.T[-]"))
                    {
                        try
                        {
                            KeywordColors[ColorPair.Split(" ¤ ")[0].Trim()] = ColorPair.Split(" ¤ ")[1].Trim();
                            //rin($"{ColorPair.Split(" ¤ ")[0].Trim()}: {ColorPair.Split(" ¤ ")[1].Trim()}");
                            Counter++;
                        } catch { }
                    }
                } catch { }
                //rin($"  Keyword colors loaded: {Counter}");

                //rin($"\n$ Loading keywords");
                Counter = 0;
                foreach (FileInfo KeywordFileInfo in new DirectoryInfo(KeywordsDirectory).GetFiles(
                    searchPattern: "*.json",
                    searchOption: SearchOption.AllDirectories
                ).Where(file => file.Name.RemovePrefix("EN_", "KR_", "JP_").StartsWith("Bufs"))) {

                    BaseTypes.Type_Keywords.Keywords TargetSite = KeywordFileInfo.Deserealize<Keywords>() as Keywords;

                    if (!TargetSite.IsNull())
                    {
                        foreach(Keyword KeywordItem in TargetSite.dataList)
                        {
                            if (!KeywordItem.ID.ContainsOneOf(DeltaConfig.PreviewSettings.CustomLanguageProperties.KeywordsIgnore))
                            {
                                KeywordsGlossary[KeywordItem.ID] = new KeywordSingleton
                                {
                                    Name = KeywordItem.Name,
                                    Description = KeywordItem.Description,
                                    StringColor = KeywordColors.ContainsKey(KeywordItem.ID) ? KeywordColors[KeywordItem.ID] : "#9f6a3a"
                                };

                                Keywords_IDName[KeywordItem.ID] = KeywordItem.Name;
                                if (!KeywordItem.ID.EndsWithOneOf(["_Re", "Re", "Mirror"]))
                                {
                                    //Fallback overwrite
                                    if (Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.ContainsValue(KeywordItem.ID) & WriteOverFallback)
                                    {
                                        Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter = Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.RemoveItemWithValue(KeywordItem.ID);
                                    }
                                    Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter[KeywordItem.Name] = KeywordItem.ID;


                                    
                                    Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter[KeywordItem.Name] = KeywordItem.ID;
                                }

                                KnownID.Add(KeywordItem.ID);

                                Counter++;
                            }
                        }
                    }
                }

                Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter = Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.OrderBy(obj => obj.Key.Length).ToDictionary(obj => obj.Key, obj => obj.Value).Reverse().ToDictionary();
                Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter = Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter.OrderBy(obj => obj.Key.Length).ToDictionary(obj => obj.Key, obj => obj.Value).Reverse().ToDictionary();
            }
            else
            {
                //rin($"\n[!] Keywords Directory \"{KeywordsDirectory}\" not found");
            }
        }
    }
}
