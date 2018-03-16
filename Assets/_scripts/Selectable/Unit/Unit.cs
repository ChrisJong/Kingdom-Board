namespace Selectable.Unit {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class Unit : Selectable {
        #region VARIABLE
        public bool hasEnded = false;
        public bool canMove = true;
        public bool canAttack = true;

        //////////////////
        /// ATTRIBUTES ///
        //////////////////
        public float currentMoveRange;
        public float maxMoveRange;
        public float attackRange;
        #endregion

        #region CLASS_METHOD
        public void MoveTo(Vector3 pos) {

        } 
        #endregion
    }
}