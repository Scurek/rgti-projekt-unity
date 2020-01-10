using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {
    public float mouseSensitivity = 50f;
    public Transform playerBody;
    public float xRotation = 48.3f;

    private CharacterController playerController;
    private Game game;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        playerController = GameObject.FindWithTag("player").GetComponent<CharacterController>();
        xRotation = 48.3f;
        game = Game.SharedInstance;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!playerController.enabled || game.disableControlls || game.isPaused || game.isDead)
            return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * Game.SharedInstance.globalPlayerSpeedMult;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * Game.SharedInstance.globalPlayerSpeedMult;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}