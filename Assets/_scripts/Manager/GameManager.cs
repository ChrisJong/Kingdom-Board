namespace Manager {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class GameManager : Extension.SingletonMono<GameManager> {
        #region VARIABLES
        public int _numberOfPlayers = 2;

        public Player.Player[] _playerArray;
        #endregion

        #region UNITY_METHODS

        #endregion

        #region GAMEMANAGER_METHODS
        public override void Init() {
            this._playerArray = new Player.Player[_numberOfPlayers];
        }

        private void AddPlayer() {

        }
        #endregion
    }
}