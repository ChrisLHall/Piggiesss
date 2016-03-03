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
    class FarmerAction {
        public FarmerActionType type;
        public Vector2 target;
    }

	private Vector2 startPos;
	private Vector2 target;
    LinkedList<FarmerAction> actions = new LinkedList<FarmerAction>();

	public enum FarmerState { Idle, Moving };
	private FarmerState state;

	/* Configuration:
	 * moveSpeed - movement speed in units per second
	 */

	private const float moveSpeed = 0.6f;
	private const float stepHeight = 0.02f;

	private float moveStartTime;
	private float moveDuration;

    public Vector3 CameraLookPos { get; private set; }

	// Use this for initialization
	void Start () {
		state = FarmerState.Idle;
        CameraLookPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
            Vector2 point = Map.inst.Bound(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            EnqueueAction(FarmerActionType.Move, point);
        }

        if (state == FarmerState.Idle) {
            DequeueAction();
        } else if (state == FarmerState.Moving) {
			float duration = Time.time - moveStartTime;
			float progress = duration / moveDuration;
			Vector2 vertical = Vector2.up * (1f - Mathf.Pow(Mathf.Sin (Mathf.PI * progress * 3f * moveDuration), 4)) * stepHeight;
			transform.position = Vector2.Lerp (startPos, target, duration / moveDuration) + vertical;
            CameraLookPos = transform.position - new Vector3(vertical.x, vertical.y);
			if (duration > moveDuration) {
				state = FarmerState.Idle;
			}
		}

        foreach (FarmerAction action in actions) {
            Debug.DrawLine(action.target, action.target + new Vector2(0f, 0.1f), Color.magenta);
        }
	}

    void DequeueAction () {
        if (actions.Count == 0) {
            return;
        }
        FarmerAction action = actions.First.Value;
        actions.RemoveFirst();

        if (action.type == FarmerActionType.Move) {
            target = action.target;
            startPos = transform.position;
            Debug.DrawLine(startPos, target, Color.blue, 0.1f);
            //Debug.Log(target);
            state = FarmerState.Moving;
            moveStartTime = Time.time;

            float moveDistance = Vector2.Distance(transform.position, target);
            moveDuration = moveDistance / moveSpeed;
        }
    }

    public void ClearActions () {
        actions.Clear();
    }

    public void EnqueueAction (FarmerActionType actionType, Vector2 target) {
        // don't judge me for not having a constructor
        FarmerAction newAction = new FarmerAction {
            type = actionType,
            target = target
        };

        actions.AddLast(newAction);
    }
}
