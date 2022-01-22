using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
	public class PreScriptExecution : MonoBehaviour {

		public delegate void Event();
		public static Event EarlyUpdate;

		private void Update() {
			
			if (EarlyUpdate != null)
				EarlyUpdate();
		}

		private static PreScriptExecution instance = null;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static PreScriptExecution Instance() {
			if (instance == null) {
				instance = new GameObject("Dugan.PreScriptExecution").AddComponent<PreScriptExecution>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
}
