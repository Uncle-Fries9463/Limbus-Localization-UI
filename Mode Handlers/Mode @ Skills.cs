using Limbus_Localization_UI.Additions;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using static Limbus_Localization_UI.Additions.Consola;

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
        public static void AdjustUI(bool IsEGO = false, bool IsEnemies = false)
        {
            try
            {
                T["Save Changes Buttons"].Margin = new Thickness(236, IsEGO? -270 : - 237, 0, 0);
                T["Save Changes Buttons"].Height = IsEGO ? 270 : 237;
                T["Save Menu Buttons Box SubBox"].Height = Double.NaN;


                T["PreviewLayout Background"].Source = new BitmapImage(new Uri("pack://application:,,,/Images/Фон Навыков.png"));

                T["Splitter"].Width = new GridLength(3);

                T["Window"].MinHeight = 421; // Минимальная высота и ширина под фон предпросмтора навыка
                T["Window"].MinWidth = 717;

                T["Window"].MaxWidth = 1005; // Максимальная ширина с боковым меню

                T["Window"].Width = 1005;    // Ширина и высота по умолчанию
                T["Window"].Height = IsEGO ? 572 : 543;

                T["JsonIO Column"].Width = new GridLength(705); // Ширина поля предпросмотра и редактора json элемента
                T["Json EditBox"].Width = 693.5;

                T["PreviewLayout @ EGO Gift"].Margin = new Thickness(5300, 0, 0, 60);  // Скрыть предпросмотр ЭГО дара
                T["PreviewLayout @ Skill"   ].Margin = new Thickness(11, 0, 0, 40);    // Показать предпросмотр Навыка

                if (!IsEnemies)
                {
                    T["Left Menu Buttons box"].Margin = new Thickness(0, 56, 0, 0);
                    T["Skill UptieLevel Selection Box"].Height = 1000;
                }
                else
                {
                    T["Left Menu Buttons box"].Margin = new Thickness(0, 0, 0, 0);
                    T["Skill UptieLevel Selection Box"].Height = 0;
                }

                T["Coin Desc Selection Box"].Height = 42;
                T["Coin Desc Selection Box sub"].Height = 3.5;

                // Отключить все боковые кнопки сохранения кроме имени, сохранение только на Ctrl + S
                T["SaveChanges Desc"].Margin = new Thickness(4000, 0, 0, 0);
                T["SaveChanges Desc [UnavalibleCover]"].Margin = new Thickness(4000, 0, 0, 0);
                T["EditorSwitch Desc"].Width = 265;
                T["EditorSwitch Desc [UnavalibleCover]"].Width = 265;

                for (int i = 1; i <= 5; i++)
                {
                    T[$"SaveChanges SubDesc {i}"].Margin = new Thickness(4000, 0, 0, 0);
                    T[$"SaveChanges SubDesc {i} [UnavalibleCover]"].Margin = new Thickness(4000, 0, 0, 0);
                    T[$"EditorSwitch SubDesc {i}"].Width = 265;
                    T[$"EditorSwitch SubDesc {i}"].Height = 30;
                    T[$"EditorSwitch SubDesc {i} [UnavalibleCover]"].Width = 265;
                    T[$"EditorSwitch SubDesc {i} [UnavalibleCover]"].Height = 30;
                }

                T["Unsaved Changes Tooltip"].Width = 190;

                string s = InterfaceTextContent["[Left Menu] Skill Coin № Button"];
                for (int i = 1; i <= 5;  i++) // Изменить текст на 5 кнопкахс Простого описания на Монету
                {
                    T[$"EditorSwitch SubDesc {i}"].Content = s.Exform(i);
                }
            }
            catch { }
        }

        /// <summary>
        /// Обновить информацию в боковом меню в соответствии с параметрами редактируемого навыка по его уровню связи<br />
        /// Уровень связи по умолчани 0 (При переключении между ID навыков он разный и тогда наименьший доступный)<br />
        /// При переключении между уровнями связи на странице навыка он указывается<br />
        /// </summary>
        public static void UpdateMenuInfo(int SkillID, int UptieLevel = 0)
        {
            try
            {
                // Если уровень связи не указан (0 — Переключение между ID), взять самый первый доступный
                List<int> UptieLevelsAvalible = CheckUptieLevelsAvalible(SkillID); ;
                if (UptieLevel == 0)
                {
                    UptieLevel = UptieLevelsAvalible[0];
                }
                Skills_Json_Dictionary_CurrentUptieLevel = UptieLevel; // Обновить значение текущего уровня связи для MainWindow

                Skills_CurrentEditingField = "Desc"; // Выбирать по умолчанию при любом переходе описание в качестве редактируемого объекта..
                T["Current Highlight"].RenderTransform = new TranslateTransform(2, CurrentHighlight_YOffset + 61);
                ReEnableAvalibleCoinDescs(Disable: true);

                // По умолчанию сделать недоступными все кнопки монет и скрыть их на предпросмотре вместе с описанием навыка
                ResetInformationVisiblity();

                //rin(T["Coin Desc Selection Box"].ActualHeight);      // 73

                T["Coin Desc Selection Box"].Height = 72;              // Без этого тоже 73, НО КНОПОК НЕТ БЛЯТЬ ЧТО ЗА МАГИЯ ЕБАНАЯ

                //rin(T["Coin Desc Selection Box"].ActualHeight);      // 73

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
                        try
                        {
                            // Если это описание монеты не пустое в буфере редактирования
                            if (Skills_EditBuffer[SkillID][UptieLevel]["Coins"][CoinNumber][DescIndex].Equals("{unedited}") & !CoinDesc.Equals("{empty}"))
                            {
                                T[$"Skill PreviewLayout Coin {CoinNumber} Desc {DescIndex + 1}"].Height = Double.NaN;
                                MainWindow.Call_UpdatePreview(CoinDesc, T[$"Skill PreviewLayout Coin {CoinNumber} Desc {DescIndex+1}"]);
                            }
                            else if (!CoinDesc.Equals("{empty}"))
                            {
                                T[$"Skill PreviewLayout Coin {CoinNumber} Desc {DescIndex + 1}"].Height = Double.NaN;
                                MainWindow.Call_UpdatePreview(Skills_EditBuffer[SkillID][UptieLevel]["Coins"][CoinNumber][DescIndex], T[$"Skill PreviewLayout Coin {CoinNumber} Desc {DescIndex+1}"]);
                            }
                        }
                        catch{}

                        DescIndex++;
                    }
                    CoinDescExportString = String.Join("\n", CoinDescExport);
                }

                // При переходе между ID или уровнем связи обновить текст описания
                if (Skills_EditBuffer[SkillID][UptieLevel]["Desc"].Equals("{unedited}"))
                {
                    T["Json EditBox"].Text = Skills_Json_Dictionary[SkillID][UptieLevel]["Desc"];
                }
                else // Иначе вставлять из буфера
                {
                    T["Json EditBox"].Text = Skills_EditBuffer[SkillID][UptieLevel]["Desc"];
                }





                // Отобразить в боковом меню ID и имя навыка
                T["Name EditBox Shadow"].Content = "";
                T["Name EditBox"].Text = Skills_Json_Dictionary[SkillID][UptieLevel]["Name"]; // Имя в поле редактирования
                // Фоновое имя ЭГО
                if (Skills_Json_Dictionary[SkillID][UptieLevel]["ABName"] != "{none}")
                {
                    T["ABName EditBox Shadow"].Content = "";
                    T["ABName EditBox"].Text = Skills_Json_Dictionary[SkillID][UptieLevel]["ABName"];
                }
            
                T["Name Label"].Text = Skills_Json_Dictionary[SkillID][UptieLevel]["Name"];   // Имя на заголовке
                T["ID Label"].Content = $"{SkillID}"; // ID на кнопке копирования ID

                // Сброс буфера отмены
                T["Json EditBox"].IsUndoEnabled = false;
                T["Json EditBox"].IsUndoEnabled = true;
            }
            catch (Exception ex) { rin(ex.ToString()); }
        }



        // Вспомогательные функции

        /// <summary>
        /// Получить список из доступных для редактирования уровней связи по ID навыка<br />
        /// Для первых и вторых навыков это чаще всего [1, 2, 4], для третьих [3, 4]<br />
        /// </summary>
        public static List<int> CheckUptieLevelsAvalible(int SkillID)
        {
            List<int> UptieLevelsAvalible = new List<int>();

            try
            {
                // Для каждого ключа уровней связи в списке ID навыка (Например 1, 2, 4)
                foreach (var UptieLevel in Skills_Json_Dictionary[SkillID].Keys)
                {
                    UptieLevelsAvalible.Add(UptieLevel);
                }
            }
            catch { }

            return UptieLevelsAvalible;
        }

        /// <summary>
        /// Получить список из доступных монет навыка по уровню связи (1, 2, 4; 1, 3; 4 и т.д.)
        /// </summary>
        public static List<int> CheckCoinsAvalible(int SkillID, int UptieLevel)
        {
            List<int> CoinsAvalible = new List<int>();
            try
            {
                // Для каждого ключа монеты в списке монет навыка по его ID и уровню связи (Например 1, 3)
                foreach (var CoinNumber in Skills_Json_Dictionary[SkillID][UptieLevel]["Coins"].Keys)
                {
                    if(Skills_Json_Dictionary[SkillID][UptieLevel]["Coins"][CoinNumber].Count != 0)
                    {
                        if (!Skills_Json_Dictionary[SkillID][UptieLevel]["Coins"][CoinNumber][0].Equals("{empty}"))
                        {
                            CoinsAvalible.Add(CoinNumber);
                        }
                    }
                }
            }
            catch { }

            return CoinsAvalible;
        }

        /// <summary>
        /// Отключить доступность всех кнопок монет и описания, а так же скрыть их на предпросмотре
        /// </summary>
        public static void ResetInformationVisiblity()
        {
            try
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
                    for (int CoinDescNumber = 1; CoinDescNumber <= 12; CoinDescNumber++)
                    {
                        T[$"Skill PreviewLayout Coin {CoinNumber} Desc {CoinDescNumber}"].Height = 0;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Разблокировать кнопки доступных монет и показать их на предпросмотре
        /// </summary>
        public static void UnlockAvalibleCoins(List<int> CoinsAvalible)
        {
            try
            {
                foreach (var CoinNumber in CoinsAvalible)
                {
                    T[$"EditorSwitch SubDesc {CoinNumber} [UnavalibleCover]"].Height = 0;
                    T[ $"SaveChanges SubDesc {CoinNumber} [UnavalibleCover]"].Height = 0;

                    T[$"Skill PreviewLayout Coin {CoinNumber} Panel"].Height = Double.NaN;  // Height="Auto"
                    T[$"Skill PreviewLayout Coin {CoinNumber} Panel"].MinHeight = 48;
                }
            }
            catch { }
        }

        /// <summary>
        /// При нажатии на кнопку монеты проверить сколько у неё доступно описаний и разблокировать соответствующие кнопки
        /// </summary>
        public static void ReEnableAvalibleCoinDescs(int DescsCount = 1, bool Disable = false, List<int> EmptydescExceptions = null)
        {
            try
            {
                // Отключить все 6 кнопок
                for (int i = 1; i <= 12; i++)
                {
                    T[$"Coin Descs {i} Button"].Foreground = РазноеДругое.GetColorFromAHEX("#FF333333");
                    T[$"Coin Descs {i} Button"].BorderBrush = РазноеДругое.GetColorFromAHEX("#FF333333");
                    T[$"Coin Descs {i} Button"].IsEnabled = false;
                    T[$"Coin Descs {i} Button"].Content = $"№{i}";
                }

                // Включить соответствующее количество (Или же не включать и оставить все недоступными при Disable = true (Смена на 'Desc'))
                if (!Disable)
                {
                    for (int i = 1; i <= DescsCount; i++)
                    {
                        if (!EmptydescExceptions.Contains(i))
                        {
                            T[$"Coin Descs {i} Button"].Foreground = РазноеДругое.GetColorFromAHEX("#FFA69885");
                            T[$"Coin Descs {i} Button"].BorderBrush = РазноеДругое.GetColorFromAHEX("#FF6B6B6B");
                            T[$"Coin Descs {i} Button"].IsEnabled = true;
                            if (!Skills_EditBuffer[Skills_Json_Dictionary_CurrentID][Skills_Json_Dictionary_CurrentUptieLevel]["Coins"][Skills_CurrentCoinNumber][i-1].Equals("{unedited}"))
                            {
                                T[$"Coin Descs {i} Button"].Content = $"№{i}*";
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Подсветить кнопку текущего редактируемого описания монеты
        /// </summary>
        public static void SetCurrentCoinDescHighlight(int DescIndex, int DescsCount, List<int> EmptydescExceptions = null)
        {
            try
            {
                rin(DescIndex + 1);
                // Отключить подсветку для всех остальных
                for (int i = 1; i <= DescsCount; i++)
                {
                    if (!EmptydescExceptions.Contains(i))
                    {
                        T[$"Coin Descs {i} Button"].BorderBrush = РазноеДругое.GetColorFromAHEX("#FF6B6B6B");
                    }
                }
                // Включить для выбранного
                T[$"Coin Descs {DescIndex+1} Button"].BorderBrush = РазноеДругое.GetColorFromAHEX("#FFD1CDC5");
            }
            catch { }
        }
    }
}
