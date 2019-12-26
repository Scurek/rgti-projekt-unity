using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {
    // Start is called before the first frame update
    private GameObject interactingObject;
    //private GameObject kamera;
    private GameObject bazooka;
    private GameObject bazookaExit;
    private Camera kamera;
    private PlayerMovement playerMovement;
    private AudioSource rocketFireSound;
    
    private int currentHelper;
    public Text helperDisplay;
    
    void Start() {
        //kamera = GameObject.FindWithTag("MainCamera");
        kamera = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();
        rocketFireSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1") && bazookaExit) {
            Rocket rocket = Game.SharedInstance.GetRocketFromPool();
            if (rocket) {
                Ray ray = kamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hit;
                Vector3 targetPoint;
                if (Physics.Raycast(ray, out hit))
                    targetPoint = hit.point;
                else
                    targetPoint = ray.GetPoint(1000);
                //Debug.Log(targetPoint);
                Debug.DrawLine(kamera.transform.position, targetPoint, Color.green, 5);
                rocketFireSound.Stop();
                rocketFireSound.Play();
                rocket.InitRocket(bazookaExit.transform, targetPoint, playerMovement);
            }
        }
        if (Input.GetButtonDown("Interact") && interactingObject && interactingObject.CompareTag("bazooka") ) {
            // Debug.Log("Pobiram");
            Destroy(interactingObject.GetComponent<BoxCollider>());
            helperDisplay.text = "";
            currentHelper = 0;
            interactingObject.transform.parent = kamera.transform;
            interactingObject.transform.localPosition = new Vector3(0.371f, -0.037f, 0.399f);
            interactingObject.transform.localRotation = Quaternion.Euler(new Vector3(-2.159f, -6.595f, 0));
            interactingObject.transform.localScale = new Vector3(2, 2, 1);
            bazooka = interactingObject;
            bazookaExit = bazooka.transform.Find("ExitPoint").gameObject;
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("bazooka")) {
            helperDisplay.text = "Press <color=orange><b>F</b></color> to pick up the weapon!";
            currentHelper = 1;
            interactingObject = other.gameObject;
        }
    }


    private void OnTriggerExit(Collider other) {
        if (currentHelper != 0) {
            helperDisplay.text = "";
            currentHelper = 0;
        }
    }
}