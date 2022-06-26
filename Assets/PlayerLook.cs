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

    //Variables for Jumping
    [SerializeField] private float jumpHeight;

    //Variables for Sliding
    private Vector3 slideVector;
    [SerializeField] private float intialSlideForce;
    [SerializeField] private float slideDecrease;

    void Start()
    {   
        //Locks cusrsor for easy input for rotation using the mouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {   
        //Calls all processes
        Move();
        ApplyGravity();

        //Checks for input for jumping
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
            Jump();
        }
        
        ///Check for input for sliding
        if(Input.GetKeyDown(KeyCode.LeftControl) && isGrounded){
            //Coroutine as sliding takes place over multiple seconds
            StartCoroutine("Slide");
        }

    }

    // Calls Late Processes
    void LateUpdate() {
        //Look is delayed so that it is processed after moving the player
        Look();
    }

    //Handles sliding, which must take place over multiple seconds
    IEnumerator Slide(){
        //Introduces any starting values
        float countdown = 0.5f;
        slideVector = Vector3.forward * intialSlideForce * Time.deltaTime;

        //Moves camera down to provide effect of sliding
        playerCamera.transform.Translate(Vector3.down * 0.5f);

        //Timed Loop
        while(countdown > 0){
            countdown -= Time.deltaTime;

            //Decreases force of slide over time
            slideVector.z -= slideDecrease * Time.deltaTime;
            //Transforms the vector from local to world space, and uses it in the controller.Move() function
            Vector3 localSlideVector = transform.TransformDirection(slideVector); 
            controller.Move(localSlideVector);

            //Does something important
            yield return null;
        }  

        //Moves camera back up before ending the slide
        playerCamera.transform.Translate(Vector3.up * 0.5f);
    }


    //Handles moving the player upwards during a jump
    void Jump(){
        //Formula allows to instatly determine the height of the jump
        gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityStength);
    }

    void ApplyGravity(){
        //Checks if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);

        //Removes any velocity build-up when the floor is touched
        if(isGrounded && gravityVelocity.y < 0){
            //Negative velocity is used to ensure to player never hovers above the ground
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
