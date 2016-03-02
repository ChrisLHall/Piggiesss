using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	void Update () {
        Farmer f = FindObjectOfType<Farmer>();
        if (f != null) {
            transform.position = new Vector3(f.transform.position.x, f.transform.position.y, transform.position.z);
        }
	}
}
