using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnablePauseMenu : MonoBehaviour
{
    static  public EnablePauseMenu Singleton { get; private set; }
    [SerializeField] private GameObject pauseMenu;
    public PlayerInputHandler playerInput;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        // Open UI
        if (playerInput.PauseTriggered)
        {
            pauseMenu.SetActive(true);
            playerInput.EnableUIActionMap();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true; 
        }
        // Exit UI
        if (playerInput.ExitTriggered)
        {
            gameObject.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t=> t.gameObject.activeSelf && t != gameObject.transform).gameObject.SetActive(false);
            playerInput.EnablePlayerActionMap();
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }     
    }
}
