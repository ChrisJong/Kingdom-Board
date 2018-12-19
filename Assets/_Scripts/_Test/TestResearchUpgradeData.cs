namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    public class TestResearchUpgradeData {

        #region VARIABLE

        [SerializeField] private int _id = 0;
        
        [SerializeField] private bool enabled = false;

        [SerializeField] private TestUpgradeScriptable upgradeData;

        public ClassType ClassType {
            get { return this.upgradeData.classType; }
        }

        public List<UnitType> UnitTypes {
            get { return this.upgradeData.unitType; }
        }

        public UnitUpgrade UpgradeType {
            get { return this.upgradeData.upgrade; }
        }

        public float Value {
            get { return this.upgradeData.value; }
        }

        public TestResearchUpgradeData(TestUpgradeScriptable data, bool enabled = true) {
            this.upgradeData = data;

            this.enabled = enabled;
        }

        public void EnableUpgrade() {
            if(!this.enabled)
                this.enabled = true;
            else
                return;
        }

        public void DisableUpgrade() {
            if(this.enabled)
                this.enabled = false;
            else
                return;
        }

        #endregion
    }
}