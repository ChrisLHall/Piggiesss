using UnityEngine;
using System.Collections;

public class SkeleGhost : MonoBehaviour {
    const float CHASE_INTERVAL = 8f;
    const float CHASE_DELTA = 4f;
    const float MAX_SPEED = 5f;
    Transform target;

    const float DAMPING = 0.3f;
    const float ACCEL = 2.5f;
    Vector2 vel;

    SpriteRenderer srend;

	// Use this for initialization
	void Start () {
        srend = GetComponent<SpriteRenderer>();
        vel = Vector2.zero;
        StartCoroutine(ChooseTargets());
	}
	
	// Update is called once per frame
	void Update () {
        vel *= Mathf.Pow(DAMPING, Time.deltaTime);
	    if (target != null) {
            Vector3 added = (target.position - transform.position) * ACCEL * Time.deltaTime;
            vel += new Vector2(added.x, added.y);
        }
        vel = Vector2.ClampMagnitude(vel, MAX_SPEED);

        Vector2 newPos = vel * Time.deltaTime + new Vector2(transform.position.x, transform.position.y);
        newPos = Map.Inst.Bound(newPos);
        transform.position = new Vector3(newPos.x, newPos.y);

        srend.flipX = (vel.x < 0);
	}

    IEnumerator ChooseTargets () {
        for (;;) {
            Pig[] allPigs = FindObjectsOfType<Pig>();
            if (allPigs.Length > 0) {
                target = allPigs[Mathf.FloorToInt(Random.value * allPigs.Length)].transform;
            } else {
                target = null;
            }
            yield return new WaitForSeconds(CHASE_INTERVAL + Random.value * CHASE_DELTA);
        }
    }
}
