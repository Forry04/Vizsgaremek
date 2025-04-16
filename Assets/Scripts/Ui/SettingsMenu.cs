using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameObject BackToMenuUiObject;
    public PlayerInputHandler playerInput;
    public AudioManager audioManager =>AudioManager.Instance;
    public SettingsManager settingsmanager => SettingsManager.Instance;
    private VisualElement settingsMenuUi;

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

    private Toggle MuteAllToggle;

    List<Button> Buttons;
    List<Slider> Sliders;
    List<Toggle> Toggles;
    List<DropdownField> DropDownFields;

    //List<string> resolutions = new() { "1280x720 (720p) – HD", "1920x1080 (1080p) – Full HD", "2560x1440 (1440p) – QHD", "3840x2160 (4K) – Ultra HD" };
    List<string> displayModes = new() { "Fullscreen", "Windowed", "Borderless Windowed" };

    private void OnEnable()
    {
        //Getting the elements of the UXML
        GettingElements();
       
        Buttons = GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList();
        Sliders = GetComponent<UIDocument>().rootVisualElement.Query<Slider>().ToList();
        Toggles = GetComponent<UIDocument>().rootVisualElement.Query<Toggle>().ToList();
        DropDownFields = GetComponent<UIDocument>().rootVisualElement.Query<DropdownField>().ToList();

        backButton.clicked += OnBackClicked;
        soundButton.clicked += OnSoundClicked;
        controlsButton.clicked += OnControlsClicked;
        graphicsButton.clicked += OnGraphicsClicked;
        saveButton.clicked += OnSaveClicked;
        resetButton.clicked += OnResetClicked;
        
        MasterVolumeSlider.RegisterValueChangedCallback(evt => OnMasterVolumeChanged(evt.newValue));
        MusicSlider.RegisterValueChangedCallback(evt => OnMusicChanged(evt.newValue));
        SESlider.RegisterValueChangedCallback(evt => OnSEChanged(evt.newValue));

        soundContent.style.display = DisplayStyle.Flex;
        controlsContent.style.display = DisplayStyle.None;
        graphicsContent.style.display = DisplayStyle.None;
        activeContent = soundContent;


        foreach (var res in Screen.resolutions)
        {
            resDropDown.choices.Add($"{res.width}x{res.height} {res.refreshRateRatio}Hz");
        }
        //resDropDown.choices = resolutions;

        displayDropDown.choices = displayModes;
        resDropDown.index = System.Array.FindIndex(Screen.resolutions, r=>
        r.width == Screen.currentResolution.width &&
        r.height == Screen.currentResolution.height);
        displayDropDown.index = 0;


        //Assign element sounds
        AssignButtonSounds();
        AssignSliderSounds();
        AssignToggleSounds();
        AssignDropDownSounds();

        //SetCurrentValues
        SetCurrentValues();

        //Setting séiders interval
        SetSliderIntervals();

        //settingsMenuUi.MarkDirtyRepaint();
        soundButton.Focus();
    }

    private void GettingElements()
    {
        //audioManager = FindObjectOfType<AudioManager>();
        settingsMenuUi = gameObject.GetComponent<UIDocument>().rootVisualElement;

        //Visual Elements
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

        //Toggles
        MuteAllToggle = settingsMenuUi.Q<Toggle>("MuteAll-toggle");
    }

    private void SetCurrentValues()
    {
        if (settingsmanager == null)
        {
            Debug.LogWarning("SettingsManager is null in SetCurrentValues");
            return;
        }

        Debug.Log($"Vlaue to be set: {settingsmanager.CurrentSettings.masterVolume}");
        MasterVolumeSlider.value = settingsmanager.CurrentSettings.masterVolume;
        MusicSlider.value = settingsmanager.CurrentSettings.musicVolume;
        SESlider.value = settingsmanager.CurrentSettings.sfxVolume;

    }

    private void SetValues()
    {

    }
    private void SetSliderIntervals()
    {
        foreach (var slider in Sliders)
        {
            slider.lowValue = 0.0f;
            slider.highValue = 1.0f;
        }

    }

    private void OnMusicChanged(float newValue)
    {
        settingsmanager.CurrentSettings.musicVolume = newValue;
        audioManager.SetMusicVolume(newValue);
    }

    private void OnSEChanged(float newValue)
    {
        settingsmanager.CurrentSettings.sfxVolume = newValue;
        audioManager.SetSEVolume(newValue);
    }

    private void OnMasterVolumeChanged(float newValue)
    {
        settingsmanager.CurrentSettings.masterVolume = newValue;
        audioManager.SetMusicVolume(newValue);
        audioManager.SetSEVolume(newValue);
        Debug.Log($"Cahnged Value: {settingsmanager.CurrentSettings.masterVolume}");
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

    }

    private void OnSaveClicked()
    {

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
