using LitJson;
using System.IO;
using UnityEngine;

public static class JsonManager
{
    public enum JsonType
    {
        JsonUtility,
        LitJson
    }

    public enum JsonLocation
    {
        PersistentDataPath,
        Resources
    }

    public static void Save(object data, string fileName, JsonType jsonType)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string jsonStr;

        if (jsonType == JsonType.JsonUtility)
            jsonStr = JsonUtility.ToJson(data);
        else if (jsonType == JsonType.LitJson)
            jsonStr = JsonMapper.ToJson(data);
        else
        {
            Debug.LogError("使用了未知的JsonType");
            return;
        }

        File.WriteAllText(path, jsonStr);
    }

    public static T Load<T>(string fileName, JsonType jsonType,
        JsonLocation location = JsonLocation.PersistentDataPath)
    {
        string jsonStr;

        if (location == JsonLocation.Resources)
        {
            string resourcePath = Path.ChangeExtension(fileName, null);
            var textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset == null)
            {
                Debug.LogWarning($"Resources中未找到: {resourcePath}");
                return default;
            }
            jsonStr = textAsset.text;
        }
        else
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"文件不存在: {path}");
                return default;
            }
            jsonStr = File.ReadAllText(path);
        }

        if (jsonType == JsonType.JsonUtility)
            return JsonUtility.FromJson<T>(jsonStr);
        else if (jsonType == JsonType.LitJson)
            return JsonMapper.ToObject<T>(jsonStr);

        return default;
    }
}
