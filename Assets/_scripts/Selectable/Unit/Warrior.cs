namespace Unit {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class Warrior : Unit {

        private void Awake() {
            this.Init();
        }

        private void Init() {
            this._currentHP = 15;
            this._maxHP = 15;
            this._movementRange = 3;
            this._rangeProfile = RangeProfile.MELEE;
        }
    }
}