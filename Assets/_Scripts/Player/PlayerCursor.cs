namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    [System.Serializable]
    public class PlayerCursor : MonoBehaviour {

        [SerializeField] private Texture2D _default;
        [SerializeField] private Texture2D _selection;
        [SerializeField] private Texture2D _moveReady;
        [SerializeField] private Texture2D _moveNotReady;
        [SerializeField] private Texture2D _attackReady;
        [SerializeField] private Texture2D _attackNotReady;

        [SerializeField] private CursorMode _mode = CursorMode.Auto;

        [SerializeField] private Vector3 _hotSpot = Vector3.zero;

        public void SetDefault() {
            Cursor.SetCursor(this._default, this._hotSpot, this._mode);
        }

        public void SetMoveReady() {
            Cursor.SetCursor(this._moveReady, this._hotSpot, this._mode);
        }

        public void SetMoveNotReady() {
            Cursor.SetCursor(this._moveNotReady, this._hotSpot, this._mode);
        }

        public void SetAttackReady() {
            Cursor.SetCursor(this._attackReady, this._hotSpot, this._mode);
        }

        public void SetAttackNotReady() {
            Cursor.SetCursor(this._attackNotReady, this._hotSpot, this._mode);
        }
    }

}