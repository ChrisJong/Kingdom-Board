namespace GameInfo {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class GameInfo {

        ///////////////
        //// GAME /////
        ///////////////
        public enum GameState {
            NONE = 0,
            START,
            MIDDLE,
            END
        };

        ////////////////
        //// Player ////
        ////////////////
        public int castleHP = 100;
        public int unitCap = 250;
        public int goldCap = 999999;

        //////////////
        //// UNIT ////
        //////////////
        public enum Units {
            NONE = 0,
            ARCHER,
            MAGICIAN,
            WARRIOR
        }

        public enum UnitState {
            NONE = 0,
            MOVING,
            ATTACKING,
            END
        };
    }
}