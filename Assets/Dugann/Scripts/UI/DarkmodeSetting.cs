using UnityEngine;

namespace Dugan.UI {
	public class DarkmodeSetting : MonoBehaviour {
		[Header ("Light Theme")]
		public Color normalColor = Color.white;
		public Sprite normalSprite = null;
		public Texture2D normalTexture = null;
		public Material normalMaterial = null;

		[Header ("Dark Theme")]
		public Color darkColor = Color.white;
		public Sprite darkSprite = null;
		public Texture2D darkTexture = null;
		public Material darkMaterial = null;

		private MeshRenderer mr = null;//Only swaps materials. Will not change material color.
		private UnityEngine.UI.Image uiImage = null;
		private UnityEngine.UI.RawImage uiRawImage = null;
		private UnityEngine.UI.Text uiText = null;
		private TMPro.TextMeshProUGUI tmproText = null;

		//For when this is being applied in editor
		private void Awake() {
			mr = GetComponent<MeshRenderer>();
			uiImage = GetComponent<UnityEngine.UI.Image>();
			uiRawImage = GetComponent<UnityEngine.UI.RawImage>();
			uiText = GetComponent<UnityEngine.UI.Text>();
			tmproText = GetComponent<TMPro.TextMeshProUGUI>();

			ApplyMode(Dugan.NativeExtentions.Plugin.IsDarkmodeEnabled());
			Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged += ApplyMode;
		}

		public void ForceApply() {
			ApplyMode(Dugan.NativeExtentions.Plugin.IsDarkmodeEnabled());
		}

		private void ApplyMode(bool isDarkmodeEnabled) {
			if (mr != null && darkMaterial != null && normalMaterial != null)
				mr.material = (isDarkmodeEnabled? darkMaterial : normalMaterial);

			if (uiImage != null) {
				uiImage.color = (isDarkmodeEnabled? darkColor : normalColor);
				if (darkSprite != null && normalSprite != null)
					uiImage.sprite = (isDarkmodeEnabled? darkSprite : normalSprite);
			}

			if (uiRawImage != null) {
				uiRawImage.color = (isDarkmodeEnabled? darkColor : normalColor);
				if (darkTexture != null && normalTexture != null)
					uiRawImage.texture = (isDarkmodeEnabled? darkTexture : normalTexture);
			}

			if (uiText != null)
				uiText.color = (isDarkmodeEnabled? darkColor : normalColor);
			
			if (tmproText != null)
				tmproText.color = (isDarkmodeEnabled? darkColor : normalColor);
		}

		private void OnDestroy() {
			Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged -= ApplyMode;
		}
	}
}
