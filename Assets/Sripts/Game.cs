using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public static Game SharedInstance;

    public GameObject player;
    public CharacterController playerController;
    
    public GameObject rocket;
    private static List <Rocket> rocketPool;
    public int rocketPoolSize = 2;

    public bool healthEnabled = false;
    private GameObject healthbarContainer;
    private GameObject healthbar;
    public float health = 100;
    private float maxHealth = 100;

    private GameObject ammocounterContainer;
    private GameObject ammocounter;
    private Text ammocounterText;
    public int ammo = 7;
    private int maxAmmo = 7;
    private GameObject crosshair;
    public bool shootingHUDEnabled;
    
    public int currentCheckpoint = 0;
    public Vector3 spawnPosition = new Vector3(-81.9f, 25.8f, 49.7f);

    public bool disableControlls;
    
    void Awake() {
        SharedInstance = this;
    }
    private void Start() {
        healthbar = GameObject.FindGameObjectWithTag("healthbar");
        healthbarContainer = healthbar.transform.parent.gameObject;
        healthbarContainer.SetActive(false);
        
        ammocounter = GameObject.FindGameObjectWithTag("ammocounter");
        ammocounterText = ammocounter.GetComponent<Text>();
        ammocounterContainer = ammocounter.transform.parent.gameObject;
        ammocounterContainer.SetActive(false);
        
        crosshair = GameObject.Find("Crosshair");
        crosshair.SetActive(false);
        
        player = GameObject.FindGameObjectWithTag("player");
        playerController = player.GetComponent<CharacterController>();
        
        rocketPool = new List<Rocket>();
        for (int i = 0; i < rocketPoolSize; i++) {
            rocketPool.Add(Instantiate(rocket).GetComponent<Rocket>());
        }
    }

    public void enableHealth() {
        if (healthEnabled)
            return;
        health = maxHealth;
        healthEnabled = true;
        healthbarContainer.SetActive(true);
    }
    
    public void enableAmmo() {
        if (shootingHUDEnabled)
            return;
        shootingHUDEnabled = true;
        ammocounterContainer.SetActive(true);
        crosshair.SetActive(true);
    }
    
    public void damage(float n) {
        health -= n;
        updateHealthBar();
        if (health <= 0) {
            death();
        }
    }

    public void death() {
        teleportPlayer(spawnPosition);
        health = maxHealth;
        updateHealthBar();
        ammo = maxAmmo;
    }

    public void teleportPlayer(Vector3 newPosition) {
        playerController.enabled = false;
        player.transform.position = newPosition;
        playerController.enabled = true;
    }

    public void addAmmo(int n) {
        ammo += n;
        updateAmmoCounter();
    }

    public void updateHealthBar() {
        healthbar.transform.localScale = new Vector3(x: health / maxHealth, y: 1f, z: 1f);
    }
    
    public void updateAmmoCounter() {
        ammocounterText.text = ammo + "/" + maxAmmo;
    }

    public List<Rocket> GetRocketPool() {
         return rocketPool;
    }
    
    public Rocket GetRocketFromPool() {
        foreach (var trRocket in rocketPool) {
            if (!trRocket.gameObject.activeInHierarchy) {
                return trRocket;
            }
        }
        return null;
    }
}
