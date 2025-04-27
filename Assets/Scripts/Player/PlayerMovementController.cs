using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField]
    private float walkingSpeed = 7.5f;
    [SerializeField]
    private float runningSpeed = 11.5f;
    [SerializeField]
    private float sneakingSpeed = 11.5f;
    [SerializeField]
    private float jumpSpeed = 8.0f;
    [SerializeField]
    private float gravity = 20.0f;

    private Vector3 currentMoevement;
    CharacterController characterController;
    private PlayerInputHandler inputHandler;
    private Animator animator;


    private readonly float raycastHeight = 0.5f;
    private readonly float checkDistance = 1.0f;


    private readonly Vector3 center_Crouch = new(0,0.6f,0.3f);
    private readonly float radius_Crouch = 0.4f;
    private readonly float height_Crouch = 1.2f;

    private readonly Vector3 center_Stand = new(0, 1, 0);
    private readonly float radius_Stand = 0.4f;
    private readonly float height_Stand = 2;
    

    private CapsuleCollider capsuleCollider;

    [HideInInspector]
    public bool canMove = true;
    public bool IsCrouching = false;
    public bool StandUp = false;
    public bool Jump = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
        animator = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        HandleMovement();
    }


    void HandleMovement()
    {
        HandleCrouch();

        float speed = canMove ? ((IsCrouching? sneakingSpeed:(inputHandler.SprintTriggered? runningSpeed:walkingSpeed))) : 0;
        Vector3 inputDirection = new(inputHandler.MoveInput.x, 0f,inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        currentMoevement.x = worldDirection.x * speed;
        currentMoevement.z = worldDirection.z * speed;

        HandleJumping();
        
        if (StandUp && !inputHandler.JumpTriggered && !IsCrouching) StandUp = false;

        characterController.Move(currentMoevement * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if ((inputHandler.CrouchTriggered || (inputHandler.JumpTriggered && IsCrouching)) && characterController.isGrounded)
        {
            if (IsCrouching)
            {
                if (!ObstacleAbove())
                {
                    StandUp = true;
                    IsCrouching = !IsCrouching;
                    ModifyController(false);
                }
            }
            else
            {
                StandUp = true;
                IsCrouching = !IsCrouching;
                ModifyController(true);
            }
        }
    }
    private void HandleJumping()
    {

        if (canMove && inputHandler.JumpTriggered && characterController.isGrounded && !IsCrouching && !StandUp && animator.GetCurrentAnimatorStateInfo(0).IsName("BasicMovements"))
        {
            currentMoevement.y = jumpSpeed;
            Jump = true;
        }
        else Jump = false;

        //if (FallBack && canMove && inputHandler.JumpTriggered && characterController.isGrounded && !IsCrouching && !StandUp)
        //{
        //    FallBack = false;
        //}

        //if (canMove && inputHandler.JumpTriggered && characterController.isGrounded && !IsCrouching && !StandUp && !FallBack)
        //{
        //    FallBack = true;
        //    Jump = true;
        //    currentMoevement.y = jumpSpeed;
        //}
        //else Jump = false;

        if (!characterController.isGrounded)
        {
            currentMoevement.y -= gravity * Time.deltaTime;
        }

    }

    private bool ObstacleAbove()
    {
        Vector3 origin = characterController.transform.position + Vector3.up * raycastHeight;
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.up, out hit, checkDistance))
        {
            Debug.DrawRay(origin, Vector3.up * raycastHeight, Color.blue);
            if (hit.collider != null && hit.collider != characterController.GetComponent<Collider>())
            {
                return true;
            }
        }
        return false; 
    }

    private void ModifyController(bool toCrouch)
    {
        if (toCrouch)
        {
            characterController.center = center_Crouch;
            characterController.radius = radius_Crouch;
            characterController.height = height_Crouch;
            capsuleCollider.center = center_Crouch;
            capsuleCollider.radius = radius_Crouch;
            capsuleCollider.height = height_Crouch;
        }
        else
        {
            characterController.center = center_Stand;
            characterController.radius = radius_Stand;
            characterController.height = height_Stand;
            capsuleCollider.center = center_Stand;
            capsuleCollider.radius = radius_Stand;
            capsuleCollider.height = height_Stand;
        }
    }
}
