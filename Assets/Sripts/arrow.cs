using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : MonoBehaviour {
    // Start is called before the first frame update
    public Vector3 direction;
    public float velocity = 0.25f;
    public bool moving = false;
    void Start() {
        direction = transform.forward;
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (moving)
            transform.position += direction * (velocity * Game.SharedInstance.globalSpeedMult);
    }
    
    private void OnTriggerEnter(Collider other) {
        if (!other.isTrigger && enabled && other.gameObject.name != "Crossbow") {
            // Debug.Log(other.name);
            if (other.gameObject && other.gameObject.name == "Player") {
                Game.SharedInstance.damage(100);
            }
            Destroy(gameObject);
        }
    }
}