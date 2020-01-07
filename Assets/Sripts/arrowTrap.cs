using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowTrap : MonoBehaviour {
    private GameObject player;

    private CharacterController playerController;
    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private GameObject moveTarget;
    private GameObject arrow;
    private arrow arrowController;

    public bool started;
    public bool done;

    private short step;
    private float startTime;

    public float journeyTime = 0.2f;

    // Start is called before the first frame update
    void Start() {
        moveTarget = GameObject.Find("ArrowContainer");
        arrow = GameObject.Find("Arrow");
        arrowController = arrow.GetComponent<arrow>();
        // arrow.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (done)
            return;
        if (step == 1) {
            rotateTowardTarget(moveTarget.transform);
        } else if (step == 2) {
            if (!arrow) {
                step = 3;
                done = true;
                Game.SharedInstance.disableControlls = false;
                playerController.enabled = true;
            }
            float distance = Vector3.Magnitude(player.transform.position - arrow.transform.position);
            // Debug.Log(distance);
            if (distance < 5f) {
                step = 3;
                activateSlowMo();
            }
        }
    }

    private void rotateTowardTarget(Transform targetTransform) {
        Vector3 targetLookAtPoint = targetTransform.position - playerTransform.position;
        targetLookAtPoint = new Vector3(targetLookAtPoint.x, playerTransform.position.y, targetLookAtPoint.z);
        targetLookAtPoint.Normalize();
        float fracComplete = (Time.time - startTime) / journeyTime;
        targetLookAtPoint = Vector3.Slerp(playerTransform.forward, targetLookAtPoint,
            fracComplete);
        targetLookAtPoint += playerTransform.position;
        targetLookAtPoint.y = playerTransform.position.y;
        player.transform.LookAt(targetLookAtPoint);
        if (fracComplete >= 1) {
            step = 2;
            fireTrap();
        }
    }

    private void activateSlowMo() {
        Game.SharedInstance.setGlobalSpeed();
        Game.SharedInstance.disableControlls = false;
        playerController.enabled = true;
        done = true;
    }

    private void fireTrap() {
        arrowController.moving = true;
        arrowController.direction = Vector3.Normalize(player.transform.position - arrow.transform.position);
        // arrow.transform.rotation = Quaternion.Euler(arrowController.direction.x, arrowController.direction.y + 180f, arrowController.direction.z);
        arrow.transform.rotation = Quaternion.LookRotation(arrowController.direction, Vector3.up);
        arrow.transform.Rotate(0, 90f, 0);
        arrow.SetActive(true);
    }

    private void OnTriggerEnter(Collider other) {
        if (started)
            return;
        if (other.gameObject.CompareTag("player")) {
            started = true;
            Game.SharedInstance.disableControlls = true;
            player = other.gameObject;
            playerController = player.GetComponent<CharacterController>();
            playerTransform = player.transform;
            playerMovement = player.GetComponent<PlayerMovement>();
            startTime = Time.time;
            playerController.enabled = false;
            step = 1;
        }
    }
}