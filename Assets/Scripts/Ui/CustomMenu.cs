using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomMenu : MonoBehaviour
{
    public VisualTreeAsset skinCardTemplate;
    [SerializeField] private GameObject BackToMenuUiObject;

    public SettingsManager SettingsManager;
    public PreviewManager PreviewManager;

    private VisualElement customMenuUi;
    private TextField searchBarTextField;
    private DropdownField rarityDropdown;
    private VisualElement gridContainerElement;
    //searchBarTextField.SetValueWithoutNotify(SettingsManager.CurrentSettings.CustomSearch);
    private Button backButton;

    List<string> rarities = new() { "All", "Common", "Rare", "Epic", "Legendary" };

    public List<Skindata> skinList;

    private void OnEnable()
    {
        Debug.Log("Custom ONenable!");
        GettingElements();

        backButton.clicked += BackButtonClicked;

        customMenuUi.RegisterCallback<GeometryChangedEvent>(evt => {
            WaitAndAdjustCards();
        });
        searchBarTextField.RegisterValueChangedCallback(evt => OnSearchBarChanged(evt.newValue));
        rarityDropdown.RegisterValueChangedCallback(evt => OnRarityChanged(evt.newValue));
        skinList = Resources.LoadAll<Skindata>("Skins").ToList();
        StartCoroutine(DelayedAssignValues());
    }
    private IEnumerator DelayedAssignValues()
    {
        while (SettingsManager.Instance == null || PreviewManager.instance == null) {
        }
        yield return null;
        SettingsManager = SettingsManager.Instance;
        PreviewManager = PreviewManager.instance;

        AssignValues();
        PopulateSkins();
    }

    private void OnRarityChanged(string newValue)
    {

    }

    private void OnSearchBarChanged(string newValue)
    {

    }

    private void GettingElements()
    {
        customMenuUi = gameObject.GetComponent<UIDocument>().rootVisualElement;
        searchBarTextField = customMenuUi.Q<TextField>("Search-textfield");
        rarityDropdown = customMenuUi.Q<DropdownField>("Rarity-dropdown");
        gridContainerElement = customMenuUi.Q<VisualElement>("gridcontainer-element");
        backButton = customMenuUi.Q<Button>("Back-button");
        rarityDropdown.choices = rarities;
    }
    private void AssignValues()
    {
        searchBarTextField.value = SettingsManager.CurrentSettings.CustomSearch;
        rarityDropdown.index = rarities.FindIndex(r => r == SettingsManager.CurrentSettings.CustomRarity);
        skinList = Resources.LoadAll<Skindata>("Skins").ToList();
    }
    private void BackButtonClicked()
    {
        gameObject.SetActive(false);
        BackToMenuUiObject.SetActive(true);
    }

    void PopulateSkins()
    {
       gridContainerElement.Clear();
        foreach (var skin in skinList)
        {
            var card = skinCardTemplate.Instantiate();
            var cardMaterial = skin.skinMaterial;
            var renderVisualElement = card.Q<VisualElement>("Render-element");
            card.Q<Label>("SkinName").text = skin.skinName;
            card.Q<Button>("EquipButton").clicked += () => EquipSkin();
            renderVisualElement.style.backgroundImage = new StyleBackground(skin.previewImage);

            card.RegisterCallback<MouseEnterEvent>(evt => OnCardHovered(cardMaterial,renderVisualElement));
            card.RegisterCallback<MouseLeaveEvent>(evt => OnCardExited(renderVisualElement, skin.previewImage));

            gridContainerElement.Add(card);
        }
        gridContainerElement[0].Focus();
        gridContainerElement.schedule.Execute(WaitAndAdjustCards).Every(1);
    }

    private void OnCardExited(VisualElement targetElement, Sprite previeImage)
    {
        targetElement.style.backgroundImage = new StyleBackground(previeImage);
        PreviewManager.ClosePreview();
    }

    private void OnCardHovered(Material material, VisualElement targetElement)
    {
        PreviewManager.ShowPreview(material,targetElement);
    }

    private void WaitAndAdjustCards()
    {
        float containerWidth = gridContainerElement.resolvedStyle.width;
        if (containerWidth <= 0)
            return;

        AdjustCards();
        gridContainerElement.schedule.Execute(WaitAndAdjustCards).Pause();
    }
    private void AdjustCards()
    {
        //float x = gridContainerElement.resolvedStyle.width / (4 + 5 * gap);
        //foreach (var card in gridContainerElement.Children())
        //{
        //    card.style.width = x;
        //    card.style.height = x * 1.5f;
        //    card.style.marginRight = x * gap;
        //    card.style.marginBottom = x * gap;
        //}
        //gridContainerElement.style.paddingLeft = x * gap;

        float containerWidth = gridContainerElement.resolvedStyle.width;
        float minCardWidth = 400f; // whatever minimum you want for a card
        float gapValue = 15f; // absolute pixel gap

        int columns = Mathf.FloorToInt((containerWidth + gapValue) / (minCardWidth + gapValue));
        columns = Mathf.Max(columns, 1); // avoid zero

        float cardWidth = (containerWidth - (columns + 1) * gapValue) / columns;

        foreach (var card in gridContainerElement.Children())
        {
            card.style.width = cardWidth;
            card.style.height = cardWidth * 1.5f;
            card.style.marginRight = gapValue;
            card.style.marginBottom = gapValue;
        }

        gridContainerElement.style.paddingLeft = gapValue;

    }

    private void EquipSkin()
    {

    } 
}
