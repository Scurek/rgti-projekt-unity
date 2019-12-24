using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 50f;
    public Transform playerBody;
    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Cursor locked in game
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;    // read mouse movement on X (up and down)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;    // read mouse movement on Y (left and right)

        // Only rotating the camera when looking up and down
        xRotation -= mouseY;    // if + instead of - looking up and down is fliped
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // limiting how far it can look up an down
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  // apply the rotation to local transform  

        // Moving the camera with the body around Y (playerBody references whole FP player)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
