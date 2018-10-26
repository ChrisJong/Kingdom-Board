namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Constants;
    using Enum;
    using Structure;

    public class CastleUI : ScreenSpaceUI {

        #region VARIABLE
        [SerializeField] private bool _spawnListOpen = false;
        [SerializeField] private bool _spawnListToggle = false;

        [SerializeField] private RectTransform _tSpawnQueue;
        [SerializeField] private RectTransform _tSpawnList;

        [SerializeField] private GameObject _btnQueue;

        [SerializeField] private Button _btnOpenList;
        [SerializeField] private Button _btnCloseList;

        [SerializeField] private Button _btnArcher;
        [SerializeField] private Button _btnLongbow;
        [SerializeField] private Button _btnCrossbow;

        [SerializeField] private Button _btnWarrior;
        [SerializeField] private Button _btnKnight;
        [SerializeField] private Button _btnGuardian;

        [SerializeField] private Button _btnMage;
        [SerializeField] private Button _btnWizard;
        [SerializeField] private Button _btnCleric;

        [SerializeField] private Castle _castle;

        [SerializeField] private List<UnitQueueButton> _queueList;

        [SerializeField] private UnitQueueButton _lastQueueButton = null;
        [SerializeField] private UnitQueueButton _toSpawn = null;

        [Header("Castle UI - Queue Padding")]
        [SerializeField] private float _paddingLeft = 5.0f;
        [SerializeField] private float _paddingTop = 5.0f; 
        [SerializeField] private float _paddingBetween = 35.0f;
        [SerializeField] private Vector3 _initialQueuePlacement = Vector3.zero;

        public UnitQueueButton lastQueueButton { get { return this._lastQueueButton; } }
        public UnitQueueButton toSpawn { get { return this._toSpawn; } }
        #endregion

        #region UNITY
        protected override void Awake() {
            if(this._castle == null) {
                this._castle = this.transform.GetComponent<Castle>();
                this.controller = this._castle.controller;
            }

            //this.FindUI(this.transform, UIValues.Structure.CASTLEUI);

            if(this.tUI != null)
                this._goUI = this.tUI.gameObject;

            //base.Awake();

            this.Init();

            this.ResetUI();
        }
        #endregion

        #region CLASS
        private void Init() {
            this._btnOpenList.onClick.AddListener(this.OpenSpawnLlsT);
            this._btnCloseList.onClick.AddListener(this.CloseSpawnList);

            this._initialQueuePlacement = this._btnOpenList.transform.position;

            this._btnArcher.onClick.AddListener(delegate { this.AddToQueue(UnitType.ARCHER, this._btnArcher.image.sprite); });
            this._btnLongbow.onClick.AddListener(delegate { this.AddToQueue(UnitType.LONGBOW, this._btnLongbow.image.sprite); });
            this._btnCrossbow.onClick.AddListener(delegate { this.AddToQueue(UnitType.CROSSBOW, this._btnCrossbow.image.sprite); });

            this._btnWarrior.onClick.AddListener(delegate { this.AddToQueue(UnitType.WARRIOR, this._btnWarrior.image.sprite); });
            this._btnGuardian.onClick.AddListener(delegate { this.AddToQueue(UnitType.GUARDIAN, this._btnGuardian.image.sprite); });
            this._btnKnight.onClick.AddListener(delegate { this.AddToQueue(UnitType.KNIGHT, this._btnKnight.image.sprite); });

            this._btnMage.onClick.AddListener(delegate { this.AddToQueue(UnitType.MAGE, this._btnMage.image.sprite); });
            this._btnWizard.onClick.AddListener(delegate { this.AddToQueue(UnitType.WIZARD, this._btnWizard.image.sprite); });
            this._btnCleric.onClick.AddListener(delegate { this.AddToQueue(UnitType.CLERIC, this._btnCleric.image.sprite); });
        }

        public override void UpdateUI() {
            base.UpdateUI();
        }

        public override void Display() {
            base.Display();
            this._goUI.SetActive(true);
        }

        public override void Hide() {
            base.Hide();
            this._goUI.SetActive(false);
            this.CloseSpawnList();

            this._castle.radiusDrawer.SetActive(false);
        }

        protected override void ResetUI() {
            this._tSpawnList.position = new Vector3(-200.0f, this._tSpawnList.position.y, this._tSpawnList.position.z);
            this._tSpawnQueue.position = new Vector3(0.0f, this._tSpawnQueue.position.y, this._tSpawnQueue.position.z);
        }

        private void OpenSpawnLlsT() {
            Debug.Log("Open Spawn List");

            // NOTE: need to change the spawn list movement into a StartCoroutine method.
            if(!this._spawnListOpen) {
                this._tSpawnList.position = new Vector3(0.0f, this._tSpawnList.position.y, this._tSpawnList.position.z);
                this._tSpawnQueue.position = new Vector3(200.0f, this._tSpawnQueue.position.y, this._tSpawnQueue.position.z);
            }

            this._spawnListOpen = true;
            this._spawnListToggle = true;
        }

        private void CloseSpawnList() {
            Debug.Log("Close Spawn List");

            this._spawnListOpen = false;
            this._spawnListToggle = true;

            // NOTE: need to change the spawn list movement into a StartCoroutine method.
            this._tSpawnList.position = new Vector3(-200.0f, this._tSpawnList.position.y, this._tSpawnList.position.z);
            this._tSpawnQueue.position = new Vector3(0.0f, this._tSpawnQueue.position.y, this._tSpawnQueue.position.z);
        }

        public void CheckSpawnQueue(SpawnQueueType queue) {            
            foreach(UnitQueueButton button in this._queueList) {
                Debug.Log("Button ID: " + button.id);
                Debug.Log("Queue ID: " + queue.id);
                if(button.id == queue.id)
                    button.Ready();
            }
        }

        public void AddToQueue(UnitType type) {
            Debug.Log("TRYING TO ADD " + type.ToString() + " TO " + this.controller.name + " QUEUE");
            //if(!this._castle.AddUnitToQueue(type))
                //Debug.LogWarning("UNABLE TO ADD " + type.ToString() + " TO QUEUE");
        }

        public void AddToQueue(UnitType type, Sprite sprite){
            // NOTE: need to fix this part of the code since creating and deleting an object is a waste of resources if its never going to get used.
            // Spawn The Queue Button on the Screen.

            uint id = 0;

            if(this._castle.AddUnitToQueue(type, ref id)) {
                // Add a button to the UI Queue List.
                GameObject go = GameObject.Instantiate(this._btnQueue);
                UnitQueueButton button = go.GetComponent<UnitQueueButton>();
                button.Init(id, type, sprite, this._castle, this);
                button.SetQueueType(this._castle.lastQueue);
                this._queueList.Add(button);

                go.transform.SetParent(this._tSpawnQueue);
                go.transform.position = this._btnOpenList.transform.position;

                if(this._queueList.Count < this._castle.queueLImit) {
                    if(!this._btnOpenList.IsActive())
                        this._btnOpenList.gameObject.SetActive(true);

                    // Position the button and move the OpenSpawnList (or disable it if the queue count is higher than the spawnlimit).
                    Vector3 openListPos = this._btnOpenList.transform.position;
                    this._btnOpenList.transform.position = new Vector3(openListPos.x, openListPos.y - 35.0f, openListPos.z);

                } else {
                    this._btnOpenList.gameObject.SetActive(false);
                    this.CloseSpawnList();
                }
            } else {
                // NOTE: need to display a message here to the user that the an error or a limit has been reached.
                Debug.LogWarning("Either the limit as been reached: " + this._castle.spawnQueue.Count.ToString() + ", or the Type (" + type.ToString() + ") is not Supported");
            }
        }

        public void RemoveFromQueue(SpawnQueueType queue) {
            UnitQueueButton toRemove = null;

            foreach(UnitQueueButton remove in this._queueList){
                if(remove.id == queue.id) {
                    toRemove = remove;
                    break;
                }
            }

            if(toRemove != null) {
                if(this._queueList.Contains(toRemove)) {
                    this._queueList.Remove(toRemove);
                    toRemove.Delete();
                    this.UpdateQueueUI();
                } else
                    Debug.LogError("An error has occured.");
            }
        }

        public void RemoveFromQueue(UnitQueueButton unit) {
            if(this._queueList.Contains(unit)) {
                uint id = unit.id;
                this._queueList.Remove(unit);
                this._castle.RemoveUnitFromQueue(id);
                this.UpdateQueueUI();
            } else {
                Debug.LogError("The Queued Unit Doesn't Exisit in the Queue List" + unit.typeToSpawn);
            }
        }

        public void UpdateQueueUI() {
            for(int i = 0; i < this._queueList.Count; i++) {

                // NOTE: need to fix up the placement using the padding vector instead of the inital placement.
                // NOTE: Errors occur become the button is using some other local position that isnt the panel.
                //Vector3 pos = new Vector3(this._paddingLeft, -(i * this._paddingBetween) - this._paddingTop, 0.0f);

                this._queueList[i].transform.position = new Vector3(this._initialQueuePlacement.x, this._initialQueuePlacement.y - (i * this._paddingBetween), this._initialQueuePlacement.z);
            }
        }
        #endregion
    }
}