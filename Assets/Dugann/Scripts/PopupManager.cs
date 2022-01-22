using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
	public class PopupManager : MonoBehaviour {

		private static PopupManager instance = null;
		private static Vector3 managerInstantiationPosition = new Vector3(-6000.0f, 0.0f, -6000.0f);
		private static List<Popup> popups = new List<Popup>();
		
		private static int depth = 0;
		private static int startDepth = 10;

		public void Awake() {
			popups = new List<Popup>();
			depth = startDepth;
		}

		public static void Add(GameObject gameObject, bool bKeepCameraDepth = false) {
			if (instance == null) {
				instance = new GameObject("Dugan.PopupManager").AddComponent<Dugan.PopupManager>();
				instance.transform.position = managerInstantiationPosition;
			}

			gameObject.transform.SetParent(instance.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.rotation = Quaternion.identity;

			Camera[] cameras = gameObject.GetComponentsInChildren<Camera>();
			for (int i = 0; i < cameras.Length; i++) {
				if (!bKeepCameraDepth) {
					depth ++;
					cameras[i].depth = depth;
					//Debug.Log(cameras[i].name + " " + cameras[i].depth);
				}
			}
			Popup popup = new Popup(cameras, gameObject, bKeepCameraDepth);
			gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, GetDepthOffset());

			popups.Add(popup);
		}

		public static GameObject Load(string path, bool bKeepCameraDepth = false) {
			GameObject resource = Resources.Load<GameObject>(path);
			if (resource == null)
				return null;

			GameObject popupGameObject = Instantiate(resource);
			Add(popupGameObject, bKeepCameraDepth);
			return popupGameObject;
		}

		public static T Load<T>(string path, bool bKeepCameraDepth = false) where T : MonoBehaviour {
			GameObject go = Load(path, bKeepCameraDepth);
			if (go == null)
				return null;

			T component = go.GetComponent<T>();
			if (component == null) {
				component = go.AddComponent<T>();
			}
			return component;
		}

		public static T Load<T>( bool bKeepCameraDepth = false) where T : MonoBehaviour {
			System.Type t = typeof(T);
			string path = t.Namespace + "." + t.Name;
			path = path.Replace('.', '/');
			return Load<T>(path, bKeepCameraDepth);
		}

		public static void Unload(GameObject _popup) {

			Popup popupToDelete = null;
			//Attempting to associate a popup object with a popup
			if (instance != null) {
				for (int i = 0; i < popups.Count; i++) {
					if (popups[i].gameObject == _popup) {
						popupToDelete = popups[i];
						break;
					}
				}
			}

			Destroy(_popup);

			if (popupToDelete == null || instance == null)
				return;
			
			popups.Remove(popupToDelete);

			if (popups.Count == 0) {
				Destroy(instance.gameObject);
				instance = null;
				return;
			}

			float n = 0;
			depth = startDepth;
			for (int i = 0; i < popups.Count; i++) {
				if (popups[i].bKeepCameraDepth)
					continue;
					
				popups[i].gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, n);
				for (int j = 0; j < popups[i].cameras.Length; j++) {
					n += popups[i].cameras[j].farClipPlane + 100.0f;
					depth ++;
					popups[i].cameras[j].depth = depth;
					//Debug.Log(popups[i].cameras[j].name + " " + popups[i].cameras[j].depth);
				}
			}
		}

		public static int GetCurrentDepth() {
			return startDepth + depth;
		}

		private static float GetDepthOffset() {
			if (popups != null) {
				float n = 0;
				for (int i = 0; i < popups.Count; i++) {
					for (int j = 0; j < popups[i].cameras.Length; j++) {
						n += popups[i].cameras[j].farClipPlane + 100.0f;
					}
				}
				return n;
			}
			return 0;
		}

		private class Popup {
			public Camera[] cameras;
			public GameObject gameObject;
			public bool bKeepCameraDepth;

			public Popup(Camera[] _cameras, GameObject _gameObject, bool _bKeepCameraDepth) {
				cameras = _cameras;
				gameObject = _gameObject;
				bKeepCameraDepth = _bKeepCameraDepth;
			}
		}
	}
}
