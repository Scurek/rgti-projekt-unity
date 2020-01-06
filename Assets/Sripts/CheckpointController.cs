using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {

    public int checkpointNumber = 1;
    public bool isLast;
    public Vector3 spawnDiff = new Vector3(1, 0, 1);

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
            visited = true;
            Game.SharedInstance.currentCheckpoint = checkpointNumber;
            Game.SharedInstance.spawnPosition = transform.position + spawnDiff;
            zastava.SetActive(true);
            InteractionSound.Play();
        }
        // Debug.Log(other.gameObject.tag);
    }
}