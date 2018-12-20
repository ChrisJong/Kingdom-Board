namespace Research {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;


    public enum CardState {
        NONE = 9,
        ANY = ~0,
        START = 1,
        ROTATE,
        FADE,
        MOVE,
        FINISHED
    }

    public class ResearchCardAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

        [SerializeField] private ResearchCard _card;

        [SerializeField] private CardState _state = CardState.NONE;

        [SerializeField] private Animation _animation = null;

        [SerializeField] private Vector3 _moveTO = Vector3.zero;

        #region UNITY

        public void OnPointerDown(PointerEventData eventData) {
            Debug.Log("Card DOWN");
        }

        public void OnPointerUp(PointerEventData eventData) {
            Debug.Log("Card UP");

            if(this._state == CardState.FINISHED) {
                this.PlayClickAnimation();
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("Card CLICKED");
        }

        public void Update() {
            
        }

        #endregion


        #region CLASS

        public void Init(ResearchCard card) {
            this._animation = this.transform.GetComponent<Animation>() as Animation;

            this._card = card;
        }

        public void PlaySpawnAnimation() {
            Debug.Log("Play Spawn Animation");
            this._animation.Play("spawnResearchCard");
        }

        private void PlayClickAnimation() {
            Debug.Log("Play Click Animation");
            this._animation.Play("clickResearchCard");
        }
        #endregion
    }
}