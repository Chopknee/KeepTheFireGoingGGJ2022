using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
	public class Squirrel : MonoBehaviour {

		private AudioSource source = null;

		private int state = 0;

		private GameObject particles = null;
		private new GameObject light = null;
		private GameObject log = null;

		private float fireRadius = 1.0f;

		private Dugan.UI.Button button = null;

		private float targetApproachSpeed = 0.5f;
		private float approachSpeed = 0.5f;
		
		private new Dugan.Animation.QuickClips animation = null;
		private Dugan.Animation.AnimationState animateState = null;

		private float respawnCheckTime = 0.0f;
		private float respawnRestTime = 1.0f;

		private float speedChangeT = 0.0f;

		private void Awake() {

			animation = transform.Find("Squirrel_Animation").GetComponent<Dugan.Animation.QuickClips>();
			animateState = animation["Animate"];
			animateState.wrapMode = WrapMode.Loop;

			light = transform.Find("Light").gameObject;
			light.SetActive(false);
			particles = transform.Find("Particles").gameObject;
			particles.SetActive(false);

			log = transform.Find("Wood_Quarter").gameObject;
			log.SetActive(false);

			button = gameObject.AddComponent<Dugan.UI.Button>();
			button.OnPointerUp += OnClickButton;

			source = GetComponent<AudioSource>();

			respawnRestTime = Random.Range(0.5f, 2.0f);

		}

		private void Update() {
			if (Scene.bPaused)
				return;
				
			if (state == 0) {
				//Idle not spawned, checking for chance to spawn
				respawnCheckTime += Time.deltaTime;
				if (respawnCheckTime > respawnRestTime) {
					respawnCheckTime = 0.0f;
					respawnRestTime = Random.Range(0.5f, 2.0f);
					float num = Random.Range(0, 25);
					if (num == 0)
						Activate();
				}
			}

			if (state == 1) {
				//Approaching the fire, clickable at this point
				speedChangeT += Time.deltaTime;

				if (speedChangeT > 0.5f) {
					targetApproachSpeed = Random.Range(0.0f, 3.5f);

					if (targetApproachSpeed < 0.5)
						targetApproachSpeed = 0.0f;
						
					speedChangeT = 0.0f;
				}
					
				approachSpeed = Mathf.Lerp(approachSpeed, targetApproachSpeed, Time.deltaTime);
				
				transform.position += transform.forward * approachSpeed * Time.deltaTime;
				float distSquared = (transform.position - Scene.instance.firePit.transform.position).sqrMagnitude;
				animateState.speed = approachSpeed * 3f;
				if (distSquared < (fireRadius * fireRadius)) {
					state = 2;
					transform.forward = -transform.forward;
					light.SetActive(true);
					particles.SetActive(true);
					log.SetActive(true);
					//Remove some fire health
					Scene.instance.firePit.RemoveLog();
				}
			}

			if (state == 2) {
				//Grabbed a log, running away on fire or scarred away by player
				transform.position += transform.forward * 6f * Time.deltaTime;
				float distSquared = (transform.position - Scene.instance.firePit.transform.position).sqrMagnitude;
				animateState.speed = 6.0f * 3.0f;
				if (distSquared >= 12 * 12) {
					DeActivate();
				}
			}

			if (state == 3) {
				//idk
			}

		}

		private void Activate() {
			//Move from idle location to a random point in the scene.
			Vector3 centerPoint = Scene.instance.firePit.transform.position;
			float angle = Random.Range(45, 165);
			transform.position = centerPoint + (new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * 12);
			state ++;
			Vector3 forward = centerPoint - transform.position;
			forward.y = 0.0f;
			transform.forward = forward.normalized;

			source.Play();
		}

		private void DeActivate() {
			state = 0;
			transform.localPosition = Vector3.zero;
			light.SetActive(false);
			particles.SetActive(false);
			log.SetActive(false);

			source.Stop();
		}

		private void OnClickButton(Dugan.Input.PointerTarget target, string args) {
			if (state == 1) {
				state = 2;
				transform.forward = -transform.forward;
			}
		}
	}
}
