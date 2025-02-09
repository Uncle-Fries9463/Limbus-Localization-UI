using System.Windows;
using System.Windows.Media.Imaging;

using static Limbus_Localization_UI.MainWindow;

namespace Limbus_Localization_UI.Mode_Handlers
{
    /// <summary>
    /// Набор методов для работы с файлами навыков<br/>
    /// Вынесено в отдельный клас, дабы дальше не захламлять MainWindow<br/>
    /// </summary>
    internal class Mode_Skills
    {
        static Dictionary<string, dynamic> T;
        public static void InitTDictionaryHere(Dictionary<string, dynamic> FromExternal) => T=FromExternal;

        /// <summary>
        /// Адаптировать интерфейс под режим работы с Skills_x.json файлами<br />
        /// (Изменить размер, фон предпросмотра, окно предпросмотра, текс на кнопках и добавить новые)<br />
        /// </summary>
        public static void AdjustUI()
        {
            T["PreviewLayout Background"].Source = new BitmapImage(new Uri("pack://application:,,,/Images/Фон Навыков.png"));

            T["Window"].MinHeight = 421; // Минимальная высота и ширина под фон предпросмтора навыка
            T["Window"].MinWidth = 717;

            T["Window"].MaxWidth = 1005; // Максимальная ширина с боковым меню

            T["Window"].Width = 1005;    // Ширина и высота по умолчанию
            T["Window"].Height = 560;

            T["JsonIO Column"].Width = new GridLength(702); // Ширина поля предпросмотра и редактора json элемента
            T["Json EditBox"].Width = 692.2;

            T["PreviewLayout @ EGO Gift"].Margin = new Thickness(5300, 0, 0, 60);  // Скрыть предпросмотр ЭГО дара
            T["PreviewLayout @ Skill"   ].Margin = new Thickness(11, 0, 0, 40);    // Показать предпросмотр Навыка

            T["Left Menu Buttons box"].Margin = new Thickness(0, 56, 0, 0);
            T["Skill UptieLevel Selection Box"].Height = 52;

            //T["Left Menu Buttons Box"].Height = 286;

            T["Name EditBox Shadow"].Content = "Название навыка";
            for (int i = 1; i <= 5;  i++) // Изменить текст на 5 кнопкахс Простого описания на Монету
            {
                T[$"EditorSwitch SubDesc {i}"].Content = $"Монета {i}";
            }

            T["Json EditBox"].Text = "";
        }

        /// <summary>
        /// Обновить информацию в боковом меню в соответствии с параметрами редактируемого навыка по его уровню связи<br />
        /// Уровень связи по умолчани 0 (При переключении между ID навыков он разный и тогда наименьший доступный)<br />
        /// При переключении между уровнями связи на странице навыка он указывается<br />
        /// </summary>
        public static void UpdateMenuInfo(int SkillID, int UptieLevel = 0)
        {
            // Если уровень связи не указан (0 — Переключение между ID), взять самый первый доступный
            List<int> UptieLevelsAvalible = CheckUptieLevelsAvalible(SkillID); ;
            if (UptieLevel == 0)
            {
                UptieLevel = UptieLevelsAvalible[0];
                Skills_CurrentEditingField = "Desc"; // При переключении между ID выбирать Desc как стандартое редактируемое поле
            }
            Skills_Json_Dictionary_CurrentUptieLevel = UptieLevel; // Обновить значение текущего уровня связи для MainWindow



            // По умолчанию сделать недоступными все кнопки монет и скрыть их на предпросмотре вместе с описанием навыка
            ResetInformationVisiblity();





            // Скрыть все кнопки уровней связи по умолчанию
            for (int IUptieLevel = 1; IUptieLevel <= 4; IUptieLevel++)
            {
                T[$"Uptie Level {IUptieLevel}"].Content = T["Uptie Level Icons"][0]; // Подсветка кнопки пустой картинкой
                T[$"Uptie Level {IUptieLevel} [UnavalibleCover]"].Height = 41;       // Высота перекрывающей границы
                T[$"Uptie Level {IUptieLevel} [UnavalibleSubCover]"].Height = 34;
            }

            // Разблокировать кнопки досутпных уровней связи
            foreach (var IUptieLevel in UptieLevelsAvalible)
            {
                T[$"Uptie Level {IUptieLevel} [UnavalibleCover]"].Height = 0;
                T[$"Uptie Level {IUptieLevel} [UnavalibleSubCover]"].Height = 0;
            }

            // Подсветить выбранный
            T[$"Uptie Level {UptieLevel}"].Content = T["Uptie Level Icons"][UptieLevel];





            // Включить отображение описания в поле предпросмотра, если оно не пустое в DataList и не изменено в буфере редактирования
            if (!Skills_Json_Dictionary[SkillID][UptieLevel]["Desc"].Equals(""))
            {
                T["Skill PreviewLayout Desc"].Height = Double.NaN; // Height="Auto"
                T["Skill PreviewLayout Desc"].MinHeight = 30;
            }

            // Проверить какие монеты доступны для редактирования
            List<int> CoinsAvalible = CheckCoinsAvalible(SkillID, UptieLevel);

            // Включить кнопки доступных монет и показать их на предпросмотре
            UnlockAvalibleCoins(CoinsAvalible);

            // Перебрать описания каждой из монет и обновить предпросмотр

            foreach(var Coin in Skills_Json_Dictionary[SkillID][UptieLevel]["Coins"])
            {
                int CoinNumber = Coin.Key;
                int DescIndex = 0;
                string CoinDescExportString;
                List<string> CoinDescExport = new();
                foreach (var CoinDesc in Skills_Json_Dictionary[SkillID][UptieLevel]["Coins"][CoinNumber])
                {
                    // Если это описание монеты не пустое в буфере редактирования
                    if (Skills_EditBuffer[SkillID][UptieLevel]["Coins"][CoinNumber][DescIndex].Equals("{unedited}"))
                    {
                        //Console.WriteLine(CoinDesc);
                        T[$"Skill PreviewLayout Coin {CoinNumber} Desc {DescIndex + 1}"].Height = Double.NaN;
                        MainWindow.UpdatePreview(CoinDesc.Replace("\"", "\\\""), T[$"Skill PreviewLayout Coin {CoinNumber} Desc {DescIndex+1}"]);
                    }
                    else
                    {
                        //Console.WriteLine(Skills_EditBuffer[SkillID][UptieLevel]["Coins"][CoinNumber][DescIndex]);
                        //CoinDescExport.Add(Skills_EditBuffer[SkillID][UptieLevel]["Coins"][CoinNumber][DescIndex].Replace("\"", "\\\""));
                        MainWindow.UpdatePreview(Skills_EditBuffer[SkillID][UptieLevel]["Coins"][CoinNumber][DescIndex].Replace("\"", "\\\""), T[$"Skill PreviewLayout Coin {CoinNumber} Desc {DescIndex+1}"]);
                    }

                    DescIndex++;
                }
                CoinDescExportString = String.Join("\n", CoinDescExport);

            }




            // Отобразить в боковом меню ID и имя навыка
            T["Name EditBox Shadow"].Content = "";
            T["Name EditBox"].Text = Skills_Json_Dictionary[SkillID][UptieLevel]["Name"]; // Имя в поле редактирования
            T["Name Label"].Text = Skills_Json_Dictionary[SkillID][UptieLevel]["Name"];   // Имя на заголовке
            T["ID Label"].Content = $"{SkillID}"; // ID на кнопке копирования ID


            // Задать текст в поле редактирования Json (+ Проверка буфера изменений)
            if (Skills_CurrentEditingField.Equals("Desc"))
            {
                // Если буфер редактирования пустой (= "{unedited}"), показывать текст из JsonData
                if (Skills_EditBuffer[SkillID][UptieLevel][Skills_CurrentEditingField].Equals("{unedited}"))
                {
                    T["Json EditBox"].Text = Skills_Json_Dictionary[SkillID][UptieLevel][Skills_CurrentEditingField];
                }
                else // Иначе вставлять из буфера
                {
                    T["Json EditBox"].Text = Skills_EditBuffer[SkillID][UptieLevel][Skills_CurrentEditingField];
                }
            }
        }



        // Вспомогательные функции

        /// <summary>
        /// Получить список из доступных для редактирования уровней связи по ID навыка<br />
        /// Для первых и вторых навыков это чаще всего [1, 2, 4], для третьих [3, 4]<br />
        /// </summary>
        public static List<int> CheckUptieLevelsAvalible(int SkillID)
        {
            List<int> UptieLevelsAvalible = new List<int>();

            // Для каждого ключа уровней связи в списке ID навыка (Например 1, 2, 4)
            foreach (var UptieLevel in Skills_Json_Dictionary[SkillID].Keys)
            {
                UptieLevelsAvalible.Add(UptieLevel);
            }

            return UptieLevelsAvalible;
        }

        /// <summary>
        /// Получить список из доступных монет навыка по уровню связи (1, 2, 4; 1, 3; 4 и т.д.)
        /// </summary>
        public static List<int> CheckCoinsAvalible(int SkillID, int UptieLevel)
        {
            List<int> CoinsAvalible = new List<int>();

            // Для каждого ключа монеты в списке монет навыка по его ID и уровню связи (Например 1, 3)
            foreach (var CoinNumber in Skills_Json_Dictionary[SkillID][UptieLevel]["Coins"].Keys)
            {
                CoinsAvalible.Add(CoinNumber);
            }

            return CoinsAvalible;
        }

        /// <summary>
        /// Отключить доступность всех кнопок монет и описания, а так же скрыть их на предпросмотре
        /// </summary>
        public static void ResetInformationVisiblity()
        {
            // Скрыть описание навыка на предпросмтре (Высота и минимальная высота стековой панели = 0)
            T["Skill PreviewLayout Desc"].Height = 0;
            T["Skill PreviewLayout Desc"].MinHeight = 0;

            for (int CoinNumber = 1; CoinNumber <= 5; CoinNumber++)
            {
                T[$"EditorSwitch SubDesc {CoinNumber} [UnavalibleCover]"].Height = 30;
                T[ $"SaveChanges SubDesc {CoinNumber} [UnavalibleCover]"].Height = 30;

                // Скрыть описание монеты на предпросмтре (Высота и минимальная высота стековой панели = 0)
                T[$"Skill PreviewLayout Coin {CoinNumber} Panel"].Height = 0;
                T[$"Skill PreviewLayout Coin {CoinNumber} Panel"].MinHeight = 0;
                for (int CoinDescNumber = 1; CoinDescNumber <= 6; CoinDescNumber++)
                {
                    T[$"Skill PreviewLayout Coin {CoinNumber} Desc {CoinDescNumber}"].Height = 0;
                }
            }
        }

        /// <summary>
        /// Разблокировать кнопки доступных монет и показать их на предпросмотре
        /// </summary>
        public static void UnlockAvalibleCoins(List<int> CoinsAvalible)
        {
            foreach (var CoinNumber in CoinsAvalible)
            {
                T[$"EditorSwitch SubDesc {CoinNumber} [UnavalibleCover]"].Height = 0;
                T[ $"SaveChanges SubDesc {CoinNumber} [UnavalibleCover]"].Height = 0;

                T[$"Skill PreviewLayout Coin {CoinNumber} Panel"].Height = Double.NaN;  // Height="Auto"
                T[$"Skill PreviewLayout Coin {CoinNumber} Panel"].MinHeight = 48;
            }
        }

    }
}
