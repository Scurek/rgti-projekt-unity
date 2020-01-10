using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Game : MonoBehaviour {
    public static Game SharedInstance;

    public GameObject player;
    public CharacterController playerController;
    public PlayerMovement playerMovement;
    public Animator bazookaAnimator;

    public GameObject playerCamera;
    public MouseLook mouseLook;

    public GameObject rocket;
    private List<Rocket> rocketPool;
    public int rocketPoolSize = 2;
    
    // public GameObject explosion;
    // private List<ParticleSystem>

    public bool healthEnabled;
    private GameObject healthbarContainer;
    private GameObject healthbar;
    public float health = 100;
    private float maxHealth = 100;

    private GameObject ammocounterContainer;
    private GameObject ammocounter;
    private Text ammocounterText;
    public int ammo = 0;
    private int maxAmmo = 15;
    private GameObject crosshair;
    public bool shootingHUDEnabled;

    public bool specialEnabled;
    private GameObject specialContainer;
    private GameObject specialbar;
    private float maxSpecial = 10f;
    public float special = 10f;
    private AudioSource DeathSound;

    public int currentCheckpoint = 0;

    public Vector3 spawnPosition = new Vector3(-81.9f, 25.8f, 49.7f);

    public bool disableControlls = false;
    public bool disabledMovement = true;

    private GameObject timerContainer;
    private Text timer;
    public bool stopWatchEnabled;
    Stopwatch stopWatch;
    public bool intro = true;

    public bool slowMotionEnabled;
    // private GameObject slowMotionOverlay;
    private Image slowMotionOverlay;
    public float globalSpeedMult = 1;
    public float globalPlayerSpeedMult = 1;

    public Text helperDisplay;

    public float SpikeDamage = 50f;
    public float SpikeJump = 20f;

    private Image blackOverlay;
    private Text flavorText;

    private Text checkpointDisplay;

    private GameObject PauseMenu;
    public bool isPaused;
    private GameObject PauseKey;

    private Image DeathScreenContainer;
    private Text DeathScreenText;
    private Text DeathScreenRestartText;
    public bool isDead;
    // private Button DeathScreenRestartButton;

    private arrowTrap ArrowTrap;

    private GameObject victoryScreen;
    private Image victoryScreenContrainer;
    private Text[] victoryScreenTextArray;
    private Button restartGameButton;
    
    private Vector3 CheatCords = new Vector3(70f, 72f, 97f);

    void Awake() {
        SharedInstance = this;
    }

    private void Start() {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        healthbar = GameObject.FindGameObjectWithTag("healthbar");
        healthbarContainer = healthbar.transform.parent.gameObject;
        healthbarContainer.SetActive(false);

        specialbar = GameObject.Find("Specialbar");
        specialContainer = specialbar.transform.parent.gameObject;
        specialContainer.SetActive(false);

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
        playerMovement = player.GetComponent<PlayerMovement>();

        playerCamera = GameObject.Find("Main Camera");
        mouseLook = playerCamera.GetComponent<MouseLook>();

        slowMotionOverlay = GameObject.Find("SlowMoOverlay").GetComponent<Image>();
        slowMotionOverlay.enabled = false;
        // slowMotionOverlay.SetActive(false);
        special = maxSpecial;
        DeathSound = player.GetComponents<AudioSource>()[4];

        helperDisplay = GameObject.Find("HelperDisplay").GetComponent<Text>();

        blackOverlay = GameObject.Find("BlackOverlay").GetComponent<Image>();
        blackOverlay.enabled = false;
        flavorText = GameObject.Find("FlavorText").GetComponent<Text>();
        flavorText.enabled = false;
        
        checkpointDisplay = GameObject.Find("CheckpointDisplay").GetComponent<Text>();
        checkpointDisplay.enabled = false;

        PauseMenu = GameObject.Find("PauseMenu");
        PauseMenu.SetActive(false);
        PauseKey = GameObject.Find("PauseKey");

        GameObject DeathScreen = GameObject.Find("DeathScreen");
        DeathScreenContainer = DeathScreen.GetComponent<Image>();
        DeathScreenText = DeathScreen.transform.Find("Failed").GetComponent<Text>();
        GameObject DeathScreenRestart = DeathScreen.transform.Find("Respawn").gameObject;
        DeathScreenRestartText = DeathScreenRestart.GetComponent<Text>();
        DeathScreenContainer.enabled = false;
        DeathScreenText.enabled = false;
        DeathScreenRestartText.enabled = false;
        // DeathScreenRestartButton = DeathScreenRestart.GetComponent<Button>();
        
        ArrowTrap = GameObject.Find("ArrowTrap").GetComponent<arrowTrap>();

        victoryScreen = GameObject.Find("VictoryScreen");
        victoryScreenContrainer = victoryScreen.GetComponent<Image>();
        //victoryScreenTextArray = new Text[victoryScreen.transform.childCount];
        victoryScreenTextArray = victoryScreen.GetComponentsInChildren<Text>();
        restartGameButton = victoryScreenTextArray[5].gameObject.GetComponent<Button>();

        rocketPool = new List<Rocket>();
        for (int i = 0; i < rocketPoolSize; i++) {
            rocketPool.Add(Instantiate(rocket).GetComponent<Rocket>());
        }
        
        
    }

    private void Update() {
        if (isDead)
            return;
        if (Input.GetKeyDown(KeyCode.C) && currentCheckpoint > 0) {
            cheat();
        }
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {
            if (PauseMenu.activeSelf) {
                resumeGame();
            }
            else {
                pauseGame();
            }
        }
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

    private void cheat() {
        playerController.enabled = false;
        player.transform.position = CheatCords;
        player.transform.rotation = Quaternion.Euler(0, 90f, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        playerController.enabled = true;
    }

    public void pauseGame() {
        Time.timeScale = 0;
        PauseKey.SetActive(false);
        PauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void resumeGame() {
        Time.timeScale = 1;
        if (PauseMenu.activeInHierarchy)
            PauseMenu.SetActive(false);
        if (!PauseKey.activeInHierarchy)
            PauseKey.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void resetGlobalSpeed() {
        slowMotionEnabled = false;
        // slowMotionOverlay.SetActive(false);
        slowMotionOverlay.enabled = false;
        globalSpeedMult = 1f;
        globalPlayerSpeedMult = 1f;
        // specialTheme.volume = 0;
        if (bazookaAnimator)
            bazookaAnimator.speed = 1f;
    }

    public void setGlobalSpeed() {
        slowMotionEnabled = true;
        // slowMotionOverlay.SetActive(true);
        slowMotionOverlay.enabled = true;
        globalSpeedMult = 0.25f;
        globalPlayerSpeedMult = 0.35f;
        // specialTheme.volume = 1;
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

    public void enableSpecial() {
        if (specialEnabled)
            return;
        specialEnabled = true;
        special = maxSpecial;
        specialContainer.SetActive(true);
        updateSpecialBar();
    }

    public void enableAmmo() {
        if (shootingHUDEnabled)
            return;
        shootingHUDEnabled = true;
        ammocounterContainer.SetActive(true);
        updateAmmoCounter();
        crosshair.SetActive(true);
    }

    public void refillAmmo() {
        ammo = maxAmmo;
        updateAmmoCounter();
    }

    public bool hasAmmo() {
        return ammo > 0;
    }

    public void damage(float n) {
        if (!healthEnabled)
            return;
        health -= n;
        if (health < 0)
            health = 0f;
        updateHealthBar();
        if (health <= 0) {
            death();
        }
    }

    public void death() {
        showDeathScreen();
        health = maxHealth;
        ammo = maxAmmo;
        special = maxSpecial;
        updateHealthBar();
        updateAmmoCounter();
        updateSpecialBar();
        DeathSound.Play();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isDead = true;
        destroyAllRockets();
        if (currentCheckpoint < 2)
            ResetArrowTrap();
        if (slowMotionEnabled)
            resetGlobalSpeed();
    }

    public void ResetArrowTrap() {
        ArrowTrap.done = false;
        ArrowTrap.started = false;
        ArrowTrap.step = 0;
    }

    private void showDeathScreen() {
        StartCoroutine(fadeInDeathScreen(0.25f));
    }

    private IEnumerator fadeInDeathScreen(float t) {
        DeathScreenContainer.color = new Color(DeathScreenContainer.color.r, DeathScreenContainer.color.g, DeathScreenContainer.color.b, 0);
        DeathScreenText.color = new Color(DeathScreenText.color.r, DeathScreenText.color.g, DeathScreenText.color.b, 0);
        DeathScreenRestartText.color = new Color(DeathScreenRestartText.color.r, DeathScreenRestartText.color.g, DeathScreenRestartText.color.b, 0);
        DeathScreenContainer.enabled = true;
        DeathScreenText.enabled = true;
        DeathScreenRestartText.enabled = true;
        while (DeathScreenContainer.color.a < 1.0f) {
            DeathScreenContainer.color = new Color(DeathScreenContainer.color.r, DeathScreenContainer.color.g, DeathScreenContainer.color.b,
                DeathScreenContainer.color.a + (Time.deltaTime / t));
            DeathScreenText.color = new Color(DeathScreenText.color.r, DeathScreenText.color.g, DeathScreenText.color.b,
                DeathScreenText.color.a + (Time.deltaTime / t));
            DeathScreenRestartText.color = new Color(DeathScreenRestartText.color.r, DeathScreenRestartText.color.g, DeathScreenRestartText.color.b,
                DeathScreenRestartText.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }
    
    public void showVictoryScreen() {
        isDead = true;
        StartCoroutine(fadeInVictoryScreen());
    }
    
    private IEnumerator fadeInVictoryScreen() {
        victoryScreenContrainer.color = new Color(victoryScreenContrainer.color.r, victoryScreenContrainer.color.g, victoryScreenContrainer.color.b, 0);
        victoryScreenContrainer.enabled = true;
        restartGameButton.interactable = false;
        foreach (var text in victoryScreenTextArray) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            text.enabled = true;
        }
        while (victoryScreenContrainer.color.a < 0.9f) {
            victoryScreenContrainer.color = new Color(victoryScreenContrainer.color.r, victoryScreenContrainer.color.g, victoryScreenContrainer.color.b,
                Math.Max(victoryScreenContrainer.color.a + (Time.deltaTime / 0.05f), 0.9f));
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        Text currentText = victoryScreenTextArray[0];
        while (currentText.color.a < 1.0f) {
            currentText.color = new Color(currentText.color.r, currentText.color.g, currentText.color.b,
                currentText.color.a + (Time.deltaTime / 0.3f));
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        while (victoryScreenTextArray[1].color.a < 1.0f) {
            for (int i = 1; i < 3; i++) {
                currentText = victoryScreenTextArray[i];
                currentText.color = new Color(currentText.color.r, currentText.color.g, currentText.color.b, currentText.color.a + (Time.deltaTime / 0.6f));
                yield return null;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.6f);
        while (victoryScreenTextArray[3].color.a < 1.0f) {
            for (int i = 3; i < 6; i++) {
                currentText = victoryScreenTextArray[i];
                currentText.color = new Color(currentText.color.r, currentText.color.g, currentText.color.b, currentText.color.a + (Time.deltaTime / 1f));
                yield return null;
            }
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        restartGameButton.interactable = true;
    }

    public void restart() {
        SceneManager.LoadScene("Game");
    }

    public void restartFromLastCheckpoint() {
        destroyAllRockets();
        health = maxHealth;
        DeathScreenContainer.enabled = false;
        DeathScreenText.enabled = false;
        DeathScreenRestartText.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isDead = false;
        teleportPlayer(spawnPosition);
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
    
    public void destroyAllRockets() {
        foreach (var trRocket in rocketPool) {
            if (trRocket.gameObject.activeInHierarchy) {
                trRocket.gameObject.SetActive(false);
            }
        }
    }

    public void disableLighting() {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.reflectionIntensity = 0;
    }

    public void enableLighting() {
        //RenderSettings.ambientMode = AmbientMode.Skybox;
        // RenderSettings.reflectionIntensity = 0.5f;
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

    public void startFloorHit() {
        blackOverlay.enabled = true;
        flavorText.text = "Some time later...";
        StartCoroutine(someTimeLater(1));
    }

    public IEnumerator someTimeLater(float t) {
        flavorText.color = new Color(flavorText.color.r, flavorText.color.g, flavorText.color.b, 0);
        flavorText.enabled = true;
        yield return new WaitForSeconds(0.5f);
        while (flavorText.color.a < 1.0f) {
            flavorText.color = new Color(flavorText.color.r, flavorText.color.g, flavorText.color.b,
                flavorText.color.a + (Time.deltaTime / t));
            yield return null;
        }

        yield return new WaitForSeconds(1);
        while (flavorText.color.a > 0.0f) {
            flavorText.color = new Color(flavorText.color.r, flavorText.color.g, flavorText.color.b,
                flavorText.color.a - (Time.deltaTime / t));
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        flavorText.enabled = false;
        while (blackOverlay.color.a > 0.0f) {
            blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b,
                blackOverlay.color.a - (Time.deltaTime / t));
            yield return null;
        }
        blackOverlay.enabled = false;
        disabledMovement = false;
        disableControlls = false;
    }

    public void showCheckpointDisplay() {
        if (checkpointDisplay.enabled)
            return;
        StartCoroutine(showCheckpointDisplayIE(1f));
    }
    
    public IEnumerator showCheckpointDisplayIE(float t) {
        checkpointDisplay.color = new Color(checkpointDisplay.color.r, checkpointDisplay.color.g, checkpointDisplay.color.b, 0);
        checkpointDisplay.enabled = true;
        while (checkpointDisplay.color.a < 1.0f) {
            checkpointDisplay.color = new Color(checkpointDisplay.color.r, checkpointDisplay.color.g, checkpointDisplay.color.b,
                checkpointDisplay.color.a + (Time.deltaTime / t));
            yield return null;
        }

        yield return new WaitForSeconds(1);
        while (checkpointDisplay.color.a > 0.0f) {
            checkpointDisplay.color = new Color(checkpointDisplay.color.r, checkpointDisplay.color.g, checkpointDisplay.color.b,
                checkpointDisplay.color.a - (Time.deltaTime / t));
            yield return null;
        }
        checkpointDisplay.enabled = false;
    }
    
    public bool rotateTowardTarget(Transform targetTransform, Transform toRotate, float startTime, float journeyTime) {
        Vector3 targetLookAtPoint = targetTransform.position - toRotate.position;
        targetLookAtPoint = new Vector3(targetLookAtPoint.x, toRotate.position.y, targetLookAtPoint.z);
        targetLookAtPoint.Normalize();
        float fracComplete = (Time.time - startTime) / journeyTime;
        targetLookAtPoint = Vector3.Slerp(toRotate.forward, targetLookAtPoint,
            fracComplete);
        targetLookAtPoint += toRotate.position;
        targetLookAtPoint.y = toRotate.position.y;
        player.transform.LookAt(targetLookAtPoint);
        if (fracComplete >= 1) {
            return true;
        }
        return false;
    }
}