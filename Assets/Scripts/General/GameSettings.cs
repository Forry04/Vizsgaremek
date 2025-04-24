using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool MuteAll = false;

    public float mouseSensitivity = 1.0f;
    public float gamepadSensitivity = 1.0f;

    public bool invertXAxis = false;
    public bool invertYAxis = false;

    public float gamepadVibrationIntensity = 1.0f;
    public bool spatialVibration = true;

    public int width = 0;
    public int height = 0;
    public FullScreenMode displaymode = FullScreenMode.ExclusiveFullScreen;

    public string CustomSearch = "GG";
    public string CustomRarity = "All";

}