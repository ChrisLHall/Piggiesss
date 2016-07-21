using UnityEngine;
using System.Collections;

public class SwipeHelp : MonoBehaviour {
    public GameObject graphics;
    Transform targetPig;
    bool pigCuredThisGame = false;
    
    public static SwipeHelp inst { get; private set; }
    // Use this for initialization
    void Awake () {
        inst = this;
    }

    void Start () {
        graphics.SetActive(false);
    }

    const float SPEED = 0.8f;
    void Update () {
        graphics.SetActive(null != targetPig && !pigCuredThisGame);
        if (null != targetPig && !pigCuredThisGame) {
            transform.position += (targetPig.position - transform.position) * SPEED;
        }
    }

    public void PigGotSick (Transform pig) {
        if (null == targetPig) {
            targetPig = pig;
            transform.position = pig.position;
        }
    }

    public void PigGotCured (Transform pig) {
        if (pig == targetPig) {
            pigCuredThisGame = true;
        }
    }
}
