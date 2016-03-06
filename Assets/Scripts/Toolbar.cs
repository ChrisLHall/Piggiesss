using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Toolbar : MonoBehaviour {
    public FarmerActionType ToolMode {
        get;
        private set;
    }

    public ToolbarButton grassButton;
    public ToolbarButton pigButton;

    public GameObject grassPrefab;
    public GameObject pigPrefab;
    Dictionary<FarmerActionType, GameObject> prefabDict;
    Dictionary<FarmerActionType, int> actionCostDict = new Dictionary<FarmerActionType, int> {
        { FarmerActionType.Grass, 1 },
        { FarmerActionType.Pig, 5 }
    };

    public bool BlockOtherClicks { get; private set; }
    
    // Use this for initialization
    void Start () {
        ToolMode = FarmerActionType.Move;
        BlockOtherClicks = false;

        InitPrefabDict();
	}

    void InitPrefabDict () {
        prefabDict = new Dictionary<FarmerActionType, GameObject> {
            { FarmerActionType.Grass, grassPrefab },
            { FarmerActionType.Pig, pigPrefab }
        };
    }

    void LateUpdate () {
        BlockOtherClicks = false;
    }
	
    public void Clicked (ToolbarButton button) {
        if (button == grassButton) {
            ToolMode = (ToolMode == FarmerActionType.Grass)
                    ? FarmerActionType.Move : FarmerActionType.Grass;
        } else if (button == pigButton) {
            ToolMode = (ToolMode == FarmerActionType.Pig)
                    ? FarmerActionType.Move : FarmerActionType.Pig;
        }
        UpdateButtonSprites();

        BlockOtherClicks = true;
        FindObjectOfType<Farmer>().ClearActions();
    }
    
    public void UnsetTools () {
        ToolMode = FarmerActionType.Move;
        UpdateButtonSprites();
    }

    void UpdateButtonSprites () {
        grassButton.SetHighlight(false);
        pigButton.SetHighlight(false);
        if (ToolMode == FarmerActionType.Grass) {
            grassButton.SetHighlight(true);
        } else if (ToolMode == FarmerActionType.Pig) {
            pigButton.SetHighlight(true);
        }
    }
    
    public void CreatePrefabForAction (FarmerActionType action, Vector3 pos) {
        if (!prefabDict.ContainsKey(action)) {
            return;
        }
        if (actionCostDict.ContainsKey(action)) {
            PoopTracker pt = FindObjectOfType<PoopTracker>();
            if (pt.amount < actionCostDict[action]) {
                FindObjectOfType<Farmer>().ClearActions();
                return;
            } else {
                pt.poopChange(-actionCostDict[action]);
            }
        }
        GameObject prefab = prefabDict[action];
        Farmer farmer = FindObjectOfType<Farmer>();
        GameObject instance = Instantiate<GameObject>(prefab);
        instance.transform.position = new Vector3(farmer.transform.position.x,
                farmer.transform.position.y, 0f);
    }

    public bool CanAffordAction(FarmerActionType action) {
        if (!actionCostDict.ContainsKey(action)) {
            return true;
        }

        PoopTracker pt = FindObjectOfType<PoopTracker>();
        return pt.amount >= actionCostDict[action];
    }
}
