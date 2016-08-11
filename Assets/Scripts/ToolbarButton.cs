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
	void Awake () {
        toolbar = transform.parent.GetComponent<Toolbar>();
        img = GetComponent<SpriteRenderer>();
	}

    void OnMouseOver() {
        if (toolbar != null && !toolbar.BlockOtherClicks)
            toolbar.SwitchBlockedClicks();
    }

    void OnMouseExit() {
        if (toolbar != null && toolbar.BlockOtherClicks) 
            toolbar.SwitchBlockedClicks();
    }
	
	void OnMouseUp () {
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
