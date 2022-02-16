using UnityEngine;

namespace Dugan.Input.Pointers {
	public class MousePointer {
		//For now, there isn't any need to add custom code to the base pointer class. Just static functions for dealing with the cursor updates.

		//Static Functions
		public static Pointer mousePointer = null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {//Only add a mouse pointer if unity can detect a mouse
			if (UnityEngine.Input.mousePresent)
				PointerManager.AddPointerUpdateEvent(UpdateMousePointer);

			mousePointer = new Pointer();
			mousePointer.active = true;
			PointerManager.AddPointer(mousePointer);
		}

		private static int UpdateMousePointer() {
			if (mousePointer.state == Pointer.ClickState.Released && UnityEngine.Input.GetMouseButtonUp(0))
				mousePointer.state = Pointer.ClickState.Hover;//

			mousePointer.state = Pointer.ClickState.Hover;//Hover is default state for mouse cursor.

			if (UnityEngine.Input.GetMouseButtonDown(0))
				mousePointer.state = Pointer.ClickState.Down;
			else if (UnityEngine.Input.GetMouseButtonUp(0))
				mousePointer.state = Pointer.ClickState.Up;
			else if (UnityEngine.Input.GetMouseButton(0))
				mousePointer.state = Pointer.ClickState.Held;

			mousePointer.Update(UnityEngine.Input.mousePosition);

			return 1;
		}
	}
}
