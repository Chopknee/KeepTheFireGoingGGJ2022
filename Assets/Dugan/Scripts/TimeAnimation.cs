using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
	public class TimeAnimation : MonoBehaviour {
		
		public delegate void AnimationUpdateDelegate(float a);
		public AnimationUpdateDelegate OnAnimationUpdate;

		public delegate void AnimationEvent();
		public AnimationEvent OnAnimationComplete;
		public AnimationEvent OnSetDirection;

		private float seconds = 0;

		private float direction = 1;

		private bool bPaused = false;
		private bool bComplete = false;

		private float alpha = 0;

		private bool bPlaying = false;

		public void SetPaused(bool bValue) {
			bPaused = bValue;
		}

		public bool GetPaused() {
			return bPaused;
		}

		public bool GetComplete() {
			return bComplete;
		}

		public bool IsPlaying() {
			return bPlaying;
		}

		public void SetDirection(float value, bool bInstant = false) {
			float newDir = UnityEngine.Mathf.Sign(value);

			// if (newDir == direction && bPlaying)
			// 	return;
				
			direction = newDir;
			bComplete = bInstant;
			bPlaying = true;

			if (OnSetDirection != null)
				OnSetDirection();

			// if (OnAnimationUpdate != null)
			// 	OnAnimationUpdate(alpha);

			if (bInstant) {
				if (direction == 1)
					alpha = 1;
				else if (direction == -1)
					alpha = 0;

				if (OnAnimationUpdate != null)
					OnAnimationUpdate(alpha);

				bPlaying = false;
				if (OnAnimationComplete != null) {
					OnAnimationComplete();
				}
			}
		}

		public float GetDirection() {
			return direction;
		}

		public float GetTime() {
			return alpha * seconds;
		}

		public void SetLengthInSeconds(float value) {
			seconds = value;
		}

		public float GetLengthInSeconds() {
			return seconds;
		}

		public void SetNormalizedTime(float value) {
			alpha = UnityEngine.Mathf.Clamp(value, 0, 1);
		}

		public float GetNormalizedTime() {
			return alpha;
		}

		public void Update() {
			ManualUpdate();
		}

		public void ManualUpdate() {
			if (!bPaused && !bComplete) {

				alpha += direction * (Time.deltaTime / seconds);

				float sign = UnityEngine.Mathf.Sign(direction);

				if (sign == 1) {
					if (alpha >= 1) {
						alpha = 1;
						bComplete = true;
					}
				} else if (sign == -1) {
					if (alpha <= 0) {
						alpha = 0;
						bComplete = true;
					}
				}

				if (OnAnimationUpdate != null)
					OnAnimationUpdate(alpha);

				if (bComplete) {
					bPlaying = false;
					if (OnAnimationComplete != null)
						OnAnimationComplete();
				}

			}
		}

		public static float GetNormalizedTimeInTimeSlice(float currentTime, float timeSliceStart, float timeSliceLength) {
			if (currentTime <= 0)
				return 0.0f;
			if (timeSliceLength <= 0)
				return 1.0f;

			if (timeSliceStart <= 0)
				timeSliceStart = 0.0f;

			float timeSliceCurrentTime = currentTime - timeSliceStart;
			if (timeSliceCurrentTime <= 0)
				return 0.0f;
			if (timeSliceCurrentTime >= timeSliceLength)
				return 1.0f;

			return timeSliceCurrentTime / timeSliceLength;
		}
	}
}