using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePauseMenu : MonoBehaviour
{
    static  public EnablePauseMenu Singleton { get; private set; }
    [SerializeField] private GameObject pauseMenu;
    public PlayerInputHandler playerInput;

    private void Awake()
    {
        Singleton = this;
    }



    private void Update()
    {
        if (playerInput.PauseTriggered)
        {
            pauseMenu.SetActive(true);
            playerInput.EnableUIActionMap();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }
}
