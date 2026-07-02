using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class AnimateHandOnInput : MonoBehaviour
{
    public Animator animator;
    public OVRInput.Controller controller;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands))
        {
            if (animator.enabled)
                animator.enabled = false;
            return;
        }

        if (animator.enabled)
            animator.enabled = true;
        float grip = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
        float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);

        animator.SetFloat("Grip", grip);
        animator.SetFloat("Trigger", trigger);
    }
}
