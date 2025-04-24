using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject previewInstance;
    [SerializeField] private Camera previewCamera;
    [SerializeField] private GameObject parent;

    public static PreviewManager instance { get; private set; }
    private VisualElement target;
    private bool setframe = false;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ShowPreview(Material skinMaterial, VisualElement targetElement)
    {
        parent.SetActive(true);

        var renderer = previewInstance.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = skinMaterial;
        }
        target = targetElement;
        setframe = true;
    }

    private void SetFrames()
    {
        RenderTexture renderTexture = previewCamera.targetTexture;

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        target.style.backgroundImage = new StyleBackground(texture);
    }
    private void Update()
    {
        if (setframe) SetFrames();
    }

    public void ClosePreview()
    {
        setframe = false;
        parent.SetActive(false);
    }
}
