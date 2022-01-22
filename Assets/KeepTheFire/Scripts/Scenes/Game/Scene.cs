using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
    public class Scene : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private GameObject pool = null;
		private Deer[] deers = null;

		// private Transform playerTorch = null;
		private Light playerTorch = null;

		public Vector3 cursorHoverPosition = Vector3.zero;

		private void Awake() {

			//Bleh

			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();
			camera.gameObject.AddComponent<Game.Camera>();

			playerTorch = transform.Find("Level/Player/Light").GetComponent<Light>();

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
			cursorHoverPosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
			Ray ray = new Ray(cursorHoverPosition, camera.transform.forward);
			if (Physics.Raycast(ray, out RaycastHit hit, camera.farClipPlane)) {
				cursorHoverPosition = hit.point;
				playerTorch.transform.forward = hit.point - playerTorch.transform.position;
			}
		}

		private void OnDrawGizmos() {
			Gizmos.color = Color.red;
			Gizmos.DrawCube(cursorHoverPosition, Vector3.one * 0.25f);
		}
	}
}