using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class YouLose : MonoBehaviour {
    bool lost;
    TimeLeft tl;
    Text text;
    float lossTime = 0;
    const float RESTART_DELAY = 6f;

	// Use this for initialization
	void Start () {
        tl = FindObjectOfType<TimeLeft>();
        text = GetComponentInChildren<Text>();
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        lost = false;
        StartCoroutine(LoseCheck());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)
                && FindObjectOfType<Toolbar>().ToolMode == FarmerActionType.Move) {
            Application.Quit();
        }
	}

    IEnumerator LoseCheck () {
        for (;;) {
            yield return new WaitForSeconds(0.1f);
            if (tl.Done) {
                foreach (Poop poo in FindObjectsOfType<Poop>()) {
                    Destroy(poo.gameObject);
                }
                Toolbar t = FindObjectOfType<Toolbar>();
                t.poopCounter.amount = 0;
                foreach (Transform child in transform) {
                    child.gameObject.SetActive(true);
                }
                text.text = "You Lose! Final score: " + t.scoreCounter.amount + "\n\nRestart";
                lossTime = Time.time;
                lost = true;
            }

            int pigs = FindObjectsOfType<Pig>().Length;
            // TODO REMOVE
            /*
            pigs = 1;
            if (pigs == 0) {
                foreach (Poop poo in FindObjectsOfType<Poop>()) {
                    Destroy(poo.gameObject);
                }
                FindObjectOfType<Toolbar>().poopCounter.amount = 0;
                foreach (Transform child in transform) {
                    child.gameObject.SetActive(true);
                }
                lossTime = Time.time;
                lost = true;
            }
            */
        }
    }

    public void DoRestart () {
        if (!lost || Time.time < lossTime + RESTART_DELAY) {
            return;
        }
        Debug.Log("Restart.");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(scene.buildIndex);
        SceneManager.LoadScene(scene.buildIndex);
    }
}
