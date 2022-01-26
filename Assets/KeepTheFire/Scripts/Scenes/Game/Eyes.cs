using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Eyes : MonoBehaviour {

        private static Texture2D[] eyeTextures = null;

        private Dugan.TimeAnimation timeAnimation = null;

        private Transform quad = null;

		private float flashlightOver = 1;
		private float flashlightOverSmooth = 1;

        private void Awake() {

            if (eyeTextures == null) {
                eyeTextures = new Texture2D[5];
                for (int i = 0; i < eyeTextures.Length; i++) {
                    eyeTextures[i] = Resources.Load<Texture2D>("KeepTheFire/Scenes/Game/EyeTextures/Eyes_Detailed_" + (i + 1));
                }
            }

            quad = transform.Find("Quad");

            int index = Random.Range(0, eyeTextures.Length);

            GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", eyeTextures[index]);
            GetComponentInChildren<MeshRenderer>().material.SetTexture("_EmissionMap", eyeTextures[index]);

            timeAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
            timeAnimation.SetLengthInSeconds(2.0f);
            timeAnimation.OnAnimationUpdate += OnAnimationUpdate;
            timeAnimation.SetDirection(-1, true);

		}

        private void Update() {
            if (!timeAnimation.IsPlaying()) {
                if (Random.Range(0.0f, 1.0f) < 0.1) {
                    timeAnimation.SetLengthInSeconds(Random.Range(0.5f, 3.0f));
                    timeAnimation.SetDirection(-1, true);
                    timeAnimation.SetDirection(1);
                }
            }

			flashlightOverSmooth = Mathf.Lerp(flashlightOverSmooth, flashlightOver, Time.deltaTime * 20.0f);
		}

        private void OnAnimationUpdate(float a) {

            float a1 = (Dugan.Mathf.Simplex.Noise(transform.position.x + a, transform.position.y + a) + 1) * 0.5f;

            a = (1.0f - Mathf.Cos(a * Mathf.PI * 2.0f)) * 0.5f;

            quad.localScale = new Vector3(1.0f, a * a1 * flashlightOverSmooth, 1.0f);

		}

		private void OnFlashlightHovered() {
			flashlightOver = 0;
		}

		private void OnFlashlightLeft() {
			flashlightOver = 1;
		}
    }
}
