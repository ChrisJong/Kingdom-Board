namespace Utility {

    public static class Utils {
        #region VARIABLE
        private static uint _nextPoolID = 0;
        #endregion

        #region UTILS
        public static uint nextPoolID {
            get { return _nextPoolID++; }
        }
        #endregion
    }
}