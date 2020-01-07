using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {
    // Start is called before the first frame update
    public List<GameObject> interactingObjects = new List<GameObject>();
    private GameObject interactingObject;
    //private GameObject kamera;
    private GameObject bazooka;
    private GameObject bazookaExit;
    private Camera kamera;
    private PlayerMovement playerMovement;
    private AudioSource rocketFireSound;
    private Animator bazookaAnimator;
    
    private int currentHelper; //1-Bazooka, 2-TorchLight

    public float crFireCooldown = 0f;
    public float fireCooldown = 1.6f;
    private static readonly int HasAmmo = Animator.StringToHash("HasAmmo");

    private Game game;
    void Start() {
        //kamera = GameObject.FindWithTag("MainCamera");
        kamera = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();
        rocketFireSound = GetComponent<AudioSource>();
        game = Game.SharedInstance;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("r")) {
            if (!game.slowMotionEnabled) {
                game.setGlobalSpeed();
            }
            else {
                game.resetGlobalSpeed();
            }
        }
        if (Input.GetButtonDown("Fire1") && bazookaExit && game.ammo > 0 && crFireCooldown <= 0) {
            Rocket rocket = game.GetRocketFromPool();
            if (rocket) {
                crFireCooldown = fireCooldown;
                game.addAmmo(-1);
                Ray ray = kamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hit;
                Vector3 targetPoint;
                if (Physics.Raycast(ray, out hit))
                    targetPoint = hit.point;
                else
                    targetPoint = ray.GetPoint(1000);
                // targetPoint += Vector3.down*2;
                //Debug.Log(targetPoint);
                // Debug.DrawLine(kamera.transform.position, targetPoint, Color.green, 5);
                rocketFireSound.Stop();
                rocketFireSound.Play();
                bazookaAnimator.SetBool(HasAmmo, game.hasAmmo());
                bazookaAnimator.Play("BazookaShoot", 0);
                bazookaAnimator.Play("RocketShoot", 1);
                rocket.InitRocket(bazookaExit.transform, targetPoint, playerMovement);
            }
        }
        else if (crFireCooldown > 0) {
            crFireCooldown -= Time.deltaTime * game.globalPlayerSpeedMult;
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
                game.enableHealth();
                game.enableLighting();
            } else if (interactingObject.CompareTag("bazooka")) {
                // Debug.Log("Pobiram");
                Destroy(interactingObject.GetComponent<BoxCollider>());
                removeObjectFromInteractingObjects(interactingObject);
                interactingObject.transform.parent = kamera.transform;
                interactingObject.transform.localPosition = new Vector3(0.609f, -0.282f, 0.83f);
                interactingObject.transform.localRotation = Quaternion.Euler(new Vector3(-14.734f, -84.42f, -7.3f));
                interactingObject.transform.localScale = new Vector3(0.6f, 1.2f, 1.2f);
                bazooka = interactingObject;
                bazookaExit = bazooka.transform.Find("ExitPoint").gameObject;
                game.enableAmmo();
                bazookaAnimator = bazooka.GetComponent<Animator>();
                game.bazookaAnimator = bazookaAnimator;
            } else if (interactingObject.CompareTag("ammoBox")) {
                AudioSource ammoClip =  interactingObject.GetComponent<AudioSource>();
                if (ammoClip)
                    ammoClip.Play();
                game.refillAmmo();
            }
        }

        if (Input.GetKeyDown("k")) {
            game.damage(100);
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("torchlight")) {
            if (currentHelper < 3) {
                game.displayText("torch");
                currentHelper = 3;
            }
            interactingObjects.Add(other.gameObject);
        } else if (other.CompareTag("bazooka")) {
            if (currentHelper < 2) {
                game.displayText("bazooka");
                currentHelper = 2;
            }
            interactingObjects.Add(other.gameObject);
        } else if (other.CompareTag("ammoBox")) {
            if (currentHelper < 1) {
                game.displayText("ammo");
                currentHelper = 1;
            }
            interactingObjects.Add(other.gameObject);
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("torchlight") || other.CompareTag("bazooka") || other.CompareTag("ammoBox"))
            removeObjectFromInteractingObjects(other.gameObject);
    }

    private GameObject findInteractingObject(List<GameObject> interactingObjects) {
        short maxpriority = 0;
        GameObject found = null;
        foreach (var interactingObject in interactingObjects) {
            if (interactingObject.CompareTag("ammoBox") && maxpriority < 1) {
                maxpriority = 1;
                found = interactingObject;
            }
            else if (interactingObject.CompareTag("bazooka") && maxpriority < 2) {
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
        currentHelper = 0;
        if (interactingObjects.Count < 1) {
            game.helperDisplay.text = "";
        }
        else {
            GameObject nextObject = findInteractingObject(interactingObjects);
            
            if (nextObject.CompareTag("torchlight") && currentHelper < 3) {
                game.displayText("torch");
                currentHelper = 3;
            } else if (nextObject.CompareTag("bazooka") && currentHelper < 2) {
                game.displayText("bazooka");
                currentHelper = 2;
            } else if (nextObject.CompareTag("ammoBox") && currentHelper < 1) {
                Debug.Log(nextObject.name);
                game.displayText("ammo");
                currentHelper = 1;
            }
            else {
                game.helperDisplay.text = "";
                currentHelper = 0;
            }
        }
    }
}