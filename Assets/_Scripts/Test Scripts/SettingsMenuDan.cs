namespace Testing
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SettingsMenuDan : MonoBehaviour
    {
        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void MoveToCamera()
        {
            anim.Play("MoveToCamera");
        }

        public void ReturnToBoard()
        {
            anim.Play("ReturnToBoard");
        }
    }

}
