using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.Input {
	public class PointerTarget : MonoBehaviour {

		public delegate void Event(Dugan.Input.PointerTarget target, string args);
		public Event OnPointerDown = null;
		public string OnPointerDownArgs = "";

		public Event OnPointerUp = null;
		public string OnPointerUpArgs = "";

		public Event OnPointerHeld = null;
		public string OnPointerHeldArgs = "";

		public Event OnPointerEnter;
		public string OnPointerEnterArgs = "";

		public Event OnPointerExit;
		public string OnPointerExitArgs = "";

		public Dugan.Input.Pointers.Pointer pointer = null;//Whichever pointer is actively interacting with me

		private List<Dugan.Input.Pointers.Pointer> myPointers = new List<Dugan.Input.Pointers.Pointer>();

		private bool bLastPointerOver = false;

		private bool bPointerDown = false;

		public virtual void UpdateTarget(Dugan.Input.Pointers.Pointer _pointer) {
			myPointers.Add(_pointer);//All we do is cache each pointer over this object.
		}

		protected virtual void Update() {
			bPointerDown = false;
			bool bOver = myPointers.Count > 0;//If we currently have a pointer hovering.
			bool bGrabbedPointer = false;//If we assigned a new pointer this update

			if (pointer == null) {
				for (int i = 0; i < myPointers.Count; i++) {
					//Figure out if any of the pointers are interacting.
					Dugan.Input.Pointers.Pointer _pointer = myPointers[i];
					if (_pointer.clickState == Dugan.Input.Pointers.Pointer.ClickState.Down) {
						pointer = _pointer;
						bGrabbedPointer = true;
						break;
					}
				}

			}

			if (pointer == null) {
				//Run the enter and exits as normal.
				if (bLastPointerOver != bOver) {
					bLastPointerOver = bOver;
					if (bLastPointerOver)
						OnPointerEnterInternal();
					else
						OnPointerExitInternal();
				}
			} else {
				if (bLastPointerOver != bOver) {
					bLastPointerOver = bOver;
					if (bLastPointerOver)
						OnPointerEnterInternal();
					else
						OnPointerExitInternal();
				}

				if (bGrabbedPointer) {
					OnPointerDownInternal();
				} else {
					if (pointer.clickState == Dugan.Input.Pointers.Pointer.ClickState.Up) {
						OnPointerUpInternal();
						pointer.pointerTarget = null;
						pointer = null;
					} else {
						bPointerDown = true;
						OnPointerHeldInternal();
					}
				}

			}

			myPointers.Clear();
		}

		public bool GetPointerOver() {
			return bLastPointerOver;
		}

		public bool GetPointerDown() {
			return bPointerDown;
		}

		protected virtual void OnPointerEnterInternal() {
			if (OnPointerEnter != null)
				OnPointerEnter(this, OnPointerEnterArgs);
		}

		protected virtual void OnPointerExitInternal() {
			if (OnPointerExit != null)
				OnPointerExit(this, OnPointerExitArgs);
		}

		protected virtual void OnPointerDownInternal() {
			if (OnPointerDown != null)
				OnPointerDown(this, OnPointerDownArgs);
		}

		protected virtual void OnPointerHeldInternal() {
			if (OnPointerHeld != null)
				OnPointerHeld(this, OnPointerHeldArgs);
		}

		protected virtual void OnPointerUpInternal() {
			if (OnPointerUp != null)
				OnPointerUp(this, OnPointerUpArgs);
		}

	}
}