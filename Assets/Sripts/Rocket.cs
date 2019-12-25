using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    // Start is called before the first frame update
    public float velocity;
    private CharacterController player;
    private Vector3 direction;
    private bool visible;
    
    void Start() {
        gameObject.SetActive(false);
    }
    
    public void InitRocket(Transform transform, Vector3 targetPoint, CharacterController player) {
        // this.transform.rotation = Quaternion.LookRotation(transform.forward);
        this.transform.rotation = transform.rotation;
        //direction = transform.forward;
        direction = Vector3.Normalize(targetPoint - transform.position);
        //this.transform.position = transform.position + direction * 1.2f;
        this.transform.position = transform.position;
        this.player = player;
        visible = true;
        gameObject.SetActive(true);
        Invoke("DestroyRocket", 2.0f);
    }

    public void DestroyRocket() {
        if (visible) {
            CancelInvoke();
            visible = false;
            Explode();
            gameObject.SetActive(false);
            ExplodeGFX();
        }
    }

    // Update is called once per frame
    void Update() {
        // transform.Translate( Time.deltaTime * direction);
        transform.position += direction * velocity;
    }
    
    void Explode() {
        player.Move(-direction * 10);
    }

    void ExplodeGFX() {
        Debug.Log("BOOM");
    }


    private void OnTriggerEnter(Collider other) {
        if (visible) {
            DestroyRocket();
        }
    }
}
