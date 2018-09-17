namespace Constants {

    using UnityEngine;

    public static class UnitValues {

        public const float MOVEMENTCOST = 0.25F;
        public const float MELEERANGE = 1.5f;

        public const float ATTACKHEIGHTTHRESHOLD = 0.5f; // the height thresehold.
        public const float ABOVEHEIGHTMULTIPLIER = 1.5f; // if the attacker is abobe the one being attacked.
        public const float BELOWHEIGHTMULTIPLIER = 0.5f; // If the attacker is below the one being attacked.

        /// <summary>
        /// Hit Positions In Degrees (0-360)
        /// </summary>
        public const float FRONT_L = 45.0f;
        public const float FRONT_R = 315.0f;
        public const float BEHIND_L = 135.0f;
        public const float BEHIND_R = 225.0f;

        #region ARCHER TREE
        public static class Archer {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

        }

        public static class Crossbow {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }

        public static class Longbow {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }
        #endregion

        #region MAGE TREE
        public static class Mage {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }

        public static class Cleric {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }

        public static class Wizard {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }
        #endregion

        #region WARRIOR TREE
        public static class Warrior {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }

        public static class Knight {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }

        public static class Guardian {
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;
        }
        #endregion
    }
}