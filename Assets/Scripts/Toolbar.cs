using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Toolbar : MonoBehaviour {
    public FarmerActionType ToolMode {
        get;
        private set;
    }

    public Counter scoreCounter;
    public Counter poopCounter;

    public ToolbarButton grassButton;
    public ToolbarButton pigButton;
    public ToolbarButton cureButton;
    public ToolbarButton golemButton;
    public ToolbarButton statueButton;

    public GameObject grassPrefab;
    public GameObject pigPrefab;
    public GameObject curePrefab;
    public GameObject golemPrefab;
    public GameObject statuePrefab;
    Dictionary<FarmerActionType, GameObject> prefabDict;
    Dictionary<FarmerActionType, int> actionCostDict = new Dictionary<FarmerActionType, int> {
        { FarmerActionType.Grass, 1 },
        { FarmerActionType.Pig, 5 },
        { FarmerActionType.Cure, 1 },
        { FarmerActionType.PooGolem, 20 },
        { FarmerActionType.Statue, 200 },
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
            { FarmerActionType.Pig, pigPrefab },
            { FarmerActionType.Cure, curePrefab },
            { FarmerActionType.PooGolem, golemPrefab },
            { FarmerActionType.Statue, statuePrefab },
        };
    }

    void Update () {
        if (!CanAffordAction(ToolMode) || Input.GetKeyDown(KeyCode.Escape)) {
            ToolMode = FarmerActionType.Move;
            UpdateButtonSprites();
        }
    }

    void LateUpdate () {
        BlockOtherClicks = false;
    }
	
    public void Clicked (ToolbarButton button) {
        ToolMode = (ToolMode == button.actionType) ? FarmerActionType.Move : button.actionType;
        UpdateButtonSprites();

        BlockOtherClicks = true;
    }
    
    public void UnsetTools () {
        ToolMode = FarmerActionType.Move;
        UpdateButtonSprites();
    }

    void UpdateButtonSprites () {
        grassButton.SetHighlight(false);
        pigButton.SetHighlight(false);
        cureButton.SetHighlight(false);
        golemButton.SetHighlight(false);
        statueButton.SetHighlight(false);
        if (ToolMode == FarmerActionType.Grass) {
            grassButton.SetHighlight(true);
        } else if (ToolMode == FarmerActionType.Pig) {
            pigButton.SetHighlight(true);
        } else if (ToolMode == FarmerActionType.Cure) {
            cureButton.SetHighlight(true);
        } else if (ToolMode == FarmerActionType.PooGolem) {
            golemButton.SetHighlight(true);
        } else if (ToolMode == FarmerActionType.Statue) {
            statueButton.SetHighlight(true);
        }
    }
    
    public void CreatePrefabForAction (FarmerActionType action, Vector3 pos) {
        if (!prefabDict.ContainsKey(action)) {
            return;
        }
        if (actionCostDict.ContainsKey(action)) {
            Counter pt = FindObjectOfType<Toolbar>().poopCounter;
            if (pt.amount < actionCostDict[action]) {
                FindObjectOfType<Farmer>().ClearActions();
                return;
            } else {
                pt.ChangeCount(-actionCostDict[action]);
            }
        }
        GameObject prefab = prefabDict[action];
        GameObject instance = Instantiate<GameObject>(prefab);
        instance.transform.position = pos;
    }

    public bool CanAffordAction(FarmerActionType action,
            ICollection<Farmer.FarmerAction> includedPendingActions = null,
            Farmer.FarmerAction includedCurrentAction = null) {
        if (!actionCostDict.ContainsKey(action)) {
            return true;
        }

        int pendingCost = 0;
        if (includedPendingActions != null) {
            foreach (Farmer.FarmerAction actionPair in includedPendingActions) {
                if (actionCostDict.ContainsKey(actionPair.type)) {
                    pendingCost += actionCostDict[actionPair.type];
                }
            }
        }
        if (includedCurrentAction != null
                && actionCostDict.ContainsKey(includedCurrentAction.type)) {
            pendingCost += actionCostDict[includedCurrentAction.type];
        }
        Counter pt = FindObjectOfType<Toolbar>().poopCounter;
        return pt.amount >= actionCostDict[action] + pendingCost;
    }
}
