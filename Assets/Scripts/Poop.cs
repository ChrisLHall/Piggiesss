using UnityEngine;
using System.Collections;

public class Poop : MonoBehaviour {
    public int value;
    const float POO_LIMIT = 1000;
    public Sprite pooSprite;
    public Sprite nonPooSprite;
    void Awake () {
        if (Toolbar.inst.ALLOW_POOP) {
            GetComponentInChildren<SpriteRenderer>().sprite = pooSprite;
        } else {
            GetComponentInChildren<SpriteRenderer>().sprite = nonPooSprite;
        }
    }
    void Start () {
        if (FindObjectsOfType<Poop>().Length > POO_LIMIT) {
            Destroy(this.gameObject);
        }
    }
}
