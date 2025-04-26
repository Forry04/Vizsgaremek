using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    private AudioManager audioManager;
    public static MusicController Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioManager = AudioManager.Instance;
        SceneManager.sceneLoaded += OnSceneLoaded;
        audioManager.Play("TitleMusic");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioManager == null) return;

        switch (scene.name)
        {
            case "Menu":
                audioManager.StartCoroutine(audioManager.CrossfadeCoroutine("TitleMusic"));
                break;
            case "Lobby":
                audioManager.StartCoroutine(audioManager.CrossfadeCoroutine("LobbyMusic"));
                break;
            case "MainScene":
                audioManager.StartCoroutine(audioManager.CrossfadeCoroutine(""));
                break;
        }
    }
}
