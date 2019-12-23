using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public float maxSpeed = 6.0f;
    public float maxSpeedSprint = 11.0f;
    public float jumpSpeed = 10.0f;
    public float gravity = 20.0f;
    public float fallingDMGTreshold = 10.0f;
    //public bool m_SlideOnTaggedObjects = false;
    public float maxSlideSpeed = 8.0f;
    public float m_AntiBumpFactor = 0.75f;
    public int jumpCooldown = 10;

    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded;
    private CharacterController controller;
    private float speed;
    private RaycastHit hit;
    // private float m_FallStartLevel;
    private bool falling;
    private float distanceFromCenter;
    private Vector3 lastContactPoint;
    private bool canMove;
    public int remJumpCooldown;


    private void Start() {
        //Nerabim vsakic ko se resetira nastavit controllerja
        controller = GetComponent<CharacterController>();
        speed = maxSpeed;
        // Tole je baje razdalja od sredine controllerja do "nog"
        distanceFromCenter = controller.height * 0.5f + controller.radius;
        remJumpCooldown = jumpCooldown;
    }


    private void Update() {
    }


    private void FixedUpdate() {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move maxSpeed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;

        if (isGrounded) {
            bool sliding = false;
            // Kot telesa pod tabo
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distanceFromCenter)) {
                if (Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit) {
                    sliding = true;
                }
            }
            // Če ne najde ničesar poglej še zadnjo točko stičišča
            else {
                Physics.Raycast(lastContactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit) {
                    sliding = true;
                }
            }
            
            if (falling) {
                falling = false;
                // if (transform.position.y < m_FallStartLevel - fallingDMGTreshold)
                // {
                //     OnFell(m_FallStartLevel - transform.position.y);
                // }
            }

            // Sprint
            speed = Input.GetKey(KeyCode.LeftShift) ? maxSpeedSprint : maxSpeed;
            
            if (sliding) // || (m_SlideOnTaggedObjects && hit.collider.tag == "Slide")
            {
                // Tole naj bi zračunalo smer drsenja
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= maxSlideSpeed;
                canMove = false;
            }
            else {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (moveDirection.magnitude > 1) {
                    moveDirection /= moveDirection.magnitude;
                }
                //Tole sm najdu ko sm nekej za premikanje gledou
                moveDirection.y = -m_AntiBumpFactor;
                moveDirection = transform.TransformDirection(moveDirection) * speed;
                canMove = true;
            }
            if (!Input.GetButtonDown("Jump")) {
                remJumpCooldown--;
            }
            else if (remJumpCooldown < 0) {
                moveDirection.y = jumpSpeed;
                remJumpCooldown = jumpCooldown;
            }
        }
        else {
            if (!falling) {
                falling = true;
                // m_FallStartLevel = transform.position.y; //Padanja z razdaljo
            }
            if (canMove) {
                float fallingSpeed = moveDirection.y;
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (moveDirection.magnitude > 1) {
                    moveDirection /= moveDirection.magnitude;
                }
                moveDirection *= speed;
                moveDirection.y = fallingSpeed;
                moveDirection = transform.TransformDirection(moveDirection);
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
        //https://docs.unity3d.com/ScriptReference/CharacterController-collisionFlags.html
        isGrounded = (controller.collisionFlags & CollisionFlags.Below) != 0;
    }


    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnControllerColliderHit.html
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        lastContactPoint = hit.point;
    }
    
    private void OnFell(float fallDistance) {
        print("Ouch! Fell " + fallDistance + " units!");
    }
}