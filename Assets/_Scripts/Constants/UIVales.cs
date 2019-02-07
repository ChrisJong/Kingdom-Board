namespace Constants {

    public static class UIValues {

        public const string BUTTON_SUFFIX = "_BTN";
        public const string UI_SUFFIX = "_UI";
        public const string TEXT_SUFFIX = "_TXT";
        public const string SPRITE_SUFFIX = "_SP";
        public const string IMAGE_SUFFIX = "_IMG";

        public const string MAIN_GROUP = UI_SUFFIX + "_MAIN";
        public const string HOVER_GROUP = UI_SUFFIX + "_HOVER";
        public const string PERSISTANT_GROUP = UI_SUFFIX + "_PERSISTANT";

        public static class Structure {

            public const string STRUCTURE_SUFFIX = "Structure";
            public const string STRUCTURE_UI_MAIN = STRUCTURE_SUFFIX + MAIN_GROUP;
            public const string STRUCTURE_UI_HOVER = STRUCTURE_SUFFIX + HOVER_GROUP;

        }

        public static class Unit {

            public const string UNIT_SUFFIX = "Unit";
            public const string UNIT_UI_MAIN = UNIT_SUFFIX + MAIN_GROUP;
            public const string UNIT_UI_HOVER = UNIT_SUFFIX + HOVER_GROUP;

            // World Space UI Values.
            public const float FULL_HEALTH = 0.9f;
            public const float FULL_STAMINA = 0.9f;
            public const float POSITION_Y = 0.25f;
        }

        public static class Player {

        }
    }
}