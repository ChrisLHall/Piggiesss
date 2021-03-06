using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

    public bool isPaused; // True if paused, else false
    public GameObject background;
    public GameObject MainMenu;
    public GameObject Restart;
    public GameObject Sound;
    Text text;
    public Text sfxText;
    public Text musicText;

    void Start() {
        Time.timeScale = 1;
        isPaused = false;
        background.SetActive(false);
        text = GetComponentInChildren<Text>();
        MainMenu.SetActive(false);
        Restart.SetActive(false);
        Sound.SetActive(false);
        musicText.gameObject.SetActive(false);
    }
    

    public void PauseGame() {

        // Unpause.
        if (isPaused) {
            Time.timeScale = 1;
            background.SetActive(false);
            text.text = "";            
            MainMenu.SetActive(false);
            Restart.SetActive(false);
            Sound.SetActive(false);
            musicText.gameObject.SetActive(false);
            StartCoroutine(UnpauseWait());

        // Pause game.
        } else {
            Time.timeScale = 0;
            background.SetActive(true);
            text.text = "PAUSED:\nPress Pause Button \nAgain to Unpause";
            MainMenu.SetActive(true);
            Restart.SetActive(true);
            Sound.SetActive(true);
            musicText.gameObject.SetActive(true);
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

    public void ToggleSFX () {
        FindObjectOfType<AudioVol>().ToggleSFX();
    }

    public void ToggleMusic () {
        FindObjectOfType<AudioVol>().ToggleMusic();
    }

    // Toggles sound on and off and saves the user preferences.
    // If onOff == -1, then toggles sound, otherwise sets to value.
    public void Update () { // int onOff = -1) {
        /*
        bool audioState; // If true, audio is on; false, audio is off.

        // Initializing value.
        if (onOff > -1) {

            audioState = (onOff == 1);

        // If preferences exist flip them.
        } else if (PlayerPrefs.HasKey("sound")) {

            audioState = !(PlayerPrefs.GetInt("sound") == 1);

        // Otherwise set to false.
        } else {

            audioState = false;

        }

        AudioListener.pause = !audioState;
        PlayerPrefs.SetInt("sound", audioState ? 1 : 0);
        */
        // Set the text in the pause menu.
        var av = FindObjectOfType<AudioVol>();
        if (av.SFXOn) {
            sfxText.text = "Sound FX: On";
        } else {
            sfxText.text = "Sound FX: Off";
        }
        if (av.MusicOn) {
            musicText.text = "Music: On";
        } else {
            musicText.text = "Music: Off";
        }
    }
}
