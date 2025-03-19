using Limbus_Localization_UI.Additions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

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

    public static class TagDefier
    {
        public static List<string> InnerTags(string Source)
        {
            List<string> OutputTags = new();

            Regex InnerTags = new Regex(@"⟦InnerTag/(.*?)⟧");
            foreach (Match InnerTagMatch in InnerTags.Matches(Source))
            {
                OutputTags.Add(InnerTagMatch.Groups[1].Value.Replace("InnerTag/", ""));
            }

            return OutputTags;
        }

        public static string ClearText(string Source)
        {
            if (!Source.Contains("⟦LevelTag/"))
            {
                Source = Regex.Replace(Source, @"⟦InnerTag/(.*)⟧", Match => { return ""; });
            }
            else
            {
                Source = Regex.Replace(Source, @"⟦LevelTag/SpriteLink@(\w+):«(.*)»⟧", Match => { return ClearText(Match.Groups[2].Value); });
            }

            return Source;
        }

        public static void ApplyTags(ref Run TargetRun, List<string> Tags)
        {
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
                                TargetRun.FontFamily = new FontFamily("Arial");
                                TargetRun.FontStyle = FontStyles.Italic;
                                break;

                            case "Bold":
                                //TargetRun.FontFamily = new FontFamily(new Uri("pack://application:,,,/Fonts/Pretendard/public/static"), "./#Pretendard-Bold");
                                TargetRun.FontWeight = FontWeights.SemiBold;
                                break;

                        }

                        break;
                }
            }
        }
    }
}
