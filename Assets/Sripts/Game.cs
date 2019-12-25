using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Game SharedInstance;
    
    public GameObject rocket;
    private static List <Rocket> rocketPool;
    public int rocketPoolSize;
    
    void Awake() {
        SharedInstance = this;
    }
    private void Start() {
        rocketPool = new List<Rocket>();
        for (int i = 0; i < rocketPoolSize; i++) {
            rocketPool.Add(Instantiate(rocket).GetComponent<Rocket>());
        }
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
