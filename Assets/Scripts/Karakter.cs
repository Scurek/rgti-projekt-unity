using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Karakter : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 16.0f;
    public float gravity = 20.0f;
    public float rotSpeed = 6.0f;
    
    private float yaw = 0.0f;

    private float prevY;
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController _characterController;
    private Animator _animator;
    public RuntimeAnimatorController idle;
    public RuntimeAnimatorController run;
    public RuntimeAnimatorController jump;
    public RuntimeAnimatorController back;
    void Start() {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        prevY = transform.position.y;
    }
    

    void Update() {
        
        if (_characterController.isGrounded) {
            // float YRot = transform.rotation.y;
            // Vector3 forward = new Vector3((float)-Math.Sin(YRot), 0, (float)-Math.Cos(YRot)) * Input.GetAxis("Horizontal");
            // Vector3 right = new Vector3((float)Math.Cos(YRot), 0, (float)-Math.Sin(YRot)) * Input.GetAxis("Vertical");
            // moveDirection = forward + right;
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            _moveDirection = transform.TransformVector(_moveDirection);
            _moveDirection *= speed;
            if (Input.GetButton("Jump"))
            {
                _moveDirection.y = jumpSpeed;
            }
        } else
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        float premik = transform.position.y - prevY;
        // Debug.Log(premik);
        if (!_characterController.isGrounded)
        {
            if (_animator.runtimeAnimatorController != jump)
                _animator.runtimeAnimatorController = jump;
        }
        else if (Input.GetAxis("Vertical") < 0f)
        {
            _animator.runtimeAnimatorController = back;
        }
        else if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            _animator.runtimeAnimatorController = run;
        }
        else
        {
            _animator.runtimeAnimatorController = idle;
        }
        _characterController.Move(_moveDirection * Time.deltaTime);
        yaw += rotSpeed * Input.GetAxis("Mouse X");
        
        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
        prevY = transform.position.y;
    }
}
