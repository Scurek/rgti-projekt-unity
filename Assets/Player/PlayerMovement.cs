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

    /*
    private Vector3 _moveDirection = Vector3.zero;
    //
    private Vector3 hitNormal;
   
    //public float jumpSpeed = 16.0f;
    public CharacterController controller;
    
    public float slideSpeed = 24f;
    
    public float jumpHeight = 3f;

    public Transform groundCheck;   // reference to ground check object
    public float groundDistance = 0.4f; // radious od ground check sphere
    public LayerMask groundMask;    // what objects does the sphere check for 
    
    private bool isGrounded;
    private bool isOnSlope;*/
    // Start is called before the first frame update
    void Start() {
        display = FindObjectOfType<Text>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        m_GroundNormal = hit.normal;
        isGrounded =  Vector3.Dot(hit.normal, transform.up) > 0f && Vector3.Angle(transform.up, m_GroundNormal) <= controller.slopeLimit;
        isSloped = Vector3.Dot(hit.normal, transform.up) > 0f && Vector3.Angle(transform.up, m_GroundNormal) > controller.slopeLimit;
    }


    // Update is called once per frame
    void Update() {
        // if (isGrounded && velocity.y < 0) {
        //     velocity.y = gravity / 2;
        // }

        // velocity.y += gravity * Time.deltaTime;
        // controller.Move(velocity * Time.deltaTime);
        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");
        // Vector3 move = transform.right * x + transform.forward * z;
        // controller.Move(move * (speed * Time.deltaTime));


        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        Vector3 worldspaceMoveInput = transform.TransformVector(move);
        isGroundedC = controller.isGrounded;
        if (isGrounded && controller.isGrounded && !isSloped) {
            Vector3 targetVelocity = worldspaceMoveInput * speed;
            Vector3 directionRight = Vector3.Cross(targetVelocity.normalized, transform.up);
            targetVelocity = Vector3.Cross(m_GroundNormal, directionRight).normalized * targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            characterVelocity = targetVelocity;

            // jumping
            if (Input.GetButtonDown("Jump"))
            {
                // force the crouch state to false
                // start by canceling out the vertical component of our velocity
                characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
        
                // then, add the jumpSpeed value upwards
                characterVelocity += Vector3.up * jumpForce;
                
                // Force grounding to false
                isGrounded = false;
                m_GroundNormal = Vector3.up; 
            }

            // footsteps sound
            // float chosenFootstepSFXFrequency = (isSprinting ? footstepSFXFrequencyWhileSprinting : footstepSFXFrequency);
            // if (m_footstepDistanceCounter >= 1f / chosenFootstepSFXFrequency)
            // {
            //     m_footstepDistanceCounter = 0f;
            //     audioSource.PlayOneShot(footstepSFX);
            // }
            //
            // // keep track of distance traveled for footsteps sound
            // m_footstepDistanceCounter += characterVelocity.magnitude * Time.deltaTime;
            slopeVelocity = Vector3.zero;

        } else if (isSloped && controller.isGrounded) {
            //characterVelocity += worldspaceMoveInput * 25f * Time.deltaTime;
            characterVelocity.x = 0;
            characterVelocity.z = 0;
            slopeVelocity.x += (1f - m_GroundNormal.y) * m_GroundNormal.x * (0.7f); 
            slopeVelocity.z += (1f - m_GroundNormal.y) * m_GroundNormal.z * (0.7f); 
            display.text = isSloped + " " + characterVelocity;
        } else {
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

        





        //OLD
        //display.text = ""+velocity.y;

        /*
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isOnSlope = Vector3.Angle (Vector3.up, hitNormal) > controller.slopeLimit;

        if(isGrounded && velocity.y < 0 ){
            velocity.y = gravity / 2;
        }

        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //Vector3 move = transform.right * x + transform.forward * z;
        _moveDirection = transform.right * x + transform.forward * z;
        controller.Move(_moveDirection * (speed * Time.deltaTime));
        
        // if (isOnSlope && isGrounded) {
        //     _moveDirection.x = (1f - hitNormal.y) * hitNormal.x * slideSpeed;
        //     _moveDirection.z = (1f - hitNormal.y) * hitNormal.z * slideSpeed;
        // }
        // controller.Move(_moveDirection * (Time.deltaTime));
        
        //controller.Move(move * (speed * Time.deltaTime));

        if(Input.GetButtonDown("Jump") && isGrounded){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime ;     // free fall formula
        controller.Move(velocity * Time.deltaTime);
        display.text = velocity.y + " " + isGrounded;
        */
        /*if (controller.isGrounded) {
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            _moveDirection = transform.TransformVector(_moveDirection);
            _moveDirection *= speed;
            if (Input.GetButton("Jump"))
            {
                _moveDirection.y = jumpSpeed;
            }
        } else
        {
            _moveDirection.y += gravity * Time.deltaTime;

            // Debug.Log(_moveDirection);
        }*/


        //controller.Move(_moveDirection * Time.deltaTime);
    }
}