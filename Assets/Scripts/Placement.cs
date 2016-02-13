using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour {
    public GameObject grassPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonDown(0)) {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject instance = Instantiate<GameObject>(grassPrefab);
            instance.transform.position = new Vector3(point.x, point.y, 0f);
        }
	}
}
