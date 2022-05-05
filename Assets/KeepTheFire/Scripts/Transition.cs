using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire {
	public class Transition : MonoBehaviour {

		public delegate void Event();
		public Event OnClosed;
		public Event OnOpened;

		private static Transition _instance = null;
		public static Transition instance {
			get {
				if (_instance == null) {
					GameObject prefab = Resources.Load<GameObject>("KeepTheFire/Transition");
					_instance = Instantiate(prefab, new Vector3(0.0f, 0.0f, 10000.0f), Quaternion.identity).AddComponent<Transition>();
					DontDestroyOnLoad(_instance.gameObject);
				}

				return _instance;
			}
		}

		public bool bIsAnimating {
			get { return timeAnimation != null && timeAnimation.IsPlaying(); }
		}

		private new Camera camera = null;

		private UnityEngine.UI.Image fade = null;

		private Dugan.TimeAnimation timeAnimation = null;

		private new Collider collider = null;

		private void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			fade = transform.Find("Fade").GetComponent<UnityEngine.UI.Image>();
			collider = fade.GetComponent<Collider>();

			timeAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			timeAnimation.bUseUnscaledDeltaTime = true;
			timeAnimation.OnAnimationUpdate += OnAnimationUpdate;
			timeAnimation.SetDirection(-1, true);
			timeAnimation.OnAnimationComplete += OnAnimationComplete;
			timeAnimation.SetLengthInSeconds(1.0f);

			Dugan.Screen.OnResize += OnResize;

			OnResize();
		}

		public void SetDirection(float direction, bool bInstant = false) {
			timeAnimation.SetDirection(direction, bInstant);
			if (direction > 0)
				collider.enabled = true;
		}

		private void OnAnimationUpdate(float a) { 
			a = Dugan.Mathf.Easing.EaseInOutCirc(a);
			fade.color = new Color(0.0f, 0.0f, 0.0f, a);
		}

		private void OnAnimationComplete() {
			if (timeAnimation.GetDirection() == -1) {
				collider.enabled = false;
				if (OnClosed != null)
					OnClosed();
			} else if (timeAnimation.GetDirection() == 1) {
				if (OnOpened != null)
					OnOpened();
			}

			//Force sync transforms? Probably better to move this to the popups which set time scale
			Physics.SyncTransforms();
		}

		private void OnResize() {
			camera.orthographicSize = Dugan.Screen.screenSizeInUnits.y;

			(transform as RectTransform).sizeDelta = Dugan.Screen.layoutSize;
		}
	}
}