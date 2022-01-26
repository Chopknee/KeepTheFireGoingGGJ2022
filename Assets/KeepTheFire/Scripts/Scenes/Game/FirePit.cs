using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
	public class FirePit : MonoBehaviour {

		private new Light light = null;
		private ParticleSystem particles = null;
		private ParticleSystem.MainModule particlesMainModule;
		private ParticleSystem.EmissionModule emissionModule;

		private ParticleSystem sparksParticles = null;

		public GameObject umbrella = null;

		public List<float> burningLogs = new List<float>();

		private float _health = 0.0f;
		public float health {
			get {
				return _health;
			}
		}

		private void Awake() {
			light = transform.Find("Light").GetComponent<Light>();
			particles = transform.Find("Particles").GetComponent<ParticleSystem>();
			particlesMainModule = particles.main;
			emissionModule = particles.emission;
			sparksParticles = transform.Find("Particles/Sparks").GetComponent<ParticleSystem>();
			umbrella = transform.Find("Umbrella").gameObject;
		}

		private float lastHealth = 0.0f;

		private void Update() {

			//Scene.instance.fireHealth
			lastHealth = Mathf.Lerp(lastHealth, _health, Time.deltaTime * 10.0f);

			emissionModule.rateOverTime = Mathf.Lerp(0, 50, lastHealth);
			light.intensity = Mathf.Lerp(0, 8, lastHealth);

			if (Scene.instance.menu.GetDirection() > 0)
				return;

			if (burningLogs.Count > 0) {
				for (int i = 0; i < burningLogs.Count; i++) {
					//Update logs health
					float decayValue = 0.01f;
					if (Scene.instance.bIsRaining)
						decayValue += 0.01f;

					float fireDecay = decayValue * Time.deltaTime;//Loose 1% of total health per second (100 seconds til death)

					fireDecay *= -1;
					burningLogs[i] += fireDecay;

					if (burningLogs[i] < 0) {
						burningLogs.RemoveAt(i);
						continue;
					}
				}
				
				UpdateHealth();
			} else {
				_health = 0.0f;
			}
		}

		public void BurstSparks() {
			sparksParticles.Play();
		}

		public void AddLog() {
			burningLogs.Add(1.0f);
			UpdateHealth();
		}

		public void RemoveLog() {
			if (burningLogs.Count > 0) {
				burningLogs.RemoveAt(0);
				UpdateHealth();
			}
		}

		private void UpdateHealth() {
			_health = 0.0f;
			for (int i = 0; i < burningLogs.Count; i++) {
				_health += burningLogs[i];
			}
			_health = _health / burningLogs.Count;
		}
	}
}