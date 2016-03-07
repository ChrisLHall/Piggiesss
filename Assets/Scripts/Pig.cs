using UnityEngine;
using System.Collections;
using System.Linq;

public class Pig : MonoBehaviour {

	public LeftRightSprite[] sprites;
    readonly int[] GRASS_PER_AGE = new int[] { 3, 9, 20 };
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
    int grassEaten;
    GameObject targetObj;

    void Awake () {
        poopLeft = POOPS_PER_GRASS;
        grassEaten = 0;
        targetObj = null;
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
        TargetGrassIfHungry();
        if (targetObj == null) {
            target = Map.Inst.Bound(Random.insideUnitCircle * jumpRange
                    + (Vector2)transform.position);
        } else {
            Vector3 vecToTarget = targetObj.transform.position
                    - transform.position;
            vecToTarget = Vector3.ClampMagnitude(vecToTarget, jumpRange);
            vecToTarget += Random.insideUnitSphere * jumpRange * 0.1f;
            target = Map.Inst.Bound(transform.position + vecToTarget);
        }

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
        for (;;) {
            while (poopLeft > 0) {
                yield return new WaitForSeconds(POOP_DELAY + (-0.5f + Random.value)
                        * POOP_DELAY_RAND);
                Poop();
                poopLeft--;
            }
            while (poopLeft <= 0) {
                yield return new WaitForSeconds(0.2f);

            }
        }
    }

    void Poop () {
        GameObject newPoop = Instantiate<GameObject>(poopPrefab);
        newPoop.transform.position = transform.position;
        Debug.Log("POOO");
    }

    void Eat () {
        poopLeft += POOPS_PER_GRASS;
        grassEaten++;
        if (sprite < sprites.Length - 1 && grassEaten >= GRASS_PER_AGE[sprite]) {
            sprite++;
        }
    }

    void TargetGrassIfHungry () {
        if (poopLeft == 0) {
            Grass[] allGrass = FindObjectsOfType<Grass>();
            if (allGrass.Length > 0) {
                Grass grassTarg = allGrass.OrderBy((Grass g) => {
                            if (!g.Edible) {
                                return float.PositiveInfinity;
                            }
                            return (g.transform.position - transform.position).sqrMagnitude;
                        }).First();
                if (grassTarg.Edible) {
                    targetObj = grassTarg.gameObject;
                }
            }
        }
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

    void OnTriggerStay2D (Collider2D other) {
        Grass otherGrass = other.gameObject.GetComponent<Grass>();
        if (otherGrass != null && otherGrass.Edible && poopLeft == 0) {
            Destroy(other.gameObject);
            Eat();
        }
    }
}
