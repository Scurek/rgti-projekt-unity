using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Game : MonoBehaviour {
    public static Game SharedInstance;

    public GameObject player;
    public CharacterController playerController;
    private PlayerShooting playerShooting;
    public Animator bazookaAnimator;
    
    public GameObject rocket;
    private static List <Rocket> rocketPool;
    public int rocketPoolSize = 2;

    public bool healthEnabled;
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
    
    public bool specialEnabled;
    private GameObject specialContainer;
    private GameObject specialbar;
    private float maxSpecial = 10f;
    public float special = 10f;
    
    public int currentCheckpoint = 0;
    
    public Vector3 spawnPosition = new Vector3(-81.9f, 25.8f, 49.7f);

    public bool disableControlls = true;

    private GameObject timerContainer;
    private Text timer;
    public bool stopWatchEnabled;
    Stopwatch stopWatch;
    public bool intro = true;
    
    public bool slowMotionEnabled;
    private GameObject slowMotionOverlay;
    public float globalSpeedMult = 1;
    public float globalPlayerSpeedMult = 1;

    public Text helperDisplay;

    

    void Awake() {
        SharedInstance = this;
    }
    private void Start() {
        healthbar = GameObject.FindGameObjectWithTag("healthbar");
        healthbarContainer = healthbar.transform.parent.gameObject;
        healthbarContainer.SetActive(false);
        
        specialbar = GameObject.Find("Specialbar");
        specialContainer = specialbar.transform.parent.gameObject;
        // specialContainer.SetActive(false);
        
        ammocounter = GameObject.FindGameObjectWithTag("ammocounter");
        ammocounterText = ammocounter.GetComponent<Text>();
        ammocounterContainer = ammocounter.transform.parent.gameObject;
        ammocounterContainer.SetActive(false);
        
        timer = GameObject.Find("Timer").GetComponent<Text>();
        timerContainer = GameObject.Find("TimerBackground");
        timerContainer.SetActive(false);

        crosshair = GameObject.Find("Crosshair");
        crosshair.SetActive(false);
        
        player = GameObject.FindGameObjectWithTag("player");
        playerController = player.GetComponent<CharacterController>();
        playerShooting = player.GetComponent<PlayerShooting>();

        slowMotionOverlay = GameObject.Find("SlowMoOverlay");
        slowMotionOverlay.SetActive(false);
        specialEnabled = true;
        special = maxSpecial;

        helperDisplay = GameObject.Find("HelperDisplay").GetComponent<Text>();
        
        rocketPool = new List<Rocket>();
        for (int i = 0; i < rocketPoolSize; i++) {
            rocketPool.Add(Instantiate(rocket).GetComponent<Rocket>());
        }
    }

    private void Update() {
        if (specialEnabled) {
            if (slowMotionEnabled && special > 0) {
                special -= Time.deltaTime;
                updateSpecialBar();
            }
            else if (slowMotionEnabled) {
                resetGlobalSpeed();
            }
            else if (special < maxSpecial) {
                special += Time.deltaTime;
                if (special > maxSpecial) {
                    special = maxSpecial;
                }
                updateSpecialBar();
            }
        }
        
        if (stopWatchEnabled) {
            TimeSpan ts = stopWatch.Elapsed;
            timer.text = $"{ts.Minutes:00}:{ts.Seconds:00}";
        }

        if (intro && RenderSettings.reflectionIntensity >= 0.1f) {
            RenderSettings.reflectionIntensity *= 0.99f;
        }
        
    }
    
    public void resetGlobalSpeed() {
        slowMotionEnabled = false;
        slowMotionOverlay.SetActive(false);
        globalSpeedMult = 1f;
        globalPlayerSpeedMult = 1f;
        if (bazookaAnimator)
            bazookaAnimator.speed = 1f;
    }

    public void setGlobalSpeed() {
        slowMotionEnabled = true;
        slowMotionOverlay.SetActive(true);
        globalSpeedMult = 0.25f;
        globalPlayerSpeedMult = 0.35f;
        if (bazookaAnimator)
            bazookaAnimator.speed = globalPlayerSpeedMult;
    }
    
    public void startStopwatch() {
        stopWatch = new Stopwatch();
        timerContainer.SetActive(true);
        stopWatchEnabled = true;
        stopWatch.Start();
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

    public bool hasAmmo() {
        return ammo > 0;
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
        ammo = maxAmmo;
        special = maxSpecial;
        updateHealthBar();
        updateAmmoCounter();
        updateSpecialBar();
        if (slowMotionEnabled)
            resetGlobalSpeed();
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
    
    public void updateSpecialBar() {
        specialbar.transform.localScale = new Vector3(x: special / maxSpecial, y: 1f, z: 1f);
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

    public void disableLighting() {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.reflectionIntensity = 0;
    }

    public void enableLighting() {
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.reflectionIntensity = 0.5f;
    }
    
    private const String torchText = "Press <color=orange><b>F</b></color> to pick up the flashlight!";
    private const String bazookaText = "Press <color=orange><b>F</b></color> to pick up the weapon!";
    private const String ammoText = "Press <color=orange><b>F</b></color> to refill Ammo!";

    public void displayText(String objectType) {
        String output = "";
        switch (objectType) {
            case "torch":
                output = torchText;
                break;
            case "bazooka":
                output = bazookaText;
                break;
            case "ammo":
                output = ammoText;
                break;
        }
        helperDisplay.text = output;
    }
}
