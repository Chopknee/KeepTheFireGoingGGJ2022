using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game
{
    public class Deer : MonoBehaviour
    {

        public int state = 0;
        public float speed = 1.7f;

        // starting elevation
        public float yPos = 3.0f;

        // x,z for starting pos vector
        public float startX = -10.0f;
        public float startZ;
        public float moveZ;

        public float stopX = 10.0f;

        // Start is called before the first frame update
        private void Awake()
        {
            // gameObject.SetActive(false);
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
                // check for ending movement
                if(transform.position.x >= stopX) {
                    deactivate();
                    state = 0;
                }
            }

        }

        void activate() { // puts deer in starting position
            moveZ = Random.Range(-1.0f, 1.0f);
            startZ = Random.Range(-5.0f, 6.0f);
            transform.position = new Vector3(startX, yPos, startZ);
        }

        void movement() { // moves the deer across screen
            // move distance
            float stepX = speed*Time.deltaTime;
            float stepZ = moveZ*Time.deltaTime;

            // set position of deer based on step size
            transform.position = transform.position + new Vector3(stepX, 0f, stepZ);
        }

        void deactivate() {
            // set deer to off screen position
            transform.position = new Vector3(0.0f, -90.0f, 0.0f);
        }
    }
}