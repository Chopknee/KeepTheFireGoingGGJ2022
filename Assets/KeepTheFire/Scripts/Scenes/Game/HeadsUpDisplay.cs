using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game {
    public class HeadsUpDisplay : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private UnityEngine.UI.Image imgOverlay = null;


		private void Awake() {
			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();

			imgOverlay = transform.Find("Canvas/imgOverlay").GetComponent<UnityEngine.UI.Image>();

			
		}


	}
}