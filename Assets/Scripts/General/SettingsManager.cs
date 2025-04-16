using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public GameSettings CurrentSettings { get; private set; }

    private string settingsFilePath => Path.Combine(Application.persistentDataPath, "settings.json");

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public void LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            CurrentSettings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            CurrentSettings = GetDefaultSettings();
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(CurrentSettings, true);
        File.WriteAllText(settingsFilePath, json);
    }

    public void ResetToDefaults()
    {
        CurrentSettings = GetDefaultSettings();
        SaveSettings();
    }

    private GameSettings GetDefaultSettings()
    {
        return new GameSettings();
    }
}
