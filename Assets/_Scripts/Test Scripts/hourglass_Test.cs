namespace Testing
{

    using System.Collections;
    using UnityEngine;

    public class hourglass_Test : MonoBehaviour
    {
        public float roundTimer = 60; //total amount of seconds in a round: default to 60 seconds
        private float currentTimer; //the amount of seconds passed since the beginning of the round

        float ElapsedTimePercentage
        {
            get
            {
                return currentTimer / roundTimer; //returns the percentage of how completed the current round is
            }
        }

        public Transform sandTop; //the top sand gameobject
        public Transform sandBot; //the bottom sand gameobject
        public ParticleSystem sandParticles; //the particle system to simulate sand dropping into the bottom

        public AnimationCurve sandTopCurve; //the curve which determines the size of the top sand pile
        public AnimationCurve sandBotCurve; //the curve which determines the size of the bottom sand pile

        public AnimationCurve resetRotationCurve; //the curve which determines the hourglass rotation when reset
        public float timeToReset = 2f; //the time it takes for the reset animation to complete
        public bool autoStartHourglassOnReset = true; //a check for if we want to automatically start the hourglass again after reset

        private Coroutine countDownCoroutine; //the coroutine which counts up the time, save this so we can cancel it if neccessary
        private bool isCountingDown = false; //a check for whether the hourglass is currently counting down
        private bool autoStartHourglassOnFinish = true; //a check for if we want to automatically start the hourglass again after finish

        private void Start()
        {
            StartHourglass();
        }

        private void InitializeVariables()
        {
            currentTimer = 0;
            sandTop.localScale = Vector3.one;
            sandBot.localScale = Vector3.zero;

            sandTopCurve.keys[1].value = 0;
            sandBotCurve.keys[1].value = 1;
        }

        public void StartHourglass()
        {
            InitializeVariables();

            countDownCoroutine = StartCoroutine(CountdownHourglass());
            sandParticles.Play();
        }

        public void StopHourglass()
        {
            isCountingDown = false;

            StopCoroutine(countDownCoroutine);
            sandParticles.Stop();
        }

        public void RestartHourglass()
        {
            if (isCountingDown)
                StopHourglass();

            StartCoroutine(HourglassResetAnimation());
        }

        private IEnumerator CountdownHourglass()
        {
            isCountingDown = true;

            while (currentTimer <= roundTimer)
            {
                currentTimer += Time.deltaTime;

                sandTop.localScale = Vector3.one * sandTopCurve.Evaluate(ElapsedTimePercentage); //shrinks the top sand as time increases
                sandBot.localScale = Vector3.one * sandBotCurve.Evaluate(ElapsedTimePercentage); //grows the bottom sand as time increases

                yield return new WaitForEndOfFrame();
            }

            sandParticles.Stop();
            isCountingDown = false;

            if (autoStartHourglassOnFinish)
                RestartHourglass();

            yield return null;
        }

        private IEnumerator HourglassResetAnimation()
        {
            SetResetVariableValues();

            float resetTimer = timeToReset;
            Vector3 currentRotation = transform.rotation.eulerAngles;

            while (resetTimer > 0)
            {
                resetTimer -= Time.deltaTime;
                float invertedElapsedTime = resetTimer / timeToReset;

                sandTop.localScale = Vector3.one * sandTopCurve.Evaluate(invertedElapsedTime);
                sandBot.localScale = Vector3.one * sandBotCurve.Evaluate(invertedElapsedTime);

                currentRotation.z = resetRotationCurve.Evaluate(invertedElapsedTime);
                transform.rotation = Quaternion.Euler(currentRotation);

                yield return new WaitForEndOfFrame();
            }

            if (autoStartHourglassOnReset)
                StartHourglass();

            yield return null;
        }

        private void SetResetVariableValues()
        {
            sandTopCurve.keys[1].value = sandTop.transform.localScale.x;
            sandBotCurve.keys[1].value = sandBot.transform.localScale.x;
        }
    }

}
