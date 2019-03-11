namespace KingdomBoard.Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Player;
    using Scriptable;
    using Structure;

    public class ResearchUpgradeData {

        #region VARIABLE

        private Player _controller = null;
        private Castle _castle = null;

        private UpgradeScriptable _data = null;

        public int _keyID = -1;
        private int _researchCount = 0; // 0 = Not Researched/Upgraded, 1> is First Instance of Research/Upgrade
        private int _researchLimit = 5;

        private float _currentValue = 0.0f;
        private float _previousValue = 0.0f;
        private float _currIncrementValue = 0.0f;
        private float _prevIncrementValue = 0.0f;

        public int KeyID { get { return this._keyID; } }

        public int ResearchCount { get { return this._researchCount; } }

        public float CurrentValue { get { return this._currentValue; } }

        public float PreviousValue { get { return this._previousValue; } }

        public float CurrentincrementValue { get { return this._currIncrementValue; } }

        public float UpgradeValue { get { return this._data.value; } }

        public UnitClassType ClassType { get { return this._data.classType; } }

        public List<UnitType> UnitTypes { get { return this._data.unitTypes; } }

        public UnitUpgradeType UpgradeType { get { return this._data.upgradeType; } }

        #endregion

        #region CLASS

        public ResearchUpgradeData(int keyID = -1, UpgradeScriptable data = null) {
            this._keyID = keyID;
            this._data = data;

            this._researchCount = 0;
            this._currentValue = data.value;
            this._previousValue = data.value;
        }

        public bool CheckUpgradeType(UnitClassType classType, UnitType unitType, UnitUpgradeType upgradeType) {

            if(this._data.classType == classType) {
                if(this._data.unitTypes.Contains(unitType))
                    if(this._data.upgradeType == upgradeType)
                        return true;
            }

            return false;
        }

        public void ResearchUpgrade() {
            this._researchCount++;

            //this.IncreaseValue();

            // apply the research to the units for the player.
        }

        private void IncreaseValue() {
            if(this._researchCount == 1)
                return;

            this._previousValue = this._currentValue;
            this._currentValue += this._currIncrementValue;
        }

        #endregion
    }
}