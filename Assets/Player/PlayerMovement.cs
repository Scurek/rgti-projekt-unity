using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;
    public float speed = 12f;
    public Vector3 characterVelocity = Vector3.zero;
    public Vector3 slopeVelocity = Vector3.zero;
    private Vector3 m_GroundNormal;
    public bool isGrounded;
    public bool isGroundedC;
    public bool isSloped;
    private Vector3 velocity;
    private Text display;
    public float jumpForce = 5f;

    public float gravity = -9.81f;


    // Start is called before the first frame update
    void Start() {
        display = FindObjectOfType<Text>();
    }

    // void OnControllerColliderHit(ControllerColliderHit hit) {
        // m_GroundNormal = hit.normal;
        // isGrounded = Vector3.Dot(hit.normal, transform.up) > 0f && Vector3.Angle(transform.up, m_GroundNormal) <= controller.slopeLimit;
        // isSloped = Vector3.Dot(hit.normal, transform.up) > 0f &&
                   // Vector3.Angle(transform.up, m_GroundNormal) > controller.slopeLimit;
    // }

    // Update is called once per frame
    void FixedUpdate() {
        GroundCheck();
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        Vector3 worldspaceMoveInput = transform.TransformVector(move);
        isGroundedC = controller.isGrounded;
        if (isGrounded && !isSloped) {
            Vector3 targetVelocity = worldspaceMoveInput * speed;
            Vector3 directionRight = Vector3.Cross(targetVelocity.normalized, transform.up);
            targetVelocity = Vector3.Cross(m_GroundNormal, directionRight).normalized * targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            characterVelocity = targetVelocity;

            // jumping
            if (Input.GetButtonDown("Jump")) {
                // force the crouch state to false
                // start by canceling out the vertical component of our velocity
                characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                // then, add the jumpSpeed value upwards
                characterVelocity += Vector3.up * jumpForce;

                // Force grounding to false
                isGrounded = false;
                m_GroundNormal = Vector3.up;
            }

            slopeVelocity = Vector3.zero;
        }
        else if (isSloped) {
            //characterVelocity += worldspaceMoveInput * 25f * Time.deltaTime;
            characterVelocity.x = 0;
            characterVelocity.z = 0;
            slopeVelocity.x += (1f - m_GroundNormal.y) * m_GroundNormal.x * (0.7f);
            slopeVelocity.z += (1f - m_GroundNormal.y) * m_GroundNormal.z * (0.7f);
            display.text = isSloped + " " + slopeVelocity;
        }
        else {
            //Debug.Log("TUSEM");
            // add air acceleration
            slopeVelocity = Vector3.zero;
            characterVelocity += worldspaceMoveInput * 25f * Time.deltaTime;

            // limit air speed to a maximum, but only horizontally
            float verticalVelocity = characterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, 10f);
            characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            // apply the gravity to the velocity
            characterVelocity += Vector3.down * -gravity * Time.deltaTime;
        }

        controller.Move((characterVelocity + slopeVelocity) * Time.deltaTime);
    }

    void GroundCheck() {
        float chosenGroundCheckDistance = isGrounded ? (controller.skinWidth + 0.05f) : 0.07f;
        isSloped = false;
        isGrounded = false;
        m_GroundNormal = Vector3.up;
        if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(controller.height),
            controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, -1,
            QueryTriggerInteraction.Ignore)) {
            m_GroundNormal = hit.normal;
            if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(m_GroundNormal)) {
                isGrounded = true;
                if (hit.distance > controller.skinWidth) {
                    controller.Move(Vector3.down * hit.distance);
                }
            }
            else if (Vector3.Dot(hit.normal, transform.up) > 0f) {
                Debug.Log("HAHA");
                isSloped = true;
            }
        }
    }

    Vector3 GetCapsuleBottomHemisphere() {
        return transform.position + (transform.up * controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight) {
        return transform.position + (transform.up * (atHeight - controller.radius));
    }

    bool IsNormalUnderSlopeLimit(Vector3 normal) {
        return Vector3.Angle(transform.up, normal) <= controller.slopeLimit;
    }
}