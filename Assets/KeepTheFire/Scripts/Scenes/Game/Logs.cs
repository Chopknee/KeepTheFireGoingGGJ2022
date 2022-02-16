using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
	public class Logs : MonoBehaviour {

		private GameObject[] logFillStates = null;

		private AudioSource audioSource = null;

		public int count = 0;
		private int lastCount = 0;

		private void Awake() {

			audioSource = GetComponent<AudioSource>();

			Transform buttons = transform.Find("Buttons");

			logFillStates = new GameObject[buttons.childCount];

			for (int i = 0; i < logFillStates.Length; i++) {
				logFillStates[i] = buttons.GetChild(i).gameObject;
				Dugan.UI.Button btn = logFillStates[i].AddComponent<Dugan.UI.Button>();
				btn.OnPointerUp += OnClickButton;
			}

			RenderPile();

		}

		private void Update() {
			if (count != lastCount) {
				lastCount = count;
				RenderPile();
			}
		}

		private void RenderPile() {
			for (int i = 0; i < logFillStates.Length; i++) {
				bool on = i < count;
				logFillStates[i].SetActive(on);
			}
		}

		private void OnClickButton(Dugan.Input.PointerTarget pointerTarget, string args) {
			if (count <= 0)
				return;

			audioSource.Play();
			
			count -= 1;
			Scene.instance.firePit.AddLog();

			Scene.instance.firePit.BurstSparks();
			RenderPile();

		}
	}
}