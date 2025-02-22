using Newtonsoft.Json;
using System.IO;
using static Limbus_Localization_UI.Additions.Consola;

namespace Limbus_Localization_UI.Json
{
    internal class JsonLoader_Skills
    {
        // ID > Уровень связи > Описания и список монет

        public static JsonData JSON;
        public static Dictionary<int, int> ID_AND_INDEX = new();
        public static Dictionary<int, Dictionary<int, int>> UPTIELEVEL_AND_INDEX = new();
        public static Dictionary<int, Dictionary<int, Dictionary<int, int>>> COIN_AND_INDEX = new();
        public static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, int>>>> COINDESC_AND_INDEX = new();

        public static (Dictionary<int, dynamic>, Dictionary<int, dynamic>) GetJsonDictionary(string Json_Файл)
        {
            JSON = JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(Json_Файл));
            bool WriteInfo = false;

            

            Dictionary<int, dynamic> Skills_Json_Dictionary = new();
            Dictionary<int, dynamic> Skills_EditBuffer = new();      // EditBuffer — копия верхнего, но вместо всех строковых значений числится {unedited}. При изменении текста и отличии его от оригинала из Json_Dictionary, EditBuffer принимает новое значение вместо {unedited}
            
            // Для каждого ID в списке всех ID файла
            int ID_Index = 0;
            foreach (var ThisID in JSON.dataList)
            {
                int Skill_ID = int.Parse($"{ThisID.id}");
                ID_AND_INDEX[Skill_ID] = ID_Index;////////////////////////////////////////////////////////////////////////////////////////////////////////

                Skills_Json_Dictionary[Skill_ID] = new Dictionary<int, dynamic>();
                Skills_EditBuffer     [Skill_ID] = new Dictionary<int, dynamic>();
                if (WriteInfo) Console.WriteLine($"--(ID {Skill_ID})------------------------------------------------");

                // Для каждого навыка по уровню его уровню связи в списке навыков по ID
                int UptieLevelIndex = 0;
                UPTIELEVEL_AND_INDEX[Skill_ID] = new();///////////////////////////////////////////////////////////////////////////////////////////////////
                COIN_AND_INDEX[Skill_ID] = new();/////////////////////////////////////////////////////////////////////////////////////////////////////////
                COINDESC_AND_INDEX[Skill_ID] = new();/////////////////////////////////////////////////////////////////////////////////////////////////////
                foreach (var ThisUptieLevel in ThisID.levelList)
                {
                    int Skill_UptieLevel = ThisUptieLevel.level;
                    Skills_Json_Dictionary[Skill_ID][Skill_UptieLevel] = new Dictionary<int, dynamic>();
                    Skills_EditBuffer     [Skill_ID][Skill_UptieLevel] = new Dictionary<int, dynamic>();

                    string Skill_Name = ThisUptieLevel.name;
                    string EGOSkill_ABName = ThisUptieLevel.abName;
                    string Skill_Desc = ThisUptieLevel.desc;

                    // Если значения нет в json (пустое описание навыков как например в Skills_Abnormality.json)

                    try { EGOSkill_ABName.Equals(null); }
                    catch { EGOSkill_ABName = ""; }

                    try { Skill_Desc.Equals(null); }
                    catch { Skill_Desc = ""; }
                    




                    UPTIELEVEL_AND_INDEX[Skill_ID][Skill_UptieLevel] = UptieLevelIndex;////////////////////////////////////////////////////////////////////
                    COIN_AND_INDEX[Skill_ID][Skill_UptieLevel] = new();////////////////////////////////////////////////////////////////////////////////////
                    COINDESC_AND_INDEX[Skill_ID][Skill_UptieLevel] = new();////////////////////////////////////////////////////////////////////////////////

                    Dictionary<int, List<string>> Skill_CoinDescs      = new();
                    Dictionary<int, List<string>> EditBuffer_CoinDescs = new();

                    if (WriteInfo) Console.WriteLine($"Name: {Skill_Name} (Tier {Skill_UptieLevel}):\n  {Skill_Desc.Replace("\n", "\\n")}");

                    int CoinCounter = 0;
                    int COININDEX = 0;

                    try
                    {
                        // В некоторых навыках нет списка монет впринципе
                        ThisUptieLevel.coinlist.Equals(null);


                        // Для каждой монеты в списке монет уровня связи
                        foreach (var Coin in ThisUptieLevel.coinlist)
                        {
                            CoinCounter++;

                            COIN_AND_INDEX[Skill_ID][Skill_UptieLevel][CoinCounter] = COININDEX;/////////////////////////////////////////////////////////////////
                            COINDESC_AND_INDEX[Skill_ID][Skill_UptieLevel][CoinCounter] = new();/////////////////////////////////////////////////////////////////

                            Skill_CoinDescs[CoinCounter] = new List<string>();
                            EditBuffer_CoinDescs[CoinCounter] = new List<string>();
                            try
                            {
                                // Если у монеты есть описание
                                var EmptyCoinTryCheck = Coin.coindescs[0];
                                if (WriteInfo) Console.WriteLine($"    (Coin {CoinCounter}):");
                                int COINDESC_INDEX = 0;
                                foreach (var coindesc in Coin.coindescs)
                                {
                                    COINDESC_AND_INDEX[Skill_ID][Skill_UptieLevel][CoinCounter][COINDESC_INDEX] = COINDESC_INDEX;
                                    string CoinDesc = coindesc.desc;
                                    if (WriteInfo) Console.WriteLine($"      - '{coindesc.desc}'");

                                    if (CoinDesc.Equals(""))
                                    {
                                        // Если описание монеты есть, но оно пустое
                                        Skill_CoinDescs.Remove(CoinCounter);
                                        EditBuffer_CoinDescs.Remove(CoinCounter);
                                    }
                                    else
                                    {
                                        // Добавить описание монеты в список её описаний
                                        Skill_CoinDescs[CoinCounter].Add(coindesc.desc);
                                        EditBuffer_CoinDescs[CoinCounter].Add("{unedited}");
                                    }
                                    COINDESC_INDEX++;
                                }
                            }
                            catch
                            {
                                // Если у монеты нет описания, удалить ключ (Монеты  1, 2(Пусто), 3  ->  1, 3)
                                Skill_CoinDescs.Remove(CoinCounter);
                                EditBuffer_CoinDescs.Remove(CoinCounter);
                            }

                            COININDEX++;
                        }
                        if (WriteInfo) Console.WriteLine("\n");
                    }
                    catch
                    {
                        // У уровня связи нет coinlist
                    }
                    

                    // Словарь со ссылками на json datalist
                    Skills_Json_Dictionary[Skill_ID][Skill_UptieLevel] = new Dictionary<string, dynamic>()
                    {
                        ["Name"] = Skill_Name,
                        ["Desc"] = Skill_Desc,
                        ["Coins"] = Skill_CoinDescs
                    };

                    try
                    {
                        Skills_Json_Dictionary[Skill_ID][Skill_UptieLevel]["ABName"] = EGOSkill_ABName;
                    }
                    catch
                    {
                        Skills_Json_Dictionary[Skill_ID][Skill_UptieLevel]["ABName"] = "";
                    }

                    // Буфер не сохранённых изменений
                    Skills_EditBuffer[Skill_ID][Skill_UptieLevel] = new Dictionary<string, dynamic>()
                    {
                        ["Desc"] = "{unedited}",
                        ["Coins"] = EditBuffer_CoinDescs
                    };

                    UptieLevelIndex++;
                }
                if (WriteInfo) Console.WriteLine("\n\n");
                ID_Index++;
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

        public static (string, int) GetUnsavedChanges(Dictionary<int, dynamic> CheckDictionary)
        {
            string Info = "";
            string Info_Sub;
            


            int EditsCount = 0;
            string ex_Info = "";

            foreach (var ID in CheckDictionary)
            {
                int CurrentID = ID.Key;
                bool IsEditedID = false;
                //Console.WriteLine("¤ ID " + CurrentID);
                List<int> EditedUptieLevels = new();
                foreach (var UptieLevel in CheckDictionary[CurrentID])
                {
                    int CurrentUptieLevel = UptieLevel.Key;
                    //Console.WriteLine("  - UL " + CurrentUptieLevel);
                    //Console.WriteLine("    - DESC " + UptieLevel.Value["Desc"]);
                    if (!UptieLevel.Value["Desc"].Equals("{unedited}"))
                    {
                        IsEditedID = true;
                        if (!EditedUptieLevels.Contains(CurrentUptieLevel)) EditedUptieLevels.Add(CurrentUptieLevel);
                        EditsCount++;
                    }

                    foreach(var Coin in CheckDictionary[CurrentID][CurrentUptieLevel]["Coins"])
                    {
                        int CoinNumber = Coin.Key;
                        //Console.WriteLine("      - COIN " + CoinNumber);

                        int CoinDescNumber = 1;
                        foreach(var CoinDesc in Coin.Value)
                        {
                            //Console.WriteLine("        - COINDESC " + CoinDesc);
                            if (!CoinDesc.Equals("{unedited}"))
                            {
                                IsEditedID = true;
                                if(!EditedUptieLevels.Contains(CurrentUptieLevel)) EditedUptieLevels.Add(CurrentUptieLevel);
                                EditsCount++;
                            }
                            CoinDescNumber++;
                        }
                    }

                }
                if (IsEditedID)
                {
                    Info += $"ID {CurrentID}\n - Уровни связи: {String.Join(", ", EditedUptieLevels)}\n\n";
                }
            }
            return (Info, EditsCount);
        }
    }
}
