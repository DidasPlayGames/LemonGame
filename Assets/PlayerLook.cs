using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    //Major Variable used to move the player horizontally
    private Vector3 velocity;

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
    private bool isSliding;

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
    private float slideDistance;
    [SerializeField] private float intialSlideForce;
    [SerializeField] private float slideDecrease;
    [SerializeField] private float initialSlideCooldown;
    private float slideCooldown;

    //Variables for Detecting Slope Angle
    [SerializeField] private Transform rearRayTransform;
    [SerializeField] private Transform frontRayTransform;
    [SerializeField] private LayerMask slopeMask;
    private float slopeAngle;
    private float rearRayDistance;
    private float frontRayDistance;

    //Variables to determine player's direction during movement on the slope
    private bool uphill;
    private bool downhill;
    private bool flatSurface;

    //Applying slope direction and angle into sliding
    [SerializeField] private float angleEffectMultiplier;

    void Start()
    {   
        //Locks cusrsor for easy input for rotation using the mouse
        Cursor.lockState = CursorLockMode.Locked;

        //Sets the slide cooldown to its intended value that was set in the editor
        slideCooldown = initialSlideCooldown;
    }

    void Update()
    {   
        //Updates the Sliding Cooldown
        slideCooldown -= Time.deltaTime;

        //Check for input for sliding
        if(Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && slideCooldown < 0 && !isSliding){
            //Coroutine as sliding takes place over multiple seconds
            StartCoroutine("Slide");
        }

        //Checks for input for jumping
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
            Jump();
        }

        //Calls all processes
        Move();
        ApplyGravity();
        DetectSlope();

        
        //Applies all movement processes and calculation. This line actually translates the player
        controller.Move(velocity);
        //Clears the vector, allowing all movement calculations to be added and recalculated once again
        velocity = Vector3.zero;

        Debug.Log(slideCooldown);
    }

    // Calls Late Processes
    void LateUpdate() {
        //Look is delayed so that it is processed after moving the player
        Look();
    }

    //Handles calculating the slope and determining if the player is travelling uphill or downhill
    void DetectSlope(){
        //Sets direction for spawning objects, in order to always keep the ray pointing downwards
        rearRayTransform.rotation = Quaternion.Euler(-transform.rotation.x, 0, 0);
        frontRayTransform.rotation = Quaternion.Euler(-transform.rotation.x, 0, 0);

        //Important and needed variable
        Vector3 rearRayNormal;

        //Initializes raycast outputs
        RaycastHit backHit;
        RaycastHit frontHit;

        //Casts rear ray
        if(Physics.Raycast(rearRayTransform.position, rearRayTransform.TransformDirection(Vector3.down), out backHit, Mathf.Infinity, slopeMask)){
            //Collects the distance of the ray
            rearRayDistance = backHit.distance;

            //Calculates and outputs slope angle
            rearRayNormal = backHit.normal;
            slopeAngle = Vector3.Angle(Vector3.up, rearRayNormal);

            //Draws the ray that has been shot
            Debug.DrawRay(rearRayTransform.position, rearRayTransform.TransformDirection(Vector3.down) * rearRayDistance, Color.black);
        }

        //Casts front ray
        if(Physics.Raycast(frontRayTransform.position, frontRayTransform.TransformDirection(Vector3.down), out frontHit, Mathf.Infinity, slopeMask)){
            //Collects the distance of the ray
            frontRayDistance = frontHit.distance;
            
            //Draws the ray that has been shot
            Debug.DrawRay(frontRayTransform.position, frontRayTransform.TransformDirection(Vector3.down) * frontRayDistance, Color.yellow);
        }

        //Compares slope distances to determine direction on slope
        if(frontRayDistance < rearRayDistance){
            uphill = true;
            downhill = false;
            flatSurface = false;
        }
        else if(frontRayDistance > rearRayDistance){
            uphill = false;
            downhill = true;
            flatSurface = false;
        }
        else{
            uphill = false;
            downhill = false;
            flatSurface = true;
        }
    }



    //Handles sliding, which must take place over multiple seconds
    IEnumerator Slide(){
        //Introduces any starting values
        slideDistance = 1 * intialSlideForce * Time.deltaTime;

        //Loop that ends if velocity is too low
        while(slideDistance > 0.01){

            //Cancels any vertical input, disabling the option to move forwards or backwards
            isSliding = true;

            //Decreases force of slide over time
            slideDistance -= slideDecrease * Time.deltaTime;

            //Adds slide velocity depending on angle of the slope. The players must be travelling downhill
            if(downhill && isGrounded){
                slideDistance += slopeAngle * angleEffectMultiplier * Time.deltaTime;
            }

            //Moves the variable into a vector, which is already transfromed into local space. This is then applied to the global velocity vector
            Vector3 slideVector = transform.forward * slideDistance;
            velocity += slideVector;

            //Does something important (I have no idea, but the code doesn't work if I remove it)
            yield return null;
        }  

        //Reanbles movement
        isSliding = false;

        //Resets the slide cooldown. Must be done at the end to prevent instantly sliding after a long slide
        slideCooldown = initialSlideCooldown;
    }


    //Handles moving the player upwards during a jump
    void Jump(){
        //Formula allows to instatly determine the height of the jump
        //This must be aaplied to the gravityVelocity vector, as it has to cancel the gravity force in order to work.
        gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityStength);
    }

    //Handles pushing the player downwards if they are not touching the ground. This function also handles isGrounded boolean calculations
    void ApplyGravity(){
        //Checks if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);

        //Removes any velocity build-up when the floor is touched
        if(isGrounded && gravityVelocity.y < 0){
            //Negative velocity is used to ensure to player never hovers above the ground
            gravityVelocity.y = -2f;

            //Checks if the player is siliding. Applies extra gravity to counteract the additional velocity of a slide off a slope
            if(isSliding){
                gravityVelocity.y = -10f;
            }
        }

        //Returns to traditional gravity stength if the player is sliding but above the ground
        if(isSliding && !isGrounded){
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
        //Vertical input is in an if statement because it must be disable during sliding
        if(!isSliding){
            verticalInput = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        }
        else{
            verticalInput = 0f;
        }


        //Creates the movement vector based on the input gathered, and applies to the global velocity vector
        Vector3 move = Vector3.right * horizontalInput + Vector3.forward * verticalInput;
        Vector3 localMove = transform.TransformDirection(move);
        velocity += localMove;
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
