using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pogled : MonoBehaviour
{
    private float pitch = 0.0f;
    public float rotSpeed = 6.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pitch -= rotSpeed * Input.GetAxis("Mouse Y");
        if (pitch > 90) {
            pitch = 90;
        }
        if (pitch < -90) {
            pitch = -90;
        }
        transform.eulerAngles = new Vector3(pitch, transform.eulerAngles.y, transform.eulerAngles.z);
        
    }
}
