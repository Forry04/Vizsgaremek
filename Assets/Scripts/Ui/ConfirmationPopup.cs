using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationPopup : MonoBehaviour
{
    private VisualElement rootElement;
    private VisualElement popupContainer;
    private Label messageLabel;
    private Button confirmButton;
    private Button cancelButton;

    private System.Action onConfirmAction;
    private System.Action onCancelAction;

    public void Initialize(VisualElement uiRoot)
    {
        var visualTree = Resources.Load<VisualTreeAsset>("ConfirmationPopup/ConfirmationPopup");
        rootElement = visualTree.CloneTree();
        rootElement.AddToClassList("popup-main");
        uiRoot.Add(rootElement);


        popupContainer = rootElement;
        messageLabel = popupContainer.Q<Label>(className: "popup-text");
        confirmButton = popupContainer.Q<Button>("confirm-button");
        cancelButton = popupContainer.Q<Button>("cancel-button");

         var buttons = GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList();
        foreach (var button in buttons)
        {
            button.RegisterCallback<MouseEnterEvent>(evt =>
            {
                FindObjectOfType<AudioManager>().Play("ButtonHover");
            });

            button.RegisterCallback<FocusEvent>(evt =>
            {

                FindObjectOfType<AudioManager>().Play("ButtonHover");
            });

            button.RegisterCallback<ClickEvent>(evt =>
            {
                //PlayClickSound();
            });
        }
 

        popupContainer.style.display = DisplayStyle.None;

        confirmButton.clicked += OnConfirmClicked;
        cancelButton.clicked += OnCancelClicked;
    }

    public void Show(string message, System.Action onConfirm, System.Action onCancel)
    {
        messageLabel.text = message;
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;
        popupContainer.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        popupContainer.style.display = DisplayStyle.None;
    }

    private void OnConfirmClicked()
    {
        onConfirmAction?.Invoke();
        Hide();
    }

    private void OnCancelClicked()
    {
        onCancelAction?.Invoke();
        Hide();
    }
}
