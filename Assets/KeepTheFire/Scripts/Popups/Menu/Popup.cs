using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Popups.Menu {
	public class Popup : KeepTheFire.Popup {

		private new UnityEngine.Camera camera = null;

		private UnityEngine.UI.Image imgBackground = null;

		private Transform root = null;

		private Dugan.UI.Button btnResume = null;
		private Dugan.UI.Button btnCredits = null;
		private Dugan.UI.Button btnQuit = null;

		public static bool bOpened { get; internal set; }

		protected override void Awake() {
			
			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();

			imgBackground = transform.Find("Blocker").GetComponent<UnityEngine.UI.Image>();

			root = transform.Find("Root") as RectTransform;

			btnResume = root.Find("BtnResume").gameObject.AddComponent<Dugan.UI.Button>();
			btnResume.tintOnClick = true;
			btnResume.OnPointerUp += OnClickBtnResume;

			btnCredits = root.Find("BtnCredits").gameObject.AddComponent<Dugan.UI.Button>();
			btnCredits.tintOnClick = true;
			btnCredits.OnPointerUp += OnClickBtnCredits;

			btnQuit = root.Find("BtnQuit").gameObject.AddComponent<Dugan.UI.Button>();
			btnQuit.tintOnClick = true;
			btnQuit.OnPointerUp += OnClickBtnQuit;

			base.Awake();
		}

		public override void PostAwake() {
			base.PostAwake();
			bOpened = true;
			UnityEngine.Time.timeScale = 0.0f;
		}

		private void OnClickBtnResume(Dugan.Input.PointerTarget pointerTarget, string args) {
			SetDirection(-1);
		}

		private void OnClickBtnCredits(Dugan.Input.PointerTarget pointerTarget, string args) {
			KeepTheFire.Popup p = Dugan.PopupManager.Load<Popups.Credits.Popup>();
			p.PostAwake();
		}

		private void OnClickBtnQuit(Dugan.Input.PointerTarget pointerTarget, string args) {
			Application.Quit();
		}

		protected override void OnAnimationUpdate(float a) {
			a = Dugan.Mathf.Easing.EaseInOutCirc(a);
			root.localScale = Vector3.one * a;
			imgBackground.color = new Color(0.0f, 0.0f, 0.0f, a) * 0.5f;
		}

		protected override void OnResize() {
			camera.orthographicSize = Dugan.Screen.screenSizeInUnits.y;
			(transform as RectTransform).sizeDelta = Dugan.Screen.layoutSize;
		}

		protected override void OnDisable() {
			bOpened = false;
			UnityEngine.Time.timeScale = 1.0f;
		}
	}
}