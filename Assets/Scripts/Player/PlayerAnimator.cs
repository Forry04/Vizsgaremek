using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimator : MonoBehaviour
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

    CharacterController characterController;
    Animator animator;
    PlayerMovementController pmc;
    (float, float) velocityZX = (0.0f, 0.0f);
    (float, float) blendXY = (0.0f,1.0f);
    (float, float) blendJumpXY = (1.0f,0.0f);
    (float, float) blendCrouchXY = (-1.0f,0.0f);
    (float, float) blendStandXY = (0.0f,1.0f);
    //(float, float) blendBaseXY = (0.0f,0.0f);

    float currentMaxVelocity;
    bool isRunning;
    bool forwardPressed;
    bool backwardPressed;
    bool rightPressed;
    bool leftPressed;
    void Start()
    {
        animator = transform.GetComponentInChildren<Animator>();
        characterController = transform.GetComponent<CharacterController>();
        pmc = gameObject.GetComponent<PlayerMovementController>();
    }

    void Update()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);
        forwardPressed = Input.GetKey(KeyCode.W);
        backwardPressed = Input.GetKey(KeyCode.S);
        rightPressed = Input.GetKey(KeyCode.D);
        leftPressed = Input.GetKey(KeyCode.A);
        if (pmc.IsCrouching) currentMaxVelocity = maximumWalk_CrouchVelocity;
        else currentMaxVelocity = isRunning ? maximumRunVelocity : maximumWalk_CrouchVelocity;

        //Animation
        //Velocity acceleration
        if (forwardPressed && velocityZX.Item1 < currentMaxVelocity)
        {
            velocityZX.Item1 += Time.deltaTime * acceleration;
        }
        if (backwardPressed && velocityZX.Item1 > -currentMaxVelocity)
        {
            velocityZX.Item1 -= Time.deltaTime * acceleration;
        }
        if (rightPressed && velocityZX.Item2 < currentMaxVelocity)
        {
            velocityZX.Item2 += Time.deltaTime * acceleration;
        }
        if (leftPressed && velocityZX.Item2 > -currentMaxVelocity)
        {
            velocityZX.Item2 -= Time.deltaTime * acceleration;
        }

        //Lock
        Lock(forwardPressed, true, velocity: ref velocityZX.Item1);
        Lock(backwardPressed, false, velocity: ref velocityZX.Item1);
        Lock(leftPressed, false, velocity: ref velocityZX.Item2);
        Lock(rightPressed, true, velocity: ref velocityZX.Item2);

        ////Velocity deceleration
        if (velocityZX.Item1 != 0.0f && !forwardPressed && !backwardPressed)
        {
            Deceleration(velocity: ref velocityZX.Item1, true);
            Deceleration(velocity: ref velocityZX.Item1, false);
        }
        if (velocityZX.Item2 != 0.0f && !leftPressed && !rightPressed)
        {
            Deceleration(velocity: ref velocityZX.Item2, true);
            Deceleration(velocity: ref velocityZX.Item2, false);
        }

        //Blend crouch
        if (pmc.IsCrouching && blendXY != blendCrouchXY)
        {
            Blend(blendCrouchXY, false, false);
        }
        else if (!pmc.IsCrouching && !pmc.Jump && (blendXY != blendStandXY))
        {
            Blend(blendStandXY, true, true);
        }

        //Blend jump
        if (pmc.Jump && (blendXY != blendJumpXY))
        {
            Blend(blendJumpXY, true, false);
        }
        else if (characterController.isGrounded && !pmc.IsCrouching && (blendXY != blendStandXY))
        {
            pmc.Jump = false;
            Blend(blendStandXY, false, true);
        }

        animator.SetFloat("VelocityZ", velocityZX.Item1);
        animator.SetFloat("VelocityX", velocityZX.Item2);
        animator.SetFloat("BlendX", blendXY.Item1);
        animator.SetFloat("BlendY", blendXY.Item2);
    }
    private void Deceleration(ref float velocity, bool incrase)
    {
        if (incrase ? velocity < 0.0f : velocity > 0.0f)
        {

            velocity = incrase ? velocity + Time.deltaTime * deceleration : velocity - Time.deltaTime * deceleration;
            if (incrase ? velocity > 0.0f : velocity < 0.0f) 
            {
                velocity = 0.0f;
            }
        }
    }
    private void Lock(bool d, bool incrase, ref float velocity)
    {
        if (d && isRunning && (incrase? velocity > currentMaxVelocity : velocity < -currentMaxVelocity))
        {
            velocity = incrase ? currentMaxVelocity : -currentMaxVelocity;
        }
        else if (d && (incrase ? velocity > currentMaxVelocity : velocity < -currentMaxVelocity))
        {
            velocity = incrase ? velocity - Time.deltaTime * deceleration : velocity + Time.deltaTime * deceleration;
            if (incrase? velocity < currentMaxVelocity: velocity > -currentMaxVelocity)
            {
                velocity = incrase? currentMaxVelocity : -currentMaxVelocity;
            }
        }
    }

    private void Blend((float,float) blendTo, bool incrasex, bool incrasey)
    {
        if (blendXY.Item1 != blendTo.Item1) blendXY.Item1 = incrasex ? blendXY.Item1 += Time.deltaTime * blendSpeed : blendXY.Item1 -= Time.deltaTime * blendSpeed;
        if (blendXY.Item2 != blendTo.Item2) blendXY.Item2 = incrasey ? blendXY.Item2 += Time.deltaTime * blendSpeed : blendXY.Item2 -= Time.deltaTime * blendSpeed;
        if ( ((incrasex ?  blendXY.Item1 > blendTo.Item1 : blendXY.Item1 < blendTo.Item1) || (blendXY.Item1 == blendTo.Item1)) &&
             ((incrasey ? blendXY.Item2 > blendTo.Item2 : blendXY.Item2 < blendTo.Item2) ||(blendXY.Item2 == blendTo.Item2)) )
        {
            blendXY = blendTo;
        }
    }
}
