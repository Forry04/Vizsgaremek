using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameObject BackToMenuUiObject;
    public PlayerInputHandler playerInput;
    public AudioManager audioManager =>AudioManager.Instance;
    public SettingsManager settingsmanager => SettingsManager.Instance;

    private ConfirmationPopup confirmationPopup;

    private VisualElement settingsMenuUi;

    private VisualElement MainContent;

    private VisualElement contentContent;

    private VisualElement soundContent;
    private VisualElement controlsContent;
    private VisualElement graphicsContent;
    private VisualElement activeContent;

    private Button backButton;
    private Button soundButton;
    private Button controlsButton;
    private Button graphicsButton;
    private Button saveButton;
    private Button resetButton;

    private DropdownField resDropDown;
    private DropdownField displayDropDown;

    private Slider MasterVolumeSlider;
    private Slider MusicSlider;
    private Slider SESlider;
    private Slider MouseSlider;
    private Slider GamePadSlider;
    private Slider VibrationIntensitylider;


    private Toggle MuteAllToggle;
    private Toggle invertXToggle;
    private Toggle invertYToggle;
    private Toggle SpatialVibrationToggle;

    List<Button> Buttons;
    List<Slider> Sliders;
    List<Toggle> Toggles;
    List<DropdownField> DropDownFields;

    List<string> displayModes = new() { "Fullscreen", "Windowed", "Borderless" };
    string GetFriendlyName(FullScreenMode mode)
    {
        return mode switch
        {
            FullScreenMode.ExclusiveFullScreen => "Fullscreen",
            FullScreenMode.FullScreenWindow => "Borderless",
            FullScreenMode.Windowed => "Windowed",
            _ => "Windowed"
        };
    }
    FullScreenMode ParseDisplayMode(string choice)
    {
        return choice switch
        {
            "Fullscreen" => FullScreenMode.ExclusiveFullScreen,
            "Borderless" => FullScreenMode.FullScreenWindow,
            "Windowed" => FullScreenMode.Windowed,
            _ => FullScreenMode.Windowed
        };
    }

    

    private void OnEnable()
    {
        //Getting the elements of the UXML
        GettingElements();
        
        Buttons = GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList();
        Sliders = GetComponent<UIDocument>().rootVisualElement.Query<Slider>().ToList();
        Toggles = GetComponent<UIDocument>().rootVisualElement.Query<Toggle>().ToList();
        DropDownFields = GetComponent<UIDocument>().rootVisualElement.Query<DropdownField>().ToList();

        SetDropDownLists();
        SetSliderIntervals();


        backButton.clicked += OnBackClicked;
        soundButton.clicked += OnSoundClicked;
        controlsButton.clicked += OnControlsClicked;
        graphicsButton.clicked += OnGraphicsClicked;
        saveButton.clicked += OnSaveClicked;
        resetButton.clicked += OnResetClicked;

        MasterVolumeSlider.RegisterValueChangedCallback(evt => OnMasterVolumeChanged(evt.newValue));
        MusicSlider.RegisterValueChangedCallback(evt => OnMusicChanged(evt.newValue));
        SESlider.RegisterValueChangedCallback(evt => OnSEChanged(evt.newValue));
        MouseSlider.RegisterValueChangedCallback(evt => OnMouseChanged(evt.newValue));
        GamePadSlider.RegisterValueChangedCallback(evt => OnGamepadChanged(evt.newValue));
        VibrationIntensitylider.RegisterValueChangedCallback(evt => OnVibrationIntensityChanged(evt.newValue));

        resDropDown.RegisterValueChangedCallback(evt => OnResChanged(evt.newValue));
        displayDropDown.RegisterValueChangedCallback(evt => OnDisplayChanged(evt.newValue));

        MuteAllToggle.RegisterValueChangedCallback(evt => OnMuteAllChanged(evt.newValue));
        invertXToggle.RegisterValueChangedCallback(evt => OnInvertXChanged(evt.newValue));
        invertYToggle.RegisterValueChangedCallback(evt => OnInvertYChanged(evt.newValue));
        SpatialVibrationToggle.RegisterValueChangedCallback(evt => OnSpatialVibrationChanged(evt.newValue));
        
        soundContent.style.display = DisplayStyle.Flex;
        controlsContent.style.display = DisplayStyle.None;
        graphicsContent.style.display = DisplayStyle.None;
        activeContent = soundContent;


        //Assign element sounds
        AssignButtonSounds();
        AssignSliderSounds();
        AssignToggleSounds();
        AssignDropDownSounds();

        confirmationPopup = gameObject.AddComponent<ConfirmationPopup>();
        confirmationPopup.Initialize(settingsMenuUi);

        //Set setting values to ui elemnts
        StartCoroutine(DelayedSettingsSetup());

        //settingsMenuUi.MarkDirtyRepaint();
        soundButton.Focus();
    }

    private void OnSpatialVibrationChanged(bool newValue)
    {
        settingsmanager.CurrentSettings.spatialVibration = newValue;
    }

    private void OnInvertYChanged(bool newValue)
    {
        settingsmanager.CurrentSettings.invertYAxis = newValue;
    }

    private void OnInvertXChanged(bool newValue)
    {
        settingsmanager.CurrentSettings.invertXAxis = newValue;
    }

    private void OnVibrationIntensityChanged(float newValue)
    {
        settingsmanager.CurrentSettings.gamepadVibrationIntensity = newValue;
    }

    private void OnGamepadChanged(float newValue)
    {
        settingsmanager.CurrentSettings.gamepadSensitivity = newValue;
    }

    private void OnMouseChanged(float newValue)
    {
        settingsmanager.CurrentSettings.mouseSensitivity = newValue;
    }

    private void OnMuteAllChanged(bool newValue)
    {
        settingsmanager.CurrentSettings.MuteAll = newValue;
        if (newValue == true) audioManager.SetMasterVolume(0, 0);    
        else audioManager.SetMasterVolume(settingsmanager.CurrentSettings.musicVolume*settingsmanager.CurrentSettings.masterVolume,
                settingsmanager.CurrentSettings.sfxVolume*settingsmanager.CurrentSettings.sfxVolume);  

    }

    private IEnumerator DelayedSettingsSetup()
    {
        while (settingsmanager == null) yield return null;
        AssignValues();
    }

    private void AssignValues()
    {
        //Sound
        MasterVolumeSlider.value = settingsmanager.CurrentSettings.masterVolume;
        MusicSlider.value = settingsmanager.CurrentSettings.musicVolume;
        SESlider.value = settingsmanager.CurrentSettings.sfxVolume;
        MuteAllToggle.value = settingsmanager.CurrentSettings.MuteAll;

        //Graphics
        displayDropDown.index = System.Array.FindIndex(displayDropDown.choices.ToArray(), d => d == GetFriendlyName(settingsmanager.CurrentSettings.displaymode));

        if (Screen.resolutions.Any(r => r.width == settingsmanager.CurrentSettings.width && r.height == settingsmanager.CurrentSettings.height))
            resDropDown.index = System.Array.FindIndex(resDropDown.choices.ToArray(), r => r == $"{settingsmanager.CurrentSettings.width}x{settingsmanager.CurrentSettings.height}");
        else
        {
            resDropDown.index = -1;
            resDropDown.value = "Custom";
        }

        //Controls
        MouseSlider.value = settingsmanager.CurrentSettings.mouseSensitivity;
        GamePadSlider.value = settingsmanager.CurrentSettings.gamepadSensitivity;
        invertXToggle.value = settingsmanager.CurrentSettings.invertXAxis;
        invertYToggle.value = settingsmanager.CurrentSettings.invertYAxis;
        VibrationIntensitylider.value = settingsmanager.CurrentSettings.gamepadVibrationIntensity;
        SpatialVibrationToggle.value = settingsmanager.CurrentSettings.spatialVibration;
    }
    private void SetDropDownLists()
    {
        foreach (var res in Screen.resolutions)
        {
            resDropDown.choices.Add($"{res.width}x{res.height}");
        }
        displayDropDown.choices = displayModes;
    }

    private void OnDisplayChanged(string newValue)
    {
        settingsmanager.CurrentSettings.displaymode = ParseDisplayMode(newValue);
        if (!Screen.resolutions.Any(r => r.width == settingsmanager.CurrentSettings.width && r.height == settingsmanager.CurrentSettings.height))
        {
            settingsmanager.CurrentSettings.width = Screen.currentResolution.width;
            settingsmanager.CurrentSettings.height = Screen.currentResolution.height;
        }
        SetScreen(settingsmanager.CurrentSettings.width,settingsmanager.CurrentSettings.height,settingsmanager.CurrentSettings.displaymode);
    }

    private void OnResChanged(string newValue)
    {
        string[] res = newValue.Split('x');
        settingsmanager.CurrentSettings.width = int.Parse(res[0]);
        settingsmanager.CurrentSettings.height = int.Parse(res[1]);
        SetScreen(settingsmanager.CurrentSettings.width, settingsmanager.CurrentSettings.height, settingsmanager.CurrentSettings.displaymode);
    }

    private void SetScreen(int width, int height, FullScreenMode mode)
    {
        Screen.SetResolution(width,height,mode);
    }

    private void GettingElements()
    {
        //audioManager = FindObjectOfType<AudioManager>();
        settingsMenuUi = gameObject.GetComponent<UIDocument>().rootVisualElement;

        //Visual Elements
        MainContent = settingsMenuUi.Q<VisualElement>("container");
        soundContent = settingsMenuUi.Q<VisualElement>("SoundContent-element");
        controlsContent = settingsMenuUi.Q<VisualElement>("ControlsContent-element");
        graphicsContent = settingsMenuUi.Q<VisualElement>("GraphicsContent-element");
        contentContent = settingsMenuUi.Q<VisualElement>("contentContent-element");

        //Buttons
        backButton = settingsMenuUi.Q<Button>("back-button");
        soundButton = settingsMenuUi.Q<Button>("sound-button");
        controlsButton = settingsMenuUi.Q<Button>("controls-button");
        graphicsButton = settingsMenuUi.Q<Button>("graphics-button");
        saveButton = settingsMenuUi.Q<Button>("save-button");
        resetButton = settingsMenuUi.Q<Button>("reset-button");

        //Dropdowns
        resDropDown = settingsMenuUi.Q<DropdownField>("resolution-dropdown");
        displayDropDown = settingsMenuUi.Q<DropdownField>("display-dropdown");

        //Sliders
        MasterVolumeSlider = settingsMenuUi.Q<Slider>("MVolume-slider");
        MusicSlider = settingsMenuUi.Q<Slider>("music-slider");
        SESlider = settingsMenuUi.Q<Slider>("SoundE-slider");
        MouseSlider = settingsMenuUi.Q<Slider>("mouse-slider");
        GamePadSlider = settingsMenuUi.Q<Slider>("gamepad-slider");
        VibrationIntensitylider = settingsMenuUi.Q<Slider>("vibrationIntensity-slider");
        
        //Toggles
        MuteAllToggle = settingsMenuUi.Q<Toggle>("MuteAll-toggle");
        invertXToggle = settingsMenuUi.Q<Toggle>("axis_x-toggle");
        invertYToggle = settingsMenuUi.Q<Toggle>("axis_y-toggle");
        SpatialVibrationToggle = settingsMenuUi.Q<Toggle>("spatialvibration-toggle");
    }

    private void SetSliderIntervals()
    {
        foreach (var slider in Sliders)
        {
            slider.lowValue = 0.0f;
            slider.highValue = 1.0f;
        }
        MouseSlider.highValue = 50f;
        GamePadSlider.highValue = 500f;
    }

    private void OnMusicChanged(float newValue)
    {
        settingsmanager.CurrentSettings.musicVolume = newValue;
        if (!settingsmanager.CurrentSettings.MuteAll)audioManager.SetMusicVolume(settingsmanager.CurrentSettings.musicVolume*newValue);
    }

    private void OnSEChanged(float newValue) 
    {
        settingsmanager.CurrentSettings.sfxVolume = newValue;
        if (!settingsmanager.CurrentSettings.MuteAll) audioManager.SetSEVolume(settingsmanager.CurrentSettings.sfxVolume*newValue);
    }

    private void OnMasterVolumeChanged(float newValue)
    {
        settingsmanager.CurrentSettings.masterVolume = newValue;
        if (!settingsmanager.CurrentSettings.MuteAll) audioManager.SetMasterVolume(settingsmanager.CurrentSettings.musicVolume*newValue,
           settingsmanager.CurrentSettings.sfxVolume*newValue);
    }

    private void AssignButtonSounds()
    {
        foreach (var button in Buttons)
        {
            button.RegisterCallback<MouseEnterEvent>(evt =>
            {

                audioManager.Play("ButtonHover");
            });

            button.RegisterCallback<FocusEvent>(evt =>
            {

                audioManager.Play("ButtonHover");
            });

            button.RegisterCallback<ClickEvent>(evt =>
            {
                //PlayClickSound();
            });
        }
    }

    private void AssignSliderSounds()
    {
        foreach (var button in Sliders)
        {
            button.RegisterCallback<MouseEnterEvent>(evt =>
            {

                audioManager.Play("ElementHover");
            });

            button.RegisterCallback<FocusEvent>(evt =>
            {

                audioManager.Play("ElementHover");
            });

            button.RegisterCallback<ClickEvent>(evt =>
            {
                //PlayClickSound();
            });
        }
    }

    private void AssignToggleSounds()
    {
        foreach (var button in Toggles)
        {
            button.RegisterCallback<MouseEnterEvent>(evt =>
            {

                audioManager.Play("ElementHover");
            });

            button.RegisterCallback<FocusEvent>(evt =>
            {

                audioManager.Play("ElementHover");
            });

            button.RegisterCallback<ClickEvent>(evt =>
            {
                //PlayClickSound();
            });
        }
    }

    private void AssignDropDownSounds()
    {
        foreach (var button in DropDownFields)
        {
            button.RegisterCallback<MouseEnterEvent>(evt =>
            {

                audioManager.Play("ElementHover");
            });

            button.RegisterCallback<FocusEvent>(evt =>
            {

                audioManager.Play("ElementHover");
            });

            button.RegisterCallback<ClickEvent>(evt =>
            {
                //PlayClickSound();
            });
        }
    }

    private void OnDisable()
    {
        backButton.clicked -= OnBackClicked;
        soundButton.clicked -= OnSoundClicked;
        controlsButton.clicked -= OnControlsClicked;
        graphicsButton.clicked -= OnGraphicsClicked;
        saveButton.clicked -= OnSaveClicked;
        resetButton.clicked -= OnResetClicked;
    }
    private void OnResetClicked()
    {
        confirmationPopup.Show(
            "Are you sure you want reset your settings?",
            onConfirm: () =>
            {
                settingsmanager.ResetToDefaults();
                AssignValues();
            },
            onCancel: () =>
            {
                // Do nothing on cancel
            });
    }

    private void OnSaveClicked()
    {
        settingsmanager.SaveSettings();
    }

    private void SwitchContent(VisualElement content)
    {
        if (content.style.display == DisplayStyle.None)
        {
            activeContent.style.display = DisplayStyle.None;
            content.style.display = DisplayStyle.Flex;
            activeContent = content;
        }
    }
    private void OnGraphicsClicked()
    {
        SwitchContent(graphicsContent);
    }

    private void OnControlsClicked()
    {
        SwitchContent(controlsContent);
    }

    private void OnSoundClicked()
    {
        SwitchContent(soundContent);
    }

    private void OnBackClicked()
    {
        gameObject.SetActive(false);
        BackToMenuUiObject.SetActive(true);
    }
}
