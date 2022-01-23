using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Logs : MonoBehaviour {

        private Dugan.UI.Button button = null;

        private float lastLogStashe = 0.0f;

        private GameObject[] logFillStates = null;

        private AudioSource audioSource = null;

        private void Awake() {

            audioSource = GetComponent<AudioSource>();

            logFillStates = new GameObject[4];

            for (int i = 0; i < logFillStates.Length; i++) {
                logFillStates[i] = transform.Find(i.ToString()).gameObject;
			}

            button = transform.Find("Button").gameObject.AddComponent<Dugan.UI.Button>();
            button.OnClicked += OnClickButton;

        }

        private void Update() {
            if (Scene.instance.logStashe != lastLogStashe) {
                lastLogStashe = Scene.instance.logStashe;
                RenderPile();
			}
		}

        private void RenderPile() {
            int index = Mathf.FloorToInt(logFillStates.Length * Scene.instance.logStashe);
            index = Mathf.Max(0, Mathf.Min(logFillStates.Length - 1, index));
            for (int i = 0; i < logFillStates.Length; i++) {
                bool on = i <= index;
                if (Scene.instance.logStashe == 0)
                    on = false;
                if (Scene.instance.logStashe >= 1)
                    on = true;

                logFillStates[i].SetActive(on);
			}
		}

        private void OnClickButton(Dugan.Input.PointerTarget pointerTarget, string args) {
            if (Scene.instance.logStashe <= 0.0f)
                return;

            audioSource.Play();
            //Add logs to the fire.
            //Update pile rendering.
            Scene.instance.logStashe -= 0.01f;
            Scene.instance.fireHealth += 0.01f;

            Scene.instance.firePit.BurstSparks();
            RenderPile();

		}
    }
}