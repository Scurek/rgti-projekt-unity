using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rocket : MonoBehaviour {
    // Start is called before the first frame update
    public float velocity = 2;
    private PlayerMovement playerMovement;
    private Vector3 direction;
    private bool visible;
    private AudioSource explosionSound;
    private ParticleSystem explosion;
    private Light rocketLight;
    private ParticleSystem flame;
    private MeshRenderer rocket;

    public float remainingTime;

    void Start() {
        gameObject.SetActive(false);
        explosionSound = GetComponent<AudioSource>();
        rocket = GetComponent<MeshRenderer>();
        explosion = transform.Find("TinyExplosion").GetComponent<ParticleSystem>();
        rocketLight = transform.Find("Point Light").GetComponent<Light>();
        flame = transform.Find("RocketFlame").GetComponent<ParticleSystem>();
    }

    public void InitRocket(Transform transform, Vector3 targetPoint, PlayerMovement playerMovement) {
        // this.transform.rotation = transform.rotation;
        flame.Play();
        rocketLight.enabled = true;
        rocket.enabled = true;
        direction = Vector3.Normalize(targetPoint - transform.position);
        this.transform.position = transform.position;
        this.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        this.transform.Rotate(0, -90f, 0);
        this.playerMovement = playerMovement;
        visible = true;
        gameObject.SetActive(true);
        remainingTime = 2;
        // Invoke("DestroyRocket", 2f);
    }

    public void DestroyRocket() {
        if (visible) {
            // CancelInvoke();
            visible = false;
            flame.Stop();
            // rocketLight.enabled = false;
            rocket.enabled = false;
            Explode();
            // gameObject.SetActive(false);
            ExplodeGFX();
            if (isActiveAndEnabled)
                StartCoroutine(SetInactiveAfterTime(2f));
        }
    }

    IEnumerator SetInactiveAfterTime(float time) {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate() {
        // transform.Translate( Time.deltaTime * direction);
        if (visible) {
            transform.position += direction * (velocity * Game.SharedInstance.globalSpeedMult);
            remainingTime -= Time.deltaTime * Game.SharedInstance.globalSpeedMult;
            if (remainingTime <= 0) {
                DestroyRocket();
            }
        }
    }

    void Explode() {
        AudioSource.PlayClipAtPoint(explosionSound.clip, transform.position);
        // explosionSound.Stop();
        // explosionSound.Play();
        playerMovement.explosionPush(this.transform.position);
    }


    void ExplodeGFX() {
        if (isActiveAndEnabled) {
            explosion.Play();
            StartCoroutine(explosionLightEffect(0.05f, 0.75f));
        }
    }

    private IEnumerator explosionLightEffect(float time1, float time2) {
        while (rocketLight.intensity < 35.0f) {
            rocketLight.intensity += (Time.deltaTime / time1) * 20;
            rocketLight.range += (Time.deltaTime / time1) * 20;
            yield return null;
        }

        // yield return new WaitForSeconds(1);
        while (rocketLight.intensity > 0.0f) {
            rocketLight.intensity -= (Time.deltaTime / time2) * 35;
            rocketLight.range -= (Time.deltaTime / time2) * 35;
            yield return null;
        }

        rocketLight.enabled = false;
        rocketLight.intensity = 15f;
        rocketLight.range = 15f;
    }


    private void OnTriggerEnter(Collider other) {
        if (!other.isTrigger && visible && !(other.gameObject && other.gameObject.name == "Player")) {
            DestroyRocket();
        }
    }
}