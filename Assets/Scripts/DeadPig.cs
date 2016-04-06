using UnityEngine;
using System.Collections;

public class DeadPig : MonoBehaviour {
    public LeftRightSprite[] sprites;
    public Sprite cleanSkeleton;
    public Sprite bloodSkeleton;
    SpriteRenderer rend;

    const float SKELETON_DELAY = 20f;
    const float GHOST_DELAY = 10f;

    bool isSkeleton;

    Coroutine becomeSkeleton;

    public GameObject ghostPrefab;

    void Awake () {
        rend = GetComponentInChildren<SpriteRenderer>();
        isSkeleton = false;
        becomeSkeleton = StartCoroutine(BecomeSkeleton());
    }

    IEnumerator BecomeSkeleton () {
        yield return new WaitForSeconds(SKELETON_DELAY + Random.value * 3f);
        SetSkeleton(true);
        if (Random.value < 0.1f) {
            yield return new WaitForSeconds(GHOST_DELAY + Random.value * 3f);
            Instantiate(ghostPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
	
    public void SetSprites (int index, bool left) {
        isSkeleton = false;
        rend.sprite = left ? sprites[index].leftSprite : sprites[index].rightSprite;
    }

    public void SetSkeleton (bool clean) {
        isSkeleton = true;
        if (clean) {
            rend.sprite = cleanSkeleton;
        } else {
            rend.sprite = bloodSkeleton;
        }
        StopCoroutine(becomeSkeleton);
    }
}
