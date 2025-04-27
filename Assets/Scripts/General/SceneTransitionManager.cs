using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    private GameObject chatObject;
    private GameObject pauseMenuObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionToMenu()
    {
        // Destroy Chat and EnablePauseMenu objects if they exist
        if (Chat.Singleton != null)
        {
            chatObject = Chat.Singleton.gameObject;
            Destroy(chatObject);
        }

        if (EnablePauseMenu.Singleton != null)
        {
            pauseMenuObject = EnablePauseMenu.Singleton.gameObject;
            Destroy(pauseMenuObject);
        }

        // Load the menu scene
        SceneManager.LoadScene("Menu");
    }

    public void TransitionToLobby()
    {
        // Load the lobby scene
        SceneManager.LoadScene("Lobby");

        // Recreate Chat and EnablePauseMenu objects after the scene loads
        SceneManager.sceneLoaded += OnLobbySceneLoaded;
    }

    private void OnLobbySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            // Recreate Chat object
            if (chatObject != null)
            {
                Instantiate(chatObject);
                chatObject = null;
            }

            // Recreate EnablePauseMenu object
            if (pauseMenuObject != null)
            {
                Instantiate(pauseMenuObject);
                pauseMenuObject = null;
            }
        }

        SceneManager.sceneLoaded -= OnLobbySceneLoaded;
    }
}