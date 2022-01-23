using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
	public class Squirrel : MonoBehaviour {

		private int state = 0;

		private GameObject particles = null;
		private new GameObject light = null;
		private GameObject log = null;

		private float fireRadius = 1.0f;

		private Dugan.UI.Button button = null;

		private float approachSpeed = 0.5f;

		private void Awake() {

			light = transform.Find("Light").gameObject;
			light.SetActive(false);
			particles = transform.Find("Particles").gameObject;
			particles.SetActive(false);

			log = transform.Find("Wood_Quarter").gameObject;
			log.SetActive(false);

			button = gameObject.AddComponent<Dugan.UI.Button>();
			button.OnClicked += OnClickButton;

		}

		float a = 0;

		private void Update() {
			if (state == 0) {
				//Idle not spawned, checking for chance to spawn
				if (Random.Range(0.0f, 1.0f) < 0.01f) {
					//Spawn 
					Activate();
				}
			}

			if (state == 1) {
				//Approaching the fire, clickable at this point
				transform.position += transform.forward * 0.5f * Time.deltaTime;
				float distSquared = (transform.position - Scene.instance.firePit.transform.position).sqrMagnitude;
				if (distSquared < (fireRadius * fireRadius)) {
					state = 2;
					transform.forward = -transform.forward;
					light.SetActive(true);
					particles.SetActive(true);
					log.SetActive(true);
					//Remove some fire health
					Scene.instance.fireHealth += -0.05f;
				}
			}

			if (state == 2) {
				//Grabbed a log, running away on fire or scarred away by player
				transform.position += transform.forward * 2f * Time.deltaTime;
				float distSquared = (transform.position - Scene.instance.firePit.transform.position).sqrMagnitude;
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
		}

		private void DeActivate() {
			state = 0;
			transform.localPosition = Vector3.zero;
			light.SetActive(false);
			particles.SetActive(false);
			log.SetActive(false);
		}

		private void OnClickButton(Dugan.Input.PointerTarget target, string args) {
			if (state == 1) {
				state = 2;
				transform.forward = -transform.forward;
			}
		}
	}
}
