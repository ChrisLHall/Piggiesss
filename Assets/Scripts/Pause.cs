using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

    public bool isPaused; // True if paused, else false
    public GameObject background;
    public GameObject MainMenu;
    public GameObject Restart;
    Text text;

    void Start() {
        Time.timeScale = 1;
        isPaused = false;
        background.SetActive(false);
        text = GetComponentInChildren<Text>();
        MainMenu.SetActive(false);
        Restart.SetActive(false);
    }
	

    public void PauseGame() {

        // Unpause.
        if (isPaused) {
            Time.timeScale = 1;
            background.SetActive(false);
            text.text = "";            
            MainMenu.SetActive(false);
            Restart.SetActive(false);
            StartCoroutine(UnpauseWait());

        // Pause game.
        } else {
            Time.timeScale = 0;
            background.SetActive(true);
            text.text = "PAUSED\nPress Pause Button \nAgain to Unpause";
            MainMenu.SetActive(true);
            Restart.SetActive(true);
            isPaused = !isPaused;
        }
    }

    // Delay after unpause so no move is enqueued immediately following
    // unpause.
    IEnumerator UnpauseWait() {
        yield return new WaitForSeconds(.1f);
        isPaused = !isPaused;
    }

    public void ReturnToMainMenu() {
        SceneManager.UnloadScene("Menu");
        SceneManager.LoadScene("Menu");
    }

    public void RestartLevel() {
        SceneManager.UnloadScene("onthefarm");
        SceneManager.LoadScene("onthefarm");
    }
}
