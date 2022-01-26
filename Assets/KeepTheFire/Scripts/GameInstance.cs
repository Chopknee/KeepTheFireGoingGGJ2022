using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire {
	public class AppInstance : MonoBehaviour {
		private static AppInstance instance = null;

		public static bool bDontSaveEdits = false;

		public void Awake() {
			//Do some stuff to set the game up!

			Application.targetFrameRate = 60;
			Dugan.Screen.referenceSize = new Vector2(1920.0f, 1080.0f);
			Dugan.Input.Raycaster.queryTriggerInteraction = QueryTriggerInteraction.Ignore;

		}


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {

			if (instance != null)
				return;

			GameObject inputManager = new GameObject("PasswordManager.AppInstance");
			instance = inputManager.AddComponent<AppInstance>();

			DontDestroyOnLoad(inputManager);
		}
	}
}