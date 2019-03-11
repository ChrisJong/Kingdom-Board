namespace KingdomBoard.UI {
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class BookToggle : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler {

        private CastleUI _castleUI = null;

        #region UNITY
        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {
            if(!this._castleUI.SpawnGroupToggle)
                this._castleUI.ToggleSpawnGroup();
        }
        #endregion

        #region CLASS
        public void Init(CastleUI parent) {
            this._castleUI = parent;
        }
        #endregion
    }
}