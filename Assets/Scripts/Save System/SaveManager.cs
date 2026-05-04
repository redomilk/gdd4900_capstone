using System.IO;
using UnityEngine;

public static class SaveManager
{
    static string GetSavePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    public static void Save(SaveData data, int slot)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);
        Debug.Log("Game saved to slot " + slot);
    }

    public static SaveData Load(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.Log("No save found in slot " + slot);
            return new SaveData();
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool SaveExists(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    public static void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);

        if (File.Exists(path))
            File.Delete(path);
    }
}