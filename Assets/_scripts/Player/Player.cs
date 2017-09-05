namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class Player : MonoBehaviour {
        #region VARIABLES

        private int _playerID;
        public int PlayerID { get { return this._playerID; } }

        public List<GameObject> _unitList; // Change to Unit Class
        public List<GameObject> _unitSpawnList; // Change to Unit Class

        public int _maxUnitCap = 25;
        public int _currentUnitCap = 0;

        public GameObject _castle; // Change to Structure Class
        public GameObject _currentSelection; // Change to Unit Class

        public bool _turnEnded = false;
        public bool _isAttacking = false;

        #endregion

        #region UNITY_METHODS
        private void Update() {
            
        }
        #endregion


    }
}