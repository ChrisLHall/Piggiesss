using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToolbarButton : MonoBehaviour {
    Toolbar toolbar;

    public FarmerActionType actionType;

    public Sprite enabledSprite;
    public Sprite disabledSprite;
    SpriteRenderer img;

    public bool Enabled { get; set; }

	// Use this for initialization
	void Start () {
        toolbar = transform.parent.GetComponent<Toolbar>();
        img = GetComponent<SpriteRenderer>();
	}
	
	void OnMouseDown () {
        toolbar.Clicked(this);
	}

    public void SetHighlight (bool highlight) {
        if (highlight) {
            img.sprite = enabledSprite;
        } else {
            img.sprite = disabledSprite;
        }
    }
}
