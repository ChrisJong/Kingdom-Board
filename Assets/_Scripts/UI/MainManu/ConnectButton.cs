namespace KingdomBoard.UI {

	using System.Collections;
	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	using Manager;

	public class ConnectButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {

		#region UNITY
		public void OnPointerClick(PointerEventData eventData) { }

		public void OnPointerDown(PointerEventData eventData) { }

		public void OnPointerUp(PointerEventData eventData) {
			if(!NetworkManager.instance.IsConnectedToServer)
				NetworkManager.instance.ConnectToServer();
		}
		#endregion
	}
}