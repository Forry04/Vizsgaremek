using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Scrollbar;

public class CustomMenu : MonoBehaviour
{
    public VisualTreeAsset skinCardTemplate;
    [SerializeField] private GameObject BackToMenuUiObject;
    public AudioManager audiomanager => AudioManager.Instance;

    public SettingsManager SettingsManager;
    public PreviewManager PreviewManager;

    private VisualElement customMenuUi;
    private TextField searchBarTextField;
    private DropdownField rarityDropdown;
    private VisualElement gridContainerElement;
    private ScrollView scrollElement;

    
    //searchBarTextField.SetValueWithoutNotify(SettingsManager.CurrentSettings.CustomSearch);
    private Button backButton;

    List<string> rarities = new() { "All", "Common", "Rare", "Epic", "Legendary" };

    public List<Skindata> skinList;

    private void OnEnable()
    {
        GettingElements();

        backButton.clicked += BackButtonClicked;

        customMenuUi.RegisterCallback<GeometryChangedEvent>(evt => {
            WaitAndAdjustCards();
        });
        searchBarTextField.RegisterValueChangedCallback(evt => OnSearchBarChanged(evt.newValue));
        rarityDropdown.RegisterValueChangedCallback(evt => OnRarityChanged(evt.newValue));


        backButton.RegisterCallback<MouseEnterEvent>(evt => {
            audiomanager.Play("ButtonHover");
        });
        backButton.RegisterCallback<FocusEvent>(evt => {
            audiomanager.Play("ButtonHover");
        });

        StartCoroutine(DelayedAssignValues());
        backButton.Focus();

        //change scroll speed
        scrollElement.RegisterCallback<WheelEvent>(e => {
            float scrollSpeedMultiplier = 250.0f;
            scrollElement.scrollOffset += new Vector2(0, e.delta.y * scrollSpeedMultiplier);
            e.StopPropagation();
        }, TrickleDown.TrickleDown);
    }
    private IEnumerator DelayedAssignValues()
    {
        while (SettingsManager.Instance == null || PreviewManager.instance == null) {
        }
        yield return null;
        SettingsManager = SettingsManager.Instance;
        PreviewManager = PreviewManager.instance;

        AssignValues();
        PopulateSkins(skinList);
    }

    private void OnRarityChanged(string newValue)
    {
        Filter(newValue,searchBarTextField.text);      
    }

    private void Filter(string rarity, string seacrch)
    {
        if (rarityDropdown.value == "All") PopulateSkins(skinList.Where(s => s.skinName.Contains(seacrch, StringComparison.OrdinalIgnoreCase)).ToList());
        else PopulateSkins(skinList.Where(s => s.skinName.Contains(seacrch, StringComparison.OrdinalIgnoreCase) && s.rarity.ToString() == rarity).ToList());
    }
    private void OnSearchBarChanged(string newValue)
    {
        Filter(rarityDropdown.value,newValue);
    }

    private void GettingElements()
    {
        customMenuUi = gameObject.GetComponent<UIDocument>().rootVisualElement;
        searchBarTextField = customMenuUi.Q<TextField>("Search-textfield");
        rarityDropdown = customMenuUi.Q<DropdownField>("Rarity-dropdown");
        gridContainerElement = customMenuUi.Q<VisualElement>("gridcontainer-element");
        backButton = customMenuUi.Q<Button>("Back-button");
        scrollElement = customMenuUi.Q<ScrollView>("Content-scrollview");
        rarityDropdown.choices = rarities;
    }
    private void AssignValues()
    {
        rarityDropdown.index = rarities.FindIndex(r => r == "All");
        skinList = Resources.LoadAll<Skindata>("Skins").ToList();
    }
    private void BackButtonClicked()
    {
        gameObject.SetActive(false);
        BackToMenuUiObject.SetActive(true);
    }

    void PopulateSkins(List<Skindata> skinlist)
    {
       gridContainerElement.Clear();
        foreach (var skin in skinlist)
        {
            var card = skinCardTemplate.Instantiate();
            var cardMaterial = skin.skinMaterial;
            var renderVisualElement = card.Q<VisualElement>("Render-element");
            card.Q<Label>("SkinName").text = skin.skinName;
            renderVisualElement.style.backgroundImage = new StyleBackground(skin.previewImage);
            card.Q<VisualElement>("container-element").AddToClassList(skin.rarity.ToString());

            //ha nincs meg
            if (false)
            {
                card.Q<VisualElement>("Locked-element").style.display = DisplayStyle.Flex;
                card.Q<Button>("Equip-button").style.display = DisplayStyle.None;
                card.Q<Button>("Locked-Button").style.display = DisplayStyle.Flex;
                card.Q<Button>("Locked-Button").clicked += () => GetSkin();
            }
            else
                card.Q<Button>("Equip-button").clicked += () => EquipSkin(skin);



            card.RegisterCallback<MouseEnterEvent>(evt => OnCardHovered(cardMaterial,renderVisualElement));
            card.RegisterCallback<MouseLeaveEvent>(evt => OnCardExited(renderVisualElement, skin.previewImage));

            card.RegisterCallback<FocusInEvent>(evt => OnCardFocused(cardMaterial, renderVisualElement));
            card.RegisterCallback<FocusOutEvent>(evt => OnCardExitedFocuse(renderVisualElement, skin.previewImage));
            gridContainerElement.Add(card);
        }
        gridContainerElement.schedule.Execute(WaitAndAdjustCards).Every(1);
    }

    private void OnCardExitedFocuse(VisualElement targetElement, Sprite previeImage)
    {
        targetElement.style.backgroundImage = new StyleBackground(previeImage);
        PreviewManager.ClosePreview();
    }

    private void OnCardFocused(Material cardMaterial, VisualElement renderVisualElement)
    {
        PreviewManager.ShowPreview(cardMaterial, renderVisualElement);
        audiomanager.Play("ElementHover");
    }

    private void GetSkin()
    {
        Application.OpenURL("https://www.youtube.com/");
    }

    private void OnCardExited(VisualElement targetElement, Sprite previeImage)
    {
        targetElement.style.backgroundImage = new StyleBackground(previeImage);
        PreviewManager.ClosePreview();
    }

    private void OnCardHovered(Material material, VisualElement targetElement)
    {
        PreviewManager.ShowPreview(material,targetElement);
        audiomanager.Play("ElementHover");
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
        float containerWidth = gridContainerElement.resolvedStyle.width;
        float minCardWidth = 400f;
        float gapValue = 25f;

        int columns = Mathf.FloorToInt((containerWidth + gapValue) / (minCardWidth + gapValue));
        columns = Mathf.Max(columns, 1);

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

    private void EquipSkin(Skindata skin)
    {
        PlayerPrefs.SetInt("CurrentSkin", skin.skinId);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (!player.TryGetComponent<NetworkObject>(out var PlayerNetworkObJect)) continue;
            if (PlayerNetworkObJect.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                player.GetComponent<SetUpPlayer>().EquipSkinRpc(NetworkManager.Singleton.LocalClientId, skin.skinId);
            }
        }
    }
   
   
    
}
