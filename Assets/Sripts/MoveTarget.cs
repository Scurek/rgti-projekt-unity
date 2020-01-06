using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour {

    private FallingRocks fallingRocks;
    // Start is called before the first frame update
    void Start() {
        fallingRocks = transform.parent.gameObject.GetComponent<FallingRocks>();
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnTriggerEnter(Collider other) {
        if (fallingRocks.started && !fallingRocks.done) {
            fallingRocks.arrived = true;
        }
    }
}