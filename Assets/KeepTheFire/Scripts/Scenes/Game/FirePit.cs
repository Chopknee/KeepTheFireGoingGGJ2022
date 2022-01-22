using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class FirePit : MonoBehaviour {

        private new Light light = null;
        private ParticleSystem particles = null;
        private ParticleSystem.MainModule particlesMainModule;
        private ParticleSystem.EmissionModule emissionModule;

        private void Awake() {

            light = transform.Find("Light").GetComponent<Light>();
            particles = transform.Find("Particles").GetComponent<ParticleSystem>();
            particlesMainModule = particles.main;
            emissionModule = particles.emission;

        }

        private void Update() {
            //Scene.instance.fireHealth
            emissionModule.rateOverTime = Mathf.Lerp(0, 100, Scene.instance.fireHealth);
            light.intensity = Mathf.Lerp(0, 6, Scene.instance.fireHealth);
            //light.color = Color.Lerp()
            //particlesMainModule
		}
    }
}