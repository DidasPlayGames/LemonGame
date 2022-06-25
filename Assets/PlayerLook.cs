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
    [SerializeField] private int movementSpeed;

    //Variables for Jumping
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float jumpForce;

    void Start()
    {   
        //Locks cusrsor for easy input for rotation using the mouse
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {   
        //Calls all processes
        Look();
        Move();

    }

    //Handles translating the player with input from the keyboard
    void Move(){
        horizontalInput = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        verticalInput = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;

        Debug.Log(verticalInput);

        transform.Translate(Vector3.forward * verticalInput);
        transform.Translate(Vector3.right * horizontalInput);
    }

    //Handles looking around and input with the mouse
    void Look(){
        
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}
