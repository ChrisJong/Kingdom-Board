namespace Movement {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEditor;
    using UnityEngine.AI;

    public class MoveTo : MonoBehaviour {

        public Transform goal;
        public NavMeshAgent agent;

        private void Start() {
            //this.agent = GetComponent<NavMeshAgent>();
        }

        private void Update() {
            if(Input.GetMouseButtonDown(0)) {
                RaycastHit HitInfo;

                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out HitInfo, 1000.0f)) {
                    Debug.Log(HitInfo.point.ToString());
                    this.agent.destination = HitInfo.point;
                }
            }
        }
    }
}