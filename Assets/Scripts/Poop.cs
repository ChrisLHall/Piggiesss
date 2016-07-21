using UnityEngine;
using System.Collections;

public class Poop : MonoBehaviour {
    public int value;
    const float POO_LIMIT = 1000;
    void Start () {
        if (FindObjectsOfType<Poop>().Length > POO_LIMIT) {
            Destroy(this.gameObject);
        }
    }
}
