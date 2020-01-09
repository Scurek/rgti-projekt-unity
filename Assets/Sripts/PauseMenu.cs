using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private Button resumeButoon;
    private Button restartButoon;
    private Text restartButoonText;
    private Button exitButoon;
    
    private Game game;
    void Start() {
        resumeButoon = transform.Find("Resume").GetComponent<Button>();
        resumeButoon.onClick.AddListener(resumeButoonF);
        restartButoon = transform.Find("Restart").GetComponent<Button>();
        restartButoon.interactable = false;
        restartButoonText = restartButoon.gameObject.transform.Find("Text").GetComponent<Text>();
        restartButoon.onClick.AddListener(restartButoonF);
        exitButoon = transform.Find("Exit").GetComponent<Button>();
        exitButoon.onClick.AddListener(exitButoonF);
        game = Game.SharedInstance;
        // Debug.Log(Game.SharedInstance);
        // gameObject.SetActive(false);
    }

    private void OnEnable() {
        if (!game)
            return;
        if (game.currentCheckpoint > 0) {
            restartButoon.interactable = true;
        }
        else {
            restartButoon.interactable = false;
        }
    }
    
    private void resumeButoonF() {
        Game.SharedInstance.resumeGame();
    }

    private void restartButoonF() {
        Game.SharedInstance.teleportPlayer(Game.SharedInstance.spawnPosition);;
    }

    public void restartButtonOnHoverEnter() {
        if (!restartButoon.interactable)
            restartButoonText.text = "Reach a checkpoint first!";
    }
    
    public void restartButtonOnHoverExit() {
        if (!restartButoon.interactable)
            restartButoonText.text = "Restart";
    }

    private void exitButoonF() {
        // SceneManager.UnloadSceneAsync("Game");
        SceneManager.LoadScene("MainMenu");
    }
    
}
