using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Wolf : MonoBehaviour
    {

        private AudioSource source = null;

        private int state = 0;

        private float logPileRadius = 0.8f;

        private Dugan.UI.Button button = null;

        private float approachSpeed = 2f;

        void Awake() {
            button = gameObject.AddComponent<Dugan.UI.Button>();
            button.OnPointerUp += OnClickButton;
            source = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Scene.bPaused)
				return;
            if(state == 0) {
                if(Random.Range(0.0f, 1.0f) < 0.01f) {
                    Activate();
                }
            }

            if(state == 1) {
                transform.position += transform.forward * approachSpeed * Time.deltaTime;
                float distSquared = (transform.position - Scene.instance.logs.transform.position).sqrMagnitude;
                if(distSquared < (logPileRadius * logPileRadius)) {
                    state = 2;
                    transform.forward = -transform.forward;
                    // Remove Log from pile
                    Scene.instance.logs.count -= 1;
                    transform.Find("Wood_Whole").gameObject.SetActive(true);
                }
            }

            if(state == 2) {
                transform.position += transform.forward * 2f * Time.deltaTime;
                float distSquared = (transform.position - Scene.instance.logs.transform.position).sqrMagnitude;
                if(distSquared >= 12 * 12) {
                    Deactivate();
                }
            }
        }

        void Activate() {
            Vector3 centerPoint = Scene.instance.logs.transform.position;
            float angle = Random.Range(60, 105);
            transform.position = centerPoint + (new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle * Mathf.Deg2Rad) * 12));
            state ++;
            Vector3 forward = centerPoint - transform.position;
            forward.y = 0.0f;
            transform.forward = forward.normalized;
            transform.Find("Wood_Whole").gameObject.SetActive(false);

            source.Play();
        }

        void Deactivate() {
            state = 0;
            transform.localPosition = Vector3.zero;
            source.Stop();
        }

        void OnClickButton(Dugan.Input.PointerTarget target, string args) {
            if(state == 1) {
                state = 2;
                transform.forward = -transform.forward;
            }
           
        }
    }
}