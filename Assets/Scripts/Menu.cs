using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Canvas timeCanvas;
    public Canvas instCanvas;
    public Canvas highScoreCanvas;
    public Canvas creditsCanvas;

    public HighScoreGUI highScoreGUI;

	// Use this for initialization
    void Start () {
        instCanvas.enabled = false;
        timeCanvas.enabled = false;
        highScoreCanvas.enabled = false;
        creditsCanvas.enabled = false;
    }
	
    public void ViewPlay() {
        timeCanvas.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }

    public void ViewInstructions() {
        instCanvas.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }

    public void ViewHighScores () {
        highScoreGUI.RefreshScores();
        highScoreCanvas.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }

    public void ViewCredits () {
        creditsCanvas.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }

    public void PlayWithTime(int duration) {
        PlayerPrefs.SetInt("Duration", duration);
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu() {
        timeCanvas.enabled = false;
        instCanvas.enabled = false;
        highScoreCanvas.enabled = false;
        creditsCanvas.enabled = false;
        GetComponent<Canvas>().enabled = true;
    }

}
