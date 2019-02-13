namespace Testing
{

    using UnityEngine;

    public class openSeqCamera : MonoBehaviour
    {
        private Animation anim;

        private void Start()
        {
            anim = GetComponent<Animation>();
        }

        public void PlayOpenSequenceMoveToCastle(bool _isAtk, openingSequenceGM.GMCallBacks _callBack)
        {
            if (_isAtk)
                anim.Play("OpeningLerpToPlayerOne");
            else
                anim.Play("OpeningLerpToPlayerTwo");

            if (_callBack != null)
                _callBack.Invoke();
        }
    }

}
