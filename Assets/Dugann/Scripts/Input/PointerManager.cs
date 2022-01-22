using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.Input {
    public class PointerManager : MonoBehaviour {

        private static PointerManager instance = null;

        private static List<Pointers.Pointer> pointers = null;

		public static int pointerCount { get; private set;}//If a mouse is connected, this always returns 1
		public static bool bAnyPointerDown { get; private set;}
		public static bool bAnyPointerHeld { get; private set;}
		public static bool bAnyPointerUp { get; private set;}

		public delegate int UpdatePointerManager();
		private static List<UpdatePointerManager> pointerManagerUpdates = new List<UpdatePointerManager>();

		public static void AddPointerUpdateEvent(UpdatePointerManager manager) {
			pointerManagerUpdates.Add(manager);
		}

		public static void RemovePointerUpdateEvent(UpdatePointerManager manager) {
			pointerManagerUpdates.Remove(manager);
		}

        public static PointerManager Instance() {
            return instance;
        }

		public static int GetPointerCacheCount() {
			return pointers.Count;
		}

        public static Pointers.Pointer GetPointer(int index) {
            if (index < pointers.Count)
                return pointers[index];

            return null;
        }

        public void ManualUpdate() {
			//Run each of the registered pointer managers.
			pointerCount = 0;
			for (int i = 0; i < pointerManagerUpdates.Count; i++) {
				if (pointerManagerUpdates[i] != null)
					pointerCount += pointerManagerUpdates[i]();
			}

			bAnyPointerDown = false;
			bAnyPointerHeld = false;
			bAnyPointerUp = false;
			
			if (pointers == null)
				return;

			for (int i = 0; i < pointers.Count; i++) {
				bAnyPointerDown = bAnyPointerDown | (pointers[i].active && pointers[i].clickState == Dugan.Input.Pointers.Pointer.ClickState.Down);
				bAnyPointerHeld = bAnyPointerHeld | (pointers[i].active && pointers[i].clickState == Dugan.Input.Pointers.Pointer.ClickState.Held);
				bAnyPointerUp = bAnyPointerUp | (pointers[i].active && pointers[i].clickState == Dugan.Input.Pointers.Pointer.ClickState.Up);
			}
        }

        public static int AddPointer(Pointers.Pointer pointer) {//Grants an ID to a pointer manager
			if (pointers == null)
				pointers = new List<Pointers.Pointer>();
			
			pointer.pointerID = pointers.Count;
			pointers.Add(pointer);
            return pointer.pointerID;
        }

		public static void RemovePointer(Pointers.Pointer pointer) {
			if (pointers == null)
				pointers = new List<Pointers.Pointer>();
			
			pointers.Remove(pointer);
			for (int i = 0; i < pointers.Count; i++) {
				pointers[i].pointerID = i;
			}
		}

		public static void ReleaseAllPointers() {//Kills all active pointers.
		}

		public static Vector2 GetConvertedMouseCoordinates(Vector2 _dpiCoords, bool bLetterbox) {
			return _dpiCoords * Dugan.Screen.GetScreenRatioPercentOfDefaultRatio(bLetterbox);

		}
    }
}
