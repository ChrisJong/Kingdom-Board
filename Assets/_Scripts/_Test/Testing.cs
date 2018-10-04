using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

partial class CygnetMath {
    public static float DistancePointToRectangle(Vector2 point, Rect rect) {
        //  Calculate a distance between a point and a rectangle.
        //  The area around/in the rectangle is defined in terms of
        //  several regions:
        //
        //  O--x
        //  |
        //  y
        //
        //
        //        I   |    II    |  III
        //      ======+==========+======   --yMin
        //       VIII |  IX (in) |  IV
        //      ======+==========+======   --yMax
        //       VII  |    VI    |   V
        //
        //
        //  Note that the +y direction is down because of Unity's GUI coordinates.

        if(point.x < rect.xMin) { // Region I, VIII, or VII
            if(point.y < rect.yMin) { // I
                Vector2 diff = point - new Vector2(rect.xMin, rect.yMin);
                return diff.magnitude;
            } else if(point.y > rect.yMax) { // VII
                Vector2 diff = point - new Vector2(rect.xMin, rect.yMax);
                return diff.magnitude;
            } else { // VIII
                return rect.xMin - point.x;
            }
        } else if(point.x > rect.xMax) { // Region III, IV, or V
            if(point.y < rect.yMin) { // III
                Vector2 diff = point - new Vector2(rect.xMax, rect.yMin);
                return diff.magnitude;
            } else if(point.y > rect.yMax) { // V
                Vector2 diff = point - new Vector2(rect.xMax, rect.yMax);
                return diff.magnitude;
            } else { // IV
                return point.x - rect.xMax;
            }
        } else { // Region II, IX, or VI
            if(point.y < rect.yMin) { // II
                return rect.yMin - point.y;
            } else if(point.y > rect.yMax) { // VI
                return point.y - rect.yMax;
            } else { // IX
                return 0f;
            }
        }
    }
}

public class Testing : MonoBehaviour {

    [SerializeField] private float _radius = 0.1f;

    [SerializeField] private bool _toggle = false;

    [SerializeField] private Vector3 _mousePoiunt = Vector3.zero;
    [SerializeField] private BoxCollider _boxCollider = null;

    [SerializeField] private Bounds _bounds;
    [SerializeField] private Vector3 _bounds_min = Vector3.zero;
    [SerializeField] private Vector3 _bounds_max = Vector3.zero;
    [SerializeField] private Vector3 _bounds_size = Vector3.zero;
    [SerializeField] private Vector3 _closestPoiht = Vector3.zero;

    private void Awake() {
        this._boxCollider = this.GetComponent<BoxCollider>() as BoxCollider;

        this._bounds = this._boxCollider.bounds;
        this._bounds_min = this._boxCollider.bounds.min;
        this._bounds_max = this._boxCollider.bounds.max;
        this._bounds_size = this._boxCollider.bounds.size;

        this._boxCollider.enabled = false;
    }

    private void Update() {
        if(Input.GetMouseButtonUp(0)) {
            this.MoveToMouse();
            if(!this._toggle)
            this._toggle = true;
        }

        if(this._toggle) {
            this._closestPoiht = this.ClosestPoint(this._bounds, this._mousePoiunt);
            if(this.PointInAABB(this._mousePoiunt, this._bounds)) {
                Debug.Log("Point Outside of Bounds");
                this._closestPoiht = this.ClosestPoint(this._bounds, this._mousePoiunt);
            } else
                Debug.Log("Point Inside of Bounds");

            this._toggle = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(this._bounds_min, this._radius);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(this._bounds_max, this._radius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(this._bounds_size, this._radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this._mousePoiunt, this._radius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this._closestPoiht, this._radius);
    }

    private bool PointInAABB(Vector3 point, Bounds bound) {
        Vector3 min = bound.min;
        Vector3 max = bound.max;

        if(point.x < min.x || point.y < min.y || point.z < min.z) {
            Debug.Log("Less Than Min Bounds");
            return false;
        }

        if(point.x > max.x || point.y > max.y || point.z > max.z) {
            Debug.Log("Greater than Max Bounds");
            return false;
        }

        return true;
    }

    private Vector3 ClosestPoint(Bounds bound, Vector3 point) {
        Vector3 result = point;
        Vector3 min = bound.min;
        Vector3 max = bound.max;

        result.x = (result.x < min.x) ? min.x : result.x;
        result.y = (result.y < min.x) ? min.y : result.y;
        result.z = (result.z < min.x) ? min.z : result.z;

        result.x = (result.x > max.x) ? max.x : result.x;
        result.y = (result.y > max.x) ? max.y : result.y;
        result.z = (result.z > max.x) ? max.z : result.z;

        return result;
    }

    private void MoveToMouse() { 
        this._mousePoiunt = this.MousePoint();
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
