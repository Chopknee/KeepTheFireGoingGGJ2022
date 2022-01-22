using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Class Dugan.UI.Button
 *
*/
namespace Dugan.UI {
	public class Button : Dugan.Input.PointerTarget {

		protected bool bSelected = false;
		public bool unSelectOnPointerUp = true;
		
		private bool _tint = false;
		public bool tintOnClick {
			get {
				return _tint;
			}
			set {
				bool lastTint = _tint;
				_tint = value;

				if (lastTint != _tint) {
					if (_tint) {
						SetUpTinting();
					} else {
						if (buttonParts == null)
							return;
						for (int i = 0; i < buttonParts.Count; i++) {
							buttonParts[i].ResetTintAndAlpha();
						}
						buttonParts.Clear();
					}
				}
			}
		}

		protected Dictionary<int, bool> interactiveStates = null;
		protected bool bInteractive = true;

		public Event OnClicked;
		public string OnClickedArgs = "";

		protected List<ButtonPart> buttonParts = null;

		protected float heldColorMultiplier = 0.5f;
		protected bool bUpdateColor = false;

		protected virtual void Awake() {
			interactiveStates = new Dictionary<int, bool>();
			SetInteractive(true, 0);

			buttonParts = new List<ButtonPart>();
		}

		private void SetUpTinting() {
			UnityEngine.UI.Image[] images = GetComponentsInChildren<UnityEngine.UI.Image>();
			for (int i = 0; i < images.Length; i++) {
				buttonParts.Add(new ButtonPart(images[i]));
			}

			UnityEngine.UI.RawImage[] rawImages = GetComponentsInChildren<UnityEngine.UI.RawImage>();
			for (int i = 0; i < rawImages.Length; i++) {
				buttonParts.Add(new ButtonPart(rawImages[i]));
			}

			UnityEngine.UI.Text[] texts = GetComponentsInChildren<UnityEngine.UI.Text>();
			for (int i = 0; i < texts.Length; i++) {
				buttonParts.Add(new ButtonPart(texts[i]));
			}

			TMPro.TextMeshProUGUI[] textMeshes = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
			for (int i = 0; i < textMeshes.Length; i++) {
				buttonParts.Add(new ButtonPart(textMeshes[i]));
			}

			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < renderers.Length; i++) {
				buttonParts.Add(new ButtonPart(renderers[i]));
			}
		}

		public bool GetSelected() {
			return bSelected;
		}

		public void SetSelected(bool bValue) {
			bSelected = bValue;
			UpdateHighlight();
		}

		protected override void OnPointerHeldInternal() {
			if (!bInteractive)
				return;
				
			base.OnPointerHeldInternal();
		}

		protected override void OnPointerExitInternal() {
			if (!bInteractive)
				return;

			base.OnPointerExitInternal();
			UpdateHighlight();
		}

		protected override void OnPointerEnterInternal() {
			if (!bInteractive)
				return;

			base.OnPointerEnterInternal();
			UpdateHighlight();
		}

		protected override void OnPointerDownInternal() {
			if (!bInteractive)
				return;

			base.OnPointerDownInternal();
			bSelected = true;
			UpdateHighlight();
		}

		protected override void OnPointerUpInternal() {
			if (!bInteractive)
				return;

			base.OnPointerUpInternal();
			if (base.GetPointerOver() && OnClicked != null && bInteractive)
				OnClicked(this, OnClickedArgs);
			
			if (unSelectOnPointerUp)
				bSelected = false;

			UpdateHighlight();
		}

		public void SetInteractive(bool bInteractive, int interactiveIndex = 0) {
			if (!interactiveStates.ContainsKey(interactiveIndex))
				interactiveStates.Add(interactiveIndex, !bInteractive);//Force an update in this case

			if (interactiveStates[interactiveIndex] == bInteractive)
				return;

			interactiveStates[interactiveIndex] = bInteractive;

			this.bInteractive = true;
			foreach (KeyValuePair<int, bool> state in interactiveStates) {
				this.bInteractive &= state.Value;
			}
		}

		public bool GetInteractiveState(int interactiveIndex = 0) {
			if (!interactiveStates.ContainsKey(interactiveIndex))
				SetInteractive(true, interactiveIndex);
			
			return interactiveStates[interactiveIndex];
		}

		public bool GetInteractive() {
			return bInteractive;
		}

		protected virtual void UpdateHighlight() {
			if (bSelected && base.GetPointerOver() && bInteractive) {
				//Make everything go dark
				for (int i = 0; i < buttonParts.Count; i++) {
					buttonParts[i].SetTint(heldColorMultiplier);
					
				}
			} else {
				//Make everything go light
				for (int i = 0; i < buttonParts.Count; i++) {
					buttonParts[i].SetTint(0.0f);
				}
			}
		}

		public void SetAlpha(float alpha) {
			for (int i = 0; i < buttonParts.Count; i++) {
				buttonParts[i].SetAlpha(alpha);
			}
		}

		protected class ButtonPart {//Each graphic item on the button tree gets one of these associated with it
			private Color baseColor;
			private float tintAmount = 0.0f;//tint of '0' means original color
			private float alpha = 1.0f;
			//private float baseColorMagnitude = 0.0f;
			private UnityEngine.UI.Image image = null;
			private UnityEngine.UI.RawImage rawImage = null;
			private UnityEngine.UI.Text text = null;
			private TMPro.TextMeshProUGUI textMesh = null;
			private Renderer renderer = null;
			private bool bBaseColorIsInvalid = true;

			public Color color {
				get {//Applies the tint amount and color appropriately.
					Color c = baseColor;
					c *= (1.0f - tintAmount);//Darken to the tint level
					c.a = baseColor.a * alpha;
					return c;
				}
			}

			public ButtonPart(UnityEngine.UI.Image image) {
				this.image = image;
				Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged += OnDarkmodeSettingChanged;
				//ApplyColor();
			}

			public ButtonPart(UnityEngine.UI.RawImage rawImage) {
				this.rawImage = rawImage;
				Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged += OnDarkmodeSettingChanged;
				//ApplyColor();
			}

			public ButtonPart(UnityEngine.UI.Text text) {
				this.text = text;
				Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged += OnDarkmodeSettingChanged;
				//ApplyColor();
			}

			public ButtonPart(TMPro.TextMeshProUGUI textMesh) {
				this.textMesh = textMesh;
				Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged += OnDarkmodeSettingChanged;
				//ApplyColor();
			}

			public ButtonPart(Renderer renderer) {
				this.renderer = renderer;
				Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged += OnDarkmodeSettingChanged;
				//ApplyColor();
			}

			public void SetTint(float tintAmount) {
				this.tintAmount = tintAmount;
				ApplyColor();
			}

			public void SetAlpha(float alpha) {
				this.alpha = alpha;
				ApplyColor();
			}

			public void ResetTintAndAlpha() {
				this.tintAmount = 0.0f;
				this.alpha = 1.0f;
				ApplyColor();
			}

			private void ApplyColor() {
				if (bBaseColorIsInvalid)
					UpdateBaseColor();
					
				if (image != null)
					image.color = color;

				if (rawImage != null)
					rawImage.color = color;
				
				if (text != null)
					text.color = color;

				if (textMesh != null)
					textMesh.color = color;
				
				if (renderer != null)
					Dugan.RendererExtensions.SetMaterialColor(renderer, color);
			}

			private void OnDarkmodeSettingChanged(bool isDarkmodeEnabled) {
				bBaseColorIsInvalid = true;
			}

			private void UpdateBaseColor() {
				if (image != null) {
					baseColor = image.color;
				}

				if (rawImage != null)
					baseColor = rawImage.color;
				
				if (text != null)
					baseColor = text.color;

				if (textMesh != null)
					baseColor = textMesh.color;
				
				if (renderer != null)
					baseColor = Dugan.RendererExtensions.GetMaterialColor(renderer);
				
				bBaseColorIsInvalid = false;
			}

			~ButtonPart() {
				Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged -= OnDarkmodeSettingChanged;
			}

		}
	}
}
