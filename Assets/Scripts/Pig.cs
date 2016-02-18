using UnityEngine;
using System.Collections;

public class Pig : MonoBehaviour {

	public LeftRightSprite[] sprites;
	private int sprite;
	SpriteRenderer sr;

	/* Configuration:
	 * meanMoveDelay - the average amount of time in seconds between jumps
	 * devMoveDelay - how much the jump delays should deviate
	 * jumpRange - how far should the pig jump
	 * jumpDuration - how long is the jump (seconds)
	 * jumpHeight - how high should the pig jump
	 */
	const float meanMoveDelay = 2f;
	const float devMoveDelay = 1f;
	const float jumpRange = 0.2f;
	const float jumpDuration = 0.5f;
	const float jumpHeight = 0.03f;

    const float POOP_DELAY = 3f;
    const float POOP_DELAY_RAND = 1f;

	private Vector2 startPos;
	private Vector2 target;

	private float jumpStartTime;

	public enum PigState {Jumping, Idle};
	private PigState state;

    public GameObject poopPrefab;
    Coroutine poopCoroutine;

    int poopLeft;
    const int POOPS_PER_GRASS = 2;

    void Awake () {
        poopLeft = POOPS_PER_GRASS;
    }

	// Use this for initialization
	void Start () {
		sr = transform.FindChild("Graphics").GetComponent<SpriteRenderer>();
		sprite = 0;
		sr.sprite = sprites[sprite].leftSprite;
		state = PigState.Idle;
		ScheduleJump();
        poopCoroutine = StartCoroutine(PoopSometimes());
	}

	private void ScheduleJump() {
		StartCoroutine(JumpWithDelay(Random.Range(meanMoveDelay - devMoveDelay, meanMoveDelay + devMoveDelay)));
	}

	/* Delays by a random amount
	 * and then jump
	 */
	IEnumerator JumpWithDelay(float delay) {
		yield return new WaitForSeconds(delay);
		target = Map.Bound(Random.insideUnitCircle * jumpRange + (Vector2) transform.position);
		startPos = transform.position;
		if (target.x > transform.position.x) {
			sr.sprite = sprites[sprite].rightSprite;
		} else {
			sr.sprite = sprites[sprite].leftSprite;
		}
		state = PigState.Jumping;
		jumpStartTime = Time.time;
	}

    IEnumerator PoopSometimes () {
        while (poopLeft > 0) {
            yield return new WaitForSeconds(POOP_DELAY + (-0.5f + Random.value)
                    * POOP_DELAY_RAND);
            Poop();
            poopLeft--;
        }
    }

    void Poop () {
        GameObject newPoop = Instantiate<GameObject>(poopPrefab);
        newPoop.transform.position = transform.position;
        Debug.Log("POOO");
    }
		
	// Update is called once per frame
	void Update () {
		if (state == PigState.Jumping) {
			float duration = Time.time - jumpStartTime;
			float progress = duration / jumpDuration;
			Vector2 vertical = Vector2.up * Mathf.Sin (Mathf.PI * progress) * jumpHeight;
			transform.position = Vector2.Lerp (startPos, target, duration / jumpDuration) + vertical;
			if (duration > jumpDuration) {
				state = PigState.Idle;
				ScheduleJump ();
			}
		}
	}
}
