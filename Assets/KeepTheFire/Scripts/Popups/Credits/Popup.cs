using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Popups.Credits {
	public class Popup : KeepTheFire.Popup {

		private CanvasGroup canvasGroup = null;

		private new UnityEngine.Camera camera = null;

		private Dugan.UI.Button btnClose = null;

		protected override void Awake() {

			canvasGroup = GetComponent<CanvasGroup>();
			
			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();

			btnClose = transform.Find("BtnClose").gameObject.AddComponent<Dugan.UI.Button>();
			btnClose.tintOnClick = true;
			btnClose.OnPointerUp += OnClickBtnClose;

			base.Awake();
		}

		private void OnClickBtnClose(Dugan.Input.PointerTarget pointerTarget, string args) {
			SetDirection(-1);
		}

		protected override void OnAnimationUpdate(float a) {
			a = Dugan.Mathf.Easing.EaseInOutCirc(a);
			canvasGroup.alpha = a;
		}

		protected override void OnResize() {
			camera.orthographicSize = Dugan.Screen.screenSizeInUnits.y;
			(transform as RectTransform).sizeDelta = Dugan.Screen.layoutSize;
		}
	}
}