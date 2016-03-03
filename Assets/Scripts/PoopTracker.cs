using UnityEngine;
using System.Collections;
using System;

public class PoopTracker : MonoBehaviour {

    public long amount;
    public GameObject digitsPrefab;
    private GameObject[] oldDigits;

    void Start() {
        oldDigits = new GameObject[10];
        updateSprite();
    }


    // Called on every update of poop count to update sprite.
    private void updateSprite() {

        // Remove old game digits.
        foreach (GameObject g in oldDigits) {
            Destroy(g);
        }

        Vector3 newPosition = new Vector3(2.15f, .75f, 0f);

        // Maximum amount of poop allowed.
        amount = Math.Min(amount, 999999);
        string str_num = amount.ToString();

        // Create new objects based on the new string.
        for (int i = 0; i < str_num.Length; i++) {

            // Create and add to list.
            GameObject newDigit = Instantiate<GameObject>(digitsPrefab);
            oldDigits[i] = newDigit;

            // Set position.
            newDigit.transform.position = newPosition;
            newPosition.x += .08f;

            // Set sprite's number value.
            Digit script = newDigit.GetComponent<Digit>();
            script.SpriteUpdate((int)Char.GetNumericValue(str_num[i]));
        }
    }

    public void poopChange(int count) {
        amount += count;
        updateSprite();
    }

}
