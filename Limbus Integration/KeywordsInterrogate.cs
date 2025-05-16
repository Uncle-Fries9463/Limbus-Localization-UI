using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
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

        internal protected static Dictionary<string, KeywordSingleton> KeywordsGlossary = [];
        internal protected static Dictionary<string, string> Keywords_IDName = [];
        /// <summary>
        /// Contains matches of keyword names and their IDs in descending order of name length
        /// </summary>
        internal protected static Dictionary<string, string> Keywords_IDName_OrderByLength = [];
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
        internal protected static void InitializeGlossaryFrom(string KeywordsDirectory, bool WriteOverFallback = false)
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

                if (File.Exists(@$"{KeywordsDirectory}\SkillTag.json"))
                {
                    BaseTypes.Type_SkillTag.SkillTags SkillTagsJson = JsonConvert.DeserializeObject<SkillTags>(File.ReadAllText(@$"{KeywordsDirectory}\SkillTag.json"));
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
                    foreach(string ColorPair in File.ReadAllLines(@"⇲ Assets Directory\[+] Keywords\Colors.T[-]"))
                    {
                        try
                        {
                            KeywordColors[ColorPair.Split(" ¤ ")[0].Trim()] = ColorPair.Split(" ¤ ")[1].Trim();
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
                            if (!KeywordItem.ID.ContainsOneOf(Settings.Preview.KeywordsIgnore))
                            {
                                KeywordsGlossary[KeywordItem.ID] = new KeywordSingleton
                                {
                                    Name = KeywordItem.Name,
                                    Description = KeywordItem.Description,
                                    StringColor = KeywordColors.ContainsKey(KeywordItem.ID) ? KeywordColors[KeywordItem.ID] : "#9f6a3a"
                                };

                                Keywords_IDName[KeywordItem.ID] = KeywordItem.Name;
                                if (!KeywordItem.Name.Equals(KeywordItem.ID))
                                {
                                    // Fallback overwrite
                                    //if (Keywords_IDName_OrderByLength.ContainsValue(KeywordItem.ID) & WriteOverFallback)
                                    //{
                                    //    Keywords_IDName_OrderByLength = Keywords_IDName_OrderByLength.RemoveItemWithValue(KeywordItem.ID);
                                    //}

                                    Keywords_IDName_OrderByLength[KeywordItem.Name] = KeywordItem.ID;
                                }

                                KnownID.Add(KeywordItem.ID);

                                Counter++;
                            }
                        }
                    }
                }

                Keywords_IDName_OrderByLength = Keywords_IDName_OrderByLength.OrderBy(obj => obj.Key.Length).ToDictionary(obj => obj.Key, obj => obj.Value).Reverse().ToDictionary();
            }
            else
            {
                //rin($"\n[!] Keywords Directory \"{KeywordsDirectory}\" not found");
            }
        }
    }
}
