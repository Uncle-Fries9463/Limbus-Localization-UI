using Newtonsoft.Json;
using System.IO;

namespace Limbus_Localization_UI.Json
{
    internal class JsonLoader_Skills
    {
        // ID > Уровень связи > Описания и список монет
        public static (Dictionary<int, dynamic>, Dictionary<int, dynamic>) GetJsonDictionary(string Json_Файл)
        {
            var JSON = JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(Json_Файл));
            bool WriteInfo = false;

            Dictionary<int, dynamic> Skills_Json_Dictionary = new();
            Dictionary<int, dynamic> Skills_EditBuffer = new();      // EditBuffer — копия верхнего, но вместо всех строковых значений числится {unedited}. При изменении текста и отличии его от оригинала из Json_Dictionary, EditBuffer принимает новое значение вместо {unedited}
            
            // Для каждого ID в списке всех ID файла
            foreach (var ThisID in JSON.DataList)
            {
                int Skill_ID = ThisID.id;
                Skills_Json_Dictionary[Skill_ID] = new Dictionary<int, dynamic>();
                Skills_EditBuffer     [Skill_ID] = new Dictionary<int, dynamic>();
                if (WriteInfo) Console.WriteLine($"--(ID {Skill_ID})------------------------------------------------");

                // Для каждого навыка по уровню его уровню связи в списке навыков по ID
                foreach (var ThisUptieLevel in ThisID.levelList)
                {
                    int Skill_UptieLevel = ThisUptieLevel.level;
                    Skills_Json_Dictionary[Skill_ID][Skill_UptieLevel] = new Dictionary<int, dynamic>();
                    Skills_EditBuffer     [Skill_ID][Skill_UptieLevel] = new Dictionary<int, dynamic>();

                    string Skill_Name = ThisUptieLevel.name;
                    string Skill_Desc = ThisUptieLevel.desc;

                    Dictionary<int, List<string>> Skill_CoinDescs      = new();
                    Dictionary<int, List<string>> EditBuffer_CoinDescs = new();

                    if (WriteInfo) Console.WriteLine($"Name: {Skill_Name} (Tier {Skill_UptieLevel}):\n  {Skill_Desc.Replace("\n", "\\n")}");

                    int CoinCouner = 0;
                    // Для каждой монеты в списке монет
                    foreach (var Coin in ThisUptieLevel.coinlist)
                    {
                        CoinCouner++;
                        Skill_CoinDescs     [CoinCouner] = new List<string>();
                        EditBuffer_CoinDescs[CoinCouner] = new List<string>();
                        try
                        {
                            // Если у монеты есть описание
                            var EmptyCoinTryCheck = Coin.coindescs[0];
                            if (WriteInfo) Console.WriteLine($"    (Coin {CoinCouner}):");
                            foreach (var coindesc in Coin.coindescs)
                            {
                                string CoinDesc = coindesc.desc;
                                if (WriteInfo) Console.WriteLine($"      - '{coindesc.desc}'");

                                if (CoinDesc.Equals(""))
                                {
                                    // Если описание монеты есть, но оно пустое
                                    Skill_CoinDescs     .Remove(CoinCouner);
                                    EditBuffer_CoinDescs.Remove(CoinCouner);
                                }
                                else
                                {
                                    // Добавить описание монеты в список её описаний
                                    Skill_CoinDescs     [CoinCouner].Add(coindesc.desc);
                                    EditBuffer_CoinDescs[CoinCouner].Add("{unedited}");
                                }
                            }
                        }
                        catch
                        {
                            // Если у монеты нет описания, удалить ключ (Монеты  1, 2(Пусто), 3  ->  1, 3)
                            Skill_CoinDescs     .Remove(CoinCouner);
                            EditBuffer_CoinDescs.Remove(CoinCouner);

                        }
                    }
                    if (WriteInfo) Console.WriteLine("\n");

                    // Словарь со ссылками на json datalist
                    Skills_Json_Dictionary[Skill_ID][Skill_UptieLevel] = new Dictionary<string, dynamic>()
                    {
                        ["Name"] = Skill_Name,
                        ["Desc"] = Skill_Desc,
                        ["Coins"] = Skill_CoinDescs
                    };

                    // Буфер не сохранённых изменений
                    Skills_EditBuffer[Skill_ID][Skill_UptieLevel] = new Dictionary<string, dynamic>()
                    {
                        ["Name"] = "{unedited}",
                        ["Desc"] = "{unedited}",
                        ["Coins"] = EditBuffer_CoinDescs
                    };

                }
                if (WriteInfo) Console.WriteLine("\n\n");

            }

            if (WriteInfo)
            {
                // По Skills_personality-03.json

                // Навык с id 1030703, третий уровень связи, в списке монет третья монета, её третье описание (индекс 2)
                Console.WriteLine(Skills_Json_Dictionary[1030703][3]["Coins"][3][2]);
                Console.WriteLine(Skills_EditBuffer     [1030703][3]["Coins"][3][2]);

                // Доступные уровни связи навыка
                foreach(var UptieLevel in Skills_Json_Dictionary[1030703].Keys)
                {
                    Console.WriteLine(UptieLevel);
                }

                // Доступные номера монет на 3 уровне связи
                foreach(var CoinNumber in Skills_Json_Dictionary[1030703][3]["Coins"].Keys)
                {
                    Console.WriteLine(CoinNumber);
                }
            }


            return (Skills_Json_Dictionary, Skills_EditBuffer);
        }
    }
}
