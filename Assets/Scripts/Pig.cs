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
    const int PIG_LIMIT = 30;

    const float SICK_TIME = 0f;
    Coroutine sickRoutine;
    public bool infectious;
    bool sick;
    int infections;
    const int INFECTIONS_TO_DIE = 2;
    public bool followPigsWhenInfected = false; 
    public static int numPigs = 0;

	/* Configuration:
	 * meanMoveDelay - the average amount of time in seconds between jumps
	 * devMoveDelay - how much the jump delays should deviate
	 * jumpRange - how far should the pig jump
	 * jumpDuration - how long is the jump (seconds)
	 * jumpHeight - how high should the pig jump 
	 */
	const float meanMoveDelay = 1.2f;
	const float devMoveDelay = 0.5f;
    const float hungerDelayReduction = 0.4f;
	const float jumpRange = 0.3f;
    const float JUMP_DUR = 0.4f;
	float jumpDuration;
	const float jumpHeight = 0.03f;

    const float POOP_DELAY = 2.5f;
    const float POOP_DELAY_RAND = 1f;

	private Vector2 startPos;
	private Vector2 target;

	private float jumpStartTime;

	public enum PigState {Jumping, Idle};
	private PigState state;

    public GameObject poopPrefab;
    Coroutine poopCoroutine;
    public AudioClip[] nonPoopSounds;
    public AudioClip[] poopSounds;

    const float STARVE_TIME = 20f;
    Coroutine starveCoroutine;
    public GameObject infectCloudPrefab;

    int poopLeft;
    const int POOPS_PER_GRASS = 2;
    int grassEaten;
    GameObject targetObj;

    public bool Hungry { get { return poopLeft == 0; } }

    public SkeleGhost ScaredOf { get; private set; }
    public bool Scared {
        get {
            return ScaredOf != null && Vector3.Distance(transform.position, ScaredOf.transform.position) < 0.7f;
        }
    }
    const float scaredDelayReduction = 0.1f;

    public GameObject deadPrefab;

    Coroutine randomInfectCoroutine;

    public AudioClip[] coughs;

    void Awake () {
        jumpDuration = JUMP_DUR;
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
        if (Scared && !infectious) {
            timeDelay *= scaredDelayReduction;
        } else if (Hungry && !infectious) {
            timeDelay *= hungerDelayReduction;
        }
		StartCoroutine(JumpWithDelay(timeDelay));
	}

	/* Delays by a random amount
	 * and then jump
	 */
	IEnumerator JumpWithDelay(float delay) {
		yield return new WaitForSeconds(delay);
        jumpDuration = (Scared) ? JUMP_DUR / 3f : JUMP_DUR;
        targetObj = null;
        if (infectious && followPigsWhenInfected) {
            TargetCleanPig();
        } else if (!infectious && Scared) {
            RunAway();
        } else if (!infectious) {
            TargetGrassIfHungry();
        }
        if (targetObj == null) {
            target = Map.Inst.Bound(Random.insideUnitCircle * jumpRange
                    + (Vector2)transform.position);
        } else {
            Vector3 vecToTarget = targetObj.transform.position
                    - transform.position;
            vecToTarget = Vector3.ClampMagnitude(vecToTarget, jumpRange);
            float extra = (Scared) ? 0.7f : 0.1f;
            vecToTarget += Random.insideUnitSphere * jumpRange * extra;
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
        AudioClip[] sounds = Toolbar.inst.ALLOW_POOP ? poopSounds : nonPoopSounds;
        GetComponent<AudioSource>().PlayOneShot(
                sounds[Random.Range(0, sounds.Length)]);
    }

    void Eat () {
        poopLeft += POOPS_PER_GRASS;
        grassEaten++;
        if (sprite < sprites.Length - 2 && grassEaten >= GRASS_PER_AGE[sprite]) {
            sprite++;
        }

        StopCoroutine(starveCoroutine);
        starveCoroutine = StartCoroutine(Starve());
    }

    IEnumerator Starve () {
        if (infectious) {
            yield return new WaitForSeconds(STARVE_TIME + Random.value * 5f);
        } else {
            yield return new WaitForSeconds(60f);
        }
        yield return StartCoroutine(Shake(0.1f, 2f));
        
        if (infectious) {
            Die(true, false);
            Instantiate(infectCloudPrefab, transform.position, transform.rotation);
        } else {
            Die(false, false);
        }
    }

    IEnumerator Shake (float maxDist, float time) {
        Vector3 origPos = transform.position;
        for (float t = 0f; t < time; t += 0.05f) {
            Vector2 off = Random.insideUnitCircle;
            transform.position = origPos + new Vector3(off.x, off.y, 0f) * maxDist * (t / time);
            yield return new WaitForSeconds(0.05f);
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

    void RunAway () {
        targetObj = gameObject;
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
        for (int i = 0; i < 3; i++) {
            GetComponent<AudioSource>().PlayOneShot(
                    coughs[Random.Range(0, coughs.Length)]);
            yield return new WaitForSeconds(SICK_TIME / (float)3);
        }
        infectious = true;
        sprites = infectedSprites;
        UpdateSprite();
        StopCoroutine(starveCoroutine);
        starveCoroutine = StartCoroutine(Starve());
        SwipeHelp.inst.PigGotSick(transform);
    }

    public void Cure () {
        sprites = regSprites;
        UpdateSprite();
        if (sickRoutine != null) {
            StopCoroutine(sickRoutine);
        }
        if (infectious || sick) {
            infectious = false;
            sick = false;
            StopCoroutine(starveCoroutine);
            starveCoroutine = StartCoroutine(Starve());
        }
        SwipeHelp.inst.PigGotCured(transform);
    }

    IEnumerator RandomlyInfectSometime () {
        for (;;) {
            yield return new WaitForSeconds(10f + Random.value * 5f);
            float exp = 1f - Mathf.Exp(-(Time.timeSinceLevelLoad) / 600f);
            float poopExp = 1f - Mathf.Exp(-FindObjectsOfType<Poop>().Length / 1000f);
            float threshold = exp * 0.2f + poopExp * 0.02f;
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
        UpdateScaredOf();
        
	}
    void UpdateScaredOf () {
        SkeleGhost[] ghosts = FindObjectsOfType<SkeleGhost>();
        if (ghosts.Length == 0) {
            ScaredOf = null;
            return;
        }
        float dist = float.PositiveInfinity;
        SkeleGhost nearGhost = ghosts[0];
        foreach (SkeleGhost ghost in ghosts) {
            float newDist = Vector3.Distance(ghost.transform.position, transform.position);
            if (newDist < dist) {
                nearGhost = ghost;
            }
        }
        ScaredOf = nearGhost;
    }

    void OnTriggerStay2D (Collider2D other) {
        Grass otherGrass = other.gameObject.GetComponent<Grass>();
        if (otherGrass != null && otherGrass.Edible && Hungry && !infectious && !Scared) {
            Destroy(other.gameObject);
            Eat();
        }
        Pop otherCure = other.gameObject.GetComponent<Pop>();
        if (otherCure != null) {
            if (otherCure.type == PopType.CURE) {
                Cure();
            } else if (otherCure.type == PopType.INFECT) {
                MakeSick();
            }
        }
    }
}
