using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {
    // Start is called before the first frame update
    private List<GameObject> interactingObjects = new List<GameObject>();
    private GameObject interactingObject;
    //private GameObject kamera;
    private GameObject bazooka;
    private GameObject bazookaExit;
    private Camera kamera;
    private PlayerMovement playerMovement;
    private AudioSource rocketFireSound;
    
    private int currentHelper; //1-Bazooka, 2-TorchLight
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
                Ray ray = kamera.ViewportPointToRay(new Vector3(0.5F, 0.4F, 0));
                RaycastHit hit;
                Vector3 targetPoint;
                if (Physics.Raycast(ray, out hit))
                    targetPoint = hit.point;
                else
                    targetPoint = ray.GetPoint(1000);
                // targetPoint += Vector3.down*2;
                //Debug.Log(targetPoint);
                Debug.DrawLine(kamera.transform.position, targetPoint, Color.green, 5);
                rocketFireSound.Stop();
                rocketFireSound.Play();
                rocket.InitRocket(bazookaExit.transform, targetPoint, playerMovement);
            }
        }
        if (Input.GetButtonDown("Interact") && interactingObjects.Count > 0) {
            interactingObject = findInteractingObject(interactingObjects);
            if (interactingObject.CompareTag("torchlight")) {
                Destroy(interactingObject.GetComponent<BoxCollider>());
                removeObjectFromInteractingObjects(interactingObject);
                interactingObject.transform.parent = kamera.transform;
                interactingObject.transform.localPosition = new Vector3(-2f, -0.6f, -0.7f);
                interactingObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                interactingObject.transform.localScale = new Vector3(2, 2, 2);
            } else if (interactingObject.CompareTag("bazooka")) {
                // Debug.Log("Pobiram");
                Destroy(interactingObject.GetComponent<BoxCollider>());
                removeObjectFromInteractingObjects(interactingObject);
                interactingObject.transform.parent = kamera.transform;
                
                // interactingObject.transform.localPosition = new Vector3(0.371f, -0.037f, 0.399f);
                // interactingObject.transform.localRotation = Quaternion.Euler(new Vector3(-2.159f, -6.595f, 0));
                // interactingObject.transform.localScale = new Vector3(2, 2, 1);
                // interactingObject.transform.localPosition = new Vector3(0.297f, -0.301f, 0.825f);
                // interactingObject.transform.localRotation = Quaternion.Euler(new Vector3(-111.78f, -107.26f, 365.85f));
                // interactingObject.transform.localScale = new Vector3(0.55f, 0.7f, 0.7f);
                
                // interactingObject.transform.localPosition = new Vector3(1f, -0.7f, -0.6f);
                interactingObject.transform.localPosition = new Vector3(1f, -0.7f, 1.5f);
                interactingObject.transform.localRotation = Quaternion.Euler(new Vector3(-87.437f, 0.4f, 90f));
                interactingObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                bazooka = interactingObject;
                bazookaExit = bazooka.transform.Find("ExitPoint").gameObject;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("torchlight") && currentHelper < 3) {
            helperDisplay.text = "Press <color=orange><b>F</b></color> to pick up the flashlight!";
            currentHelper = 3;
            interactingObjects.Add(other.gameObject);
        } else if (other.CompareTag("bazooka") && currentHelper < 2) {
            helperDisplay.text = "Press <color=orange><b>F</b></color> to pick up the weapon!";
            currentHelper = 2;
            interactingObjects.Add(other.gameObject);
        }
    }


    private void OnTriggerExit(Collider other) {
        removeObjectFromInteractingObjects(other.gameObject);
    }

    private GameObject findInteractingObject(List<GameObject> interactingObjects) {
        short maxpriority = 0;
        GameObject found = null;
        foreach (var interactingObject in interactingObjects) {
            if (interactingObject.CompareTag("bazooka") && maxpriority < 2) {
                maxpriority = 2;
                found = interactingObject;
            } else if (interactingObject.CompareTag("torchlight") && maxpriority < 3) {
                maxpriority = 3;
                found = interactingObject;
            }
        }
        return found;
    }

    private void removeObjectFromInteractingObjects(GameObject gameObject) {
        interactingObjects.Remove(gameObject);
        if (interactingObjects.Count < 1 && currentHelper != 0) {
            helperDisplay.text = "";
            currentHelper = 0;
        }
    }
}