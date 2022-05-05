using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game {
	public class Deer : MonoBehaviour {

		private AudioSource source = null;

		private Dugan.UI.Button button = null;

		private new Dugan.Animation.QuickClips animation = null;
		private Dugan.Animation.AnimationState animateState = null;


		private float normalSpeed = 3.0f;
		private float targetNormalSpeed = 3.0f;

		private float runawaySpeed = 6.0f;

		private float respawnCheckTime = 0.0f;
		private float respawnRestTime = 1.0f;

		private float speedChangeT = 0.0f;

		private float animationSpeedRatio = 1.5f;

		private int state = 0;

		private float elevation = 0.0f;
		private float startOffsetX = -16.0f;
		private float startOffsetZ = 0f;
		private float offsetZ = 0f;
		private float bounds = 24.0f;

		private float xPos;

		private void Awake() {
			animation = transform.Find("Buck_Animation").GetComponent<Dugan.Animation.QuickClips>();
			animateState = animation["Animate"];
			animateState.wrapMode = WrapMode.Loop;

			button = gameObject.AddComponent<Dugan.UI.Button>();
			button.OnPointerUp += OnButtonClicked;
			source = GetComponent<AudioSource>();
		}

		void Update() {
			if (Scene.bPaused)
				return;

			if (state == 0) {
				//Idle not spawned, checking for chance to spawn
				respawnCheckTime += Time.deltaTime;
				if (respawnCheckTime > respawnRestTime) {
					respawnCheckTime = 0.0f;
					respawnRestTime = Random.Range(0.5f, 2.0f);
					float num = Random.Range(0, 15);
					if (num == 0)
						Activate();
				}
			}

			if (state == 1) {

				speedChangeT += Time.deltaTime;
				if (speedChangeT > 0.5f) {
					targetNormalSpeed = Random.Range(0.25f, 4.5f);
					speedChangeT = 0.0f;
				}
				
				normalSpeed = Mathf.Lerp(normalSpeed, targetNormalSpeed, Time.deltaTime);

				transform.position += transform.forward * normalSpeed * Time.deltaTime;
				animateState.speed = normalSpeed * animationSpeedRatio;
				CheckStopCycle();//Checking for out of bounds
			}

			if (state == 2) {
				transform.position += transform.forward * runawaySpeed * Time.deltaTime;
				animateState.speed = runawaySpeed * animationSpeedRatio;
				CheckStopCycle();//Checking for out of bounds
			}
		}

		private void Activate() { // puts deer in starting position
			startOffsetZ = Random.Range(4f, 5f);
			if (Random.Range(-1f, 1f) < 0) {
				startOffsetX = -startOffsetX;
			}

			if (startOffsetX < 0) {
				transform.forward = new Vector3(90f, 0f, 0f);
			} else {
				transform.forward = new Vector3(-90f, 0f, 0f);
			}

			transform.position = new Vector3(startOffsetX, elevation, startOffsetZ);

			offsetZ = Random.Range(-0.006f, 0.006f);
			source.Play();
			state = 1;
		}

		private void Deactivate() {
			transform.position = new Vector3(0.0f, -90.0f, 0.0f);
			transform.Find("Wood_Whole").gameObject.SetActive(true);
			source.Stop();
		}

		private void CheckStopCycle() {
			if (Mathf.Abs(transform.position.x) >= bounds || Mathf.Abs(transform.position.z) >= bounds) {
				Deactivate();
				state = 0;
			}
		}

		private void OnButtonClicked(Dugan.Input.PointerTarget target, string args) {
			if (state != 1)
				return;

			transform.forward = new Vector3(0f, 0f, 1f);
			transform.Find("Wood_Whole").gameObject.SetActive(false);
			Scene.instance.logs.count += 1;
			state = 2;
		}
	}
}