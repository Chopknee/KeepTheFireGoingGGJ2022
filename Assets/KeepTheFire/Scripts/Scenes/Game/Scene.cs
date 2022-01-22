using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Scene : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private HeadsUpDisplay headsUpDisplay = null;

		private void Awake() {

			//Bleh

			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();
			camera.gameObject.AddComponent<Game.Camera>();

			// GameObject hudPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/HeadsUpDisplay");
			// GameObject hudGO = Instantiate(hudPrefab);
			// hudGO.transform.position = new Vector3(0.0f, 0.0f, 1000f);
			// headsUpDisplay = hudGO.AddComponent<HeadsUpDisplay>();
			
			
		}

		private void Update() {

		}
	}
}