using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Canvas timeCanvas;
    public Canvas instCanvas;

	// Use this for initialization
	void Start () {
       timeCanvas = timeCanvas.GetComponent<Canvas>();
	   instCanvas = instCanvas.GetComponent<Canvas>();
       instCanvas.enabled = false;
       timeCanvas.enabled = false;
	}
	
    public void ViewPlay() {
        timeCanvas.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }

    public void ViewInstructions() {
        instCanvas.enabled = true;
        GetComponent<Canvas>().enabled = false;
    }

    public void PlayWithTime(int duration) {
        PlayerPrefs.SetInt("Duration", duration);
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu() {
        timeCanvas.enabled = false;
        instCanvas.enabled = false;
        GetComponent<Canvas>().enabled = true;
    }

}
