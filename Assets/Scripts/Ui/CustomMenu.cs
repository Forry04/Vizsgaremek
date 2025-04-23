using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomMenu : MonoBehaviour
{
    public VisualTreeAsset skinCardTemplate;

    private VisualElement customMenuUi;
    private TextField searchBarTextField;
    private DropdownField rarityDropdown;
    private VisualElement gridContainerElement;
    private float gap = 0.2f;
    private ScrollView contentScrollView;
    //public List<> allskins;
    private void OnEnable()
    {
        customMenuUi = gameObject.GetComponent<UIDocument>().rootVisualElement;
        searchBarTextField = customMenuUi.Q<TextField>("search-textfield");
        rarityDropdown = customMenuUi.Q<DropdownField>("Rarity-dropdown");
        contentScrollView = customMenuUi.Q<ScrollView>("Content-scrollview");
        gridContainerElement = customMenuUi.Q<VisualElement>("gridcontainer-element");

        PopulateSkins();
    }


    void PopulateSkins()
    {
       gridContainerElement.Clear();
        for (int i = 0; i < 10; i++)
        {
            var card = skinCardTemplate.Instantiate();
            card.Q<Label>("SkinName").text = skinCardTemplate.name;
            card.Q<Button>("EquipButton").clicked += () => EquipSkin();
            gridContainerElement.Add(card);
        }
        gridContainerElement.schedule.Execute(AdjustCards).ExecuteLater(100);

    }

    private void AdjustCards()
    {
        float x = Screen.width / (4+5*gap);
        foreach (var card in gridContainerElement.Children())
        {
            card.style.width = x;
            card.style.height = x * 1.5f;
            card.style.marginRight = x * gap;
            card.style.marginBottom = x * gap;
        }
        gridContainerElement.style.paddingLeft = x*gap;
    }

    private void EquipSkin()
    {

    }

    void Start()
    {
        
    }


    void Update()
    {
        float x = gridContainerElement.resolvedStyle.width / (4 + 5 * gap);
        foreach (var card in gridContainerElement.Children())
        {
            card.style.width = x;
            card.style.height = x * 1.5f;
            card.style.marginRight = x * gap;
            card.style.marginBottom = x * gap;
        }
        gridContainerElement.style.paddingLeft = x * gap;
    }
}
