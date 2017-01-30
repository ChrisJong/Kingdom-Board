namespace Player {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct PlayerInfo {
        public enum PlayerState {
            ATTACKING = 0,
            DEFENDING = 1,
            WIN,
            LOSE,
            DRAW
        }

        public int id;
        public bool turnEnded;
        public PlayerState turnState;

        public float goldResource;
        public float resourceLImit;

        public float masteryPoints;
        public float masteryLimit;

        public int unitCount;
        public int unitLimit;
    }
}