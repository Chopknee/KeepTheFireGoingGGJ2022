using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game {
	public class HeadsUpDisplay : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private RectTransform canvas = null;

		private UnityEngine.UI.RawImage imgVingette = null;

		private Dugan.UI.Button btnMenu = null;

		private UnityEngine.UI.Image imgFade = null;
		private Color imgFadeColor = Color.black;

		private CanvasGroup introCG = null;

		private TMPro.TextMeshProUGUI txtTime = null;
		private int[] times = { 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7};

		private string strFormat = "D2";
		private string strColon = ":";
		private string strSpace = " ";
		private string strAM = " AM";
		private string strPM = " PM";

		private Dugan.TimeAnimation fadeAnimation = null;
		private Dugan.TimeAnimation introAnimation = null;

		private void Awake() {
			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();

			canvas = transform.Find("Canvas") as RectTransform;

			imgVingette = canvas.Find("ImgVingette").GetComponent<UnityEngine.UI.RawImage>();

			btnMenu = canvas.Find("BtnMenu").gameObject.AddComponent<Dugan.UI.Button>();
			btnMenu.OnClicked += OnClickBtnMenu;

			Dugan.Screen.OnResize += OnResize;

			imgFade = canvas.Find("ImgFade").GetComponent<UnityEngine.UI.Image>();
			imgFadeColor = imgFade.color;
			imgFadeColor.a = 0;
			imgFade.color = imgFadeColor;

			introCG = canvas.Find("Intro").GetComponent<CanvasGroup>();

			txtTime = canvas.Find("Watch/TxtTime").GetComponent<TMPro.TextMeshProUGUI>();

			// fadeAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			// fadeAnimation.SetLengthInSeconds(1.0f);
			// fadeAnimation.OnAnimationUpdate += OnFadeAnimationUpdate;
			// fadeAnimation.SetDirection(1, true);
			// fadeAnimation.SetDirection(-1);

			introAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			introAnimation.SetLengthInSeconds(5.0f);
			introAnimation.OnAnimationUpdate += OnIntroAnimationUpdate;
			introAnimation.SetDirection(1, true);
			introAnimation.SetDirection(-1);
		}


		private Color hotColor = new Color(0.9529412f, 0.2907785f, 0.2039216f);
		private Color coldColor = new Color(0.2037497f, 0.8857332f, 0.9528302f);

		private void Update() {
			//Cold color (0.2037497, 0.8857332, 0.9528302)
			//Hot color (0.9529412, 0.2907785, 0.2039216)
			//Max alpha 0.2509804

			Color col = Color.Lerp(coldColor, hotColor, Scene.instance.fireHealth);

			float a = (Mathf.Cos(Scene.instance.fireHealth * Mathf.PI * 2.0f) + 1.0f) * 0.5f;
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
			
		}

		private void OnFadeAnimationUpdate(float a) {
			imgFadeColor.a = a;
			imgFade.color = imgFadeColor;
		}

		private void OnIntroAnimationUpdate(float a) {
			a = Dugan.TimeAnimation.GetNormalizedTimeInTimeSlice(a, 0.0f, 0.25f);
			a = Dugan.Mathf.Easing.EaseInOutCubic(a);
			introCG.alpha = a;
		}

	}
}