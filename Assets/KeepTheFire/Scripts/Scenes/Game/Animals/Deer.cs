using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game
{
    public class Deer : MonoBehaviour
    {

        private Dugan.UI.Button button = null;

        public int state = 0;

        // starting elevation
        public float elevation = 3.0f;

        // how far left the deers start
        public float startOffsetX = -16.0f;
        public float startOffsetZ = 0f;
        public float offsetZ = 0f;
        public float bounds = 24.0f;

        public float xPos;
        float faceForward;

        private Vector3 homePos = new Vector3(0.0f, -90.0f, 0.0f);

        // Start is called before the first frame update
        private void Awake()
        {
            // gameObject.SetActive(false);
            button = gameObject.AddComponent<Dugan.UI.Button>();
            button.OnClicked += OnButtonClicked;
        }

        // Update is called once per frame
        void Update()
        {
            if(state == 0) {
                if(Random.Range(0.0f, 1.0f) < 0.005f) {
                    activate();
                    state++;
                }
            }
            else if(state == 1) {
                // do movement
                movement(1.7f);
                checkStopCycle();
            }
            else if(state == 2) {
                movement(-2.7f);
                checkStopCycle();
            }
        }

        private void activate() { // puts deer in starting position
            startOffsetZ = Random.Range(3.5f, 4.5f);
            if(Random.Range(-1f,1f) < 0) {
                startOffsetX = -startOffsetX;
            }
            transform.position = new Vector3(startOffsetX, elevation, startOffsetZ);
            offsetZ = Random.Range(-0.008f, 0.008f);
        }

        private void movement(float speed) { // moves the deer across screen
            if(startOffsetX < 0) {
                xPos = transform.position.x + speed*Time.deltaTime;
                transform.forward = new Vector3(speed, 0f, offsetZ);
            }
            else {
                xPos = transform.position.x - speed*Time.deltaTime;
                transform.forward = new Vector3(-speed, 0f, offsetZ);
            }
            
            startOffsetZ += offsetZ;
            transform.position = new Vector3(xPos, elevation, startOffsetZ);
            
        }

        private void deactivate() {
            // set deer to off screen position
            transform.position = homePos;
        }

        private void checkStopCycle() {
             // check for finished cycle
            if(Mathf.Abs(transform.position.x) >= bounds) {
                deactivate();
                state = 0;
            }
        }

        private void OnButtonClicked(Dugan.Input.PointerTarget target, string args) {
            // check if already flipped
            if(state != 1) {
                return;
            }
            // add to log pile
            Scene.instance.logStashe += 0.1f;

            // Adjust for stupid offset when rotating
            if(startOffsetX < 0) {
                transform.position = transform.position - new Vector3(6f, 0f, 0f);
            }
            else {
                transform.position = transform.position + new Vector3(6f, 0f, 0f);
            }

            // move to state 2
            state = 2;
        }
    }
}