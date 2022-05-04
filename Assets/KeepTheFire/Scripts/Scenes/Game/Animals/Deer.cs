using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game
{
    public class Deer : MonoBehaviour
    {

        private AudioSource source = null;

        private Dugan.UI.Button button = null;

        public int state = 0;

        public float elevation = 0.0f;
        public float startOffsetX = -16.0f;
        public float startOffsetZ = 0f;
        public float offsetZ = 0f;
        public float bounds = 24.0f;

        public float xPos;
        float faceForward;

        // Start is called before the first frame update
        private void Awake()
        {
            // gameObject.SetActive(false);
            button = gameObject.AddComponent<Dugan.UI.Button>();
            button.OnPointerUp += OnButtonClicked;
            source = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
			if (Scene.bPaused)
				return;

            if(state == 0) {
                if(Random.Range(0.0f, 1.0f) < 0.001f) {
                    Activate();
                    state++;
                }
            }
            else if(state == 1) {
                // do Movement
                Movement(Random.Range(0.8f, 2f));
                CheckStopCycle();
            }
            else if(state == 2) {
                offsetZ = Random.Range(0.15f, 0.05f);
                Movement(Random.Range(2.7f, 3.0f));
                CheckStopCycle();
            }
        }

        private void Activate() { // puts deer in starting position
            startOffsetZ = Random.Range(4f, 5f);
            if(Random.Range(-1f,1f) < 0) {
                startOffsetX = -startOffsetX;
            }

            if(startOffsetX < 0) {
                transform.forward = new Vector3(90f, 0f, 0f);
            }
            else {
                transform.forward = new Vector3(-90f, 0f, 0f);
            }

            transform.position = new Vector3(startOffsetX, elevation, startOffsetZ);
            
            offsetZ = Random.Range(-0.006f, 0.006f);
            source.Play();
        }

        private void Movement(float speed) { // moves the deer across screen
            if(startOffsetX < 0) {
                xPos = transform.position.x + speed*Time.deltaTime;
            }
            else {
                xPos = transform.position.x - speed*Time.deltaTime;
            }
            startOffsetZ += offsetZ;
            transform.position = new Vector3(xPos, elevation, startOffsetZ);   
        }

        private void Deactivate() {
            transform.position = new Vector3(0.0f, -90.0f, 0.0f);
            transform.Find("Wood_Whole").gameObject.SetActive(true);
            source.Stop();
        }

        private void CheckStopCycle() {
            if(Mathf.Abs(transform.position.x) >= bounds || Mathf.Abs(transform.position.z) >= bounds) {
                Deactivate();
                state = 0;
            }
        }

        private void OnButtonClicked(Dugan.Input.PointerTarget target, string args) {
            if(state != 1) {
                return;
            }
            transform.forward = new Vector3(0f, 0f, 90f);

            // Adjust for stupid offset when rotating
            if(startOffsetX < 0) {
                transform.position = transform.position - new Vector3(3f, 0f, 0f);
            }
            else {
                transform.position = transform.position + new Vector3(3f, 0f, 0f);
            }

            transform.Find("Wood_Whole").gameObject.SetActive(false);
            Scene.instance.logs.count += 1;
            state = 2;
        }
    }
}