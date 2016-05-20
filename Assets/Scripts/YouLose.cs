using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class YouLose : MonoBehaviour {
    bool lost;
    TimeLeft tl;
    Text text;
    float lossTime = 0;
    const float RESTART_DELAY = 6f;
    public Counter score;
    public bool endGame;
    public bool resetScores;
    bool prevSavedScore;
    public GameObject EndBG;


	// Use this for initialization
	void Start () {
        EndBG.SetActive(false);
        tl = FindObjectOfType<TimeLeft>();
        text = GetComponentInChildren<Text>();
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        lost = false;
        prevSavedScore = false;
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
            if ((tl.Done || endGame) && !lost) {

                EndBG.SetActive(true);
                Toolbar t = FindObjectOfType<Toolbar>();
                t.poopCounter.amount = 0;
                FindObjectOfType<Farmer>().GetComponent<Collider2D>().enabled = false;
                foreach (Transform child in transform) {
                    child.gameObject.SetActive(true);
                }
                text.text = "Time up!\nFinal score: " + t.scoreCounter.amount + "\nTap to restart";
                lossTime = Time.time;
                lost = true;

                int current = score.amount;
                int duration = PlayerPrefs.GetInt("Duration");
                List<string> scores = null;

                // If the scores were already saved.
                if (!prevSavedScore) {

                    string savedScores = "";

                    if (!PlayerPrefs.HasKey("scores" + duration.ToString()) || resetScores) {
                        savedScores = ",0,0,0,0";
                        PlayerPrefs.SetString("scores"  + duration.ToString(), current.ToString() + savedScores);
                        PlayerPrefs.Save();
                        resetScores = false;
                    } else {

                        // Load the current high scores and compare with the current.
                        scores = new List<string>(PlayerPrefs.GetString("scores"  + duration.ToString()).Split(','));
                        for (int i = 0; i < 5; i ++) {
                            if (current > Int32.Parse(scores[i])) {
                                scores.Insert(i, current.ToString());
                                break;
                            }
                        }

                        while (scores.Count > 5) {
                            scores.RemoveAt(scores.Count - 1);
                        }
                        // Save the newly found high scores to a string and save.
                        savedScores += string.Join(",", scores.ToArray());

                        PlayerPrefs.SetString("scores"  + duration.ToString(), savedScores);
                        PlayerPrefs.Save();

                        prevSavedScore = true;
                        Debug.Log("Current savedScores: " + savedScores);
                    }

                }

                // Display high scores.
                string durationSuffix = " minutes\n";
                if (duration == 1) {
                    durationSuffix = " minute\n";
                }
                text.text += "\n\nHigh scores - " + duration.ToString() + durationSuffix;
                for (int i = 0; i < 5; i++) {
                    if (Int32.Parse(scores[i]) > 0) {
                        text.text += scores[i] + "\n";
                    }
                }
            }
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
