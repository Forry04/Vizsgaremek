using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovementController : NetworkBehaviour
{
    private Animator animator;

    [SerializeField]
    private float walkingSpeed = 7.5f;
    [SerializeField]
    private float runningSpeed = 11.5f;
    [SerializeField]
    private float jumpSpeed = 8.0f;
    [SerializeField]
    private float gravity = 20.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    

    [HideInInspector]
    public bool canMove = true;
    public bool IsCrouching = false;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        if (!IsOwner) return;


        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
       
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.LeftControl) && !characterController.isGrounded)
        {
            IsCrouching = !IsCrouching;
        }
        bool forwardD = Input.GetKey(KeyCode.W);
        bool rightD = Input.GetKey(KeyCode.D);
        float curSpeedX = canMove ? ((IsCrouching? walkingSpeed: (isRunning ? runningSpeed : walkingSpeed)) * Input.GetAxis("Vertical")) : 0;
        float curSpeedY = canMove ? ((IsCrouching? walkingSpeed: (isRunning ? runningSpeed : walkingSpeed)) * Input.GetAxis("Horizontal")) : 0;
        //float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        //Animations
        animator.SetBool("IsRunning", canMove && isRunning && Input.GetAxisRaw("Vertical") != 0);
        animator.SetBool("IsWalking", canMove && !isRunning && Input.GetAxisRaw("Vertical") != 0);
        animator.SetBool("IsJumping", canMove && Input.GetButton("Jump"));
        animator.SetBool("IsCrouching", IsCrouching && Input.GetAxisRaw("Vertical") == 0);
        animator.SetBool("IsSneaking", IsCrouching && Input.GetAxisRaw("Vertical") != 0);

    }
}
