using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
    public float minTime;
    public float deltaTime;
    public GameObject spawnThis;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnSometimes());
	}
	
    IEnumerator SpawnSometimes () {
        for (;;) {
            yield return new WaitForSeconds(minTime + Random.value * deltaTime);
            Instantiate(spawnThis, transform.position, transform.rotation);
        }
    }
}
