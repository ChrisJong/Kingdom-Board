﻿namespace KingdomBoard.Research {

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
        PLAYING,
        FINISHED
    }

    public class ResearchCardAnimation : MonoBehaviour, IPointerUpHandler {

        [SerializeField] private ResearchCard _card;

        [SerializeField] private CardState _state = CardState.NONE;

        [SerializeField] private IEnumerator _currentPlayingAnim = null;

        [SerializeField] private Animation _animation = null;

        [SerializeField] private Vector3 _moveTo = Vector3.zero;
        [SerializeField] private Vector3 _currentRotation = Vector3.zero;

        [SerializeField] private float _moveSpeed = 15.0f;
        [SerializeField] private float _rotationSpeed = 5.0f;
        [SerializeField] private float _yCoord = 0.0f;

        public CardState State { get { return this._state; } set { this._state = value; } }

        #region UNITY
        public void OnPointerUp(PointerEventData eventData) {
            if(this._state == CardState.FINISHED) {
                this.PlayClickAnimation();
            }
        }
        #endregion

        #region CLASS

        public void Init(ResearchCard card) {
            this._animation = this.transform.GetComponent<Animation>() as Animation;

            this._card = card;
        }

        public void PlaySpawnAnimation() {
            this._state = CardState.START;
            this._animation.Play("CardSpawn_Anim");
        }

        public void PlayClickAnimation() {
            this._animation.Play("CardClick_Anim");
        }

        public void PlayFadeAnimation() {
            this._state = CardState.FADE;

            this._animation.Play("CardFade_Anim");
        }

        public void DisableCard() {
            this.StopCoroutineAnimation();
            this._currentPlayingAnim = this.RotateAndFade();
            StartCoroutine(this._currentPlayingAnim);
        }

		/// <summary>
		/// Changes the current state of the card.
		/// </summary>
        public void ChangeState() {
            if(this._state == CardState.START) {
                this.StopCoroutineAnimation();
                this._currentPlayingAnim = this.StartRotation();
                StartCoroutine(this._currentPlayingAnim);
            } else if(this._state == CardState.FADE) {
                this._state = CardState.FINISHED;
                this._card.HideCard();
            } else if(this._state == CardState.MOVE) {
                this.StopCoroutineAnimation();
                this._currentPlayingAnim = this.MoveToPostion();
                StartCoroutine(this._currentPlayingAnim);
            }
        }

        public void Back(Vector3 value) {
            this._moveTo = value;

            this._state = CardState.PLAYING;
            this._card.Ready = false;

            this.StopCoroutineAnimation();
            this._currentPlayingAnim = this.MoveAndRotate();
            StartCoroutine(this._currentPlayingAnim);
        }

        public void SetMoveTo(Vector3 value) {
            this._moveTo = value;

            if(this._card.rectTransform.anchoredPosition.Equals(value)) {
                this.StopCoroutineAnimation();
                this._currentPlayingAnim = this.StartRotation();
                StartCoroutine(this._currentPlayingAnim);
                return;
            }

            this.StopCoroutineAnimation();
            this._currentPlayingAnim = this.MoveToPostion();
            StartCoroutine(this._currentPlayingAnim);
        }

        public IEnumerator MoveToPostion() {
            if(this._state == CardState.MOVE)
                yield break;

            this._state = CardState.MOVE;

            Vector3 startpos = this._card.rectTransform.anchoredPosition;
            float distanceRemnaining = Mathf.Abs(startpos.x - this._moveTo.x);
            int rightOrLeft = (startpos.x > this._moveTo.x) ? -1 : 1;

            while(distanceRemnaining > 0) {

                distanceRemnaining = Mathf.Abs(this._card.rectTransform.anchoredPosition.x - this._moveTo.x);
                Vector3 newPos = this._card.rectTransform.anchoredPosition;

                if(distanceRemnaining < this._moveSpeed) {
                    newPos.x = this._moveTo.x;
                    this._card.rectTransform.anchoredPosition = newPos;
                    break;
                } else {
                    newPos.x += this._moveSpeed * rightOrLeft;
                    this._card.rectTransform.anchoredPosition = newPos;
                }

                yield return new WaitForEndOfFrame();

            }

            this._state = CardState.FINISHED;

            // Rotate Cards around.
			// NOTE: dangerous coroutine call for the rotation need to be a blocked in code.
            StartCoroutine(StartRotation());

            yield return null;
        }

        /// <summary>
        /// Full Rotation to Display the opposite face of a card.
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartRotation() {

            if(this._state == CardState.ROTATE)
                yield break;
            else {

                this._state = CardState.ROTATE;

                if(this._card.GetType() == typeof(ResearchUpgradeCard))
                    ((ResearchUpgradeCard)this._card).ActivateText(false);

                while(this._yCoord < 90.0f) {

                    this.RotateCard();
                    yield return new WaitForEndOfFrame();
                }

                this._card.ChangeFace();
                this.FlipXScale();

                if(this._card.GetType() == typeof(ResearchUpgradeCard))
                    ((ResearchUpgradeCard)this._card).ActivateText(true);

                while(this._yCoord < 180.0f) {

                    this.RotateCard();
                    yield return new WaitForEndOfFrame();
                }

                this.RotateCard(0.0f);
                this.FlipXScale();

                this._state = CardState.FINISHED;

                yield return null;
            }
        }

        public IEnumerator RotateAndFade() {
            if(this._state == CardState.ROTATE)
                yield break;
            else {

                this._state = CardState.ROTATE;

                while(this._yCoord < 90.0f) {

                    this.RotateCard();
                    yield return new WaitForEndOfFrame();
                }

                this._card.ChangeFace();
                this.FlipXScale();

                if(this._card.GetType() == typeof(ResearchUpgradeCard))
                    ((ResearchUpgradeCard)this._card).ActivateText(false);

                while(this._yCoord < 180.0f) {

                    this.RotateCard();
                    yield return new WaitForEndOfFrame();
                }

                this.RotateCard(0.0f);
                this.FlipXScale();

                this._state = CardState.FINISHED;
                this.PlayFadeAnimation();
                this._card.Finished();

                yield return null;

            }
        }

        public IEnumerator MoveAndRotate() {

            if(this._card.GetType() == typeof(ResearchUpgradeCard))
                ((ResearchUpgradeCard)this._card).ActivateText(true);

            while(this._yCoord < 90.0f) {

                this.RotateCard();
                yield return new WaitForEndOfFrame();
            }

            this._card.ChangeFace();
            this.FlipXScale();

            if(this._card.GetType() == typeof(ResearchUpgradeCard))
                ((ResearchUpgradeCard)this._card).ActivateText(false);

            while(this._yCoord < 180.0f) {

                this.RotateCard();
                yield return new WaitForEndOfFrame();
            }

            this.RotateCard(0.0f);
            this.FlipXScale();

            if(!this._card.rectTransform.anchoredPosition.Equals(this._moveTo)) {

                if(this._state == CardState.MOVE)
                    yield break;

                this._state = CardState.MOVE;

                Vector3 startpos = this._card.rectTransform.anchoredPosition;
                float distanceRemnaining = Mathf.Abs(startpos.x - this._moveTo.x);
                int rightOrLeft = (startpos.x > this._moveTo.x) ? -1 : 1;

                while(distanceRemnaining > 0) {

                    distanceRemnaining = Mathf.Abs(this._card.rectTransform.anchoredPosition.x - this._moveTo.x);
                    Vector3 newPos = this._card.rectTransform.anchoredPosition;

                    if(distanceRemnaining < this._moveSpeed) {
                        newPos.x = this._moveTo.x;
                        this._card.rectTransform.anchoredPosition = newPos;
                        break;
                    } else {
                        newPos.x += this._moveSpeed * rightOrLeft;
                        this._card.rectTransform.anchoredPosition = newPos;
                    }

                    yield return new WaitForEndOfFrame();

                }
            }

            this._state = CardState.FINISHED;
            this._card.Ready = true;

            yield return null;

        }

        private void RotateCard() {
            this._yCoord += this._rotationSpeed;
            this._currentRotation.y = this._yCoord;

            this._card.rectTransform.rotation = Quaternion.Euler(this._currentRotation);
        }

        private void RotateCard(float value) {
            this._yCoord = value;
            this._currentRotation.y = this._yCoord;

            this._card.rectTransform.rotation = Quaternion.Euler(this._currentRotation);
        }

        private void FlipXScale() {
            Vector3 newScale = new Vector3(this._card.rectTransform.localScale.x * -1.0f, 1.0f, 1.0f);
            this._card.rectTransform.localScale = newScale;
        }

        private void StopCoroutineAnimation() {
            if(this._currentPlayingAnim != null)
                StopCoroutine(this._currentPlayingAnim);

            this._state = CardState.NONE;
            this._currentPlayingAnim = null;
        }
        #endregion
    }
}