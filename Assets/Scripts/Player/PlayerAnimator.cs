using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using Unity.Mathematics;
using UnityEngine.Rendering;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(CharacterController))]
public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField]
    private float acceleration = 2.0f;
    [SerializeField]
    private float deceleration = 2.0f;
    [SerializeField]
    private float blendSpeed = 5.0f;
    [SerializeField]
    private float maximumWalk_CrouchVelocity = 0.5f;
    [SerializeField]
    private float maximumRunVelocity = 2.0f;

    private CharacterController characterController;
    private Animator animator;
    private PlayerMovementController pmc;
    private PlayerInputHandler inputhandler;
    private (float Z, float X) velocityZX = (0.0f, 0.0f);
    private readonly float BlendStand = 0;
    private readonly float BlendCrouch = 1;
    private float BlendState;
    private bool jump;
    private bool fall;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        pmc = GetComponent<PlayerMovementController>();
        inputhandler = GetComponent<PlayerInputHandler>();
        Time.fixedDeltaTime = 0.01f;
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        // Velocity change
        ChangeVelocity();
      
        // Blend to/from crouch
        Crouch();

        // Jump transition
        JumpTransition();

        // Fall transition
        FallTransition();

        //Setting animator parameters
        SetAnimator();


    }

    private void ChangeVelocity()
    {
        velocityZX.Z = Mathf.MoveTowards(velocityZX.Z,
            inputhandler.SprintTriggered ? inputhandler.MoveInput.y * maximumRunVelocity : inputhandler.MoveInput.y * maximumWalk_CrouchVelocity,
            Mathf.Abs(inputhandler.MoveInput.y) > Mathf.Abs(velocityZX.Z) ? Time.deltaTime * acceleration : Time.deltaTime * deceleration);

        velocityZX.X = Mathf.MoveTowards(velocityZX.X,
            inputhandler.SprintTriggered ? inputhandler.MoveInput.x * maximumRunVelocity : inputhandler.MoveInput.x * maximumWalk_CrouchVelocity,
            Mathf.Abs(inputhandler.MoveInput.x) > Mathf.Abs(velocityZX.X) ? Time.deltaTime * acceleration : Time.deltaTime * deceleration);
    }
    private void Crouch()
    {
        if (pmc.IsCrouching && (BlendState != BlendCrouch)) BlendState = Mathf.MoveTowards(BlendState, BlendCrouch, Time.deltaTime * blendSpeed);
        else if ((!pmc.IsCrouching || inputhandler.JumpTriggered) && (BlendState != BlendStand)) BlendState = Mathf.MoveTowards(BlendState, BlendStand, Time.deltaTime * blendSpeed);
    }
    private void JumpTransition()
    {
        if (pmc.Jump) jump = true;
        else if (characterController.isGrounded && !pmc.Jump && animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) jump = false;
    }

    private void FallTransition()
    {
        fall = (!characterController.isGrounded && characterController.velocity.y < 0);
        Debug.Log(fall);
    }

    private void SetAnimator()
    {
        animator.SetFloat("VelocityZ", velocityZX.Z);
        animator.SetFloat("VelocityX", velocityZX.X);
        animator.SetFloat("Blend", BlendState);
        animator.SetBool("Jump", jump);
        animator.SetBool("Fall", fall);
    }
}
