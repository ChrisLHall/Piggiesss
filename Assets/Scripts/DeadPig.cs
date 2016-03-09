using UnityEngine;
using System.Collections;

public class DeadPig : MonoBehaviour {
    public LeftRightSprite[] sprites;
    public Sprite cleanSkeleton;
    public Sprite bloodSkeleton;
    SpriteRenderer rend;

    const float SKELETON_DELAY = 20f;

    bool isSkeleton;

    void Awake () {
        rend = GetComponentInChildren<SpriteRenderer>();
        isSkeleton = false;
        StartCoroutine(BecomeSkeleton());
    }

    IEnumerator BecomeSkeleton () {
        yield return new WaitForSeconds(SKELETON_DELAY + Random.value * 3f);
        SetSkeleton(true);
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
    }
}
