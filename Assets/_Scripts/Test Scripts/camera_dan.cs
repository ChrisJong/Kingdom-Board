namespace Testing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class camera_dan : MonoBehaviour
    {
        public float moveSpeed = 5f;

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Input.GetKey(KeyCode.W))
                MoveCamera(Vector3.forward);

            if (Input.GetKey(KeyCode.S))
                MoveCamera(Vector3.back);

            if (Input.GetKey(KeyCode.A))
                MoveCamera(Vector3.left);

            if (Input.GetKey(KeyCode.D))
                MoveCamera(Vector3.right);
        }

        private void MoveCamera(Vector3 _direction)
        {
            transform.position += _direction * moveSpeed * Time.deltaTime;
        }
    }

}
