﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public float maxSpeed = 6.0f;
    public float maxSpeedSprint = 11.0f;
    public float jumpSpeed = 10.0f;
    public float gravity = 20.0f;
    public float upor = 0.01f;
    public float uporOnGround = 0.4f;

    public float fallingDMGTreshold = 5.0f;

    //public bool m_SlideOnTaggedObjects = false;
    public float maxSlideSpeed = 6.0f;
    public float m_AntiBumpFactor = 0.75f;
    public float jumpCooldown = 0.05f;

    public Vector3 moveDirection = Vector3.zero;
    public Vector3 explosionMoveVelocity = Vector3.zero;
    public bool isGrounded;
    private CharacterController controller;
    private float speed;
    private RaycastHit hit;
    private float fallFrom;
    public bool falling;
    private float distanceFromCenter;
    private Vector3 lastContactPoint;
    private bool canMove;
    public float remJumpCooldown;
    private Game game;
    public bool sliding;
    // public float footstepfraction;

    private AudioSource fallSound;
    private AudioSource nononoSound;
    // private AudioSource footStepSound;
    
    // private Vector3 xydm = new Vector3();


    private void Start() {
        //Nerabim vsakic ko se resetira nastavit controllerja
        controller = GetComponent<CharacterController>();
        speed = maxSpeed;
        // Tole je baje razdalja od sredine controllerja do "nog"
        distanceFromCenter = controller.height * 0.5f + controller.radius;
        remJumpCooldown = jumpCooldown;
        game = Game.SharedInstance;
        fallSound = GetComponents<AudioSource>()[0];
        nononoSound = GetComponents<AudioSource>()[3];
        // footStepSound = GetComponents<AudioSource>()[5];
    }


    private void Update() {
    }


    private void FixedUpdate() {
        if (!controller.enabled || game.isPaused || game.isDead)
            return;
        if (remJumpCooldown > 0) {
            remJumpCooldown -= Time.deltaTime * game.globalPlayerSpeedMult;
        }
        if (isGrounded) {
            sliding = false;
            // Kot telesa pod tabo
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distanceFromCenter)) {
                if (Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit) {
                    sliding = true;
                }
            }
            // Če ne najde ničesar poglej še zadnjo točko stičišča
            else {
                Physics.Raycast(lastContactPoint + Vector3.up, -Vector3.up, out hit);
                Debug.DrawLine(lastContactPoint + Vector3.up, -Vector3.up, Color.green);
                if (Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit) {
                    sliding = true;
                }
            }

            if (falling) {
                falling = false;
                // print(transform.position.y + " " + fallFrom + " " + fallingDMGTreshold);
                if (transform.position.y < fallFrom - fallingDMGTreshold) {
                    OnFell(fallFrom - fallingDMGTreshold - transform.position.y);
                }
            }

            // Sprint
            
            bool sprinting = Input.GetKey(KeyCode.LeftShift);
            speed = sprinting ? maxSpeedSprint : maxSpeed;
            if (sliding) // || (m_SlideOnTaggedObjects && hit.collider.tag == "Slide")
            {
                // Tole naj bi zračunalo smer drsenja
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= maxSlideSpeed;
                // canMove = false;
            }
            else {
                if (movementDisabled())
                    moveDirection = new Vector3();
                else
                    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (moveDirection.magnitude > 1) {
                    moveDirection /= moveDirection.magnitude;
                }

                //Tole sm najdu ko sm nekej za premikanje gledou
                moveDirection.y = -m_AntiBumpFactor;
                
                moveDirection = transform.TransformDirection(moveDirection) * speed;
                canMove = true;
                // float chosenFootstepSFXFrequency = (sprinting ? 0.4f : 0.3f);
                // if (footstepfraction >= 1f / chosenFootstepSFXFrequency)
                // {
                //     footstepfraction = 0f;
                //     footStepSound.Play();
                // }
                //
                // xydm.x = moveDirection.x;
                // xydm.z = moveDirection.z;
                // footstepfraction += xydm.magnitude * Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump") && remJumpCooldown <= 0 && !movementDisabled()) {
                moveDirection.y = jumpSpeed;
                remJumpCooldown = jumpCooldown;
            }
        }
        else {
            if (!falling) {
                falling = true;
                fallFrom = transform.position.y; //Padanja z razdaljo
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
        controller.Move((moveDirection + explosionMoveVelocity) * (Time.deltaTime * game.globalPlayerSpeedMult));
        //https://docs.unity3d.com/ScriptReference/CharacterController-collisionFlags.html
        isGrounded = (controller.collisionFlags & CollisionFlags.Below) != 0;
        if (controller.collisionFlags == CollisionFlags.Above &&  moveDirection.y > 0) {
            moveDirection.y = 0;
        }

        explosionMoveVelocity.x = dodajUpor(explosionMoveVelocity.x, isGrounded);
        explosionMoveVelocity.z = dodajUpor(explosionMoveVelocity.z, isGrounded);
    }

    private bool movementDisabled() {
        return game.disabledMovement || game.disableControlls;
    }


    // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnControllerColliderHit.html
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // if ((controller.collisionFlags & CollisionFlags.Below) != 0)
            lastContactPoint = hit.point;
        if (falling && Vector3.Angle(hit.normal, Vector3.up) > 95 && moveDirection.y > 0) {
            moveDirection.y = 0;
        }

        // if (falling && moveDirection.y > 0) {
        //     moveDirection.y = 0;
        // }    
    }

    private void OnFell(float fallDistance) {
        // print("Ouch! Fell " + fallDistance + " units!");
        game.damage((float)Math.Pow(fallDistance,2));
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
        }
        else {
            // moveDirection.y = Math.Max(explosionPlayer.y, moveDirection.y);
            moveDirection.y += explosionPlayer.y;
            isGrounded = false;
        }
    }

    public void spikeJump() {
        if (moveDirection.y < game.SpikeJump)
            moveDirection.y = game.SpikeJump;
    }

    public float powerScalar(float distance) {
        int maxPower = 18;
        float power = 0;
        if (distance > 20) {
            return 0;
        }

        if (distance > 6) {
            maxPower /= 2;
        }

        if (distance < 0) {
            power = 0;
        }
        else {
            //power = Math.pow(distance - .5, -2);

            if (distance > 20) {
                return 0;
            }

            if (distance > 15) {
                maxPower /= 2;
            }

            power = (float) ((Math.Exp(-(distance - 2)) + 1.5) * 30);
            if (power > maxPower) {
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

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "StartTrigger") {
            fallSound.Play();
            game.startFloorHit();
            controller.radius = 1.3f;
            game.disableControlls = true;
            transform.localScale = new Vector3(1f,1f,1f);
            transform.rotation = Quaternion.Euler(0f, -29f, 0f);
            game.mouseLook.xRotation = 0f;
            game.mouseLook.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            game.mouseLook.gameObject.transform.localPosition = 
                new Vector3(0.5f, game.mouseLook.gameObject.transform.localPosition.y, game.mouseLook.gameObject.transform.localPosition.z);
            // transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            // transform.Rotate(-25f, 0f, 0f);
            game.intro = false;
            RenderSettings.reflectionIntensity = 0.1f;
            Destroy(GameObject.Find("FallingTorch"));
            Destroy(other.gameObject);
        } else if (other.gameObject.name == "nononoTrigger") {
            nononoSound.Play();
            
        } else if (other.gameObject.name == "FallDeath") {
            game.damage(100);
        }
            
    }
    
    
}