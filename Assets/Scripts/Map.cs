using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Map : MonoBehaviour {
    public Transform bottomLeft;
    public Transform topRight;
	private static Rect bounds = new Rect (-1.31f, -0.74f, 2.218f, 1.386f);

    static Map _inst;
    public static Map inst {
        get {
            if (_inst == null) {
                _inst = FindObjectOfType<Map>();
            }
            return _inst;
        }
    }

    void Update () {
        if (bottomLeft != null && topRight != null) {
            bounds = new Rect(bottomLeft.position.x, bottomLeft.position.y,
                    topRight.position.x - bottomLeft.position.x,
                    topRight.position.y - bottomLeft.position.y);
            Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMin, 0f),
                    new Vector3(bounds.xMin, bounds.yMax, 0f));
            Debug.DrawLine(new Vector3(bounds.xMax, bounds.yMin, 0f),
                    new Vector3(bounds.xMax, bounds.yMax, 0f));
            Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMin, 0f),
                    new Vector3(bounds.xMax, bounds.yMin, 0f));
            Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMax, 0f),
                    new Vector3(bounds.xMax, bounds.yMax, 0f));
        }
    }

	/* Returns the closest vector to v within the bounds of the map */
	public Vector2 Bound(Vector2 v) {
		/* For now, just put it within a bounding rect */
		Vector2 vPrime = new Vector2 (v.x, v.y);

		if (v.x < bounds.xMin)
			vPrime.x = bounds.xMin;
		else if (v.x > bounds.xMax)
			vPrime.x = bounds.xMax;
		else
			vPrime.x = v.x;
		
		if (v.y < bounds.yMin)
			vPrime.y = bounds.yMin;
		else if (v.y > bounds.yMax)
			vPrime.y = bounds.yMax;
		else
			vPrime.y = v.y;

		return vPrime;
	}

}
