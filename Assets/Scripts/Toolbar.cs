using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Toolbar : MonoBehaviour {
    public FarmerActionType ToolMode {
        get;
        private set;
    }

    public bool PUSH_TO_BUY_MODE;
    public bool ALLOW_SCRUBBING;

    public int grassLimit;
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
        { FarmerActionType.PooGolem, 20 },
        { FarmerActionType.Statue, 200 },
    };

    public bool BlockOtherClicks { get; private set; }
    public bool IgnoreMouseUp { get; private set; }
    const float SCRUB_TIME = 0.5f;
    const float SCRUB_INTERVAL = 0.05f;
    float clickStart;
    float lastScrubSpawned;

    static Toolbar _inst;
    public static Toolbar inst {
        get {
            if (_inst == null) {
                _inst = FindObjectOfType<Toolbar>();
            }
            return _inst;
        }
    }

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
        }
        UpdateButtonSprites();

        if (Input.GetMouseButtonDown(0)) {
            IgnoreMouseUp = false;
            clickStart = Time.time;
        } else if (Input.GetMouseButton(0)) {
            if (Time.time - clickStart > SCRUB_TIME) {
                IgnoreMouseUp = true;
                if (Time.time - lastScrubSpawned > SCRUB_INTERVAL) {
                    lastScrubSpawned = Time.time;
                    Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    spawnPos.z = -2f;
                    bool made = CreatePrefabForAction(FarmerActionType.Cure, spawnPos);
                }
            }
        }
    }

    void LateUpdate () {
        BlockOtherClicks = false;
    }
	
    public void Clicked (ToolbarButton button) {
        if (PUSH_TO_BUY_MODE) {
            Farmer f = FindObjectOfType<Farmer>();
            if (f != null) {
                Vector3 offset = Random.insideUnitCircle * 0.25f;
                CreatePrefabForAction(button.actionType, f.transform.position + offset);
            } else {
                Debug.LogError("WHERES THE FARMER");
            }
        } else {
            ToolMode = (ToolMode == button.actionType) ? FarmerActionType.Move : button.actionType;
            UpdateButtonSprites();
        }

        BlockOtherClicks = true;
    }
    
    public void UnsetTools () {
        ToolMode = FarmerActionType.Move;
        UpdateButtonSprites();
    }

    void UpdateButtonSprites () {
        grassButton.SetHighlight(false);
        pigButton.SetHighlight(false);
        //cureButton.SetHighlight(false);
        golemButton.SetHighlight(false);
        statueButton.SetHighlight(false);
        if (PUSH_TO_BUY_MODE) {
            grassButton.SetHighlight(CanAffordAction(FarmerActionType.Grass));
            pigButton.SetHighlight(CanAffordAction(FarmerActionType.Pig));
            //cureButton.SetHighlight(CanAffordAction(FarmerActionType.Cure));
            golemButton.SetHighlight(CanAffordAction(FarmerActionType.PooGolem));
            statueButton.SetHighlight(CanAffordAction(FarmerActionType.Statue));
        } else {
            if (ToolMode == FarmerActionType.Grass
                    && GameObject.FindGameObjectsWithTag("Grass").Length < grassLimit) {
                grassButton.SetHighlight(true);
            } else if (ToolMode == FarmerActionType.Pig) {
                pigButton.SetHighlight(true);
            } else if (ToolMode == FarmerActionType.Cure) {
                //cureButton.SetHighlight(true);
            } else if (ToolMode == FarmerActionType.PooGolem) {
                golemButton.SetHighlight(true);
            } else if (ToolMode == FarmerActionType.Statue) {
                statueButton.SetHighlight(true);
            }
        }
    }
    
    public bool CreatePrefabForAction (FarmerActionType action, Vector3 pos) {
        if (!prefabDict.ContainsKey(action)) {
            return false;
        }
        if (actionCostDict.ContainsKey(action)) {
            Counter pt = FindObjectOfType<Toolbar>().poopCounter;
            if (pt.amount < actionCostDict[action]) {
                FindObjectOfType<Farmer>().ClearActions();
                return false;
            } else {
                pt.ChangeCount(-actionCostDict[action]);
            }
        }
        GameObject prefab = prefabDict[action];
        if (prefab.tag != "Grass" || GameObject.FindGameObjectsWithTag("Grass").Length < grassLimit) {
            GameObject instance = Instantiate<GameObject>(prefab);
            instance.transform.position = pos;
            return true;
        } else {
            return false;
        }
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
