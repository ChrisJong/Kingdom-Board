using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest_dan : MonoBehaviour {

    public RectTransform unitPanel; //the panel which contains the 'Units to Train Panel' and 'Units Queued Panel'
    private Vector3 unitPanelPos; //copy of the position of the unit panel
    private float unitPanelXPosToBe; //x position of where the unit panel should be
    private float unitPanelXPosOn = 345f; //x position of the Unit Panel's on position
    private float unitPanelXPosOff = 0f; //x position of the Unit Panel's off position

    public RectTransform unitsQueuedPanel; //the panel which contains the queue buttons
    public RectTransform unitOrdersPanel; //the panel which contains orders given to units
    
    public Camera playerCamera; //the player's camera

    private float showSpeed = 8f; //how quickly the panel moves when shown or hidden
    private bool showUnitPanel; //whether the unit panel should be shown or hidden

    public GameObject unitSelected = null; //the unit which is selected
    private bool showUnitOrdersPanel; //whether the unit orders panel should be shown or hidden
    private Vector3 unitOrdersPanelPos; //position of the unit orders panel
    private Vector3 unitOrdersPanelSize = new Vector3(0f, 0f, 0f); //scale size of the Unit Orders Panel
    private float camFactor = 1f;
    private float camHeight = 0f;
    private Quaternion panelRotation;
    private float zRot = 180f;

    public const int numUnits = 3; //the number of units in the game, change this as new units are added
    public Button[] unitToTrainButtons = new Button[numUnits]; //buttons imbedded in the Units to Train panel
    public Image[] unitToTrainButtonLockImages = new Image[numUnits]; //the locked image sprite of the buttons imbedded in the Units to Train panel
    public UIQueueButton_dan[] unitToQueueButtonPrefabs = new UIQueueButton_dan[numUnits]; //prefab queue buttons corresponding to Units in Train panel

    public const int maxUnitsQueued = 5; //the maximum number of units allowed queued, change this as needed
    public UIQueueButton_dan[] unitsInQueue = new UIQueueButton_dan[maxUnitsQueued]; //array collection of the instantiated queue buttons

    public Text turnCountText; //text which displays the turn number
    private int turnNumber; //the current turn number

    private void Start() {
        //initialising starting positions
        unitPanelPos = unitPanel.position;
        unitPanelXPosToBe = unitPanelXPosOff;
        showUnitPanel = false;

        unitOrdersPanel.gameObject.SetActive(false);
        unitOrdersPanel.localScale = unitOrdersPanelSize;
        

        turnNumber = 1;
        turnCountText.text = "Turn: " + turnNumber.ToString();
    }

    private void Update() {
        //press m to open or close Units Panel, change m to any other hotkey you wish
        if (Input.GetKeyDown("m")) {
            ToggleShowUnitPanel();
        }
        //move the Units Panel to its proper location
        unitPanelPos.x = Mathf.Lerp(unitPanelPos.x, unitPanelXPosToBe, showSpeed * Time.deltaTime);
        unitPanel.position = unitPanelPos;


        //scale up Unit Orders Panel when activated
        if (showUnitOrdersPanel) {
            camHeight = playerCamera.transform.position.y - unitSelected.transform.position.y;
            camFactor = 18 / camHeight;

            zRot = Mathf.Lerp(zRot, 0f, showSpeed * Time.deltaTime);
            panelRotation = Quaternion.Euler(0f, 0f, zRot);
            unitOrdersPanel.rotation = panelRotation;

            unitOrdersPanelSize.x = Mathf.Lerp(unitOrdersPanelSize.x, camFactor, showSpeed * Time.deltaTime);
            unitOrdersPanelSize.y = Mathf.Lerp(unitOrdersPanelSize.y, camFactor, showSpeed * Time.deltaTime);
            unitOrdersPanelSize.z = Mathf.Lerp(unitOrdersPanelSize.z, camFactor, showSpeed * Time.deltaTime);            

            unitOrdersPanel.localScale = unitOrdersPanelSize;

            unitOrdersPanelPos = playerCamera.WorldToScreenPoint(unitSelected.transform.position);
            unitOrdersPanel.position = unitOrdersPanelPos;
        }
    }

    private void ToggleShowUnitPanel() {
        //if the Units Panel has been toggled then switch where it should be
        showUnitPanel = !showUnitPanel;
        if (showUnitPanel == true)
        {
            unitPanelXPosToBe = unitPanelXPosOn;
        } else {
            unitPanelXPosToBe = unitPanelXPosOff;
        }
    }

    public void TrainUnit(int unitID) {
        //finds the first available spot in the queue and creates a queue button
        for (int i = 0; i < maxUnitsQueued; i++) {
            if (unitsInQueue[i] == null) {                
                unitsInQueue[i] = Instantiate(unitToQueueButtonPrefabs[unitID], unitsQueuedPanel);
                unitsInQueue[i].queueID = i;
                unitsInQueue[i].uiCanvas = this;
                unitsInQueue[i].UpdateUIPosition();
                break; //stop once the queue button has been made
            }
        }
    }

    public void RefreshQueue(int startingQueueID) {
        //Updates the queue once a unit has finished training or has been cancelled
        unitsInQueue[startingQueueID] = null; //deletes the queue button of finished or cancelled unit
        for (int i = startingQueueID + 1; i < maxUnitsQueued; i++) {
            //look at the subsequent buttons in the queue
            if (unitsInQueue[i] == null)
            {
                break; //if its empty then stop
            } else {
                //otherwise, move it up in the queue
                unitsInQueue[i - 1] = unitsInQueue[i];
                unitsInQueue[i - 1].queueID = i - 1;
                unitsInQueue[i - 1].UpdateUIPosition();

                unitsInQueue[i] = null; //and clear its previous position in the queue
            }            
        }
    }

    public void SelectUnit(GameObject unit) {
        showUnitOrdersPanel = true;        
        unitSelected = unit;
        unitOrdersPanel.gameObject.SetActive(true);
    }

    public void DeselectUnit() {
        unitOrdersPanel.gameObject.SetActive(false);
        showUnitOrdersPanel = false;

        unitOrdersPanelSize.x = 0f;
        unitOrdersPanelSize.y = 0f;
        unitOrdersPanelSize.z = 0f;

        unitOrdersPanel.localScale = unitOrdersPanelSize;
        zRot = 180f;
    }
    //
    //
    //Functions underneath are temporary solutions which should be included within other game managing objects//

    public void UnlockUnit(int unitToUnlock) {
        //once a unit has been researched, unlock it in the Units to Train panel and hide the lock sprite
        unitToTrainButtons[unitToUnlock].interactable = true;
        unitToTrainButtonLockImages[unitToUnlock].enabled = false;
    }

    public void EndTurn() {
        turnNumber = turnNumber + 1;
        turnCountText.text = ("Turn: " + turnNumber.ToString());
        for (int i = 0; i < maxUnitsQueued; i++) {
            if (unitsInQueue[i] == null) {
                break;
            }
            unitsInQueue[i].NewTurn();
        }
    }
}
