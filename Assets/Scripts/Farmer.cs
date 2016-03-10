using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FarmerActionType {
    Move,
    Clean,
    Grass,
    Pig,
    Cure
}

public class Farmer : MonoBehaviour {
    [System.Serializable]
    public class FarmerAction {
        public FarmerActionType type;
        public Vector2 target;
    }
    FarmerAction currentAction;

    SpriteRenderer rend;

	private Vector2 startPos;
	private Vector2 target;
    LinkedList<FarmerAction> actions = new LinkedList<FarmerAction>();

	public enum FarmerState { Idle, Moving };
	private FarmerState state;

	/* Configuration:
	 * moveSpeed - movement speed in units per second
	 */

	private const float moveSpeed = 1.3f;
	private const float stepHeight = 0.05f;

	private float moveStartTime;
	private float moveDuration;

    Toolbar toolbar;

    public Vector3 CameraLookPos { get; private set; }

    Transform graphics;

	// Use this for initialization
	void Start () {
		state = FarmerState.Idle;
        CameraLookPos = transform.position;
        toolbar = FindObjectOfType<Toolbar>();
        rend = GetComponentInChildren<SpriteRenderer>();
        graphics = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)
                && !toolbar.BlockOtherClicks
                && toolbar.CanAffordAction(toolbar.ToolMode, actions, currentAction)) {
            Vector2 point = Map.Inst.Bound(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (toolbar.ToolMode == FarmerActionType.Move) {
                ClearActions();
            }
            EnqueueAction(toolbar.ToolMode, point);
        }

        if (state == FarmerState.Idle) {
            DequeueAction();
        } else if (state == FarmerState.Moving) {
			float duration = Time.time - moveStartTime;
			float progress = duration / moveDuration;
			Vector2 vertical = Vector2.up * (1f - Mathf.Pow(Mathf.Sin (Mathf.PI * progress * 3f * moveDuration), 4)) * stepHeight;
			transform.position = Vector2.Lerp (startPos, target, duration / moveDuration);
            graphics.transform.localPosition = vertical;
            CameraLookPos = transform.position;
			if (duration > moveDuration) {
                if (currentAction.type != FarmerActionType.Move) {
                    toolbar.CreatePrefabForAction(currentAction.type,
                            transform.position);
                }
                state = FarmerState.Idle;
                SetMarkers();
			}
		}
	}

    void DequeueAction () {
        if (actions.Count == 0) {
            currentAction = null;
            SetMarkers();
            return;
        }
        currentAction = actions.First.Value;
        actions.RemoveFirst();

        if (!toolbar.CanAffordAction(currentAction.type)) {
            toolbar.UnsetTools();
            DequeueAction();
            return;
        }

        SetMarkers();
        target = currentAction.target;
        startPos = transform.position;
        Debug.DrawLine(startPos, target, Color.blue, 0.1f);
        //Debug.Log(target);
        state = FarmerState.Moving;
        moveStartTime = Time.time;

        float moveDistance = Vector2.Distance(transform.position, target);
        moveDuration = moveDistance / moveSpeed;
        rend.flipX = (target.x > transform.position.x);
    }

    public void ClearActions () {
        actions.Clear();
        SetMarkers();
        state = FarmerState.Idle;
    }

    public void EnqueueAction (FarmerActionType actionType, Vector2 target) {
        // don't judge me for not having a constructor
        FarmerAction newAction = new FarmerAction {
            type = actionType,
            target = target
        };

        actions.AddLast(newAction);

        SetMarkers();
    }

    void SetMarkers () {
        MarkerMgr.Inst.ChangeArrowsList(actions, currentAction);
    }

    void OnTriggerStay2D (Collider2D other) {
        if (other.GetComponent<Poop>() != null) {
            Destroy(other.gameObject);
            FindObjectOfType<Toolbar>().poopCounter.ChangeCount(1);
            FindObjectOfType<Toolbar>().scoreCounter.ChangeCount(1);
            GetComponent<AudioSource>().Play();
        }
        if (other.GetComponent<DeadPig>() != null) {
            Destroy(other.gameObject);
        }
    }
}
