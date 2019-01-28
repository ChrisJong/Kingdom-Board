namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    [System.Serializable]
    public class PlayerCursor : MonoBehaviour {

        [SerializeField] private Texture2D _currentCursor = null;

        [SerializeField] private Texture2D _default;
        [SerializeField] private Texture2D _selection;
        [SerializeField] private Texture2D _moveReady;
        [SerializeField] private Texture2D _moveNotReady;
        [SerializeField] private Texture2D _attackReady;
        [SerializeField] private Texture2D _attackNotReady;
        [SerializeField] private Texture2D _healReady;
        [SerializeField] private Texture2D _healNotReady;

        [SerializeField] private CursorMode _mode = CursorMode.Auto;

        [SerializeField] private Vector3 _hotSpot = Vector3.zero;

        public void SetDefault() {

            if(this._currentCursor == this._default)
                return;

            this._currentCursor = this._default;
            Cursor.SetCursor(this._default, this._hotSpot, this._mode);
        }

        public void SetSelection() {

            if(this._currentCursor == this._selection)
                return;

            this._currentCursor = this._selection;
            Cursor.SetCursor(this._selection, this._hotSpot, this._mode);
        }

        public void SetMoveReady() {

            if(this._currentCursor == this._moveReady)
                return;

            this._currentCursor = this._moveReady;
            Cursor.SetCursor(this._moveReady, this._hotSpot, this._mode);
        }

        public void SetMoveNotReady() {

            if(this._currentCursor == this._moveNotReady)
                return;

            this._currentCursor = this._moveNotReady;
            Cursor.SetCursor(this._moveNotReady, this._hotSpot, this._mode);
        }

        public void SetAttackReady() {

            if(this._currentCursor == this._attackReady)
                return;

            this._currentCursor = this._attackReady;
            Cursor.SetCursor(this._attackReady, this._hotSpot, this._mode);
        }

        public void SetAttackNotReady() {

            if(this._currentCursor == this._attackNotReady)
                return;

            this._currentCursor = this._attackNotReady;
            Cursor.SetCursor(this._attackNotReady, this._hotSpot, this._mode);
        }

        public void SetHealReady() {
            if(this._currentCursor == this._healReady)
                return;

            this._currentCursor = this._healReady;
            Cursor.SetCursor(this._healReady, this._hotSpot, this._mode);
        }

        public void SetHealNotReady() {
            if(this._currentCursor == this._healNotReady)
                return;

            this._currentCursor = this._healNotReady;
            Cursor.SetCursor(this._healNotReady, this._hotSpot, this._mode);
        }
    }

}