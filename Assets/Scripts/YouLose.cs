using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class YouLose : MonoBehaviour {
    bool lost;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        lost = false;
        StartCoroutine(LoseCheck());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
	}

    IEnumerator LoseCheck () {
        for (;;) {
            yield return new WaitForSeconds(0.5f);
            int pigs = FindObjectsOfType<Pig>().Length;
            // TODO REMOVE
            pigs = 1;
            if (pigs == 0) {
                foreach (Poop poo in FindObjectsOfType<Poop>()) {
                    Destroy(poo.gameObject);
                }
                FindObjectOfType<Toolbar>().poopCounter.amount = 0;
                foreach (Transform child in transform) {
                    child.gameObject.SetActive(true);
                }
                lost = true;
            }
        }
    }

    public void DoRestart () {
        if (!lost) {
            return;
        }
        Debug.Log("Restart.");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(scene.buildIndex);
        SceneManager.LoadScene(scene.buildIndex);
    }
}
