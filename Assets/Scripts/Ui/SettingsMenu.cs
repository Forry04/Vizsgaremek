using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameObject PauseMenuUiObject;
    PlayerInputHandler playerInput;

    private VisualElement settingsMenuUi;

    private VisualElement contentContent;

    private VisualElement soundContent;
    private VisualElement controlsContent;
    private VisualElement graphicsContent;

    private Button backButton;
    private Button soundButton;
    private Button controlsButton;
    private Button graphicsButton;
    private Button saveButton;
    private Button resetButton;
    private VisualElement activeContent;
    private DropdownField resDropDown;
    private DropdownField displayDropDown;

    private void OnEnable()
    {
        //playerInput = Chat.Singleton.playerInput;
        //playerInput = GameObject.Find("Player").GetComponent<PlayerInputHandler>();
        settingsMenuUi = gameObject.GetComponent<UIDocument>().rootVisualElement;

        soundContent = settingsMenuUi.Q<VisualElement>("SoundContent-element");
        controlsContent = settingsMenuUi.Q<VisualElement>("ControlsContent-element");
        graphicsContent = settingsMenuUi.Q<VisualElement>("GraphicsContent-element");
        contentContent = settingsMenuUi.Q<VisualElement>("contentContent-element");

        backButton = settingsMenuUi.Q<Button>("back-button");
        soundButton = settingsMenuUi.Q<Button>("sound-button");
        controlsButton = settingsMenuUi.Q<Button>("controls-button");
        graphicsButton = settingsMenuUi.Q<Button>("graphics-button");
        saveButton = settingsMenuUi.Q<Button>("save-button");
        resetButton = settingsMenuUi.Q<Button>("reset-button");

        resDropDown = settingsMenuUi.Q<DropdownField>("resolution-dropdown");
        displayDropDown = settingsMenuUi.Q<DropdownField>("display-dropdown");

        backButton.clicked += OnBackClicked;
        soundButton.clicked += OnSoundClicked;
        controlsButton.clicked += OnControlsClicked;
        graphicsButton.clicked += OnGraphicsClicked;
        saveButton.clicked += OnSaveClicked;
        resetButton.clicked += OnResetClicked;

        soundContent.style.display = DisplayStyle.Flex;
        controlsContent.style.display = DisplayStyle.None;
        graphicsContent.style.display = DisplayStyle.None;
        activeContent = soundContent;

        List<string> resolutions = new() { "1280x720 (720p) – HD", "1920x1080 (1080p) – Full HD", "2560x1440 (1440p) – QHD", "3840x2160 (4K) – Ultra HD" };
        List<string> displayModes = new() { "Fullscreen" , "Windowed", "Borderless Windowed" };

        resDropDown.choices = resolutions;
        displayDropDown.choices = displayModes;
        resDropDown.index = 1;
        displayDropDown.index = 0;

        

        settingsMenuUi.MarkDirtyRepaint();
        soundButton.Focus();
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
        PauseMenuUiObject.SetActive(true);
    }
}
