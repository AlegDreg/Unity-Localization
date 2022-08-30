using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public static class Localization
{
    private static string LocalizationPath;//Application.streamingAssetsPath + @"\localization.txt";
    private static string LanguagesPath;// Application.streamingAssetsPath + @"\langs.txt";

    private static List<LocalizationModel> localization = new List<LocalizationModel>();
    private static List<Languages> Languages = new List<Languages>();

    private const string Undefinded = "Undefinded";

    /// <summary>
    /// Получить список доступных языков
    /// </summary>
    /// <returns></returns>
    public static List<Languages> GetLanguages()
    {
        return Languages;
    }

    /// <summary>
    /// Инициализировать данные
    /// </summary>
    public static void Initialize(string languagesPath, string localizationPath)
    {
        LocalizationPath = localizationPath;
        LanguagesPath = languagesPath;

        Languages = ReadData<List<Languages>>(LanguagesPath);
        if (Languages == null)
            Languages = new List<Languages>();

        localization = ReadData<List<LocalizationModel>>(LocalizationPath);
        if (localization == null)
            localization = new List<LocalizationModel>();
    }

    /// <summary>
    /// Чтение данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    private static T ReadData<T>(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
        {
            using (StreamWriter st = new StreamWriter(path))
            {
                string json = "";
                json = JsonConvert.SerializeObject(path, Formatting.Indented);
                st.Write(json);
            }

            return default(T);
        }

        string res = "";

        using (StreamReader st = new StreamReader(path))
        {
            res = st.ReadToEnd();
        }

        if (res == null || res == "" || res == "{}")
            return default(T);

        return JsonConvert.DeserializeObject<T>(res);
    }

    /// <summary>
    /// Получить язык по id (RU,EN ..)
    /// </summary>
    /// <param name="langId"></param>
    /// <returns></returns>
    public static Languages GetLanguage(string langId)
    {
        for (int i = 0; i < Languages.Count; i++)
        {
            if (Languages[i].LangId == langId)
                return Languages[i];
        }

        return null;
    }

    /// <summary>
    /// Добавить язык. Использовать для создания шаблона
    /// </summary>
    /// <param name="model"></param>
    public static void AddLanguages(Languages languages)
    {
        Languages.Add(languages);
        string text = JsonConvert.SerializeObject(Languages, Formatting.Indented);

        using (StreamWriter st = new StreamWriter(LanguagesPath))
        {
            st.Write(text);
        }
    }

    /// <summary> 
    /// Добавить текст. Использовать для создания шаблона
    /// </summary>
    /// <param name="model"></param>
    public static void AddText(LocalizationModel model)
    {
        localization.Add(model);
        string text = JsonConvert.SerializeObject(localization, Formatting.Indented);

        using (StreamWriter st = new StreamWriter(LocalizationPath))
        {
            st.Write(text);
        }
    }

    /// <summary>
    /// Добавить текст с локализацией
    /// </summary>
    /// <param name="obj">Объект</param>
    /// <param name="textName">Имя текста</param>
    /// <param name="param">Дополнительный параметр, добавляемый к результату локализации</param>
    /// <param name="paramFirst">Параметр вначале?</param>
    public static void GetLocalizationText(this TextMeshProUGUI obj, string textName,
        string param = null, bool paramFirst = false)
    {
        var info = localization.Where(x => x.Name == textName).FirstOrDefault();

        if (info == null)
            obj.text = Undefinded;

        //if (GlobalSettings.UserData.CurrentLanguage == null)
        //{
        //    GlobalSettings.UserData = DataManager.ReadData<UserData>();
        //}

        for (int i = 0; i < info.Info.Count; i++)
        {
            if (info.Info[i].Lang.LangId == GlobalSettings.UserData.CurrentLanguage.LangId)
            {
                if (!paramFirst)
                    obj.text = info.Info[i].Text + param;
                else
                    obj.text = param + info.Info[i].Text;

                return;
            }
        }

        obj.text = Undefinded;
    }

    /// <summary>
    /// Добавить текст с локализацией
    /// </summary>
    /// <param name="obj">Объект</param>
    /// <param name="textName">Имя текста</param>
    /// <param name="language">Язык</param>
    public static void GetLocalizationText(this TextMeshProUGUI obj, string textName, Languages language)
    {
        var info = localization.Where(x => x.Name == textName).FirstOrDefault();

        if (info == null)
            obj.text = Undefinded;

        for (int i = 0; i < info.Info.Count; i++)
        {
            if (info.Info[i].Lang.LangId == language.LangId)
            {
                obj.text = info.Info[i].Text;

                return;
            }
        }

        obj.text = Undefinded;
    }
}

public class Languages
{
    public string LangName { get; set; }
    public string LangId { get; set; }
}

public class LocalizationModel
{
    public string Name { get; set; }
    public List<TextInfo> Info { get; set; }
}

public class TextInfo
{
    [JsonIgnore]
    private string langId;
    public string LangId
    {
        get => langId;
        set
        {
            langId = value;
            lang = Localization.GetLanguages().Where(x => x.LangId == value).First();
        }
    }
    [JsonIgnore]
    private Languages lang;
    [JsonIgnore]
    public Languages Lang
    {
        get => lang;
        set
        {
            lang = value;
            LangId = value.LangId;
        }
    }
    public string Text { get; set; }
}

