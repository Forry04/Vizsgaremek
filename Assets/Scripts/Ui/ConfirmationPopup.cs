using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Handles the functionality of a confirmation popup UI element.
/// </summary>
public class ConfirmationPopup : MonoBehaviour
{
    private VisualElement rootElement;
    private VisualElement popupContainer;
    private Label messageLabel;
    private Button confirmButton;
    private Button cancelButton;

    private System.Action onConfirmAction;
    private System.Action onCancelAction;

    /// <summary>
    /// Initializes the confirmation popup by loading its UI elements and setting up event listeners.
    /// </summary>
    /// <param name="uiRoot">The root VisualElement to which the popup will be added.</param>
    public void Initialize(VisualElement uiRoot)
    {
        // Load the VisualTreeAsset for the popup and add it to the UI root.
        var visualTree = Resources.Load<VisualTreeAsset>("ConfirmationPopup/ConfirmationPopup");
        rootElement = visualTree.CloneTree();
        rootElement.AddToClassList("popup-main");
        uiRoot.Add(rootElement);

        // Query and assign UI elements.
        popupContainer = rootElement;
        messageLabel = popupContainer.Q<Label>(className: "popup-text");
        confirmButton = popupContainer.Q<Button>("confirm-button");
        cancelButton = popupContainer.Q<Button>("cancel-button");

        // Register hover and click sounds for all buttons in the popup.
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
                //todo : Play click sound
            });
        }

        // Hide the popup initially.
        popupContainer.style.display = DisplayStyle.None;

        // Register button click event listeners.
        confirmButton.clicked += OnConfirmClicked;
        cancelButton.clicked += OnCancelClicked;
    }

    /// <summary>
    /// Displays the confirmation popup with the specified message and actions.
    /// </summary>
    /// <param name="message">The message to display in the popup.</param>
    /// <param name="onConfirm">The action to execute when the confirm button is clicked.</param>
    /// <param name="onCancel">The action to execute when the cancel button is clicked.</param>
    public void Show(string message, System.Action onConfirm, System.Action onCancel)
    {
        messageLabel.text = message;
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;
        popupContainer.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Hides the confirmation popup.
    /// </summary>
    public void Hide()
    {
        popupContainer.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Handles the confirm button click event.
    /// Executes the confirm action and hides the popup.
    /// </summary>
    private void OnConfirmClicked()
    {
        onConfirmAction?.Invoke();
        Hide();
    }

    /// <summary>
    /// Handles the cancel button click event.
    /// Executes the cancel action and hides the popup.
    /// </summary>
    private void OnCancelClicked()
    {
        onCancelAction?.Invoke();
        Hide();
    }
}
