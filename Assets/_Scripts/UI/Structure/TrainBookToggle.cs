namespace UI {

    using UnityEngine;
    using UnityEngine.EventSystems;

    public class TrainBookToggle : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler {

        #region UNITY

        private CastleUI _castleUI;

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {
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