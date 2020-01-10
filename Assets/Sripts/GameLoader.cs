using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour {
    public Text m_Text;
    public Button play;
    public Button quit;

    private AsyncOperation gameLoader;

    void Start() {
        //Call the LoadButton() function when the user clicks this Button
        play = GameObject.Find("Play").GetComponent<Button>();
        quit = GameObject.Find("Quit").GetComponent<Button>();
        m_Text = GameObject.Find("Loading").GetComponent<Text>();
        play.interactable = false;
        StartCoroutine(LoadScene());
        play.onClick.AddListener(LoadButton);
        quit.onClick.AddListener(QuitButton);
    }

    void LoadButton() {
        //Start loading the Scene asynchronously and output the progress bar
        gameLoader.allowSceneActivation = true;
    }

    void QuitButton() {
        Application.Quit();
    }

    IEnumerator LoadScene() {
        yield return null;
        gameLoader = SceneManager.LoadSceneAsync("Game");
        gameLoader.allowSceneActivation = false;
        while (!gameLoader.isDone) {
            m_Text.text = "<i>Loading progress: " + (gameLoader.progress * 100) + "%</i>";
            if (gameLoader.progress >= 0.9f) {
                m_Text.text = "<i>Game loaded!</i>";
                play.interactable = true;
            }

            yield return null;
        }
    }
}