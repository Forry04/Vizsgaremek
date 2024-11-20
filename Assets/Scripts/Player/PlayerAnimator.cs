using UnityEngine;
using Unity.Netcode;

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
    private (float Z, float X) velocityZX = (0.0f, 0.0f);
    private (float X, float Y) blendXY = (0.0f, 1.0f);
    private readonly (float X, float Y) blendJumpXY = (1.0f, 0.0f);
    private readonly (float X, float Y) blendCrouchXY = (-1.0f, 0.0f);
    private readonly (float X, float Y) blendStandXY = (0.0f, 1.0f);

    private float currentMaxVelocity;
    private bool isRunning => Input.GetKey(KeyCode.LeftShift);
    private bool forwardPressed => Input.GetKey(KeyCode.W);
    private bool backwardPressed => Input.GetKey(KeyCode.S);
    private bool rightPressed => Input.GetKey(KeyCode.D);
    private bool leftPressed => Input.GetKey(KeyCode.A);

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        pmc = GetComponent<PlayerMovementController>();
        Time.fixedDeltaTime = 0.01f;

    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        currentMaxVelocity = pmc.IsCrouching ? maximumWalk_CrouchVelocity : (isRunning ? maximumRunVelocity : maximumWalk_CrouchVelocity);

        // Velocity acceleration
        if (forwardPressed) velocityZX.Z = Mathf.Min(velocityZX.Z + Time.deltaTime * acceleration, currentMaxVelocity);
        if (backwardPressed) velocityZX.Z = Mathf.Max(velocityZX.Z - Time.deltaTime * acceleration, -currentMaxVelocity);
        if (rightPressed) velocityZX.X = Mathf.Min(velocityZX.X + Time.deltaTime * acceleration, currentMaxVelocity);
        if (leftPressed) velocityZX.X = Mathf.Max(velocityZX.X - Time.deltaTime * acceleration, -currentMaxVelocity);

        // Lock
        Lock(forwardPressed, true, ref velocityZX.Z);
        Lock(backwardPressed, false, ref velocityZX.Z);
        Lock(leftPressed, false, ref velocityZX.X);
        Lock(rightPressed, true, ref velocityZX.X);

        // Velocity deceleration
        if (!forwardPressed && !backwardPressed) Deceleration(ref velocityZX.Z);
        if (!leftPressed && !rightPressed) Deceleration(ref velocityZX.X);

        // Blend crouch
        if (pmc.IsCrouching && blendXY != blendCrouchXY) Blend(blendCrouchXY);
        else if (!pmc.IsCrouching && !pmc.Jump && blendXY != blendStandXY) Blend(blendStandXY);

        // Blend jump
        if (pmc.Jump && blendXY != blendJumpXY) Blend(blendJumpXY);
        else if (characterController.isGrounded && !pmc.IsCrouching && blendXY != blendStandXY)
        {
            pmc.Jump = false;
            Blend(blendStandXY);
        }

        // Update animator parameters locally
        float speedZ = velocityZX.Z;
        float speedX = velocityZX.X;
        float blendX = blendXY.X;
        float blendY = blendXY.Y;

        animator.SetFloat("VelocityZ", speedZ);
        animator.SetFloat("VelocityX", speedX);
        animator.SetFloat("BlendX", blendX);
        animator.SetFloat("BlendY", blendY);

        // Send RPC to update parameters on the server and other clients
        UpdateAnimatorParametersServerRpc(speedZ, speedX, blendX, blendY);
    }

    [ServerRpc]
    private void UpdateAnimatorParametersServerRpc(float speedZ, float speedX, float blendX, float blendY)
    {
        // Update parameters on the server
        animator.SetFloat("VelocityZ", speedZ);
        animator.SetFloat("VelocityX", speedX);
        animator.SetFloat("BlendX", blendX);
        animator.SetFloat("BlendY", blendY);

        // Broadcast to other clients
        UpdateAnimatorParametersClientRpc(speedZ, speedX, blendX, blendY);
    }

    [ClientRpc]
    private void UpdateAnimatorParametersClientRpc(float speedZ, float speedX, float blendX, float blendY)
    {
        if (IsOwner) return; // Skip the owner client

        // Update parameters on other clients
        animator.SetFloat("VelocityZ", speedZ);
        animator.SetFloat("VelocityX", speedX);
        animator.SetFloat("BlendX", blendX);
        animator.SetFloat("BlendY", blendY);
    }

    private void Deceleration(ref float velocity)
    {
        if (velocity != 0.0f)
        {
            float decelerationAmount = Time.deltaTime * deceleration;
            if (velocity > 0.0f) velocity = Mathf.Max(velocity - decelerationAmount, 0.0f);
            else velocity = Mathf.Min(velocity + decelerationAmount, 0.0f);
        }
    }

    private void Lock(bool directionPressed, bool increase, ref float velocity)
    {
        if (!directionPressed) return;

        float maxVelocity = increase ? currentMaxVelocity : -currentMaxVelocity;
        if (isRunning || Mathf.Abs(velocity) > Mathf.Abs(maxVelocity))
        {
            velocity = Mathf.MoveTowards(velocity, maxVelocity, Time.deltaTime * deceleration);
        }
    }

    private void Blend((float X, float Y) blendTo)
    {
        blendXY.X = Mathf.MoveTowards(blendXY.X, blendTo.X, Time.deltaTime * blendSpeed);
        blendXY.Y = Mathf.MoveTowards(blendXY.Y, blendTo.Y, Time.deltaTime * blendSpeed);
        Debug.Log($"BlendX: {blendXY.X}, BlendY: {blendXY.Y}");
    }
}
