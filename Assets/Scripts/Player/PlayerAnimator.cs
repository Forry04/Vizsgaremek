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
    float velocitZ = 0.0f;
    float velocitX = 0.0f;
    float blendx = 0.0f;
    float blendy = 1.0f;
    bool isRunning;
    bool forwardPressed;
    bool backwardPressed;
    bool rightPressed;
    bool leftPressed;
    bool jumping;
    bool isCrouching = false;
    float currentMaxVelocity;

    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        characterController = transform.GetComponent<CharacterController>();

    }

    void Update()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift);
        forwardPressed = Input.GetKey(KeyCode.W);
        backwardPressed = Input.GetKey(KeyCode.S);
        rightPressed = Input.GetKey(KeyCode.D);
        leftPressed = Input.GetKey(KeyCode.A);
        jumping = Input.GetButtonDown("Jump");
        currentMaxVelocity = isRunning ? maximumRunVelocity : maximumWalk_CrouchVelocity;

        if (Input.GetKeyDown(KeyCode.LeftControl) && characterController.isGrounded)
        {
            isCrouching = !isCrouching;
        }
        if (isCrouching)
        {
            currentMaxVelocity = maximumWalk_CrouchVelocity;
        }
        else
        {
            currentMaxVelocity = isRunning ? maximumRunVelocity : maximumWalk_CrouchVelocity;
        }    
        if (Input.GetButton("Jump"))
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }

        //Animation
        //Velocity acceleration
        if (forwardPressed && velocitZ < currentMaxVelocity)
        {
            velocitZ += Time.deltaTime * acceleration;
        }
        if (backwardPressed && velocitZ > -currentMaxVelocity)
        {
            velocitZ -= Time.deltaTime * acceleration;
        }
        if (rightPressed && velocitX < currentMaxVelocity)
        {
            velocitX += Time.deltaTime * acceleration;
        }
        if (leftPressed && velocitX > -currentMaxVelocity)
        {
            velocitX -= Time.deltaTime * acceleration;
        }

        //Lock
        Lock(forwardPressed, true, velocity: ref velocitZ);
        Lock(backwardPressed, false, velocity: ref velocitZ);
        Lock(leftPressed, false, velocity: ref velocitX);
        Lock(rightPressed, true, velocity: ref velocitX);

        ////Velocity deceleration
        if (velocitZ != 0.0f && !forwardPressed && !backwardPressed)
        {
            Deceleration(velocity: ref velocitZ, true);
            Deceleration(velocity: ref velocitZ, false);
        }
        if (velocitX != 0.0f && !leftPressed && !rightPressed)
        {
            Deceleration(velocity: ref velocitX, true);
            Deceleration(velocity: ref velocitX, false);
        }

        //Blend crouch
        if (isCrouching)
        {
            Blend(-1,0,false,false);
        }
        else
        {
            Blend(0,1,true,true);
        }


        animator.SetFloat("VelocityZ", velocitZ);
        animator.SetFloat("VelocityX", velocitX);
        animator.SetFloat("BlendX", blendx);
        animator.SetFloat("BlendY", blendy);
        Debug.Log(animator.GetFloat("BlendX"));
    }
    private void Deceleration(ref float velocity, bool incrase)
    {
        if (incrase ? velocity < 0.0f : velocity > 0.0f)
        {

            velocity = incrase ? velocity + Time.deltaTime * deceleration : velocity - Time.deltaTime * deceleration;
            //Reseting velocity if overshoots 0.0f
            if (incrase ? velocity > 0.0f : velocity < 0.0f) //0.05!?
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

    private void Blend(int x, int y, bool incrasex, bool incrasey)
    {
        blendx = incrasex ? blendx += Time.deltaTime * blendSpeed : blendx -= Time.deltaTime * blendSpeed;
        blendy = incrasex ? blendy += Time.deltaTime * blendSpeed : blendy -= Time.deltaTime * blendSpeed;
        if ((incrasex ?  blendx > x : blendx < x) &&  (incrasey ? blendy > y : blendy < y))
        {
            blendx = x;
            blendy = y;
        }
    }

}
