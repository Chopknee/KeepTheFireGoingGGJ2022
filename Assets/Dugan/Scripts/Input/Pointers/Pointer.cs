using UnityEngine;

namespace Dugan.Input.Pointers {
	public class Pointer {
		//Class which gives the ability to specify custom pointer sources.
			//Example; a virtual cursor driven by Gamepad inputs
		
		public enum ClickState { Idle, Down, Held, Up, Hover }

		public Vector2 position { get; private set; }
		public Vector2 lastPosition {get; private set; }
		public int pointerID = -1;
		
		public ClickState clickState = ClickState.Idle;
		public PointerTarget pointerTarget = null;
		public bool active = false;

		public void Update(Vector2 position) {
			lastPosition = this.position;
			this.position = position;
		}

		public void Reset() {
			position = new Vector2();
			lastPosition = new Vector2();
			clickState = ClickState.Idle;
			active = false;
			pointerTarget = null;
		}
	}
}
