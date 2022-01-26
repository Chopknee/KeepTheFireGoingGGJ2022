using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepTheFire.Scenes.Game {
	public class TorchCollider : MonoBehaviour {
		
		private void Awake() {
			
		}

		private void OnTriggerEnter(Collider collider) {
			collider.gameObject.SendMessage("OnFlashlightHovered", null, SendMessageOptions.DontRequireReceiver);
		}

		private void OnTriggerExit(Collider collider) {
			collider.gameObject.SendMessage("OnFlashlightLeft", null, SendMessageOptions.DontRequireReceiver);
		}

	}
}