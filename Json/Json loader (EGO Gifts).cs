using System.IO;
using Newtonsoft.Json;

namespace Limbus_Localization_UI.Json
{
    class JsonLoader_EGOgifts
    {
        public static Dictionary<int, Dictionary<string, object>> GetJsonDictionary(string Json_Файл)
        {
            Dictionary<int, Dictionary<string, object>> Json_Dictionary = new();

            List<string> JsonLines = File.ReadAllLines(Json_Файл).ToList();

            bool WriteInfo = false;

            int ID;
            int ID_JSON;
            string Name_JSON; int LineIndex_Name;
            string Desc_JSON; int LineIndex_Desc;
            string SimpleDesc1_JSON; int LineIndex_SimpleDesc1;
            string SimpleDesc2_JSON; int LineIndex_SimpleDesc2;
            string SimpleDesc3_JSON; int LineIndex_SimpleDesc3;
            string SimpleDesc4_JSON; int LineIndex_SimpleDesc4;
            string SimpleDesc5_JSON; int LineIndex_SimpleDesc5;

            // Содержимое Json через десериализацию
            foreach (var data in JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(Json_Файл)).dataList)
            {
                ID_JSON = data.id;
                Name_JSON = data.name.Replace("\"", "\\\"").Replace("\n", "\\n");
                Desc_JSON = data.desc.Replace("\"", "\\\"").Replace("\n", "\\n");
                SimpleDesc1_JSON = "{none}";
                SimpleDesc2_JSON = "{none}";
                SimpleDesc3_JSON = "{none}";
                SimpleDesc4_JSON = "{none}";
                SimpleDesc5_JSON = "{none}";
                List<string> SimpleDescs_JSON = new() { };
                foreach (var i in data.SimpleDesc) SimpleDescs_JSON.Add(i.simpleDesc.Replace("\"", "\\\"").Replace("\n", "\\n"));
                if(WriteInfo) Console.WriteLine(SimpleDescs_JSON.Count);
                for (int i = 0; i <= 10 - SimpleDescs_JSON.Count; i++) SimpleDescs_JSON.Add("{none}");
                if (WriteInfo) Console.WriteLine(Name_JSON);

                SimpleDesc1_JSON = SimpleDescs_JSON[0];
                SimpleDesc2_JSON = SimpleDescs_JSON[1];
                SimpleDesc3_JSON = SimpleDescs_JSON[2];
                SimpleDesc4_JSON = SimpleDescs_JSON[3];
                SimpleDesc5_JSON = SimpleDescs_JSON[4];

                Json_Dictionary[ID_JSON] = new Dictionary<string, object>
                {
                    ["Name"] = Name_JSON,
                    ["Desc"] = Desc_JSON,
                    ["SimpleDesc1"] = SimpleDesc1_JSON,
                    ["SimpleDesc2"] = SimpleDesc2_JSON,
                    ["SimpleDesc3"] = SimpleDesc3_JSON,
                    ["SimpleDesc4"] = SimpleDesc4_JSON,
                    ["SimpleDesc5"] = SimpleDesc5_JSON,
                };
            }




            // Индексы строк в файле с ЭГО дарами для перезаписи конкретной строки с "simpledesc" или "name". Сериализация будет использоваться в редактировании навыков
            for (int LineIndex = 0; LineIndex < JsonLines.Count; LineIndex++)
            {
                if (JsonLines[LineIndex].Trim().StartsWith("\"id\": "))
                {
                    LineIndex_Name = -1;
                    LineIndex_Desc = -1;
                    LineIndex_SimpleDesc1 = -1;
                    LineIndex_SimpleDesc2 = -1;
                    LineIndex_SimpleDesc3 = -1;
                    LineIndex_SimpleDesc4 = -1;
                    LineIndex_SimpleDesc5 = -1;

                    // Смещение по строкам вниз для определения индексов линий со строками. Файлы ЭГО даров в этом плане монотонные, поэтому не сериализация
                    ID = Convert.ToInt32(JsonLines[LineIndex].Trim().Split("\"id\": ")[1].Split(",")[0]);
                    LineIndex_Name = LineIndex + 2;
                    LineIndex_Desc = LineIndex + 3;
                    try
                    {
                        if (JsonLines[LineIndex + 5].Trim().StartsWith($"\"abilityID\":")) LineIndex_SimpleDesc1 = LineIndex + 7;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[LineIndex + 9].Trim().StartsWith($"\"abilityID\":")) LineIndex_SimpleDesc2 = LineIndex + 11;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[LineIndex + 13].Trim().StartsWith($"\"abilityID\":")) LineIndex_SimpleDesc3 = LineIndex + 15;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[LineIndex + 17].Trim().StartsWith($"\"abilityID\":")) LineIndex_SimpleDesc4 = LineIndex + 19;
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[LineIndex + 21].Trim().StartsWith($"\"abilityID\":")) LineIndex_SimpleDesc5 = LineIndex + 22;
                    }
                    catch { }

                    Json_Dictionary[ID]["LineIndex_Name"] = LineIndex_Name;
                    Json_Dictionary[ID]["LineIndex_Desc"] = LineIndex_Desc;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc1"] = LineIndex_SimpleDesc1;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc2"] = LineIndex_SimpleDesc2;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc3"] = LineIndex_SimpleDesc3;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc4"] = LineIndex_SimpleDesc4;
                    Json_Dictionary[ID]["LineIndex_SimpleDesc5"] = LineIndex_SimpleDesc5;
                    if (WriteInfo) Console.WriteLine(Json_Dictionary[ID]["Name"]);
                    if (WriteInfo) Console.WriteLine($"LineIndex_Name: {LineIndex_Name}\n" +
                                                     $"LineIndex_Desc: {LineIndex_Desc}\n" +
                                                     $"LineIndex_SimpleDesc1: {LineIndex_SimpleDesc1}\n" +
                                                     $"LineIndex_SimpleDesc2: {LineIndex_SimpleDesc2}\n" +
                                                     $"LineIndex_SimpleDesc3: {LineIndex_SimpleDesc3}\n" +
                                                     $"LineIndex_SimpleDesc4: {LineIndex_SimpleDesc4}\n" +
                                                     $"LineIndex_SimpleDesc5: {LineIndex_SimpleDesc5}\n\n\n");

                }
            }
            if (WriteInfo) Console.WriteLine($"Прочитано {Json_Dictionary.Keys.Count} объектов из {Json_Файл}");
            return Json_Dictionary;
        }

        public static (string, int) GetUnsavedChanges(Dictionary<int, Dictionary<string, object>> CheckDictionary)
        {
            string Info = "";
            string Info_Sub = "";
            bool IsEditedID = false;
            int EditsCount = 0;
            string Element;
            foreach (var ID in CheckDictionary)
            {
                IsEditedID = false;
                Info_Sub = "";
                foreach (var Descriptor in ID.Value)
                {
                    if (!Descriptor.Value.Equals("{unedited}"))
                    {
                        Element = Descriptor.Key switch
                        {
                            "Desc" => "Описание",
                            _ => $"Простое описание {Descriptor.Key[^1]}",
                        };
                        IsEditedID = true;
                        Info_Sub += $"  - {Element}\n";
                        EditsCount++;
                    }
                }
                if (IsEditedID)
                {
                    Info += $"ID {ID.Key}:\n{Info_Sub}\n";
                }
            }

            return (Info, EditsCount);
        }


        private static void ConsoleManager(string Json_Файл, Dictionary<int, Dictionary<string, object>> Json_Dictionary)
        {
            // Для консольного отображения
            Console.WriteLine($"Json файл: \x1b[38;5;214m{Json_Файл}\x1b[0m, {Json_Dictionary.Keys.Count} ID");
            while (true)
            {
                Console.Write("\nID ЭГО Дара: \x1b[38;5;214m");
                try
                {
                    int index = Convert.ToInt32(Console.ReadLine()); Console.Write("\x1b[0m");
                    Console.WriteLine($" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_Name"]}) Name\x1b[0m: \"{Json_Dictionary[index]["Name"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_Desc"]}) Desc\x1b[0m: \"{Json_Dictionary[index]["Desc"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc1"]}) SimpleDesc1\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc1"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc2"]}) SimpleDesc2\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc2"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc3"]}) SimpleDesc3\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc3"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc4"]}) SimpleDesc4\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc4"]}\"\n" +
                                      $" \x1b[38;5;214m[-¤-] (Cтрока {Json_Dictionary[index]["LineIndex_SimpleDesc5"]}) SimpleDesc5\x1b[0m: \"{Json_Dictionary[index]["SimpleDesc5"]}\"\n\n");
                }
                catch { Console.WriteLine("Нет такого ID"); }
            }
        }
    }
}
