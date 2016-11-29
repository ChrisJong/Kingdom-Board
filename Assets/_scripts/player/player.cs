namespace Player {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    using Castle;

    public class Player : MonoBehaviour {

        // change to private;
        public int _id = 0;

        public List<GameObject> units;

        public GameObject _playerObject;
        public GameObject _castleSpawn;
        public GameObject _castle;

        void Awake() {
            
        }

        public void init(GameObject playerObj, GameObject castleSpawn, GameObject castle, int id) {
            this._id = id;
            this._playerObject = playerObj;
            this._castleSpawn = castleSpawn;

            this.SpawnCastle(castle);
        }

        private void SpawnCastle(GameObject castle) {
            this._castle = GameObject.Instantiate(assetPool.instance.castle, this._castleSpawn.transform.position, this._castleSpawn.transform.rotation) as GameObject;
            this._castle.transform.SetParent(this._playerObject.transform);
        }

        public int ID {
            get { return this._id; }
        } 
    }
}