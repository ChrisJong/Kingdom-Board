namespace Constants {

    using Enum;

    public static class UnitValues {

        public const float MOVEMENTCOST = 0.25F;
        public const float MELEERANGE = 1.5f;

        public static class Archer {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 1;
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
            public const AttackType RESISTANCETYPE = AttackType.PHYSICAL;
            public const AttackType WEAKNESSTYPE = AttackType.MAGIC;
        }

        public static class Crossbow {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 1;
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
            public const AttackType RESISTANCETYPE = AttackType.PHYSICAL;
            public const AttackType WEAKNESSTYPE = AttackType.MAGIC;
        }

        public static class Longbow {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 1;
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
            public const AttackType RESISTANCETYPE = AttackType.PHYSICAL;
            public const AttackType WEAKNESSTYPE = AttackType.MAGIC;
        }

        public static class Mage {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0F;
            public const int SPAWNCOUNT = 1;
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
            public const AttackType RESISTANCETYPE = AttackType.PROJECTFILE;
            public const AttackType WEAKNESSTYPE = AttackType.PHYSICAL;
        }

        public static class Cleric {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0F;
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

            public const float MOVESPEED = 5.0f;
            public const float MOVERADIUS = 10.0f;
            public const MovementType MOVETYPE = MovementType.GROUND;

            public const float MINDAMAGE = 10.0f;
            public const float MAXDAMAGE = 20.0f;
            public const float HEALINGAMOUNT = 10.0f;
            public const float ATTACKRADIUS = MELEERANGE;
            public const float HEALINGRADIUS = 5.0f;
            public const AttackType ATTACKTYPE = AttackType.PHYSICAL;

            public const float RESISTANCEPERCENTAGE = 50.0f;
            public const float WEAKNESSPERCENTAGE = 50.0f;
            public const AttackType RESISTANCETYPE = AttackType.PROJECTFILE;
            public const AttackType WEAKNESSTYPE = AttackType.PHYSICAL;
        }

        public static class Wizard {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0F;
            public const int SPAWNCOUNT = 1;
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
            public const AttackType RESISTANCETYPE = AttackType.PROJECTFILE;
            public const AttackType WEAKNESSTYPE = AttackType.PHYSICAL;
        }

        public static class Warrior {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

            public const float MOVESPEED = 5.0f;
            public const float MOVERADIUS = 10.0f;
            public const MovementType MOVETYPE = MovementType.GROUND;

            public const float MINDAMAGE = 10.0f;
            public const float MAXDAMAGE = 20.0f;
            public const float ATTACKRADIUS = MELEERANGE;
            public const AttackType ATTACKTYPE = AttackType.PHYSICAL;

            public const float RESISTANCEPERCENTAGE = 50.0f;
            public const float WEAKNESSPERCENTAGE = 50.0f;
            public const AttackType RESISTANCETYPE = AttackType.MAGIC;
            public const AttackType WEAKNESSTYPE = AttackType.PROJECTFILE;
        }

        public static class Knight {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

            public const float MOVESPEED = 5.0f;
            public const float MOVERADIUS = 10.0f;
            public const MovementType MOVETYPE = MovementType.GROUND;

            public const float MINDAMAGE = 10.0f;
            public const float MAXDAMAGE = 20.0f;
            public const float ATTACKRADIUS = MELEERANGE;
            public const AttackType ATTACKTYPE = AttackType.PHYSICAL;

            public const float RESISTANCEPERCENTAGE = 50.0f;
            public const float WEAKNESSPERCENTAGE = 50.0f;
            public const AttackType RESISTANCETYPE = AttackType.MAGIC;
            public const AttackType WEAKNESSTYPE = AttackType.PROJECTFILE;
        }

        public static class Guardian {
            public const float HEALTH = 100.0f;
            public const float ENERGY = 100.0f;
            public const int SPAWNCOUNT = 1;
            public const int SPAWNCOST = 500;
            public const int UNITCAPCOST = 2;

            public const float MOVESPEED = 5.0f;
            public const float MOVERADIUS = 10.0f;
            public const MovementType MOVETYPE = MovementType.GROUND;

            public const float MINDAMAGE = 10.0f;
            public const float MAXDAMAGE = 20.0f;
            public const float ATTACKRADIUS = MELEERANGE;
            public const AttackType ATTACKTYPE = AttackType.PHYSICAL;

            public const float RESISTANCEPERCENTAGE = 50.0f;
            public const float WEAKNESSPERCENTAGE = 50.0f;
            public const AttackType RESISTANCETYPE = AttackType.MAGIC;
            public const AttackType WEAKNESSTYPE = AttackType.PROJECTFILE;
        }
    }
}