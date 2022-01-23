using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Scene : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private HeadsUpDisplay headsUpDisplay = null;

		private FirePit firePit = null;

		private GameObject pool = null;
		private Deer[] deers = null;

		private Light playerTorch = null;

		public Vector3 cursorHoverPosition = Vector3.zero;

		public static Scene instance = null;

		
		public float fireHealth = 0.5f;

		[HideInInspector]
		public float logStashe = 0.5f;

		private void Awake() {
			instance = this;

			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();
			camera.gameObject.AddComponent<Game.Camera>();

			GameObject hudPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/HeadsUpDisplay");
			headsUpDisplay = Instantiate(hudPrefab).AddComponent<HeadsUpDisplay>();
			headsUpDisplay.transform.position = new Vector3(0.0f, 0.0f, 1000.0f);

			playerTorch = transform.Find("Level/Player/Light").GetComponent<Light>();

			firePit = transform.Find("Level/FirePit").gameObject.AddComponent<FirePit>();

			pool = new GameObject("Pool");
			pool.transform.SetParent(transform);
			pool.transform.localPosition = new Vector3(0, -100, 0);
			GameObject deerPrefab = Resources.Load<GameObject>("KeepTheFire/Scenes/Game/Animals/Deer");
			deers = new Deer[5];

			for(int i = 0; i < deers.Length; i++) {
				deers[i] = Instantiate(deerPrefab).AddComponent<Deer>();
				deers[i].transform.SetParent(pool.transform);
				deers[i].transform.localPosition = Vector3.zero;
            }
		}

		private void Update() {
			//Debug.Log(Input.mousePosition);
			//cursorHoverPosition = 
			//cursorScreenPosition = cursorHoverPosition;
			Ray ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));
			Debug.DrawRay(cursorHoverPosition, camera.transform.forward);
			if (Physics.Raycast(ray, out RaycastHit hit, camera.farClipPlane)) {
				cursorHoverPosition = hit.point;
				playerTorch.transform.forward = hit.point - playerTorch.transform.position;
			}

			if (fireHealth > 0.0f) {
				//Update fire health over time
				float fireDecay = 0.01f * Time.deltaTime;//Loose 1% of total health per second (100 seconds til death)
				fireDecay *= -1;
				fireHealth += fireDecay;
			}

			if (fireHealth <= 0.0f) {
				//Game over!
			}
		}

		private void OnDrawGizmos() {
			Gizmos.color = Color.red;
			Gizmos.DrawCube(cursorHoverPosition, Vector3.one * 0.25f);
		}
	}
}