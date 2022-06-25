using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    //Variables for Rotation and Looking
    private float mouseX;
    private float mouseY;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private GameObject playerBody;
    private float xRotation = 0f;

    void Start()
    {
        
    }

    void Update()
    {   
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.transform.Rotate(Vector3.up * mouseX);

        Debug.Log(mouseY);
        Debug.Log(xRotation);
    }
}
