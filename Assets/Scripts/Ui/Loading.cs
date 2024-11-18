using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Loading : MonoBehaviour
{
    [SerializeField] private GameObject loadingUiObject;
    [SerializeField] private float rotationSpeed = 330f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 0.1f; 

    private VisualElement loadingUi;
    private VisualElement loadingContainer;

    private void OnEnable()
    {
        loadingUi = loadingUiObject.GetComponent<UIDocument>().rootVisualElement;
        loadingContainer = loadingUi.Q<VisualElement>("loading-container");
        StartCoroutine(AnimateLoadingContainer());
    }

    private IEnumerator AnimateLoadingContainer()
    {
 

        while (true)
        {
          
            loadingContainer.transform.rotation *= Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);

      
            float scale = 1.0f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
            loadingContainer.transform.scale = new Vector3(scale, scale, 1);

            yield return null; 
        }
    }

    
}
