using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KeepTheFire.Scenes.Game {
    public class HeadsUpDisplay : MonoBehaviour {

		private new UnityEngine.Camera camera = null;

		private UnityEngine.UI.Image imgVingette = null;

		private Dugan.UI.Button btnMenu = null;


		private void Awake() {
			camera = transform.Find("Camera").GetComponent<UnityEngine.Camera>();

			imgVingette = transform.Find("Canvas/ImgVingette").GetComponent<UnityEngine.UI.Image>();

			btnMenu = transform.Find("Canvas/BtnMenu").gameObject.AddComponent<Dugan.UI.Button>();
			btnMenu.OnClicked += OnClickBtnMenu;

			
		}

		private void OnClickBtnMenu(Dugan.Input.PointerTarget pointerTarget, string args) {
			Debug.Log("CLICKED THE DANG BUTTON!");
		}


	}
}