﻿namespace UI {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;
    using Manager;
    using Player;
    using Unit;
    using Scriptable;
    using Structure;

    public class CastleUI : ScreenSpace {

        #region VARIABLE
        [SerializeField] private Player _player;
        [SerializeField] private Castle _castle;

        [SerializeField] private bool _spawnGroupToggle = false;

        [SerializeField] private RectTransform _spawnGroup = null;
        [SerializeField] private RectTransform _spawnQueue = null;
        [SerializeField] private RectTransform _spawnList = null;

        [SerializeField] private RectTransform _meleePanel = null;
        [SerializeField] private RectTransform _meleeButtonPanel = null;

        [SerializeField] private RectTransform _rangePanel = null;
        [SerializeField] private RectTransform _rangeButtonPanel = null;

        [SerializeField] private RectTransform _magicPanel = null;
        [SerializeField] private RectTransform _magicButtonPanel = null;

        [SerializeField] private GameObject _spawnListButton;
        [SerializeField] private GameObject _queueRibbonButton;

        [SerializeField] private Button _btnToggleBook;

        [SerializeField] private List<QueueButton> _queueList;
        [SerializeField] private List<SpawnButton> _spawnListButtons = new List<SpawnButton>();

        [SerializeField] private QueueButton _lastQueueButton = null;
        [SerializeField] private QueueButton _toSpawn = null;

        [Header("Spawn Book UI")]
        [SerializeField] private float _spawnButtonWidth = 50.0f;
        [SerializeField] private float _spawnButtonHeight = 50.0f;
        [SerializeField] private float _spawnButtonPadding = 10.0f;
        [SerializeField] private float _spawnButtonTierPadding = 60.0f;

        public QueueButton lastQueueButton { get { return this._lastQueueButton; } }
        public QueueButton toSpawn { get { return this._toSpawn; } }
        #endregion

        #region UNITY

        #endregion

        #region CLASS
        public void Init(Castle castle) {
            this._player = castle.controller;
            this._castle = castle;

            this._spawnGroup = this._player.UI.spawnGroup.transform as RectTransform;

            /*this._spawnQueue = this._spawnGroup.Find("Queue_Panel") as RectTransform;
            this._spawnList = this._spawnGroup.Find("SpawnList_Panel") as RectTransform;

            this._meleePanel = this._spawnList.Find("Melee_Panel") as RectTransform;
            this._magicButtonPanel = this._meleePanel.Find("UnitButton_Panel") as RectTransform;
            this._rangePanel = this._spawnList.Find("Range_Panel") as RectTransform;
            this._rangeButtonPanel = this._rangePanel.Find("UnitButton_Panel") as RectTransform;
            this._magicPanel = this._spawnList.Find("Magic_Panel") as RectTransform;
            this._magicButtonPanel = this._magicPanel.Find("UnitButton_Panel") as RectTransform;

            this._btnToggleBook = this._spawnList.GetComponent<Button>() as Button;
            this._btnToggleBook.onClick.AddListener(this.ToggleSpawnGroup);*/

            //this._initialQueuePlacement = this._btnOpenList.transform.position;

            //this.GenerateSpawnListButtons();

            this.HideUI();
            //this.ResetUI();
        }

        public override void DisplayUI() {
            this.CloseSpawnList();
            this._spawnGroup.gameObject.SetActive(true);
        }

        public override void HideUI() {
            this._spawnGroup.gameObject.SetActive(false);

            this.CloseSpawnList();

            this._castle.radiusDrawer.SetActive(false);
        }

        public override void ResetUI() {
            this._spawnList.position = new Vector3(-200.0f, this._spawnList.position.y, this._spawnList.position.z);
            this._spawnQueue.position = new Vector3(0.0f, this._spawnQueue.position.y, this._spawnQueue.position.z);
        }

        public override void UpdateUI() {
            throw new System.NotImplementedException();
        }

        public void CheckSpawnQueue(SpawnQueueType queue) {            
            foreach(QueueButton button in this._queueList) {
                Debug.Log("Button ID: " + button.id);
                Debug.Log("Queue ID: " + queue.id);
                if(button.id == queue.id)
                    button.Ready();
            }
        }

        public void AddToQueue(UnitType type, Sprite sprite){
            // NOTE: need to fix this part of the code since creating and deleting an object is a waste of resources if its never going to get used.
            // Spawn The Queue Button on the Screen.

            /*uint id = 0;

            if(this._castle.AddUnitToQueue(type, ref id)) {
                // Add a button to the UI Queue List.
                GameObject go = GameObject.Instantiate(this._btnQueue);
                QueueButton button = go.GetComponent<QueueButton>();
                RectTransform rect = go.transform as RectTransform;

                button.Init(id, type, sprite, this._castle, this);
                button.SetQueueType(this._castle.lastQueue);
                this._queueList.Add(button);

                //go.transform.SetParent(this._spawnQueue);
                //go.transform.position = this._btnOpenList.transform.position;

                rect.SetParent(this._spawnQueue);
                rect.anchoredPosition = ((RectTransform)this._btnOpenList.transform).anchoredPosition;

                if(this._queueList.Count < this._castle.queueLImit) {
                    if(!this._btnOpenList.IsActive())
                        this._btnOpenList.gameObject.SetActive(true);

                    // Position the button and move the OpenSpawnList (or disable it if the queue count is higher than the spawnlimit).
                    Vector3 openListPos = this._btnOpenList.transform.position;
                    this._btnOpenList.transform.position = new Vector3(openListPos.x, openListPos.y - 35.0f, openListPos.z);
                    //((RectTransform)this._btnOpenList.transform).anchoredPosition = new Vector3(openListPos.x, openListPos.y - 35.0f, openListPos.z);

                } else {
                    this._btnOpenList.gameObject.SetActive(false);
                    this.CloseSpawnList();
                }
            } else {
                // NOTE: need to display a message here to the user that the an error or a limit has been reached.
                Debug.LogWarning("Either the limit as been reached: " + this._castle.spawnQueue.Count.ToString() + ", or the Type (" + type.ToString() + ") is not Supported");
            }*/
        }

        public void RemoveFromQueue(SpawnQueueType queue) {
            QueueButton toRemove = null;

            foreach(QueueButton remove in this._queueList){
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

        public void RemoveFromQueue(QueueButton unit) {
            if(this._queueList.Contains(unit)) {

                Vector3 pos = unit.rectTransfrom.anchoredPosition;

                Debug.Log("Queue Button Position: " + pos.ToString());

                //((RectTransform)this._btnOpenList.transform).anchoredPosition = pos;

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

                //this._queueList[i].transform.position = new Vector3(this._initialQueuePlacement.x, this._initialQueuePlacement.y - (i * this._paddingBetween), this._initialQueuePlacement.z);
            }
        }

        public void UnlockSpawnButton(ClassType classType, UnitType unitType) {
            // Search for the button using the class and unit type.
            foreach(SpawnButton button in this._spawnListButtons) {
                if(button.UnitType == unitType) {
                    button.Unlock();
                }
            }
        }

        private void ToggleSpawnGroup() {
            this._spawnGroupToggle = !this._spawnGroupToggle;

            if(this._spawnGroupToggle) {
                this._spawnGroup.position = new Vector3(0.0f, this._spawnGroup.position.y, this._spawnGroup.position.z);
            } else {
                this._spawnGroup.position = new Vector3(-200.0f, this._spawnGroup.position.y, this._spawnGroup.position.z);
            }
        }

        private void OpenSpawnLlsT() {
            // NOTE: need to change the spawn list movement into a StartCoroutine method.
            if(!this._spawnGroupToggle) {
                this._spawnGroupToggle = true;
                this._spawnGroup.position = new Vector3(-200.0f, this._spawnGroup.position.y, this._spawnGroup.position.z);
            }
        }

        private void CloseSpawnList() {
            // NOTE: need to change the spawn list movement into a StartCoroutine method.
            if(this._spawnGroupToggle) {
                this._spawnGroupToggle = false;
                this._spawnGroup.position = new Vector3(0.0f, this._spawnGroup.position.y, this._spawnGroup.position.z);
            }
        }

        private void GenerateSpawnListButtons() {

            int classCount = 0;

            for(int i = 0; i < UnitPoolManager.instance.UnitDataList.Count; i++) {
                UnitScriptable unit = UnitPoolManager.instance.UnitDataList[i];
                GameObject go = Instantiate(this._spawnListButton);
                SpawnButton btn = go.GetComponent<SpawnButton>() as SpawnButton;
                RectTransform rectTransform = go.GetComponent<RectTransform>() as RectTransform;

                go.name = unit.unitType.ToString() + "_BTN";
                btn.Init(this._castle, unit.spawnIconUnlockedSprite, unit.SpawnIconLockedsprite, unit.classType, unit.unitType, true);
                this._spawnListButtons.Add(btn);
                go.transform.SetParent(this._spawnList);

                if(unit.classType == ClassType.MELEE) {
                    //Vector3 pos = new Vector3(this._meleePosition, this._listYposition - (this._listPaddaing * classCount), 0.0f);
                    //rectTransform.anchoredPosition = pos;
                } else if(unit.classType == ClassType.RANGE) {
                    //Vector3 pos = new Vector3(this._rangePosition, this._listYposition - (this._listPaddaing * classCount), 0.0f);
                    //rectTransform.anchoredPosition = pos;
                } else if(unit.classType == ClassType.MAGIC) {
                    //Vector3 pos = new Vector3(this._magicPoseition, this._listYposition - (this._listPaddaing * classCount), 0.0f);
                    //rectTransform.anchoredPosition = pos;
                }

                classCount++;
                if(classCount % 3 == 0)
                    classCount = 0;
            }
        }

        #endregion
    }
}