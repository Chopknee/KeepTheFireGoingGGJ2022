using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game
{
    public class Deer : MonoBehaviour
    {

        private Dugan.UI.Button button = null;

        public int state = 0;
        public float speed = 1.7f;

        // starting elevation
        public float elevation = 3.0f;

        // how far left the deers start
        public float startX = -10.0f;

        // how far right the deers travel
        public float stopX = 10.0f;

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
        }

        // Update is called once per frame
        void Update()
        {
            if(state == 0) {
                if(Random.Range(0.0f, 1.0f) < 0.001f) {
                    activate();
                    state++;
                }
            }
            else if(state == 1) {
                // do movement
                movement();
                // check for finished cycle
                if(transform.position.x >= stopX) {
                    deactivate();
                    state = 0;
                }
            }
        }

        void activate() { // puts deer in starting position
            moveZ = Random.Range(-1.0f, 1.0f);
            startZ = Random.Range(-5.0f, 6.0f);
            transform.position = new Vector3(startX, elevation, startZ);
        }

        private void movement() { // moves the deer across screen
            // move distance
            float stepX = speed*Time.deltaTime;
            float stepZ = moveZ*Time.deltaTime;

            // set position of deer based on step size
            transform.position = transform.position + new Vector3(stepX, 0f, stepZ);
        }

        private void deactivate() {
            // set deer to off screen position
            transform.position = homePos;
        }

        private void OnButtonClicked(Dugan.Input.PointerTarget target, string args) {
            Debug.Log("Button Clicked!");
        }
    }
}