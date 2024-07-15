using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SettingFileSaver
{
    private static string persistancePath = Application.persistentDataPath;
    private const string FileName = "Settings";
    private const string Suffix = ".save";

    private static SettingSaveData _settingSaveData = new();
    private static BinaryFormatter _converter = new BinaryFormatter();

    private static SettingSaveData Clone(SettingSaveData saveData)
    {
        byte[] bytes = ObjectToByteArray(saveData);
        SettingSaveData clone = ByteArrayToObject(bytes);
        return clone;
    }

    private static byte[] ObjectToByteArray(SettingSaveData obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    private static SettingSaveData ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        SettingSaveData obj = (SettingSaveData)binForm.Deserialize(memStream);

        return obj;
    }

    public static float GetFloat(string key, float defaultValue)
    {
        if (_settingSaveData.SettingFloats.TryGetValue(key, out float value)) return value;

        return defaultValue;
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        if (_settingSaveData.SettingBools.TryGetValue(key, out bool value)) return value;

        return defaultValue;
    }

    public static int GetInt(string key, int defaultValue)
    {
        if (_settingSaveData.SettingInts.TryGetValue(key, out int value)) return value;

        return defaultValue;
    }
    public static void SetFloat(string key, float value)
    {
        _settingSaveData.SettingFloats[key] = value;
    }

    public static void SetBool(string key, bool value)
    {
        _settingSaveData.SettingBools[key] = value;
    }

    public static void SetInt(string key, int value)
    {
        _settingSaveData.SettingInts[key] = value;
    }

    public static bool SaveToFile()
    {
        string path = GetFilePath(FileName);

        if (string.IsNullOrEmpty(path)) return false;

        try
        {
            FileStream dataStream = new FileStream(path, FileMode.Create);

            dataStream.Seek(0, SeekOrigin.Begin);
            _converter.Serialize(dataStream, _settingSaveData);
            dataStream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("An error occured during saving file: " + e.ToString());
            return false;
        }

        return true;
    }

    public static void LoadFromFile()
    {
        string path = GetFilePath(FileName);
        if (File.Exists(path))
        {
            try
            {
                FileStream dataStream = new FileStream(path, FileMode.Open);
                SettingSaveData saveData = _converter.Deserialize(dataStream) as SettingSaveData;
                dataStream.Close();

                _settingSaveData = saveData;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
        }
    }

    private static string GetFilePath(string fileName)
    {
        string path = persistancePath;
        path = Path.Combine(path, fileName);
        CheckIfFolderExist(path);

        path = Path.Combine(path, Path.ChangeExtension(fileName, Suffix));

        return path;
    }

    private static void CheckIfFolderExist(string path)
    {
        if (!Directory.Exists(path))
        {
            if (path != null)
                Debug.LogWarning($"Directory: \"{path}\" DOES NOT EXIST. Creating directory...");

            Directory.CreateDirectory(path);
        }
    }
}

[Serializable]
public class SettingSaveData
{
    public Dictionary<string, float> SettingFloats = new();
    public Dictionary<string, int> SettingInts = new();
    public Dictionary<string, bool> SettingBools = new();
}