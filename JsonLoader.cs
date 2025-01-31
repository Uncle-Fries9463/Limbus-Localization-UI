using System.IO;
using System.Text;

namespace Limbus_Json_Preview
{
    class JsonLoader
    {
        private static List<string> ReadAllLines(string file)
        {
            List<string> ReadedLines = new List<string>();

            using (var fileStream = File.OpenRead(file))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    ReadedLines.Add(line);
                }
            }
            return ReadedLines;
        }

        public static Dictionary<int, Dictionary<string, object>> Json_GetAdressBook(string Json_Файл)
        {
            List<string> JsonLines = ReadAllLines(Json_Файл);

            int ID;
            string Name; int LineIndex_Name;
            string Desc; int LineIndex_Desc;
            string SimpleDesc1; int LineIndex_SimpleDesc1;
            string SimpleDesc2; int LineIndex_SimpleDesc2;
            string SimpleDesc3; int LineIndex_SimpleDesc3;
            string SimpleDesc4; int LineIndex_SimpleDesc4;
            string SimpleDesc5; int LineIndex_SimpleDesc5;

            Dictionary<int, Dictionary<string, object>> Json_Dictionary = new();

            for (int i = 0; i < JsonLines.Count; i++)
            {
                if (JsonLines[i].StartsWith("      \"id\": "))
                {
                    SimpleDesc1 = "{none}";
                    SimpleDesc2 = "{none}";
                    SimpleDesc3 = "{none}";
                    SimpleDesc4 = "{none}";
                    SimpleDesc5 = "{none}";
                    LineIndex_Name = -1;
                    LineIndex_Desc = -1;
                    LineIndex_SimpleDesc1 = -1;
                    LineIndex_SimpleDesc2 = -1;
                    LineIndex_SimpleDesc3 = -1;
                    LineIndex_SimpleDesc4 = -1;
                    LineIndex_SimpleDesc5 = -1;

                    ID = Convert.ToInt32(JsonLines[i].Split("      \"id\": ")[1].Split(",")[0]);
                    Name = JsonLines[i + 1].Split("      \"name\": \"")[1].Split("\",\n\t")[0].Split("\",")[0]; LineIndex_Name = i + 2;
                    Desc = JsonLines[i + 2].Split("      \"desc\": \"")[1].Split("\",\n\t")[0].Split("\",")[0]; LineIndex_Desc = i + 3;
                    try
                    {
                        if (JsonLines[i + 5].StartsWith($"          \"abilityID\": {ID}"))
                        {
                            SimpleDesc1 = JsonLines[i + 6].Split("          \"simpleDesc\": \"")[1][..^1];
                            LineIndex_SimpleDesc1 = i + 7;

                        }
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 9].StartsWith($"          \"abilityID\": {ID}"))
                        {
                            SimpleDesc2 = JsonLines[i + 10].Split("          \"simpleDesc\": \"")[1][..^1];
                            LineIndex_SimpleDesc2 = i + 11;
                        }
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 13].StartsWith($"          \"abilityID\": {ID}"))
                        {
                            SimpleDesc3 = JsonLines[i + 14].Split("          \"simpleDesc\": \"")[1][..^1];
                            LineIndex_SimpleDesc3 = i + 15;
                        }
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 17].StartsWith($"          \"abilityID\": {ID}"))
                        {
                            SimpleDesc4 = JsonLines[i + 18].Split("          \"simpleDesc\": \"")[1][..^1];
                            LineIndex_SimpleDesc4 = i + 19;
                        }
                    }
                    catch { }

                    try
                    {
                        if (JsonLines[i + 21].StartsWith($"          \"abilityID\": {ID}"))
                        {
                            SimpleDesc5 = JsonLines[i + 21].Split("          \"simpleDesc\": \"")[1][..^1];
                            LineIndex_SimpleDesc5 = i + 22;
                        }
                    }
                    catch { }

                    Json_Dictionary[ID] = new Dictionary<string, object>
                    {
                        ["Name"] = Name,
                        ["Desc"] = Desc,
                        ["SimpleDesc1"] = SimpleDesc1,
                        ["SimpleDesc2"] = SimpleDesc2,
                        ["SimpleDesc3"] = SimpleDesc3,
                        ["SimpleDesc4"] = SimpleDesc4,
                        ["SimpleDesc5"] = SimpleDesc5,

                        ["LineIndex_Name"] = LineIndex_Name,
                        ["LineIndex_Desc"] = LineIndex_Desc,
                        ["LineIndex_SimpleDesc1"] = LineIndex_SimpleDesc1,
                        ["LineIndex_SimpleDesc2"] = LineIndex_SimpleDesc2,
                        ["LineIndex_SimpleDesc3"] = LineIndex_SimpleDesc3,
                        ["LineIndex_SimpleDesc4"] = LineIndex_SimpleDesc4,
                        ["LineIndex_SimpleDesc5"] = LineIndex_SimpleDesc5,
                    };
                }
            }
            Console.WriteLine($"Прочитано {Json_Dictionary.Keys.Count} объектов из {Json_Файл}");
            return Json_Dictionary;
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
