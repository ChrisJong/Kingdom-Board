namespace KingdomBoard.MainMenu {

	using UnityEngine;

	using Manager;

	public class MenuCamera : MonoBehaviour {

		#region VARIABLE
		[SerializeField] private Animation _cameraAnimation;
		#endregion

		#region CLASS

		public void PlayAnimOpeningSequence(){
			this._cameraAnimation.Play("OpeningLerpToTop");
		}


		public void PlayAnimMoveToCastle(bool attacking, MenuManager.MainMenuCallBack callback) {
			if(attacking)
				this._cameraAnimation.Play("OpeningLerpToPlayerOne");
			else
				this._cameraAnimation.Play("OpeningLerpToPlayerTwo");

			if(callback != null)
				callback.Invoke();
		}
		#endregion

	}
}