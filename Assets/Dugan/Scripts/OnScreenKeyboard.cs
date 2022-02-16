using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Dugan {
	public class OnScreenKeyboard : MonoBehaviour {

		public static int keyboardPopupLayer = 5;

		public TouchScreenKeyboard keyboard = null;
		public KeyboardSettings settings;
		public int length = 10;

		public string text {get {return keyboard.text;} set {keyboard.text = value;} }

		public delegate void Closed();
		public Closed OnClosed;

		private void Awake() {
			Dugan.UI.Button btnBlocker = transform.Find("Blocker").gameObject.AddComponent<Dugan.UI.Button>();
			btnBlocker.unSelectOnPointerUp = true;
			btnBlocker.OnPointerUp += OnClickBlocker;
		}

		public void Init(KeyboardSettings settings, int length) {
			this.settings = settings;
			this.length = length;
			TouchScreenKeyboard.hideInput = !settings.bShowTextbox;
			keyboard = TouchScreenKeyboard.Open("", settings.keyboardType, settings.bAutoCorrect, settings.bMultiline, settings.bSecure, settings.bAlert, "", length);
		}

		private void OnClickBlocker(Dugan.Input.PointerTarget target, string args) {
			if (settings.bCloseOnTouch)//If this is false, the only way to close the keyboard is by hitting return. (should not work on multi-line?)
				Close();
		}

		public void Close() {
			if (keyboard != null)
				keyboard.active = false;
			PopupManager.Unload(gameObject);
			if (OnClosed != null)
				OnClosed();
		}

		public static OnScreenKeyboard Open(KeyboardSettings settings, int length) {
			if (!TouchScreenKeyboard.isSupported)
				return null;

			GameObject go = new GameObject("Dugan.Popup.OnScreenKeyboard");
			Debug.Log("Opening oskb popup.");

			GameObject camGO = new GameObject("Camera");
			Camera cam = camGO.AddComponent<Camera>();
			cam.depth = Dugan.PopupManager.GetCurrentDepth() + 100;
			cam.orthographic = true;
			cam.orthographicSize = Dugan.Screen.GetReferenceOrthographicSize();
			cam.clearFlags = CameraClearFlags.Nothing;
			cam.cullingMask = 1 << keyboardPopupLayer;

			camGO.transform.SetParent(go.transform);
			camGO.transform.localPosition = Vector3.zero;
			camGO.transform.localScale = Vector3.one;
			camGO.transform.localRotation = Quaternion.identity;

			GameObject blockerGO = new GameObject("Blocker");
			blockerGO.transform.SetParent(go.transform);
			blockerGO.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);
			blockerGO.transform.localScale = Vector3.one;
			blockerGO.transform.localRotation = Quaternion.identity;

			BoxCollider bc = blockerGO.AddComponent<BoxCollider>();
			bc.size = new Vector3(Screen.referenceSize.x*2.0f, Screen.referenceSize.y*2.0f, 1.0f);

			Dugan.Util.SetLayerOnGameObjectTree(keyboardPopupLayer, go);

			PopupManager.Add(go);

			OnScreenKeyboard kb = go.AddComponent<OnScreenKeyboard>();
			kb.Init(settings, length);

			return kb;
		}

		public struct KeyboardSettings {
			public TouchScreenKeyboardType keyboardType;
			public bool bAutoCorrect;
			public bool bCloseOnTouch;
			public bool bMultiline;
			public bool bSecure;
			public bool bShowTextbox;
			public bool bAlert;
		}
	}
}
