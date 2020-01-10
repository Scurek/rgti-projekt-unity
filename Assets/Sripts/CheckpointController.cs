using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {

    public int checkpointNumber = 1;
    public bool isLast;
    public bool disableSound;
    public Vector3 spawnDiff = new Vector3(1, 0, 1);
    // public Vector3 spawnRot = new Vector3(0f, 0f, 0f);

    private GameObject parent;
    private GameObject zastava;

    private AudioSource InteractionSound;

    private bool visited;
    // Start is called before the first frame update
    void Start() {
        parent = transform.parent.gameObject;
        zastava = parent.transform.Find("Plane").gameObject;
        zastava.SetActive(false);
        InteractionSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
    }

    void OnTriggerEnter(Collider other) {
        if (!visited && checkpointNumber > Game.SharedInstance.currentCheckpoint) {
            zastava.SetActive(true);
            Game.SharedInstance.enableHealth();
            if (!disableSound)
                InteractionSound.Play();
            if (!isLast) {
                visited = true;
                Game.SharedInstance.currentCheckpoint = checkpointNumber;
                Game.SharedInstance.spawnPosition = transform.position + spawnDiff;
                Game.SharedInstance.showCheckpointDisplay();
            }
            else {
                Game.SharedInstance.showVictoryScreen();
                other.gameObject.GetComponents<AudioSource>()[1].volume = 0;
            }
        }
        // Debug.Log(other.gameObject.tag);
    }
    
    
}