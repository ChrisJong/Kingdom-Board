namespace Player {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    using Castle;

    [System.Serializable]
    public struct PlayerInfo {
        public int id;
        public bool turnEnded;

        public float goldResource;
        public float resourceLImit;

        public float masteryPoints;
        public float masteryLimit;

        public int unitCount;
        public int unitLimit;
    }

    public class Player : MonoBehaviour {

        public PlayerInfo _playerData = new PlayerInfo();

        public List<GameObject> _units;

        public GameObject _castleSpawn;
        public GameObject _castle;

        #region UNITY_METHODS
        void Awake() {
            
        }
        #endregion

        #region PLAYER_METHODS
        public void init(GameObject castleSpawn, int id) {
            this._playerData.id = id;
            this._castleSpawn = castleSpawn;

            this.SpawnCastle();
        }

        public void EndTurn() {
            if(this._playerData.turnEnded == false)
                this._playerData.turnEnded = true;
            else
                return;
        }

        private void SpawnCastle() {
        }
        #endregion
    }
}