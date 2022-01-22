using UnityEngine;
using System.Collections.Generic;

namespace Dugan.Input.Pointers {
	public class TouchPointer : Dugan.Input.Pointers.Pointer {
		
		public int fingerID = -1;

		//Static functions for handling the updates of each pointer.
		public static int pointerPreAllocationCount = 10;
		public static List<TouchPointer> touchPointers = null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			PointerManager.AddPointerUpdateEvent(UpdateTouchPointers);
			touchPointers = new List<TouchPointer>();
			for (int i = 0; i < pointerPreAllocationCount; i++) {
				touchPointers.Add(new TouchPointer());
				touchPointers[i].Reset();
				PointerManager.AddPointer(touchPointers[i]);
			}
		}

		public static TouchPointer GetTouchPointerByFingerID(int fingerID) {
			Debug.Log("Getting touch pointer.");
			for (int i = 0; i < touchPointers.Count; i++) {
				if (touchPointers[i].fingerID == fingerID)
					return touchPointers[i];
			}
			return null;
		}

		private static TouchPointer GetOrAllocateTouchPointer(int fingerID) {
			TouchPointer tp = GetTouchPointerByFingerID(fingerID);
			if (tp == null) {
				Debug.Log("Touch pointer not found, assigning touch pointer.");
				//Allocate a new touch pointer
				for (int i = 0; i < touchPointers.Count; i++) {
					if (touchPointers[i].active == false) {
						tp = touchPointers[i];
						tp.fingerID = fingerID;
						tp.active = true;
						break;
					}
				}

				if (tp == null) {//This caches a new touch. This should only happen if the screen in question supports more than 10 touches.
					Debug.Log("No inactive touch pointer found, caching new touch pointer.");
					tp = new TouchPointer();
					tp.fingerID = fingerID;
					touchPointers.Add(tp);
					PointerManager.AddPointer(tp);
					tp.active = true;
				}
			}
			return tp;
		}

		private static int UpdateTouchPointers() {
			//Clear the states of dead touches
			for (int i = 0; i < touchPointers.Count; i++) {
				if (touchPointers[i].clickState == ClickState.Up) {
					touchPointers[i].active = false;
					touchPointers[i].fingerID = -1;
					touchPointers[i].clickState = ClickState.Idle;
				}
			}

			//Update touches with active touch data
			for (int i = 0; i < UnityEngine.Input.touchCount; i++) {
				UnityEngine.Touch touch = UnityEngine.Input.GetTouch(i);
				TouchPointer tp = GetOrAllocateTouchPointer(touch.fingerId);
				tp.Update(touch.position);
				if (tp.clickState == ClickState.Down && touch.phase != TouchPhase.Ended)
					tp.clickState = ClickState.Held;
					
				if (touch.phase == TouchPhase.Began)
					tp.clickState = ClickState.Down;
				else if (touch.phase == TouchPhase.Ended)
					tp.clickState = ClickState.Up;
			}

			return 0;
		}
	}
}
