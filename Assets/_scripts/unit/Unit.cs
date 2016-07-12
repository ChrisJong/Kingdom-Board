namespace Unit
{

    using UnityEngine;
    using System.Collections;

    public class Unit : MonoBehaviour, IUnit
    {
        private Transform _transform;
        private Vector3 _curPosition;

        private UnitData _unitData;

        private bool _isSelectable = true;
        private bool _hasMoved = false;
        private bool _hasAttacked = false;

        public void Awake()
        {
            this._transform = this.GetComponent<Transform>() as Transform;
            this._curPosition = this._transform.position;
        }

        public void MoveUnit(Vector3 pos)
        {
            this._transform.position = pos;
            this._curPosition = pos;
        }

        public void ProjectMoveDistance()
        {

        }

        public bool UnitSelectable()
        {
            if (_isSelectable)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Vector3 CurPosition
        {
            get { return this._curPosition; }
        }
    }
}