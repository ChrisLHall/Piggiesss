using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	void Update () {
        Farmer f = FindObjectOfType<Farmer>();
        Vector3 pos = f.CameraLookPos;
        if (f != null) {
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
	}
}
