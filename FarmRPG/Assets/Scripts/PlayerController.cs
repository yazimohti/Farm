using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    //Movement Components
    private CharacterController controller;
    private Animator animator;
    private float moveSpeed = 4f;
    [Header("Movement Parameters")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    //Interact Components
    [Header("Interact Parameters")]
    PlayerInteractions playerInteractions;

    // Start is called before the first frame update
    void Start()
    {
        //Get movement components
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        //Get interactions components
        playerInteractions = GetComponentInChildren<PlayerInteractions>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move func
        Move();
        //Interact func
        Interact();
    }
    public void Interact()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            //Interact
            playerInteractions.Interact();
        }
    }
    public void Move()
    {
        //Get the horizontal and vertical inputs as a number
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //is the sprint key pressed down?
        if(Input.GetButton("Sprint"))
        {
            //set the animation running and increase our moveSpeed
            moveSpeed = runSpeed;
            animator.SetBool("Running",true);
        }
        else
        {
            //set the animation walking and decrease our moveSpeed
            moveSpeed = walkSpeed;
            animator.SetBool("Running",false);
        }


        //Direction in a normalised vector
        Vector3 dir = new Vector3(horizontal,0f,vertical).normalized;
        Vector3 velocity = moveSpeed * dir * Time.deltaTime;
        //Check if there is movement
        if(dir.magnitude >= 0.1f)
        {
            //Look towards that direction
            transform.rotation = Quaternion.LookRotation(dir);
            
            //Move
            controller.Move(velocity); 
        }

        animator.SetFloat("Speed",velocity.magnitude);

    }
}
