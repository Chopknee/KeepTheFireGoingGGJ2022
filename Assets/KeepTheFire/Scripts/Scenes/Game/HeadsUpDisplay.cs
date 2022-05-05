using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game {
	public class HeadsUpDisplay : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private RectTransform canvas = null;

		private UnityEngine.UI.RawImage imgVingette = null;

		private Dugan.UI.Button btnMenu = null;

		//private CanvasGroup introCG = null;
		private CanvasGroup gameOverCG = null;

		private TMPro.TextMeshProUGUI txtTime = null;
		private int[] times = { 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7};

		private string strFormat = "D2";
		private string strColon = ":";
		private string strSpace = " ";
		private string strAM = " AM";
		private string strPM = " PM";

		//private Dugan.TimeAnimation introAnimation = null;
		private Dugan.TimeAnimation endAnimation = null;

		private void Awake() {
			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();

			canvas = transform.Find("Canvas") as RectTransform;

			imgVingette = canvas.Find("ImgVingette").GetComponent<UnityEngine.UI.RawImage>();

			btnMenu = canvas.Find("BtnMenu").gameObject.AddComponent<Dugan.UI.Button>();
			btnMenu.tintOnClick = true;
			btnMenu.OnPointerUp += OnClickBtnMenu;

			//introCG = canvas.Find("Intro").GetComponent<CanvasGroup>();
			gameOverCG = canvas.Find("GameOver").GetComponent<CanvasGroup>();

			txtTime = canvas.Find("Watch/TxtTime").GetComponent<TMPro.TextMeshProUGUI>();

			//This plays the initial intro animation
			// introAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			// introAnimation.SetLengthInSeconds(5.0f);
			// introAnimation.OnAnimationUpdate += OnIntroAnimationUpdate;
			// introAnimation.SetDirection(1, true);
			// introAnimation.SetDirection(-1);

			endAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			endAnimation.SetLengthInSeconds(5.0f);
			endAnimation.OnAnimationUpdate += OnEndAnimationUpdate;
			endAnimation.OnAnimationComplete += OnEndAnimationComplete;
			endAnimation.SetDirection(-1, true);

			Dugan.Screen.OnResize += OnResize;
			OnResize();
		}


		private Color hotColor = new Color(0.9529412f, 0.2907785f, 0.2039216f);
		private Color coldColor = new Color(0.2037497f, 0.8857332f, 0.9528302f);

		private float lastHealth = 0.0f;

		private void Update() {
			//Cold color (0.2037497, 0.8857332, 0.9528302)
			//Hot color (0.9529412, 0.2907785, 0.2039216)
			//Max alpha 0.2509804

			lastHealth = Mathf.Lerp(lastHealth, Scene.instance.firePit.health, Time.deltaTime * 10.0f);

			Color col = Color.Lerp(coldColor, hotColor, lastHealth);
			float a = (Mathf.Cos(lastHealth * Mathf.PI * 2.0f) + 1.0f) * 0.5f;
			col.a = Mathf.Lerp(0.0f, 0.30f, a);

			imgVingette.color = col;

			//5 *60 = 300 seconds duration
			//float timeRemaining = 300.0f - Scene.instance.gameTime;

			float timeA = Scene.instance.gameTime / 300.0f;
			timeA = timeA * times.Length;

			int index = Mathf.FloorToInt(timeA);
			index = Mathf.Min(Mathf.Max(index, 0), times.Length-1);

			int mins = Mathf.FloorToInt((timeA - Mathf.Floor(timeA)) * 60.0f);

			string sep = mins % 2 == 0? strColon : strSpace;
			string post = index < 6? strPM : strAM;

			txtTime.text = times[index].ToString(strFormat) + sep + mins.ToString(strFormat) + post;

		}

		private void OnResize() {
			camera.orthographicSize = Dugan.Screen.screenSizeInUnits.y;
			canvas.sizeDelta = Dugan.Screen.layoutSize;
		}

		private void OnClickBtnMenu(Dugan.Input.PointerTarget pointerTarget, string args) {
			Popup p = Dugan.PopupManager.Load<Popups.Menu.Popup>();
			p.PostAwake();
		}

		// private void OnIntroAnimationUpdate(float a) {
		// 	a = Dugan.TimeAnimation.GetNormalizedTimeInTimeSlice(a, 0.0f, 0.25f);
		// 	a = Dugan.Mathf.Easing.EaseInOutCubic(a);
		// 	introCG.alpha = a;
		// }

		public void GameOver(bool won) {
			gameOverCG.transform.Find("YouDied").gameObject.SetActive(!won);
			gameOverCG.transform.Find("YouLived").gameObject.SetActive(won);
			endAnimation.SetDirection(1);
		}

		private void OnEndAnimationUpdate(float a) {
			a = Dugan.TimeAnimation.GetNormalizedTimeInTimeSlice(a, 0.0f, 0.25f);
			a = Dugan.Mathf.Easing.EaseInOutCubic(a);
			gameOverCG.alpha = a;
		}

		private void OnEndAnimationComplete() {
			if (endAnimation.GetDirection() == 1) {
				//Restart the scene!
				UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
			}
		}

		private void OnDisable() {
			Dugan.Screen.OnResize -= OnResize;
		}

	}
}