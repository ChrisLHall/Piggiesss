using UnityEngine;
using System.Collections;
using System.Linq;

public class Pig : MonoBehaviour {

	public LeftRightSprite[] regSprites;
    public LeftRightSprite[] sickSprites;
    public LeftRightSprite[] infectedSprites;
    LeftRightSprite[] sprites;
    readonly int[] GRASS_PER_AGE = new int[] { 3, 9, 20 };
	private int sprite;
    bool left;
	SpriteRenderer sr;

    const float SICK_TIME = 10f;
    Coroutine sickRoutine;
    public bool infectious;
    bool sick;
    int infections;
    const int INFECTIONS_TO_DIE = 2;

	/* Configuration:
	 * meanMoveDelay - the average amount of time in seconds between jumps
	 * devMoveDelay - how much the jump delays should deviate
	 * jumpRange - how far should the pig jump
	 * jumpDuration - how long is the jump (seconds)
	 * jumpHeight - how high should the pig jump 
	 */
	const float meanMoveDelay = 2f;
	const float devMoveDelay = 1f;
    const float hungerDelayReduction = 0.4f;
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
    
    const float STARVE_TIME = 30f;
    Coroutine starveCoroutine;

    int poopLeft;
    const int POOPS_PER_GRASS = 2;
    int grassEaten;
    GameObject targetObj;

    public bool Hungry { get { return poopLeft == 0; } }

    public GameObject deadPrefab;

    Coroutine randomInfectCoroutine;

    void Awake () {
        sprites = regSprites;
        left = true;
        poopLeft = POOPS_PER_GRASS;
        grassEaten = 0;
        targetObj = null;
        infections = 0;
        sick = false;
    }

	// Use this for initialization
	void Start () {
		sr = transform.FindChild("Graphics").GetComponent<SpriteRenderer>();
		sprite = 0;
		sr.sprite = sprites[sprite].leftSprite;
		state = PigState.Idle;
		ScheduleJump();
        poopCoroutine = StartCoroutine(PoopSometimes());
        starveCoroutine = StartCoroutine(Starve());
        randomInfectCoroutine = StartCoroutine(RandomlyInfectSometime());
        
        if (infectious) {
            sprites = infectedSprites;
            UpdateSprite();
        }
    }

	private void ScheduleJump() {
        float timeDelay = Random.Range(meanMoveDelay - devMoveDelay,
                meanMoveDelay + devMoveDelay);
        if (Hungry && !infectious) {
            timeDelay *= hungerDelayReduction;
        }
		StartCoroutine(JumpWithDelay(timeDelay));
	}

	/* Delays by a random amount
	 * and then jump
	 */
	IEnumerator JumpWithDelay(float delay) {
		yield return new WaitForSeconds(delay);
        targetObj = null;
        if (infectious) {
            TargetCleanPig();
        } else {
            TargetGrassIfHungry();
        }
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
            left = false;
		} else {
            left = true;
		}
        UpdateSprite();
		state = PigState.Jumping;
		jumpStartTime = Time.time;
	}

    void UpdateSprite () {
        sr.sprite = left ? sprites[sprite].leftSprite : sprites[sprite].rightSprite;
    }

    IEnumerator PoopSometimes () {
        for (;;) {
            while (poopLeft > 0) {
                yield return new WaitForSeconds(POOP_DELAY + (-0.5f + Random.value)
                        * POOP_DELAY_RAND);
                if (!infectious) {
                    Poop();
                    poopLeft--;
                }
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

        StopCoroutine(starveCoroutine);
        starveCoroutine = StartCoroutine(Starve());
    }

    IEnumerator Starve () {
        yield return new WaitForSeconds(STARVE_TIME + Random.value * 3f);
        if (infectious) {
            Die(true, false);
        } else {
            Die(false, false);
        }
    }

    public void Die (bool isSkeleton, bool cleanSkeleton) {
        Destroy(gameObject);
        DeadPig dead = Instantiate(deadPrefab).GetComponent<DeadPig>();
        dead.transform.position = transform.position;
        if (!isSkeleton) {
            dead.SetSprites(sprite, left);
        } else {
            dead.SetSkeleton(cleanSkeleton);
        }
    }

    void TargetGrassIfHungry () {
        if (Hungry) {
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

    void TargetCleanPig () {
        Pig[] allPigs = FindObjectsOfType<Pig>();
        if (allPigs.Length > 0) {
            Pig pigTarg = allPigs.OrderBy((Pig p) => {
                if (p.sick || p.infectious || p == this) {
                    return float.PositiveInfinity;
                }
                return (p.transform.position - transform.position).sqrMagnitude;
            }).First();
            if (!pigTarg.sick && !pigTarg.infectious && pigTarg != this) {
                targetObj = pigTarg.gameObject;
            }
        }
    }

    public void MakeSick () {
        if (sick == true) {
            return;
        }
        sick = true;
        sickRoutine = StartCoroutine(GetSick());
    }

    IEnumerator GetSick () {
        sprites = sickSprites;
        UpdateSprite();
        yield return new WaitForSeconds(SICK_TIME);
        infectious = true;
        sprites = infectedSprites;
        UpdateSprite();
    }

    public void Cure () {
        sprites = regSprites;
        UpdateSprite();
        if (sickRoutine != null) {
            StopCoroutine(sickRoutine);
        }
        infectious = false;
        sick = false;
    }

    IEnumerator RandomlyInfectSometime () {
        for (;;) {
            yield return new WaitForSeconds(5f + Random.value * 5f);
            float exp = 1f - Mathf.Exp(-(Time.timeSinceLevelLoad) / 120f);
            float poopExp = 1f - Mathf.Exp(-FindObjectsOfType<Poop>().Length / 50f);
            float threshold = (exp + poopExp) * 0.1f;
            if (Random.value < threshold) {
                MakeSick();
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
        if (otherGrass != null && otherGrass.Edible && Hungry && !infectious) {
            Destroy(other.gameObject);
            Eat();
        }
        Cure otherCure = other.gameObject.GetComponent<Cure>();
        if (otherCure != null) {
            Cure();
        }
    }

    void OnCollisionEnter2D (Collision2D coll) {
        GameObject other = coll.gameObject;
        Debug.Log("poke");
        Pig otherPig = other.GetComponent<Pig>();
        if (infectious && otherPig != null && !otherPig.infectious && !otherPig.sick) {
            otherPig.MakeSick();
            /*
            StopCoroutine(starveCoroutine);
            starveCoroutine = StartCoroutine(Starve());
            */
            infections++;
            if (infections > INFECTIONS_TO_DIE) {
                Die(true, false);
            }
        }
    }
}
