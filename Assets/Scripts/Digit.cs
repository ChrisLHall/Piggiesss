using UnityEngine;
using System.Collections;

public class Digit : MonoBehaviour {

    public Sprite[] digits;    
    private SpriteRenderer sr;

	public void SpriteUpdate(int num) {
        GetComponent<SpriteRenderer>().sprite = digits[num];
    }
}
