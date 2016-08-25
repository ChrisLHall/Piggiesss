using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToolbarButton : MonoBehaviour {
    Toolbar toolbar;

    public FarmerActionType actionType;

    public Sprite enabledSprite;
    public Sprite disabledSprite;
    public Sprite noPoopEnabledSprite;
    public Sprite noPoopDisabledSprite;
    SpriteRenderer img;

    public bool Enabled { get; set; }

	// Use this for initialization
	void Awake () {
        toolbar = transform.parent.GetComponent<Toolbar>();
        img = GetComponent<SpriteRenderer>();
        if (!toolbar.ALLOW_POOP && null != noPoopDisabledSprite) {
            img.sprite = noPoopDisabledSprite;
        }
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
            img.sprite = (!toolbar.ALLOW_POOP && null != noPoopEnabledSprite)
                    ? noPoopEnabledSprite : enabledSprite;
        } else {
            img.sprite = (!toolbar.ALLOW_POOP && null != noPoopDisabledSprite)
                    ? noPoopDisabledSprite : disabledSprite;
        }
    }
}
