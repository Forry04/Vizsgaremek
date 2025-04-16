using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public float masterVolume = 1f;
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;

    public float mouseSensitivity = 1.0f;
    public float gamepadSensitivity = 1.0f;

    public bool invertXAxis = false;
    public bool invertYAxis = false;

    public float gamepadVibrationIntensity = 1.0f;
    public bool spatialVibration = true;
}
