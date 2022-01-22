using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Camera : MonoBehaviour {

		private Vector3 cameraDefaultPos = Vector3.zero;

		private float t = 0.0f;
		public float shakeSpeed = 0.01f;
		public float shakeAmplitude = 0.215f;

		private void Awake() {
			cameraDefaultPos = transform.localPosition;
		}

		private void Update() {
			t += Time.deltaTime;

			AnimateCamera();
		}

		private void AnimateCamera() {
			float x = Mathf.Cos(t * Mathf.PI * 2 * shakeSpeed);
			float y = Mathf.Sin(t * Mathf.PI * 3 * shakeSpeed);
			x = Dugan.Mathf.Simplex.Noise(x, 0) * shakeAmplitude;
			y = Dugan.Mathf.Simplex.Noise(y, 0) * shakeAmplitude;

			GetComponent<UnityEngine.Camera>().transform.localPosition = cameraDefaultPos + new Vector3(x, y, 0.0f);
		}

	}
}
