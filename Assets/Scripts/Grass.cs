using UnityEngine;
using System.Collections;

public class Grass : MonoBehaviour {
    public Sprite[] states;
    int stateIndex;
    SpriteRenderer sr;
    const float growDelay = 3f;
    public int grassLimit;

    public bool Edible {
        get {
            return (stateIndex == states.Length - 1);
        }
    }

    void Start() {
        if (GameObject.FindGameObjectsWithTag("Grass").Length > grassLimit) {
            Destroy(this.gameObject);
        }
    }
    
	void Awake () {
        sr = transform.FindChild("Graphics").GetComponent<SpriteRenderer>();
        StartCoroutine(GrowRoutine(growDelay));
	}
	
    IEnumerator GrowRoutine (float growDelay) {
        while (stateIndex < states.Length - 1) {
            yield return new WaitForSeconds(growDelay);
            stateIndex++;
            sr.sprite = states[stateIndex];
        }
    }
}
