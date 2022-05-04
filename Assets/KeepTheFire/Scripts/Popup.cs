using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire {
	public class Popup : MonoBehaviour {

		public delegate void Event(Popup popup);
		public Event OnClosed;
		public Event OnCloseBegin;

		protected Dugan.TimeAnimation timeAnimation = null;

		protected Dugan.UI.Button[] buttons = null;

		protected virtual void Awake() {
			timeAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			timeAnimation.SetLengthInSeconds(0.25f);
			timeAnimation.bUseUnscaledDeltaTime = true;
		}

		public virtual void PostAwake() {
			Dugan.Screen.OnResize += OnResize;
			OnResize();

			buttons = GetComponentsInChildren<Dugan.UI.Button>();
			SetButtonsInteractive(false);//Give the buttons an initial interactive state

			timeAnimation.OnAnimationUpdate += OnAnimationUpdate;
			timeAnimation.SetDirection(-1, true);
			timeAnimation.OnAnimationComplete += OnAnimationCompleteInt;
			timeAnimation.SetDirection(1);
		}

		public void SetDirection(float direction, bool bInstant = false) {
			if (direction < 0 && OnCloseBegin != null)
				OnCloseBegin(this);

			timeAnimation.SetDirection(direction, bInstant);
			SetButtonsInteractive(false);

			OnSetDirection(direction, bInstant);
		}

		protected virtual void OnSetDirection(float direction, bool bInstant) {}

		public void SetButtonsInteractive(bool bInteractive) {
			//
			if (buttons == null) {
				Debug.Log("You probably forgot to run post awake on popup " + gameObject.name + "!");
				return;
			}

			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].SetInteractive(bInteractive, -1);
			}
		}

		protected virtual void OnAnimationUpdate(float a) { }

		private void OnAnimationCompleteInt() {
			if (timeAnimation.GetDirection() == -1) {
				if (OnClosed != null)
					OnClosed(this);
				Dugan.PopupManager.Unload(gameObject);
			} else {
				SetButtonsInteractive(true);
			}

			//Force sync transforms? Probably better to move this to the popups which set time scale
			Physics.SyncTransforms();

			OnAnimationComplete();
		}
		protected virtual void OnAnimationComplete() {}

		public virtual void OnSystemBackReleased() {}
		protected virtual void OnResize() {}

		protected virtual void OnDisable() {
			Dugan.Screen.OnResize -= OnResize;
			OnUnload();
		}

		protected virtual void OnUnload() {

		}

	}
}