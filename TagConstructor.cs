﻿using Limbus_Localization_UI.Additions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using static Limbus_Localization_UI.Additions.Consola;

namespace Limbus_Localization_UI
{
    public class TextConstructor
    {
        public string Text;
        public List<string> InnerTags;
    }

    public class SpriteConstructor
    {
        public string SpriteLink;
        public TextConstructor TextBase;
    };


    public static class TagManager
    {
        public static List<string> InnerTags(string Source)
        {
            List<string> OutputTags = new();
            try
            {
                Regex InnerTags = new Regex(@"⟦InnerTag/(.*?)⟧");
                foreach (Match InnerTagMatch in InnerTags.Matches(Source))
                {
                    OutputTags.Add(InnerTagMatch.Groups[1].Value.Replace("InnerTag/", ""));
                }
            }
            catch { }
            return OutputTags;
        }

        public static string ClearText(string Source)
        {
            try
            {
                if (!Source.StartsWith("⟦LevelTag/"))
                {
                    Source = Regex.Replace(Source, @"⟦InnerTag/(.*)⟧", Match => { return ""; });
                }
                else
                {
                    Source = Regex.Replace(Source, @"⟦LevelTag/SpriteLink@(\w+):«(.*)»⟧", Match => { return ClearText(Match.Groups[2].Value); });
                }
            }
            catch { }

            return Source;
        }



        public static void ApplyTags(ref Run TargetRun, List<string> Tags)
        {
            try
            {
                bool IsContainsFontFamily = false;
                foreach(var i in Tags)
                {
                    if (i.StartsWith("FontFamily@")) IsContainsFontFamily = true; break;
                }
                

                foreach (var Tag in Tags)
                {
                    string[] TagBody = Tag.Split('@');
                    switch (TagBody[0])
                    {
                        case "TextColor":
                            TargetRun.Foreground = РазноеДругое.GetColorFromAHEX($"#ff{TagBody[1]}");
                            break;

                        case "FontFamily":
                            try   { TargetRun.FontFamily = new FontFamily(TagBody[1]); }
                            catch { }
                            break;

                        case "FontSize":
                            try { TargetRun.FontSize = 20 * (0.01 * Convert.ToInt32(TagBody[1][..^1])); }
                            catch (Exception EX) { Console.WriteLine(EX.ToString()); }
                            break;

                        case "UptieHighlight":
                            TargetRun.Foreground = РазноеДругое.GetColorFromAHEX($"#fff8c200");
                            break;

                        case "TextStyle":
                            switch (TagBody[1])
                            {
                                case "Underline":
                                    TargetRun.TextDecorations = TextDecorations.Underline;
                                    break;

                                case "Strikethrough":
                                    TargetRun.TextDecorations = TextDecorations.Strikethrough;
                                    break;

                                case "Italic":
                                    if (!IsContainsFontFamily & !MainWindow.BattleKeywords_Type.Equals("CN"))
                                    {
                                        TargetRun.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/Pretendard/public/static/#Pretendard");
                                    }
                                    TargetRun.FontStyle = FontStyles.Italic;
                                    break;

                                case "Bold":
                                    if (!IsContainsFontFamily & !MainWindow.BattleKeywords_Type.Equals("CN"))
                                    {
                                        TargetRun.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/Pretendard/public/static/#Pretendard");
                                    }
                                    TargetRun.FontWeight = FontWeights.SemiBold;
                                    break;

                            }

                            break;
                    }
                }
                if (!IsContainsFontFamily & MainWindow.BattleKeywords_Type.Equals("CN"))
                {
                    //TargetRun.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/S_Core_Dream/OTF/#S-Core Dream 5 Medium");
                    //rin(TargetRun.Text);
                }
            }
            catch { }
        }
    }
}
