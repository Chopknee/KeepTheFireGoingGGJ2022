using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
	public class Menu : MonoBehaviour {
		
		private Dugan.TimeAnimation timeAnimation = null;

		private Dugan.UI.Button btnCredits = null;
		private Dugan.UI.Button btnQuit = null;

		private new UnityEngine.Camera camera = null;

		private RectTransform canvas = null;

		private Dugan.UI.Button btnCreditsWindow = null;

		private void Awake() {

			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();

			canvas = transform.Find("Canvas") as RectTransform;

			btnCredits = canvas.Find("BtnCredits").gameObject.AddComponent<Dugan.UI.Button>();
			btnCredits.OnClicked += OnClickCreditsWindow;
			btnCredits.gameObject.SetActive(false);

			btnQuit = canvas.Find("BtnQuit").gameObject.AddComponent<Dugan.UI.Button>();
			btnQuit.OnClicked += OnClickCreditsWindow;
			btnQuit.gameObject.SetActive(false);

			btnCreditsWindow = canvas.Find("Credits").gameObject.AddComponent<Dugan.UI.Button>();
			btnCreditsWindow.OnClicked += OnClickCreditsWindow;
			btnCreditsWindow.gameObject.SetActive(false);

			timeAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			timeAnimation.SetLengthInSeconds(0.25f);
			timeAnimation.OnAnimationComplete += OnAnimationComplete;
			timeAnimation.OnAnimationUpdate += OnAnimationUpdate;
			timeAnimation.SetDirection(-1, true);

			Dugan.Screen.OnResize += OnResize;
		}

		private void OnClickBtnCredits(Dugan.Input.PointerTarget pointerTarget, string ars) {
			btnCreditsWindow.gameObject.SetActive(true);
		}

		private void OnClickCreditsWindow(Dugan.Input.PointerTarget pointerTarget, string ars) {
			btnCreditsWindow.gameObject.SetActive(false);
		}

		private void OnClickBtnQuit(Dugan.Input.PointerTarget pointerTarget, string ars) {

		}

		private void OnAnimationUpdate(float a) {
			a = Dugan.Mathf.Easing.EaseInOutExpo(a);
			transform.localScale = Vector3.one * a;
		}

		private void OnAnimationComplete() {

		}

		public void SetDirection(int direction, bool bInstant = false) {
			timeAnimation.SetDirection(direction, bInstant);
		}

		public float GetDirection() {
			return timeAnimation.GetDirection();
		}
		private void OnResize() {
			camera.orthographicSize = Dugan.Screen.screenSizeInUnits.y;
			canvas.sizeDelta = Dugan.Screen.layoutSize;
		}

		private void OnDisable() {
			Dugan.Screen.OnResize -= OnResize;
		}
	}
}