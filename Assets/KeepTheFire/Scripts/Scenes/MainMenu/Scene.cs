using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.MainMenu {
	public class Scene : MonoBehaviour {

		private new Camera camera = null;

		private Dugan.UI.Button btnPlay = null;

		private void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			btnPlay = transform.Find("Content/BtnPlay").gameObject.AddComponent<Dugan.UI.Button>();
			btnPlay.tintOnClick = true;
			btnPlay.unSelectOnPointerUp = true;
			btnPlay.OnPointerUp += OnClickBtnPlay;

			Transition.instance.SetDirection(1, true);
			Transition.instance.SetDirection(-1);

			Dugan.Screen.OnResize += OnResize;

			OnResize();
		}

		private void OnClickBtnPlay(Dugan.Input.PointerTarget target, string args) {
			Transition.instance.OnOpened += OnTransitionOpened;
			Transition.instance.SetDirection(1);
		}

		private void OnTransitionOpened() {
			Transition.instance.OnOpened -= OnTransitionOpened;
			UnityEngine.SceneManagement.SceneManager.LoadScene("KeepTheFire.Scenes.Game");
		}

		private void OnResize() {
			camera.orthographicSize = Dugan.Screen.screenSizeInUnits.y;
			(transform as RectTransform).sizeDelta = Dugan.Screen.layoutSize;
		}

		protected virtual void OnDisable() {
			Dugan.Screen.OnResize -= OnResize;
		}

	}
}
