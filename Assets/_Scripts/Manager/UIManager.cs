﻿namespace Manager {

    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.UI;

    using Player;
    using Extension;

    public sealed class UIManager : SingletonMono<UIManager> {

        [SerializeField]
        private GameObject _playerUISeutup;

        private int _numOfPlayers;

        private readonly Dictionary<Player, GameObject> _pool;

        protected override void Awake() {
            base.Awake();
        }

        public bool SetupPlayerUI(int numOfPlayers, Player player) {



            return true;
        }
    }
}