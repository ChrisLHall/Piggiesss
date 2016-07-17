using UnityEngine;
using System.Collections;

public class URLOpener : MonoBehaviour {
    float lastOpen = 0f;
    public void OpenThis (string url) {
        if (Time.time > lastOpen + 5f) {
            Application.OpenURL(url);
            lastOpen = Time.time;
        }
    }
}
