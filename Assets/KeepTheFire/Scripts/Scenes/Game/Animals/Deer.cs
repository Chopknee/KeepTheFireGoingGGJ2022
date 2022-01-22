using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game
{
    public class Deer : MonoBehaviour
    {

        public int state = 0;
        public float speed = 1.0f;

        // Start is called before the first frame update
        private void Awake()
        {
            // gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(state == 0) {
                if(Random.Range(0.0f, 1.0f) < 0.0001f) {
                    activate();
                    state++;
                }
            }
            else if(state == 1) {
                // do movement
                movement();
                // check for ending movement
                if(transform.position.x >= 10.0f) {
                    deactivate();
                    state = 0;
                }
            }

        }

        void activate() { // puts deer on screen
            // set deer to be on screen
            transform.position = new Vector3(-10.0f, 3.0f, 1.0f);
        }

        void movement() { // moves the deer across screen
            // move distance
            float step = speed*Time.deltaTime;

            // set position of deer based on step size
            transform.position = transform.position + new Vector3(step, 0f, 0f);
        }

        void deactivate() {
            // set deer to off screen position
            transform.position = new Vector3(0.0f, -90.0f, 0.0f);
        }
    }
}