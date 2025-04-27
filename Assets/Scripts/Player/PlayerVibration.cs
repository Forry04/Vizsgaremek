using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVibration : MonoBehaviour
{
    private SettingsManager settingsmanager;
    public Vibration vibration;
    private GameObject enemy => GameObject.FindGameObjectWithTag("Enemy");
    private bool vibrate => (enemy != null && enemy.GetComponent<EnemyController>().player == gameObject && enemy.GetComponent<EnemyController>().isChasing);
    private bool vibrating = false;
    void Start()
    {
        settingsmanager = SettingsManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (vibrate)
        {
            vibration.Vibrate(settingsmanager.CurrentSettings.gamepadVibrationIntensity, settingsmanager.CurrentSettings.gamepadVibrationIntensity);
            vibrating = true;
        }
        else if (vibrating)
            vibration.StopVibration();
    }
}
