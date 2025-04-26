using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class PlayerAudio : MonoBehaviour
{
    public readonly float walkStepInterval = 0.4f;
    public readonly float runStepInterval = 0.3f;
    private float stepTimer = 0f;
    private AudioManager audioManager;
    private PlayerInputHandler inputHandler;
    private AudioSource source;
    CharacterController characterController;
    private PlayerMovementController playerMovement;
    private bool landCheck;
    private bool hasJumped = false;
    private SettingsManager settingsmanager => SettingsManager.Instance;

    private float volume => settingsmanager.CurrentSettings.sfxVolume*settingsmanager.CurrentSettings.masterVolume;

    void Start()
    {
        characterController = GetComponent<CharacterController>(); 
        inputHandler = GetComponent<PlayerInputHandler>();
        source = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovementController>();
        landCheck = characterController.isGrounded;

        StartCoroutine(DelayAudioManager());
    }

    private IEnumerator DelayAudioManager()
    {
        while (AudioManager.Instance == null) yield return null;
        audioManager = AudioManager.Instance;
    }


    void Update()
    {
        //if (audioManager == null) return;
        HandleAdio();
    }

    private void HandleAdio()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        //Move
        if (inputHandler.MoveInput.magnitude > 0.1f && characterController.isGrounded)
        {
            stepTimer += Time.deltaTime;
            float currentInterval = inputHandler.SprintTriggered ? runStepInterval : walkStepInterval;
            if (stepTimer >= currentInterval)
            {
                source.PlayOneShot(Array.Find(audioManager.sounds, s => s.name == (currentScene.name == "Lobby" ? "FootStepsW" : "FootStepsM")).clip,volume);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        //Crouch
        if ((inputHandler.CrouchTriggered || (inputHandler.JumpTriggered && playerMovement.IsCrouching)) && characterController.isGrounded)
        {
            source.PlayOneShot(Array.Find(audioManager.sounds, s => s.name == "Crouch").clip,volume);
        }

        //Jump
        if (inputHandler.JumpTriggered && characterController.isGrounded && !playerMovement.IsCrouching && !hasJumped)
        {
            source.PlayOneShot(Array.Find(audioManager.sounds, s => s.name == (currentScene.name == "Lobby" ? "FootStepsW" : "FootStepsM")).clip, volume);
            hasJumped = true;
        }
        if (!characterController.isGrounded)
        {
            hasJumped = false;
        }

        //Land
        if (!landCheck && characterController.isGrounded)
        {
            source.PlayOneShot(Array.Find(audioManager.sounds, s => s.name == (currentScene.name == "Lobby" ? "FootStepsW" : "FootStepsM")).clip,volume);
        }
        landCheck = characterController.isGrounded;
    }
}
