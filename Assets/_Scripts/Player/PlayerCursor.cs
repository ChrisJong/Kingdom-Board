namespace Player {

    using UnityEngine;

    using Enum;

    public class PlayerCursor : MonoBehaviour {

        #region VARIABLE
        [SerializeField] private bool _clicked = false;

        private float _delay = 0.25f;
        [SerializeField] private float _timer = 0.0f;

        [Space]
        [SerializeField] private CursorState _currentState;

        [Space]
        [SerializeField] private Texture2D _currentCursor = null;

        [Space]
        [SerializeField] private Texture2D _default;
        [SerializeField] private Texture2D _defaultOnclick;
        [SerializeField] private Texture2D _selection;
        [SerializeField] private Texture2D _selectionOnClick;
        [SerializeField] private Texture2D _moveReady;
        [SerializeField] private Texture2D _moveReadyOnClicked;
        [SerializeField] private Texture2D _moveNotReady;
        [SerializeField] private Texture2D _attackReady;
        [SerializeField] private Texture2D _attackReadyOnClick;
        [SerializeField] private Texture2D _attackNotReady;
        [SerializeField] private Texture2D _healReady;
        [SerializeField] private Texture2D _healReadyOnClick;
        [SerializeField] private Texture2D _healNotReady;

        [Space]
        [SerializeField] private CursorMode _mode = CursorMode.Auto;

        [SerializeField] private Vector3 _hotSpot = Vector3.zero;
        #endregion

        #region CLASS
        public void UpdateCursor() {
            if(this._clicked)
                this._timer += Time.deltaTime;

            if(Input.GetMouseButtonUp(0)) { // Left
                this.SetCursor(true);
            } else if(Input.GetMouseButtonUp(1)) {// Right
                this.SetCursor(true);
            } else {

                if(this._clicked) {

                    if(_timer > this._delay) {
                        this._clicked = false;
                        this.SetCursor();
                    }

                } else {
                    this.SetCursor();
                }
            }
        }

        public void ChangeState(CursorState state) {

            if(state == this._currentState)
                return;

            this._currentState = state;
        }

        private void SetCursor(bool clicked = false) {
            switch(this._currentState) {
                case CursorState.DEFAULT:
                this.SetDefault(clicked);
                break;

                case CursorState.SELECTED:
                this.SetSelection(clicked);
                break;

                case CursorState.MOVE:
                this.SetMoveReady(clicked);
                break;

                case CursorState.MOVE_NOTREADY:
                this.SetMoveNotReady();
                break;

                case CursorState.ATTACK:
                this.SetAttackReady(clicked);
                break;

                case CursorState.ATTACK_NOTREADY:
                this.SetAttackNotReady();
                break;

                case CursorState.HEAL:
                this.SetHealReady(clicked);
                break;

                case CursorState.HEAL_NOTREADY:
                this.SetHealNotReady();
                break;
            }
        }

        private void SetDefault(bool clicked = false) {

            if(clicked) {
                this._clicked = true;
                this._timer = 0.0f;
                this._currentCursor = this._defaultOnclick;
            } else {
                this._currentCursor = this._default;
            }

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }

        private void SetSelection(bool clicked = false) {

            if(clicked) {
                this._clicked = true;
                this._timer = 0.0f;
                this._currentCursor = this._selectionOnClick;
            } else {
                this._currentCursor = this._selection;
            }

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }

        private void SetMoveReady(bool clicked = false) {

            if(clicked) {
                this._clicked = true;
                this._timer = 0.0f;
                this._currentCursor = this._moveReadyOnClicked;
            } else
                this._currentCursor = this._moveReady;

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }

        private void SetMoveNotReady() {

            if(this._currentCursor == this._moveNotReady)
                return;

            this._currentCursor = this._moveNotReady;

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }

        private void SetAttackReady(bool clicked = false) {

            if(clicked) {
                this._clicked = true;
                this._timer = 0.0f;
                this._currentCursor = this._attackReadyOnClick;
            } else
                this._currentCursor = this._attackReady;

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }

        private void SetAttackNotReady() {

            if(this._currentCursor == this._attackNotReady)
                return;

            this._currentCursor = this._attackNotReady;

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }

        private void SetHealReady(bool clicked = false) {

            if(clicked) {
                this._clicked = true;
                this._timer = 0.0f;
                this._currentCursor = this._healReadyOnClick;
            } else
                this._currentCursor = this._healReady;

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }

        private void SetHealNotReady() {

            if(this._currentCursor == this._healNotReady)
                return;

            this._currentCursor = this._healNotReady;

            Cursor.SetCursor(this._currentCursor, this._hotSpot, this._mode);
        }
    }
}
#endregion