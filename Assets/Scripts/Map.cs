using UnityEngine;
using System.Collections;

public class Map {

	private static Rect bounds = new Rect (-1.369f, -0.74f, 1.8f, 1.0f);

	/* Returns the closest vector to v within the bounds of the map */
	public static Vector2 Bound(Vector2 v) {
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
