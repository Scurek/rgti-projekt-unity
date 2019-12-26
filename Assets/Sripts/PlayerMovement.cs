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
    public float upor = 0.01f;
    public float uporOnGround = 0.4f;
    public float fallingDMGTreshold = 10.0f;
    //public bool m_SlideOnTaggedObjects = false;
    public float maxSlideSpeed = 8.0f;
    public float m_AntiBumpFactor = 0.75f;
    public int jumpCooldown = 10;
    
    public Vector3 moveDirection = Vector3.zero;
    public Vector3 explosionMoveVelocity = Vector3.zero;
    private bool isGrounded;
    private CharacterController controller;
    private float speed;
    private RaycastHit hit;
    // private float m_FallStartLevel;
    private bool falling;
    private float distanceFromCenter;
    private Vector3 lastContactPoint;
    private bool canMove;
    private int remJumpCooldown;
    

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
        controller.Move((moveDirection + explosionMoveVelocity) * Time.deltaTime);
        //https://docs.unity3d.com/ScriptReference/CharacterController-collisionFlags.html
        isGrounded = (controller.collisionFlags & CollisionFlags.Below) != 0;
        explosionMoveVelocity.x = dodajUpor(explosionMoveVelocity.x, isGrounded);
        explosionMoveVelocity.z = dodajUpor(explosionMoveVelocity.z, isGrounded);
    }


    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnControllerColliderHit.html
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        lastContactPoint = hit.point;
    }
    
    private void OnFell(float fallDistance) {
        print("Ouch! Fell " + fallDistance + " units!");
    }

    public void explosionPush(Vector3 explosionPosition) {
        Vector3 explosionPlayer = this.transform.position - explosionPosition;
        float power = powerScalar(Vector3.Magnitude(explosionPlayer));
        explosionPlayer = Vector3.Normalize(explosionPlayer) * power;
        //explosionMoveVelocity += explosionPlayer;
        explosionMoveVelocity.x += explosionPlayer.x;
        explosionMoveVelocity.z += explosionPlayer.z;
        if (moveDirection.y < 0) {
            moveDirection.y = explosionPlayer.y;
            //controller.Move(Vector3.up * (moveDirection.y * Time.deltaTime));
            isGrounded = false;
        } else {
            moveDirection.y += explosionPlayer.y;
        }
        
    }
    
    public float powerScalar(float distance) {
        int maxPower = 32;
        float power = 0;
        if(distance > 20){
            return 0;
        }
        if(distance > 6){
            maxPower /= 2;
        }
        if(distance < 0) {
            power = 0;
        } else{
            //power = Math.pow(distance - .5, -2);
    
            if(distance > 20){
                return 0;
            }
            if(distance > 15){
                maxPower /= 2;
            }
            power = (float)((Math.Exp(-(distance-2)) + 1.5) * 30);
            if(power > maxPower){
                power = maxPower;
            }
        }
        return power;
    }
    
    private float dodajUpor(float hitrost, bool onGround) {
        if (hitrost < 0.01 && hitrost > -0.01) {
            return 0;
        }
        if (onGround) {
            return hitrost * (1f - uporOnGround);
        }
        return hitrost * (1f - upor);
    }
}