namespace Unit {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public abstract class Unit : Selectable.SelectableObject {

        public int _playerID;

        public int _currentHP;
        public int _maxHP;
        public int _minHP = 0;

        public int _CurrentMP;
        public int _maxMP;
        public int _minMP = 0;

        public int _attack;
        public int _range;
        public RangeProfile _rangeProfile = RangeProfile.NONE;
        public int _movementRange;

        public enum TargetSize {
            NONE = 0,
            SINGLE = 1,
            AOE
        }

        public enum RangeProfile {
            NONE = 0,
            MELEE = 1,
            RANGE
        }

        public enum ResistanceType {

        }

        public virtual void AttackTarget(GameObject go) {
            Unit target = go.GetComponent<Unit>() as Unit;

            target.DealDamage(this._attack);
        }

        public virtual void DealDamage(int amount) {
            this._currentHP -= amount;
            if (this._currentHP == this._minHP)
                this.Destroy();
        }
    }
}