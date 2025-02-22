using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Limbus_Localization_UI.Additions.Consola;

namespace Limbus_Localization_UI.Json
{
    /// <summary>
    /// Работает вместе с Bufs и Passive файлами
    /// </summary>
    class JsonLoader_Passives
    {
        public static JsonData JSON;
        public static (Dictionary<dynamic, Dictionary<string, object>>, Dictionary<dynamic, Dictionary<string, object>>) GetJsonDictionary(string Json_Файл)
        {
            JSON = JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(Json_Файл));

            Dictionary<dynamic, Dictionary<string, object>> Passives_Json_Dictionary = new();
            Dictionary<dynamic, Dictionary<string, object>> Passives_EditBuffer = new();

            bool WriteInfo = false;

            object ID;
            object ID_JSON;
            string Name_JSON;    int LineIndex_Name;
            string Desc_JSON;    int LineIndex_Desc;
            string Summary_JSON; int LineIndex_Summary;
            foreach (var Passive in JSON.dataList)
            {
                try
                {
                    ID_JSON = int.Parse($"{Passive.id}");
                }
                catch (Exception ex)
                {
                    ID_JSON = $"{Passive.id}";
                }
                Name_JSON = Passive.name;
                Desc_JSON = Passive.desc;
                Summary_JSON = Passive.summary;
                try
                {
                    if (Summary_JSON.Equals("")) Summary_JSON = "{none}";
                }
                catch { Summary_JSON = "{none}";  }
                Passives_Json_Dictionary[ID_JSON] = new()
                {
                    ["Name"] = Name_JSON,
                    ["Desc"] = Desc_JSON,
                    ["Summary"] = Summary_JSON,
                };
                Passives_EditBuffer[ID_JSON] = new()
                {
                    ["Name"] =    "{unedited}",
                    ["Desc"] =    "{unedited}",
                    ["Summary"] = "{unedited}",
                };
                if (WriteInfo) rin($"({ID_JSON}) {Name_JSON}:\n ¤ Desc: {Desc_JSON}\n ¤ Summary: {Summary_JSON}\n");
            }

            var JsonLines = File.ReadAllLines(Json_Файл);
            for (int LineIndex = 0; LineIndex <= JsonLines.Count() - 1; LineIndex++)
            {
                if (JsonLines[LineIndex].Trim().StartsWith("\"id\": "))
                {
                    try
                    {
                        ID = Convert.ToInt32(JsonLines[LineIndex].Trim().Split("\"id\": ")[1].Split(",")[0]);
                    }
                    catch
                    {
                        ID = JsonLines[LineIndex].Trim().Split("\"id\": ")[1].Split(",")[0][1..^1];
                    }
                    
                    LineIndex_Name = LineIndex + 2;
                    LineIndex_Desc = LineIndex + 3;
                    if (!Passives_Json_Dictionary[ID]["Summary"].Equals("{none}"))
                    {
                        LineIndex_Summary = LineIndex + 4;
                    }
                    else LineIndex_Summary = -1;

                    Passives_Json_Dictionary[ID]["LineIndex_Name"] = LineIndex_Name;
                    Passives_Json_Dictionary[ID]["LineIndex_Desc"] = LineIndex_Desc;
                    Passives_Json_Dictionary[ID]["LineIndex_Summary"] = LineIndex_Summary;
                    if (WriteInfo) rin($"({ID}) {LineIndex_Name}:\n ¤ Desc: {LineIndex_Desc}\n ¤ Summary: {LineIndex_Summary}\n");
                }
            }

            return (Passives_Json_Dictionary, Passives_EditBuffer);
        }

        public static (string, int) GetUnsavedChanges(Dictionary<dynamic, Dictionary<string, object>> CheckDictionary)
        {
            string Info = "";
            int ChangesCount = 0;

            foreach(var ID in CheckDictionary)
            {
                bool IsEditedID = false;
                string Info_Sub = "";
                //rin(ID.Value["Name"]);
                if (!ID.Value["Desc"].Equals("{unedited}"))
                {
                    IsEditedID = true;
                    Info_Sub += "\n - Описание";
                    ChangesCount++;
                }
                if (!ID.Value["Summary"].Equals("{unedited}"))
                {
                    IsEditedID = true;
                    Info_Sub += "\n - Суммарно";
                    ChangesCount++;
                }

                if (IsEditedID)
                {
                    Info += $"ID {ID.Key}:{Info_Sub}\n\n";
                }
            }

            return (Info, ChangesCount);
        }
    }
}
