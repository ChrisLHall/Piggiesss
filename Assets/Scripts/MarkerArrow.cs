using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkerArrow : MonoBehaviour {
    public Sprite moveMarker;
    public Sprite grassMarker;
    public Sprite pigMarker;

    Dictionary<FarmerActionType, Sprite> actionSprites;

    SpriteRenderer rend;

	// Use this for initialization
	void Awake () {
        rend = GetComponentInChildren<SpriteRenderer>();
        InitActionSpritesDict();
	}

    void InitActionSpritesDict () {
        actionSprites = new Dictionary<FarmerActionType, Sprite> {
            { FarmerActionType.Move, moveMarker },
            { FarmerActionType.Grass, grassMarker },
            { FarmerActionType.Pig, pigMarker }
        };
    }
	
	public void SetType (FarmerActionType type) {
        if (actionSprites.ContainsKey(type)) {
            rend.sprite = actionSprites[type];
        } else {
            rend.sprite = moveMarker;
        }
    }
}
