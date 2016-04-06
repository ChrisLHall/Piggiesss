using UnityEngine;
using System.Collections;

public class PooGolem : MonoBehaviour {
    Animator anim;
    SpriteRenderer srend;
    static Vector3[] offsets = new Vector3[] {
        Vector3.up + Vector3.right,
        Vector3.down + Vector3.right,
        Vector3.down + Vector3.left,
        Vector3.up + Vector3.left,
    };
    const float OFFSET_MULT = 0.4f;
    const float WALK_SPEED = 0.2f;

    Vector3 origPos;

	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
        srend = GetComponentInChildren<SpriteRenderer>();
        origPos = transform.position;
        StartCoroutine(WalkInCircles());
	}

    IEnumerator WalkInCircles () {
        if (Random.value < 0.5f) {
            // forward loop
            for (int i = 0; i < offsets.Length; i++) {
                Vector3 target = Map.Inst.Bound(origPos + offsets[i] * OFFSET_MULT);
                yield return StartCoroutine(MeanderToPoint(target, 5f));
                yield return new WaitForSeconds(3f);
                if (i == offsets.Length - 1) {
                    i -= offsets.Length;
                }
            }
        } else {
            // backward loop
            for (int i = offsets.Length - 1; i >= 0; i--) {
                Vector3 target = Map.Inst.Bound(origPos + offsets[i] * OFFSET_MULT);
                yield return StartCoroutine(MeanderToPoint(target, 5f));
                yield return new WaitForSeconds(3f);
                if (i == 0) {
                    i += offsets.Length;
                }
            }
        }
    }

    IEnumerator MeanderToPoint (Vector3 point, float time) {
        srend.flipX = (point.x > transform.position.x);
        anim.SetBool("walking", true);
        while ((transform.position - point).sqrMagnitude > 0.01) {
            transform.position += Vector3.ClampMagnitude(point - transform.position, WALK_SPEED * 0.02f);
            yield return new WaitForSeconds(0.02f);
        }
        anim.SetBool("walking", false);
    }
}
