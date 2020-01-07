using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    // Start is called before the first frame update
    public float velocity = 2;
    private PlayerMovement playerMovement;
    private Vector3 direction;
    private bool visible;
    private AudioSource explosionSound;
    
    void Start() {
        gameObject.SetActive(false);
        explosionSound = GetComponent<AudioSource>();
    }
    
    public void InitRocket(Transform transform, Vector3 targetPoint, PlayerMovement playerMovement) {
        // this.transform.rotation = Quaternion.LookRotation(transform.forward);
        this.transform.rotation = transform.rotation;
        // this.transform.Rotate(0, 0, 0);
        //direction = transform.forward;
        direction = Vector3.Normalize(targetPoint - transform.position);
        //this.transform.position = transform.position + direction * 1.2f;
        this.transform.position = transform.position;
        this.playerMovement = playerMovement;
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
    void FixedUpdate() {
        // transform.Translate( Time.deltaTime * direction);
        transform.position += direction * (velocity * Game.SharedInstance.globalSpeedMult);
    }
    
    void Explode() {
        AudioSource.PlayClipAtPoint(explosionSound.clip, transform.position);
        // explosionSound.Stop();
        // explosionSound.Play();
        playerMovement.explosionPush(this.transform.position);
    }
    
    

    void ExplodeGFX() {
        
    }


    private void OnTriggerEnter(Collider other) {
        if (!other.isTrigger && visible && !(other.gameObject && other.gameObject.name == "Player")) {
            DestroyRocket();
        }
    }
}
