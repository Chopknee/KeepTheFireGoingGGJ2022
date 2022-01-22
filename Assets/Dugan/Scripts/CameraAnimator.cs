using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Intended to be attached to the object that is being moved.

namespace Dugan {
	public class CameraAnimator : MonoBehaviour {

		public delegate void Event(CameraAnimator cameraAnimator);
		public Event OnMoveComplete;
		public Event OnMoveStart;

		private Dugan.TimeAnimation timeAnimation = null;

		private Vector3 startPosition = Vector3.zero;
		private Quaternion startLookDirection = Quaternion.identity;

		private Vector3 endPosition = Vector3.zero;
		private Quaternion endLookDirection = Quaternion.identity;

		private void Awake() {
			timeAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			timeAnimation.SetDirection(-1, true);
			timeAnimation.OnAnimationUpdate += OnAnimationUpdate;
			timeAnimation.OnAnimationComplete += OnAnimationComplete;
		}

		public void MoveToTime(Vector3 endPosition, Vector3 endLookDirection, float duration) {
			Debug.Log(endLookDirection + " " + Quaternion.Euler(endLookDirection));
			MoveToTime(endPosition, Quaternion.Euler(endLookDirection), duration);
		}

		public void MoveToTime(Vector3 endPosition, Quaternion endLookDirection, float duration) {
			if (timeAnimation.IsPlaying()) {
				//Set up to animate differently
			} else {
				ResetTimeAnimation();
				startPosition = transform.position;
				startLookDirection = transform.rotation;
				this.endPosition = endPosition;
				this.endLookDirection = endLookDirection;
				timeAnimation.SetLengthInSeconds(duration);
				timeAnimation.SetDirection(1);
				if (OnMoveStart != null)
					OnMoveStart(this);
			}
		}

		private void OnAnimationUpdate(float a) {
			a = Dugan.Mathf.Easing.EaseInOutQuart(a);
			transform.position = Vector3.Lerp(startPosition, endPosition, a);
			transform.rotation = Quaternion.Lerp(startLookDirection, endLookDirection, a);
		}

		private void OnAnimationComplete() {
			if (timeAnimation.GetDirection() == 1) {
				if (OnMoveComplete != null)
					OnMoveComplete(this);
			}
		}

		private void ResetTimeAnimation() {
			endPosition = transform.position;
			endLookDirection = transform.rotation;
			timeAnimation.SetDirection(-1, true);
		}
	}
}
