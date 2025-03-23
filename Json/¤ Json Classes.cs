using Newtonsoft.Json;

namespace Limbus_Localization_UI.Json
{
    // Основная структура json файлов
    public class JsonData
    {
        public List<Data> dataList { get; set; }
    }
    public class Data
    {
        public dynamic id { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public string description { get; set; }
        public string desc { get; set; }
        public string summary { get; set; } // Суммарное описание пассивки, появляется при ударе под навыком
        public string undefined { get; set; } // Что-то в Bufs файлах
        public List<SimpleDesc> simpleDesc { get; set; } // Для ЭГО даров
        public List<levelList> levelList { get; set; } // Для навыков
    }

    // Простые описания эго даров
    public class SimpleDesc
    {
        public string simpleDesc { get; set; } // Внутри SimpleDesc
        //public int abilityID { get; set; }   // ID простого описания?
    }

    // Навыки
    public class levelList // (Уровни связи)
    {
        public int level { get; set; } // (Уровень связи)
        public string name { get; set; }
        public List<string> keywords { get; set; }
        public string abName { get; set; } // Фоновое имя ЭГО
        public string desc { get; set; }
        public List<coinlist> coinlist { get; set; } // Список монет
    }
    public class coinlist // Список монет
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] // Пустая монета = "{},", (Не заменять на null)
        public List<coindescs> coindescs { get; set; } // Список построчных описаний монеты
    }
    public class coindescs
    {
        public string desc { get; set; } // Элемент списка описания монеты (Странно, что не единая строка с переносами через \n)
    }
}
