using UnityEngine;
using System.Collections;
using System;

public class PoopTracker : MonoBehaviour {

    const int MAX = 999999;

    public long amount;
    public GameObject digitsPrefab;
    private GameObject[] oldDigits;

    void Start() {
        oldDigits = new GameObject[MAX.ToString().Length];
        InitSprites();
        updateSprite();
    }

    void InitSprites () {
        for (int i = 0; i < oldDigits.Length; i++) {
            oldDigits[i] = Instantiate<GameObject>(digitsPrefab);
            oldDigits[i].transform.SetParent(transform);
        }
    }

    // Called on every update of poop count to update sprite.
    private void updateSprite() {

        Vector3 newPosition = new Vector3();

        // Maximum amount of poop allowed.
        amount = Math.Min(amount, MAX);
        string str_num = amount.ToString();

        // Create new objects based on the new string.
        for (int i = 0; i < str_num.Length; i++) {

            // Create and add to list.
            GameObject newDigit = oldDigits[i];
            newDigit.SetActive(true);

            // Set position.
            newDigit.transform.localPosition = newPosition;
            newPosition.x += .08f;

            // Set sprite's number value.
            Digit script = newDigit.GetComponent<Digit>();
            script.SpriteUpdate((int)Char.GetNumericValue(str_num[i]));
        }

        for (int i = str_num.Length; i < oldDigits.Length; i++) {
            GameObject newDigit = oldDigits[i];
            newDigit.SetActive(false);
        }
    }

    public void poopChange(int count) {
        amount += count;
        updateSprite();
    }

}
