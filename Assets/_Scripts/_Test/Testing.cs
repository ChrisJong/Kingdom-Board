using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Testing : MonoBehaviour {

    [SerializeField] private bool _toggle = false;

    [SerializeField] private GameObject _incoming;

    private void Update() {
        if(this._toggle) {
            this.MoveToMouse();
        } else {
            this.CalculateDirection();
        }
    }

    private void CalculateDirection() {
        //Vector3 headingRelative = this._incoming.transform.InverseTransformDirection(this.transform.forward);
        //Vector3 headingReverse = this.transform.InverseTransformDirection(this._incoming.transform.forward);
        //Debug.Log("Heading Direction: " + headingRelative.ToString());
        //Debug.Log("Heading Rewverse: " + headingReverse.ToString());

        Vector3 newIncoming = new Vector3(this._incoming.transform.position.x, 0.0f, this._incoming.transform.position.z);
        Vector3 newMain = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z);

        Vector3 direction = newIncoming - newMain;
        Vector3 directionNormal = direction.normalized;

        float SignedAngle = Vector3.SignedAngle(directionNormal, this.transform.forward, Vector3.up);
        float fullAngle = (SignedAngle + 360.0f) % 360;
        Debug.Log("Signed Angle: " + fullAngle.ToString());

        if(fullAngle <= 45.0f || fullAngle >= 315.0f) {
            Debug.Log("FRONT");
        } else if(fullAngle >= 135.0f && fullAngle <= 225.0f) {
            Debug.Log("BEHIND");
        } else if(fullAngle > 45.0f && fullAngle < 135.0f) {
            Debug.Log("LEFT");
        } else if(fullAngle > 135.0f && fullAngle < 315.0f) {
            Debug.Log("RIGHT");
        } else {
            Debug.Log("Angle In Degree" + fullAngle.ToString());
        }
    }

    private void MoveToMouse() { 
        Vector3 mouseToPoint = this.MousePoint();

        this.transform.position = new Vector3(mouseToPoint.x, 0.0f, mouseToPoint.z);
        this.transform.LookAt(this._incoming.transform);
    } 

    private Vector3 MousePoint() {
        Vector3 mousePos = Vector3.zero;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;

        Physics.Raycast(mouseRay, out hitinfo, 100.0f, ~0);

        mousePos = hitinfo.point;

        return mousePos;
    }
}
