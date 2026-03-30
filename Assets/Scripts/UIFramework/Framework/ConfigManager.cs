using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ConfigManager : Singleton<ConfigManager>
{
    public ConfigInfo ConfigInfo { get; private set; }
    public UserData UserData { get; private set; }

    // 入口改成协程，方便异步加载
    public IEnumerator Initial()
    {
        yield return LoadConfig();      // 加载只读配置
        LoadUserData();                // 加载可读写的用户数据
    }

    // 读取 streamingAssets 里的 config.json（只读）
    IEnumerator LoadConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");

        UnityWebRequest www;
        if (path.Contains("://") || path.Contains(":///"))
        {
            // Android / WebGL 等平台
            www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("加载 config.json 失败: " + www.error);
                yield break;
            }

            string json = www.downloadHandler.text;
            ConfigInfo = JsonConvert.DeserializeObject<ConfigInfo>(json);
        }
        else
        {
            // PC / Mac / Editor 等
            if (!File.Exists(path))
            {
                Debug.LogError("找不到 config.json: " + path);
                yield break;
            }

            string json = File.ReadAllText(path);
            ConfigInfo = JsonConvert.DeserializeObject<ConfigInfo>(json);
        }
    }

    // 读取 persistentDataPath 里的 userdata.json（可读写）
    void LoadUserData()
    {
        string path = Path.Combine(Application.persistentDataPath, "userdata.json");
        
        if (!File.Exists(path))
        {
            Debug.Log("不存在存档，使用默认 UserData");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            UserData = JsonConvert.DeserializeObject<UserData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("读取 userdata.json 出错: " + ex.Message);
        }
    }

    // 保存 UserData 到可读写目录
    public void SaveUserData(UserData userData)
    {
        UserData = userData;

        string path = Path.Combine(Application.persistentDataPath, "userdata.json");
        try
        {
            string json = JsonConvert.SerializeObject(userData, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("保存 userdata.json 出错: " + ex.Message);
        }
    }
}