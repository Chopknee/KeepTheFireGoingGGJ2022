using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Scene : MonoBehaviour {

		private new UnityEngine.Camera camera = null;


		private void Awake() {

			//Bleh

			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();
			camera.gameObject.AddComponent<Game.Camera>();
			
			
		}

		private void Update() {

		}
	}
}