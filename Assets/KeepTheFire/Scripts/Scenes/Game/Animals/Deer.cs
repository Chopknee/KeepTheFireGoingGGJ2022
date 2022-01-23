using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game
{
    public class Deer : MonoBehaviour
    {

        private Dugan.UI.Button button = null;

        public int state = 0;
        public float walkSpeed = 1.7f;
        public float startleSpeed = 2.7f;

        // starting elevation
        public float elevation = 3.0f;

        // how far left the deers start
        public float startX = -10.0f;

        // how far right the deers travel
        public float bounds = 11.0f;

        // starting pos and step size for randomized movement
        public float startZ;
        public float moveZ;

        private Vector3 homePos = new Vector3(0.0f, -90.0f, 0.0f);

        // Start is called before the first frame update
        private void Awake()
        {
            // gameObject.SetActive(false);
            button = gameObject.AddComponent<Dugan.UI.Button>();
            button.OnClicked += OnButtonClicked;

            //transform.Rotate(0, 90, 0);
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
                movement(walkSpeed);
                checkStopCycle();
            }
            else if(state == 2) {
                movement(-startleSpeed);
                checkStopCycle();
            }
        }

        private void activate() { // puts deer in starting position
            moveZ = Random.Range(-1.0f, 1.0f);
            startZ = Random.Range(-5.0f, 6.0f);
            
            if(Random.Range(-1f, 1f) > 0) {
                startX = -startX;
                // adjust rotation of deer
                //transform.Rotate(0, -180, 0);
            }

            transform.position = new Vector3(startX, elevation, startZ);
        }

        private void movement(float speed) { // moves the deer across screen
            // move distance
            float stepX = speed*Time.deltaTime;
            float stepZ = moveZ*Time.deltaTime;

            if(startX > 0) {
                stepX = -stepX;
            }

            // set position of deer based on step size
            transform.position = transform.position + new Vector3(stepX, 0f, stepZ);
            transform.forward = new Vector3(stepX, 0f, stepZ).normalized;
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
            Debug.Log("Deer Clicked!");
            if(state != 1) {
                return;
            }
            
            // Rotate deer
            //transform.Rotate(0, 180, 0);

            // add to log
            Scene.instance.logStashe += 0.1f;

            moveZ = -moveZ;
            
            if(moveZ < 0) {
                moveZ = moveZ - Random.Range(1.0f, 3.0f);
            }
            else {
                moveZ = moveZ + Random.Range(1.0f, 3.0f);
            }
            
            state = 2;
        }
    }
}