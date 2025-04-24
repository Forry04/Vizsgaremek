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
    public void Start()
    {

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

        GameObject lightGO = new GameObject("PreviewLight");
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Spot;
        light.intensity = 0.5f;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        light.shadows = LightShadows.None;

        light.cullingMask = previewCamera.cullingMask;
        lightGO.transform.SetParent(previewCamera.transform, false);
    }

    private void SetFrames()
    {
        RenderTexture renderTexture = previewCamera.targetTexture;
        
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
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
