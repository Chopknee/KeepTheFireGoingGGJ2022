using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
	public class Rotator3D : MonoBehaviour {

		public delegate void EventDelegate(Rotator3D sender);
		public EventDelegate OnDragBegin;//The moment the user grabs the object
		public EventDelegate OnDragEnd;//The moment the user lets go of the object (no matter if they let go over the collider or not)
		public EventDelegate OnRotationEnd;//The moment rotational velocity has been arrested
		
		
		private float rotationalDrag = 4.0f;
		private Dugan.Input.Pointers.Pointer dragPointer = null;
		private Vector2 rotationalVelocity = Vector2.zero;
		private float rotationalSpeed = 0.0f;

		private float rotationalSpeedCutoff = 0.0001f;

		private Dugan.Input.PointerTarget pointerTarget = null;

		private float panMultiplier = 0.25f;

		private Transform targetTransform = null;

		public bool bDragging = false;
		public bool bLastDragging = false;
		public bool bRotating = false;

		private void Awake() {
		}

		public void Init(Dugan.Input.PointerTarget pointerTarget, Transform target) {
			this.pointerTarget = pointerTarget;
			this.targetTransform = target;
		}

		private void Update() {
			dragPointer = pointerTarget.pointer;
			if (dragPointer != null) {
				Vector2 dp = dragPointer.lastPosition - dragPointer.position;
				float multiplier = (1080.0f / Dugan.Screen.GetOrthographicSize());
				rotationalVelocity = dp * multiplier * panMultiplier;
			}

			rotationalSpeed = rotationalVelocity.magnitude;
			Quaternion inverse = Quaternion.Inverse(targetTransform.rotation);
			targetTransform.rotation *= Quaternion.AngleAxis(-rotationalVelocity.y, inverse * Vector3.right);
			targetTransform.rotation *= Quaternion.AngleAxis(rotationalVelocity.x, inverse * Vector3.up);

			rotationalVelocity = Vector2.Lerp(rotationalVelocity, Vector2.zero, rotationalDrag * Time.deltaTime);
			
			if (rotationalSpeed < rotationalSpeedCutoff) {//Cut off stupidly small rotation speeds.
				rotationalVelocity = Vector3.zero;
				rotationalSpeed = 0.0f;
			}

			bDragging = dragPointer != null && dragPointer.state == Dugan.Input.Pointers.Pointer.ClickState.Held;

			if (bLastDragging != bDragging) {
				if (bDragging && !bRotating) {
					if (OnDragBegin != null)
						OnDragBegin(this);

					bRotating = true;
				} else if (!bDragging) {
					if (OnDragEnd != null)
						OnDragEnd(this);
				}

				bLastDragging = bDragging;
			}

			if (!bDragging && bRotating && rotationalSpeed == 0.0f) {
				if (OnRotationEnd != null)
					OnRotationEnd(this);

				bRotating = false;
			}
		}

		public Vector2 GetRotationalVelocity() {
			return rotationalVelocity;
		}

		public float GetRotationalSpeed() {
			return rotationalSpeed;
		}

		public void SetRotationalDrag(float value) {
			rotationalDrag = value;
		}

		public float GetRotationalDrag() {
			return rotationalDrag;
		}

		public void SetPanMultiplier(float value) {
			panMultiplier = value;
		}

		public float GetPanMultiplier() {
			return panMultiplier;
		}

		public void SetRotationalSpeedCutoff(float value) {
			rotationalSpeedCutoff = value;
		}

		public bool GetDragging() {
			return bDragging;
		}

		public bool GetRotating() {
			return bRotating;
		}

	}
}