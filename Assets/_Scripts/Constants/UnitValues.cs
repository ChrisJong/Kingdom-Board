namespace Constants {

    using Enum;

    public static class UnitValues {

        ///////////////////
        //// RIGIDBODY ////
        ///////////////////
        public const float MASS = 100.0f;
        public const float DRAG = 0.0f;
        public const float ANGULARDRAG = 0.05f;

        public static class ArcherValues {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 2;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

            public const float MOVESPEED = 5.0f;
            public const float MOVERADIUS = 10.0f;
            public const MovementType MOVETYPE = MovementType.GROUND;

            public const float MINDAMAGE = 10.0f;
            public const float MAXDAMAGE = 20.0f;
            public const float ATTACKRADIUS = 10.0f;
            public const AttackType ATTACKTYPE = AttackType.PROJECTFILE;

            public const float RESISTANCEPERCENTAGE = 50.0f;
            public const float WEAKNESSPERCENTAGE = 50.0f;
            public const AttackType RESISTANCE = AttackType.PHYSICAL;
            public const AttackType WEAKNESS = AttackType.MAGIC;
        }

        public static class MageValues {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0F;
            public const int SPAWNCOUNT = 3;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

            public const float MOVESPEED = 5.0f;
            public const float MOVERADIUS = 10.0f;
            public const MovementType MOVETYPE = MovementType.GROUND;

            public const float MINDAMAGE = 10.0f;
            public const float MAXDAMAGE = 20.0f;
            public const float ATTACKRADIUS = 10.0f;
            public const float SPLASHRADIUS = 5.0f;
            public const AttackType ATTACKTYPE = AttackType.MAGIC;

            public const float RESISTANCEPERCENTAGE = 50.0f;
            public const float WEAKNESSPERCENTAGE = 50.0f;
            public const AttackType RESISTNACE = AttackType.PROJECTFILE;
            public const AttackType WEAKNESS = AttackType.PHYSICAL;
        }

        public static class WarriorValues {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 2;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

            public const float MOVESPEED = 5.0f;
            public const float MOVERADIUS = 10.0f;
            public const MovementType MOVETYPE = MovementType.GROUND;

            public const float MINDAMAGE = 10.0f;
            public const float MAXDAMAGE = 20.0f;
            public const float ATTACKRADIUS = 1.5f;
            public const AttackType ATTACKTYPE = AttackType.PHYSICAL;

            public const float RESISTANCEPERCENTAGE = 50.0f;
            public const float WEAKNESSPERCENTAGE = 50.0f;
            public const AttackType RESISTANCE = AttackType.MAGIC;
            public const AttackType WEAKNESS = AttackType.PROJECTFILE;
        }
    }
}