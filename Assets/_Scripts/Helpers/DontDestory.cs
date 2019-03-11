namespace KingdomBoard.Helpers {

	using UnityEngine;

	public class DontDestory : MonoBehaviour {
		#region UNITY
		public void Awake() {
			DontDestroyOnLoad(this);
		}

		public void OnEnable() {
			DontDestroyOnLoad(this);
		}
		#endregion
	}
}