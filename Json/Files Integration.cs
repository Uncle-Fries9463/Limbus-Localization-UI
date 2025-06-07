using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using static LC_Localization_Task_Absolute.Json.BaseTypes;

namespace LC_Localization_Task_Absolute.Json
{
    internal static partial class FilesIntegration
    {
        internal static void MarkSerialize(this object Target, string Filename)
        {
            string Output = JsonConvert.SerializeObject(
                value:      Target,
                formatting: Formatting.Indented,
                settings:   new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            );
            File.WriteAllText(Filename, Output.Replace("\r\n", "\n"), MainWindow.CurrentFileEncoding);
        }

        internal static object? Deserealize<TargetType>(this FileInfo Target) => JsonConvert.DeserializeObject<TargetType>(Target.GetText());
    }
}