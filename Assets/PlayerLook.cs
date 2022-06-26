using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    //Variables for Rotation and Looking
    private float mouseX;
    private float mouseY;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private GameObject playerCamera;
    private float xRotation = 0f;
    
    // Variables for Moving
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private CharacterController controller;
    [SerializeField] private int movementSpeed;

    //Variables for Gravity and Ground-Checking
    Vector3 gravityVelocity;
    [SerializeField] private float gravityStength;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    void Start()
    {   
        //Locks cusrsor for easy input for rotation using the mouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {   
        //Calls all processes
        Move();
        Look();
        ApplyGravity();
    }

    void ApplyGravity(){
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);

        if(isGrounded && gravityVelocity.y < 0){
            gravityVelocity.y = -2f;
        }

        //Creates a Vector3, which will be used to apply the force of gravity
        gravityVelocity.y += gravityStength * Time.deltaTime;
        //Applies the gravityVelocity Vector3, using the CharacterController
        controller.Move(gravityVelocity * Time.deltaTime);
    }

    //Handles translating the player with input from the keyboard
    void Move(){
        //Gathers input from the keyboard and converts into appropriate value
        horizontalInput = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        verticalInput = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;

        //Moves using CharacterController, according to player's input
        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;
        controller.Move(move);
    }

    //Handles looking around and input with the mouse
    void Look(){
        //Gathers input from the mouse and converts into approriate value
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Handles generating an appropriate angle for vertical rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Rotates the player and camera according to player's input
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}
