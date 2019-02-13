namespace Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.EventSystems;

    using Manager;

    public class PlayerHourglass : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {

        #region VARIABLE
        [SerializeField] Player _controller;

        private bool _isCountingDown = false;

        [SerializeField] private float _resetTime = 2.0f;

        [SerializeField] private Transform _sandTop = null;
        [SerializeField] private Transform _sandBottom = null;
        [SerializeField] private ParticleSystem _sandParticles = null;

        [SerializeField] private AnimationCurve _sandTopCurve = null;
        [SerializeField] private AnimationCurve _sandBottomCurve = null;
        [SerializeField] private AnimationCurve _resetRotationCurve = null;

        [SerializeField] private Coroutine _currentAnimnation = null;
        #endregion

        #region UNITY
        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("Hourglass Clicked!");
        }

        public void OnPointerDown(PointerEventData eventData) {
            Debug.Log("Hourglass Down!");
        }

        public void OnPointerUp(PointerEventData eventData) {
            Debug.Log("Hourglass Up!");
            this.OnClick();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Debug.Log("Hourglass Enter!");
        }

        public void OnPointerExit(PointerEventData eventData) {
            Debug.Log("Hourglass Exit");
        }
        #endregion

        #region CLASS
        public void Setup(Player controller) {
            this._controller = controller;

            this._sandTop.localScale = Vector3.one;
            this._sandBottom.localScale = Vector3.zero;

            this._sandTopCurve.keys[1].value = 0;
            this._sandBottomCurve.keys[1].value = 1;
        }

        public void Init() {

            this.ResetHourglass();

            if(this._currentAnimnation != null)
                StopCoroutine(this._currentAnimnation);
            this._currentAnimnation = null;
            //this._currentAnimnation = StartCoroutine();
            this._sandParticles.Play();
        }

        public void OnClick() {
            if(this._controller.CurrentState == Enum.PlayerState.WAITING)
                return;

            if(this._controller.playerSelect.CanSelect) {
                this.StopHourglass();
                this._controller.EndTurn();
            }
        }

        public void StartHourglass() {
            this.ResetHourglass();
        }

        public void StopHourglass() {
            this._isCountingDown = false;

            if(this._currentAnimnation != null)
                StopCoroutine(this._currentAnimnation);

            this._currentAnimnation = null;
            this._sandParticles.Stop();
        }

        public void ResetHourglass() {

            if(this._isCountingDown)
                StopCoroutine(this._currentAnimnation);

            this._currentAnimnation = null;
            this._currentAnimnation = StartCoroutine(this.ResetHourglassAnimation());
        }

        private IEnumerator CountdownHourglass() {

            this._isCountingDown = true;

            this._sandTop.localScale = Vector3.one;
            this._sandBottom.localScale = Vector3.zero;

            this._sandTopCurve.keys[1].value = 0;
            this._sandBottomCurve.keys[1].value = 1;

            this._sandParticles.Play();

            while(this._controller.CurrentState == Enum.PlayerState.ATTACKING || this._controller.NextState == Enum.PlayerState.ATTACKING) {

                this._sandTop.localScale = Vector3.one * this._sandTopCurve.Evaluate(GameManager.instance.ElapsedTime);
                this._sandBottom.localScale = Vector3.one * this._sandBottomCurve.Evaluate(GameManager.instance.ElapsedTime);

                yield return new WaitForEndOfFrame();
            }

            this._sandParticles.Stop();
            this._isCountingDown = false;

            yield return null;
        }

        private IEnumerator ResetHourglassAnimation() {

            this._sandTopCurve.keys[1].value = this._sandTop.transform.localScale.x;
            this._sandBottomCurve.keys[1].value = this._sandBottom.transform.localScale.x;

            float resetTimer = this._resetTime;
            Vector3 currentRotation = this.transform.localRotation.eulerAngles;

            while(resetTimer > 0.0f) {

                Debug.Log("Reset Hourglass");
                resetTimer -= Time.deltaTime;
                float invertedTimer = resetTimer / this._resetTime;

                this._sandTop.localScale = Vector3.one * this._sandTopCurve.Evaluate(invertedTimer);
                this._sandBottom.localScale = Vector3.one * this._sandBottomCurve.Evaluate(invertedTimer);

                currentRotation.z = this._resetRotationCurve.Evaluate(invertedTimer);
                this.transform.localRotation = Quaternion.Euler(currentRotation);

                yield return new WaitForEndOfFrame();
            }

            this._currentAnimnation = null;
            this._currentAnimnation = StartCoroutine(this.CountdownHourglass());

            yield return null;
        }
        #endregion
    }
}