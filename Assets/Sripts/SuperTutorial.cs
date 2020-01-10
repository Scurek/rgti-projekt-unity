using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject SuperT;
    void Start() {
        Game.SharedInstance.SuperModeTutorial = SuperT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
