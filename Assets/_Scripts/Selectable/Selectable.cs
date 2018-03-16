namespace Selectable {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Player;

    public abstract class Selectable : MonoBehaviour {
        private int _playerID;
        private int _id;

        protected string _name;

        protected Player _player;

        //////////
        /// UI ///
        //////////
        public Canvas UI;

        //////////////////
        /// ATTRIBUTES ///
        //////////////////
        public int currentHealth;
        public int maxHealth;
        public int CurrentMana;
        public int maxMana;

        #region UNITY
        protected virtual void OnMouseEnter() {
        }
        protected virtual void OnMouseExit() {
        }
        #endregion

        #region CLASS_METHOD
        public virtual void Init() {
            this._name = "SELECTABLE";
        }

        public virtual void Init(Player p) {
            this._player = p;
        }

        public virtual void Init(string name) {
            this._name = name;
        }

        public virtual void ReturnToPool() {

        }

        public virtual void DisplayUI() {

        }

        public virtual void HideUI() {

        }
        #endregion
    }
}