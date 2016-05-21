using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkerMgr : MonoBehaviour {
    static MarkerMgr _inst;

    public static MarkerMgr Inst {
        get {
            if (_inst == null) {
                _inst = FindObjectOfType<MarkerMgr>();
            }
            return _inst;
        }
    }

    public GameObject markerArrowPrefab;
    List<MarkerArrow> arrows;

    void Awake () {
        arrows = new List<MarkerArrow>();
    }

    public void ChangeArrowsList (ICollection<Farmer.FarmerAction> actionLocations, params Farmer.FarmerAction[] additional) {
        foreach (MarkerArrow arrow in arrows) {
            Destroy(arrow.gameObject);
        }
        arrows.Clear();
        // These loops need to be basically identical
        foreach (Farmer.FarmerAction actionPair in actionLocations) {
            MarkerArrow temp = Instantiate(markerArrowPrefab).GetComponent<MarkerArrow>();
            temp.SetType(actionPair.type);
            temp.transform.position = actionPair.target;
            arrows.Add(temp);
        }
        foreach (Farmer.FarmerAction actionPair in additional) {
            if (actionPair == null) {
                continue;
            }
            MarkerArrow temp = Instantiate(markerArrowPrefab).GetComponent<MarkerArrow>();
            temp.SetType(actionPair.type);
            temp.transform.position = actionPair.target;
            arrows.Add(temp);
        }
    }
}
