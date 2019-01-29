namespace UI {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class TrainBookToggle : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler {

        #region UNITY

        private CastleUI _castleUI;

        public void OnPointerClick(PointerEventData eventData) {}

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData) {
            if(!this._castleUI.SpawnGroupToggle) {
                this._castleUI.ToggleSpawnGroup();
            }
        }

        #endregion

        #region UNITY

        private void Update() {
            if(!this.gameObject.activeSelf)
                return;

            if(!this._castleUI.SpawnGroupToggle)
                return;

            if(this.gameObject.activeSelf && this._castleUI.SpawnGroupToggle) {
                if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                    if(!EventSystem.current.IsPointerOverGameObject())
                        this._castleUI.ToggleSpawnGroup(false);
                }
            }
        }

        #endregion

        #region CLASS

        public void Init(CastleUI parent) {
            this._castleUI = parent;
        }

        #endregion
    }
}