using UnityEngine;
using System.Collections;

public class Farmer : MonoBehaviour {

	private Vector2 startPos;
	private Vector2 target;

	public enum FarmerState { Idle, Moving };
	private FarmerState state;

	/* Configuration:
	 * moveSpeed - movement speed in units per second
	 */

	private const float moveSpeed = 0.6f;
	private const float stepHeight = 0.02f;

	private float moveStartTime;
	private float moveDuration;

	// Use this for initialization
	void Start () {
		state = FarmerState.Idle;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			target = Map.inst.Bound(point);
			startPos = transform.position;
			Debug.DrawLine(startPos, target);
			Debug.Log (target);
			state = FarmerState.Moving;
			moveStartTime = Time.time;

			float moveDistance = Vector2.Distance(transform.position, target);
			moveDuration = moveDistance / moveSpeed;
		}

		if (state == FarmerState.Moving) {
			float duration = Time.time - moveStartTime;
			float progress = duration / moveDuration;
			Vector2 vertical = Vector2.up * (1f - Mathf.Pow(Mathf.Sin (Mathf.PI * progress * 3f * moveDuration), 4)) * stepHeight;
			transform.position = Vector2.Lerp (startPos, target, duration / moveDuration) + vertical;
			if (duration > moveDuration) {
				state = FarmerState.Idle;
			}
		}
	}
}
