using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public GameSettings CurrentSettings { get; private set; }
    public AudioManager audioManager;

    private string settingsFilePath => Path.Combine(Application.persistentDataPath, "settings.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
        ApplySettings();
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;

        //Apply Audio after instances
        //if (CurrentSettings.MuteAll) audioManager.SetMasterVolume(0, 0);     
        //else audioManager.SetMasterVolume(CurrentSettings.musicVolume*CurrentSettings.masterVolume,
        //        CurrentSettings.sfxVolume*CurrentSettings.masterVolume);
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
        GameSettings defaultSettings = new GameSettings();
        defaultSettings.width = Screen.currentResolution.width;
        defaultSettings.height = Screen.currentResolution.height;
        return defaultSettings;
    }

    private void ApplySettings()
    {
        Screen.SetResolution(CurrentSettings.width,CurrentSettings.height,CurrentSettings.displaymode);
    }


}
