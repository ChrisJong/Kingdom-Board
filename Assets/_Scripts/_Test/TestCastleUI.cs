namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;
    using Manager;

    public class TestCastleUI : MonoBehaviour {

        #region VARIABLE

        [SerializeField] private RectTransform _spawnGroup;

        [SerializeField] private RectTransform _spawnList;
        [SerializeField] private GameObject _listbutton;
        [SerializeField] private bool _isListOpen = false;
        [SerializeField] private float _openListWidth = 200.0f;

        [SerializeField] private RectTransform _spawnQueue;
        [SerializeField] private GameObject _queueButton;
        [SerializeField] private float _paddingLeft = 0.0f;
        [SerializeField] private float _paddingTop = 0.0f;
        [SerializeField] private float _paddingBetween = 0.0f;

        [SerializeField] private float _meleePosition = -55.0f;
        [SerializeField] private float _rangePosition = 0.0f;
        [SerializeField] private float _magicPoseition = 55.0f;
        [SerializeField] private float _listYposition = -100.0f;
        [SerializeField] private float _listPaddaing = 50.0f;

        [SerializeField] private GameObject _prefabSpawnListButton;
        [SerializeField] private List<TestSpawnButton> _spawnListButtons;

        [SerializeField] private Button _spawnListCloseBtn;

        #endregion

        #region UNITY
        private void Awake() {
            this._spawnListButtons = new List<TestSpawnButton>();

            this.GenerateSpawnListButtons();
        }

        private void Update() {
            if(this.GetInput()){
                this.ToggleSpawnGroup();
            }
        }
        #endregion

        #region CLASS
        public void UnlockSpawnButton(ClassType classType, UnitType unitType) {
            // Search for the button using the class and unit type.
            foreach(TestSpawnButton button in this._spawnListButtons) {
                if(button.UnitType == unitType) {
                    button.UnLock();
                }
            }
        }

        private bool GetInput() {
            if(Input.GetKeyUp(KeyCode.Tab)) {
                return true;
            }

            return false;
        }

        private void ToggleSpawnGroup() {
            this._isListOpen = !this._isListOpen;

            if(this._isListOpen) {
                this._spawnGroup.position = new Vector3(0.0f, this._spawnGroup.position.y, this._spawnGroup.position.z);
            }else {
                this._spawnGroup.position = new Vector3(-this._openListWidth, this._spawnGroup.position.y, this._spawnGroup.position.z);
            }
        }

        private void GenerateSpawnListButtons() {

            int classCount = 0;

            /*for(int i = 0; i < UnitPoolManager.instance.UnitStructure.Length; i++) {
                TestUnitScriptable unit = UnitPoolManager.instance.UnitStructure[i];
                GameObject go = Instantiate(this._prefabSpawnListButton);
                TestSpawnButton btn = go.GetComponent<TestSpawnButton>() as TestSpawnButton;
                RectTransform rectTransform = go.GetComponent<RectTransform>() as RectTransform;

                go.name = unit.unitType.ToString() + "_BTN";
                btn.Init(unit.unitType, unit.classType, unit.spawnIcon, true);
                this._spawnListButtons.Add(btn);
                go.transform.SetParent(this._spawnList);

                if(unit.classType == ClassType.MELEE) {
                    Vector3 pos = new Vector3(this._meleePosition, this._listYposition - (this._listPaddaing * classCount), 0.0f);
                    rectTransform.anchoredPosition = pos;
                } else if(unit.classType == ClassType.RANGE) {
                    Vector3 pos = new Vector3(this._rangePosition, this._listYposition - (this._listPaddaing * classCount), 0.0f);
                    rectTransform.anchoredPosition = pos;
                } else if(unit.classType == ClassType.MAGIC) {
                    Vector3 pos = new Vector3(this._magicPoseition, this._listYposition - (this._listPaddaing * classCount), 0.0f);
                    rectTransform.anchoredPosition = pos;
                }

                classCount++;
                if(classCount % 3 == 0)
                    classCount = 0;
            }*/
        }
        #endregion
    }
}