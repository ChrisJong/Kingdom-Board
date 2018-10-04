namespace Unit {

    using System;

    using UnityEngine;

    [Serializable]
    public sealed class UnitDeath {
        [SerializeField] private GameObject _deathPrefab;

        [SerializeField] private int _turnCounter = 0;

        public int TurnCounter { get { return this._turnCounter; } }

        public UnitDeath(GameObject go) {
            this._deathPrefab = go;
            this._turnCounter = 0;
        }

        public UnitDeath(GameObject go, int counter) {
            this._deathPrefab = go;
            this._turnCounter = counter;
        }

        public void Countdown() {
            this._turnCounter -= 1;
            Debug.Log(this._deathPrefab.name + ": " + this._turnCounter.ToString());

            if(this._turnCounter == 0)
                GameObject.Destroy(this._deathPrefab);
        }
    }
}