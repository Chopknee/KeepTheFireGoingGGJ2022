using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Scene : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private HeadsUpDisplay headsUpDisplay = null;

		public FirePit firePit { get; internal set; }

		private Logs logs = null;

		private GameObject pool = null;
		private Deer[] deers = null;

		private Light playerTorch = null;

		public static Scene instance = null;
		
		public float fireHealth = 0.5f;

		public float logStashe = 0.5f;

		private void Awake() {
			instance = this;

			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();
			camera.gameObject.AddComponent<Game.Camera>();

			GameObject hudPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/HeadsUpDisplay");
			headsUpDisplay = Instantiate(hudPrefab).AddComponent<HeadsUpDisplay>();
			headsUpDisplay.transform.position = new Vector3(0.0f, 0.0f, 1000.0f);

			Transform level = transform.Find("Level");

			playerTorch = level.Find("Player/Light").GetComponent<Light>();

			firePit = level.Find("FirePit").gameObject.AddComponent<FirePit>();

			logs = level.Find("Logs").gameObject.AddComponent<Logs>();

			pool = new GameObject("Pool");
			pool.transform.SetParent(transform);
			pool.transform.localPosition = new Vector3(0, -100, 0);
			GameObject deerPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/Animals/Deer_Buck");
			deers = new Deer[5];

			for(int i = 0; i < deers.Length; i++) {
				deers[i] = Instantiate(deerPrefab).AddComponent<Deer>();
				deers[i].transform.SetParent(pool.transform);
				deers[i].transform.localPosition = Vector3.zero;
            }
		}

		private void Update() {
			
			//Move player torch
			Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));
			if (Physics.Raycast(ray, out RaycastHit hit, camera.farClipPlane)) {
				playerTorch.transform.forward = hit.point - playerTorch.transform.position;
			}

			//Update fire health over time
			if (fireHealth > 0.0f) {
				
				float fireDecay = 0.01f * Time.deltaTime;//Loose 1% of total health per second (100 seconds til death)
				fireDecay *= -1;
				fireHealth += fireDecay;
			}

			fireHealth = Mathf.Min(Mathf.Max(fireHealth, 0.0f), 1.0f);

			//Check for game over
			if (fireHealth <= 0.0f) {
				
			}
		}
	}
}