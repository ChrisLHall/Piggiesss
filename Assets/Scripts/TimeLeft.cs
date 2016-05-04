using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeLeft : MonoBehaviour {
    System.TimeSpan timeLeft;
    public float timeLimitMins;
    Text text;

    public bool Done {
        get {
            return timeLeft <= System.TimeSpan.Zero;
        }
    }

	// Use this for initialization
	void Start () {
        timeLeft = System.TimeSpan.FromMinutes(timeLimitMins);
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        timeLeft -= System.TimeSpan.FromSeconds(Time.deltaTime);
	    if (!Done) {
            text.text = "Time left: " + timeLeft.Minutes + ":" + timeLeft.Seconds.ToString("D2") + "." + timeLeft.Milliseconds.ToString("D3");
        } else {
            text.text = "Time up!";
        }
	}
}
