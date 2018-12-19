namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Enum;

    [RequireComponent(typeof(TestCastleUI))]
    [RequireComponent(typeof(TestResearch))]
    public class TestCastle : MonoBehaviour {

        #region VARIBALE
        [SerializeField] private int _turnCount = 0;
        [SerializeField] private bool _turnEnded = false;

        [SerializeField] private Text _turnText = null;

        public int TurnCount { get { return this._turnCount; } }

        [SerializeField] private TestCastleUI _castleUI;

        [SerializeField] private TestResearch _research;
        [SerializeField] private Dictionary<ClassType, List<TestResearchUpgradeData>> _unitUpgrades = new Dictionary<ClassType, List<TestResearchUpgradeData>>();

        //[SerializeField] private Dictionary<ClassType, List<testresear>
        #endregion

        #region UNITY
        private void Start() {
            this._turnCount = 0;
            this._turnEnded = false;

            for(int i = 0; i < System.Enum.GetNames(typeof(ClassType)).Length -2; i++) {
                ClassType classType = (ClassType)i + 1;
                List<TestResearchUpgradeData> data = new List<TestResearchUpgradeData>();

                this._unitUpgrades.Add(classType, data);
            }

            Debug.Log("Upgade Count: " + this._unitUpgrades.Count.ToString());

            if(this._castleUI == null)
                this._castleUI = this.transform.GetComponent<TestCastleUI>() as TestCastleUI;

            if(this._research == null)
                this._research = this.transform.GetComponent<TestResearch>() as TestResearch;

            if(this._turnText != null)
                this._turnText.text = "Turn: " + this._turnCount.ToString() + "\r\n" + 
                                      "Turn Ended: " + this._turnEnded.ToString();
        }

        private void Update() {
            if(this.GetInput()) {
                
            }

            if(this._turnEnded) {
                this.NewTurn();
            } 
        }

        #endregion

        #region CLASS
        public void UnlockUnitSpawn(ClassType classType, UnitType unitType) {
            this._castleUI.UnlockSpawnButton(classType, unitType);
        }

        public void AddNewUpgrades(TestUpgradeScriptable data) {
            TestResearchUpgradeData upgradeData = new TestResearchUpgradeData(data);

            this._unitUpgrades[data.classType].Add(upgradeData);

            // Apply data to units the player controls
        }

        private bool GetInput() {
            if(Input.GetKeyUp(KeyCode.F)) {
                this.EndTurn();
                return true;
            }

            return false;
        }

        private void EndTurn() {
            this._turnEnded = true;
        }

        private void NewTurn() {

            this._turnCount++;
            this._turnEnded = false;

            this._turnText.text = "Turn: " + this._turnCount.ToString() + "\r\n" +
                                      "Turn Ended: " + this._turnEnded.ToString();

            if(this._research.IsResearchPhase(this._turnCount))
                this._research.DisplayResearchCards();
            else
                this._research.DisplayResearchCards();

            this.UpdateUI();
        }

        private void UpdateUI() {
            this._turnText.text = "Turn: " + this._turnCount.ToString() + "\r\n" + "Turn Ended: " + this._turnEnded.ToString();
        }

        public void DebugUpgradeList() {
            foreach(ClassType type in this._unitUpgrades.Keys) {
                List<TestResearchUpgradeData> upgrades = this._unitUpgrades[type];

                foreach(TestResearchUpgradeData upgrade in upgrades) {
                    Debug.Log(type.ToString() + " - " + upgrade.UpgradeType.ToString());
                    foreach(UnitType unitType in upgrade.UnitTypes) {
                        Debug.Log("List of Units Affected: " + unitType.ToString());
                    }
                }
            }
        }
        #endregion
    }
}