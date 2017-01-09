namespace Castle {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class castle : MonoBehaviour {

        // change to private
        public int _playerID = 0;

        public List<GameObject> unitQueue;

        public void init(int playerID) {
            this._playerID = playerID;
        }

        private void Awake() {
            this.unitQueue = new List<GameObject>();
        }

        public int PlayerID {
            get { return this._playerID; }
        }
    }
}