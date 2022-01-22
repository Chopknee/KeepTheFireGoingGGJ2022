using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.UI {
	public class TextInput : MonoBehaviour {

		public static TextInput selectedInstance = null;

		public delegate string TextChanged(string text);//Allows the additional processing of text.
		public TextChanged OnTextChanged;
		
		public float cursorBlinkDelay = 0.50f;
		public string placeholderText = "Placeholder...";
		public bool bClearOnSelect = false;
		public float cursorWidth = 15.0f;
		public float width = 0.0f;

		private Transform cursor = null;
		private TMPro.TextMeshProUGUI txtTMPro = null;
		private RectTransform rect = null;
		private Dugan.UI.Button btnTextbox = null;

		private bool bSelected = false;
		
		private float cursorBlinkTime = 0.0f;

		public string text = "";
		private string lastText = "";

		private OnScreenKeyboard.KeyboardSettings keyboardSettings;
		private OnScreenKeyboard onScreenKeyboard = null;
		private int maxLength = 10;

		public void Init(Transform cursor, TMPro.TextMeshProUGUI txtTMPro, Dugan.UI.Button btnTextbox, float width, OnScreenKeyboard.KeyboardSettings keyboardSettings, int maxLength) {
			this.cursor = cursor;
			this.txtTMPro = txtTMPro;
			rect = txtTMPro.GetComponent<RectTransform>();
			this.btnTextbox = btnTextbox;
			this.width = width;

			this.btnTextbox.OnClicked += OnClickBtnInput;
			this.txtTMPro.text = placeholderText;

			this.keyboardSettings = keyboardSettings;
			this.maxLength = maxLength;

			cursor.localPosition = new Vector3((-width / 2.0f) + (cursorWidth / 2.0f), cursor.localPosition.y, cursor.localPosition.z);
			txtTMPro.transform.localPosition += new Vector3(cursorWidth, 0.0f, 0.0f);
		}

		private void Update() {
			if (bSelected) {
				cursorBlinkTime += Time.deltaTime;
				if (cursorBlinkTime > cursorBlinkDelay) {
					cursorBlinkTime = 0.0f;
					cursor.gameObject.SetActive(!cursor.gameObject.activeSelf);
				}
				if (onScreenKeyboard == null) {
					foreach (char c in UnityEngine.Input.inputString) {
						if (c == '\b') {
							if (text.Length > 0)
								text = text.Substring(0, text.Length - 1);
						} else if (c == '\n' || c == '\r') {
							UnSelect();
						} else {
							text += c;
						}
					}
				} else {
					text = onScreenKeyboard.text;
					if (!onScreenKeyboard.keyboard.active)
						UnSelect();
				}
			} else {
				if (cursor.gameObject.activeSelf) {
					cursorBlinkTime = 0.0f;
					cursor.gameObject.SetActive(false);
				}
			}

			//
			if (text != lastText) {
				//Run on changed. This can possibly modify the text value.
				if (OnTextChanged != null)
					text = OnTextChanged(text);
				
				//Check again if the text was modified
				if (text != lastText) {
					lastText = text;
					cursor.gameObject.SetActive(true);
					cursorBlinkTime = 0.0f;

					if (text.Length == 0)
						txtTMPro.text = placeholderText;
					else
						txtTMPro.text = text;

					txtTMPro.ForceMeshUpdate();
					Vector2 size = txtTMPro.GetRenderedValues(false);
					rect.sizeDelta = new Vector2(UnityEngine.Mathf.Max(size.x + cursorWidth, width), rect.sizeDelta.y);
					rect.localPosition = new Vector3(-(rect.sizeDelta.x/2.0f) + (width/2.0f), rect.localPosition.y, rect.localPosition.z);
					if (rect.sizeDelta.x > width)
						rect.localPosition += new Vector3(-(cursorWidth*2), 0.0f, 0.0f);

					if (text.Length == 0)
						cursor.localPosition = new Vector3((-width / 2.0f) + (cursorWidth / 2.0f), cursor.localPosition.y, cursor.localPosition.z);
					else
						cursor.localPosition = new Vector3(UnityEngine.Mathf.Min(size.x - (width / 2.0f) + (cursorWidth / 2.0f), (width / 2.0f) - (cursorWidth)), cursor.localPosition.y, cursor.localPosition.z);

				}
			}

			if (bSelected && Dugan.Input.PointerManager.bAnyPointerDown && !btnTextbox.GetPointerOver()) {
				bSelected = false;
				CloseOnScreenKeyboard();
			}
		}

		private void OnClickBtnInput(Dugan.Input.PointerTarget pointer, string args) {
			if (bSelected) {
				selectedInstance = null;
				CloseOnScreenKeyboard();
				bSelected = false;
			} else {
				if (selectedInstance != null)
					selectedInstance.UnSelect();

				selectedInstance = this;
				bSelected = true;

				if (bClearOnSelect)
					text = string.Empty;

				OpenOnScreenKeyboard();
			}
		}

		private void UnSelect() {
			bSelected = false;
			CloseOnScreenKeyboard();
		}

		private void OpenOnScreenKeyboard() {
			onScreenKeyboard = OnScreenKeyboard.Open(keyboardSettings, maxLength);
			if (onScreenKeyboard != null)
				onScreenKeyboard.text = text;
		}

		private void CloseOnScreenKeyboard() {
			if (onScreenKeyboard != null) {
				onScreenKeyboard.Close();
				onScreenKeyboard = null;
			}
		}

		public void SetSelected(bool bSelected) {
			this.bSelected = bSelected;
			OpenOnScreenKeyboard();//??
		}

		public bool GetSelected() {
			return bSelected;
		}

	}
}