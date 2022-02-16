using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.UI {
	public class Scrollpane : MonoBehaviour {

		public enum ScrollDirection {Horizontal, Vertical};

		private new Camera camera;
		private ScrollDirection scrollDirection;

		private List<Transform> items = null;
		private List<Button> buttons = null;

		private float spacing = 100.0f;
		private float deadzone = 15.0f;
		private float overscrollDistance = 800.0f;

		public float pageLength = 0.0f;

		private float contentLength = 0.0f;

		private bool bInvertVertical = true;

		private bool bScrollLocked = false;
        private bool bScrolling = false;
		private bool bDeadzone = false;
		private bool bAnimating = false;

		public float velocity = 0.0f;
		public float step = 0.0f;
		public float scrollPosition { get; internal set; }
		private float lastScrollPosition = 0.0f;

		public float scale = 0.0f;
		private float startScrollPosition = 0.0f;
		private float lastPointerPosition = 0.0f;

		private int pointerId = -1;
		private Vector2 scrollVelocity = new Vector2();

		private void Awake() {
			buttons = new List<Button>();
			items = new List<Transform>();
		}

		public void Init(List<Transform> items, float spacing, ScrollDirection direction, float pageLength, Camera camera) {
			this.camera = camera;
			this.spacing = spacing;
			buttons.Clear();
			this.items.Clear();
			this.items.AddRange(items);
			//Debug.Log(this.items.Count);
			scrollDirection = direction;
			this.pageLength = pageLength;

			bScrolling = false;
			velocity = 0.0f;
			scrollPosition = 0.0f;
			step = 0.0f;
			bDeadzone = false;
			bAnimating = false;

			Render();
		}

		public void AddButtons(List<Button> buttons) {
			this.buttons.AddRange(buttons);
		}

		public void AddButtons(Button[] buttons) {
			this.buttons.AddRange(buttons);
		}

		private void Update() {
			if (pointerId == -1) {
				for (int i = 0; i < buttons.Count; i++) {
					if (buttons[i].pointer != null) {
						pointerId = buttons[i].pointer.pointerID;
						break;
					}
				}
			} else {
				Dugan.Input.Pointers.Pointer pointer = Dugan.Input.PointerManager.GetPointerByIndex(pointerId);
				if (pointer == null ||
					(pointer != null && 
					(pointer.state == Dugan.Input.Pointers.Pointer.ClickState.Up || 
					pointer.state == Dugan.Input.Pointers.Pointer.ClickState.Idle || 
					pointer.state == Dugan.Input.Pointers.Pointer.ClickState.Hover ||
					pointer.state == Dugan.Input.Pointers.Pointer.ClickState.Released ))) {
						
					pointerId = -1;
				}
			}

			Scroll();
			Animate();
			Render();
		}

		private void Scroll() {
			if (contentLength < pageLength)
				return;

			if (pointerId != -1) {
				Vector2 pointerPositionVector = camera.ScreenToWorldPoint(Dugan.Input.PointerManager.GetPointerByIndex(pointerId).position);
				float pointerPosition = 0.0f;

				if (scrollDirection == ScrollDirection.Horizontal)
					pointerPosition = pointerPositionVector.x;

				if (scrollDirection == ScrollDirection.Vertical)
					pointerPosition = pointerPositionVector.y;

				if (!bScrolling) {
					if (!bDeadzone) {
						startScrollPosition = pointerPosition;
						lastPointerPosition = pointerPosition;
						bDeadzone = true;
						return;
					} else {
						if (UnityEngine.Mathf.Abs(startScrollPosition - pointerPosition) > deadzone) {
							bDeadzone = false;
							bScrolling = true;
							bAnimating = true;
							for (int i = 0; i < buttons.Count; i++) {
								buttons[i].Release();
							}
						} else {
							return;
						}
					}
				}

				velocity = pointerPosition - lastPointerPosition;

				step = velocity;

				float nextPosition = scrollPosition + step;
				scale = 1.0f;

				if (nextPosition < 0.0f) {
					float overscroll = UnityEngine.Mathf.Abs(nextPosition);
					overscroll = 1.0f - UnityEngine.Mathf.Clamp01(overscroll / overscrollDistance);
					overscroll *= 0.25f;
					scale = overscroll;
				}

				if (((nextPosition - contentLength) + pageLength) > 0.0f) {
					float overscroll = UnityEngine.Mathf.Abs((nextPosition - contentLength) + pageLength);
					overscroll = 1.0f - UnityEngine.Mathf.Clamp01(overscroll / overscrollDistance);
					overscroll *= 0.25f;
					scale = overscroll;
				}

				step *= scale;

				lastPointerPosition = pointerPosition;
			} else {
				//End user input scrolling, start velocity animation
				bDeadzone = false;
				if (bScrolling) {
					bScrolling = false;
					velocity *= (1.0f / Time.unscaledDeltaTime) * scale;
					velocity *= 0.5f;
					//
				}
			}
		}

		private void Animate() {

			if (!bScrolling) {
				float deltaTime = Time.unscaledDeltaTime;
				step = velocity * deltaTime;

				float nextPosition = scrollPosition + step;
				bool bAttracting = (nextPosition < 0.0f) || (((nextPosition - contentLength) + pageLength) > 0.0f);

				if (bAnimating) {
					float scrollVelocityLerpMultiplier = 4.0f;
					if (bAttracting)
						scrollVelocityLerpMultiplier = 6.0f;

					velocity = UnityEngine.Mathf.Lerp(velocity, 0.0f, deltaTime * scrollVelocityLerpMultiplier);

					bool bScrollVelocity = UnityEngine.Mathf.Abs(velocity) >= 0.0f;

					if (bAttracting) {
						float attractPoint = 0.0f;

						if (nextPosition < 0.0f)
							attractPoint = 0.0f;
						if (((nextPosition - contentLength) + pageLength) > 0.0f)
							attractPoint = contentLength - pageLength;

						if (!bScrollVelocity && UnityEngine.Mathf.Abs(scrollPosition - attractPoint) < 0.25f) {
							step = scrollPosition - attractPoint;
							bAttracting = false;
						} else {
							scrollPosition = UnityEngine.Mathf.Lerp(scrollPosition, attractPoint, deltaTime * 6.0f);
							step += (scrollPosition - lastScrollPosition) * 0.5f;
						}
					}

					if (!bScrollVelocity && !bAttracting) {
						velocity = 0.0f;
						bAnimating = false;
					}
				}

			}

			if (contentLength < pageLength) {
				velocity = 0.0f;
				bScrolling = false;
				bAnimating = false;
				bDeadzone = false;
				step = 0.0f;
				scrollPosition = 0.0f;
			}

			lastScrollPosition = scrollPosition;
			scrollPosition += step;
		}

		private void Render() {
			//No matter what, update the position of all the children
			for (int i = 0; i < items.Count; i++) {
				if (scrollDirection == ScrollDirection.Vertical)
					items[i].position = new Vector3(transform.position.x, transform.position.y + (-i * spacing + scrollPosition), items[i].position.z);

				if (scrollDirection == ScrollDirection.Horizontal)
					items[i].position = new Vector3(transform.position.x + (-i * spacing + scrollPosition), transform.position.y, items[i].position.z);
			}

			contentLength = items.Count * spacing;
		}

		public void SetScrollPosition(float scrollPosition) {
			bScrolling = false;
			bAnimating = false;
			bDeadzone = false;
			step = 0.0f;
			this.scrollPosition = UnityEngine.Mathf.Max(UnityEngine.Mathf.Min(scrollPosition, contentLength), 0);
			Render();
		}
	}
}