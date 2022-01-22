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
		private float deadzone = 90.0f;
		public float viewLength = 0.0f;

		public bool bInvertVertical = true;

		public bool scrollLocked = false;
        public bool scrolling = false;

		public float scrollPosition = 0.0f;
		public float lastScrollPosition = 0.0f;
		public float overscroll = 0.0f;

		public Button draggingButton = null;
		public Vector2 pointerDragStart = new Vector2();
		public Vector2 scrollVelocity = new Vector2();

		//public float paneLength = 0.0f;

		public float lastDelta = 0.0f;

		private float overscrollSquish = 4000.0f;
		private float overscrollMovement = 500.0f;
		private float startVelocity = 0.0f;

		private float overscrollStart = 0.0f;

		public float totalLength = 0.0f;

		private void Awake() {
			buttons = new List<Button>();
			items = new List<Transform>();
		}

		public void Init(List<Transform> items, float spacing, ScrollDirection direction, float viewLength, Camera camera) {
			this.camera = camera;
			this.spacing = spacing;
			buttons.Clear();
			draggingButton = null;
			scrollLocked = false;
			this.items.Clear();
			this.items.AddRange(items);
			//Debug.Log(this.items.Count);
			scrollDirection = direction;
			this.viewLength = viewLength;

			for (int i = 0; i < this.items.Count; i++) {
				if (scrollDirection == ScrollDirection.Horizontal)
					items[i].transform.localPosition = new Vector3(i * spacing, 0.0f, items[i].transform.localPosition.z);
				else if (scrollDirection == ScrollDirection.Vertical)
					items[i].transform.localPosition = new Vector3(0.0f,- i * spacing, items[i].transform.localPosition.z);
			}
		}

		public void AddButtons(List<Button> buttons) {
			this.buttons.AddRange(buttons);
		}

		public void AddButtons(Button[] buttons) {
			this.buttons.AddRange(buttons);
		}

		private void Update() {
			if (draggingButton != null && draggingButton.pointer != null) {

			}

			//No matter what, update the position of all the children
			for (int i = 0; i < items.Count; i++) {
				if (scrollDirection == ScrollDirection.Vertical)
					items[i].position = new Vector3(transform.position.x, transform.position.y + (-i * spacing + scrollPosition), items[i].position.z);

				if (scrollDirection == ScrollDirection.Horizontal)
					items[i].position = new Vector3(transform.position.x + (-i * spacing + scrollPosition), transform.position.y, items[i].position.z);
			}
		}

		private void LateUpdate() {

			// if (items.Count * spacing <= length)
			// 	return;

			totalLength = (items.Count * spacing) - viewLength;

			if (draggingButton != null && (totalLength > viewLength)) {//If we have a button
				if (!draggingButton.GetPointerDown() || draggingButton.pointer == null) {
					draggingButton = null;
					lastScrollPosition = scrollPosition;
					scrollLocked = false;
                    scrolling = false;
                    if (draggingButton is Dugan.UI.ScrollpaneButton)
                        (draggingButton as Dugan.UI.ScrollpaneButton).SetScrolling(false);
                    return;
				}

                if (scrolling) {
                    Vector2 pointerWorldPos = camera.ScreenToWorldPoint(draggingButton.pointer.position);
                    Vector2 pointerLastWorldPos = camera.ScreenToWorldPoint(draggingButton.pointer.lastPosition);

                    Vector2 relative = pointerWorldPos - pointerDragStart;
                    scrollVelocity = pointerWorldPos - pointerLastWorldPos;

					float delta = 0.0f;
                    if (scrollDirection == ScrollDirection.Vertical)
						delta = relative.y;
                    else if (scrollDirection == ScrollDirection.Horizontal)
						delta = relative.x;

					float realScrollPosition = lastScrollPosition + delta;

					if (realScrollPosition <= 0.0f) {
						overscroll = UnityEngine.Mathf.Clamp01(((UnityEngine.Mathf.Abs(delta) - UnityEngine.Mathf.Abs(lastScrollPosition)) / overscrollSquish));
						overscroll = Dugan.Mathf.Easing.EaseOutQuart(overscroll);
						scrollPosition = -overscrollMovement * overscroll;
					} else if (realScrollPosition >= totalLength) {
						overscroll = UnityEngine.Mathf.Clamp01(((lastScrollPosition - totalLength) + delta) / overscrollSquish);
						overscroll = Dugan.Mathf.Easing.EaseOutQuart(overscroll);
						scrollPosition = totalLength + overscrollMovement * overscroll;
					} else {
						scrollPosition = lastScrollPosition + delta;
					}
					lastDelta = delta;
                    //scrollPosition = UnityEngine.Mathf.Clamp(scrollPosition, 0, (items.Count * spacing) - length);
                } else {
                    //Looking to get past the deadzone?
                    Vector2 pointerWorldPos = camera.ScreenToWorldPoint(draggingButton.pointer.position);
                    if ((pointerWorldPos - pointerDragStart).magnitude > deadzone) {
                        scrolling = true;
                        pointerDragStart = pointerWorldPos;
                        if (draggingButton is Dugan.UI.ScrollpaneButton)
                            (draggingButton as Dugan.UI.ScrollpaneButton).SetScrolling(true);
                    }
                }
			} else {
				for (int i = 0; i < buttons.Count; i++) {
					Button b = this.buttons[i];
					if (b.GetPointerDown() && b.pointer != null) {
						draggingButton = b;
						Vector2 pointerWorldPos = camera.ScreenToWorldPoint(draggingButton.pointer.position);
						pointerDragStart = pointerWorldPos;
						scrollLocked = true;
						break;
					}
				}
			}

			if (!scrollLocked) {

				if (totalLength < viewLength)
					totalLength = 0.0f;//items.Count * spacing;

				//Apply some "friction"
				float vel = 0.0f;
				//Apply the scroll velocity to position
				if (scrollDirection == ScrollDirection.Vertical)
					vel = scrollVelocity.y;
				if (scrollDirection == ScrollDirection.Horizontal)
					vel = scrollVelocity.x;

				if (scrollPosition > totalLength) {
					//Gravitate toward the total length position
					float delta = scrollPosition - totalLength;//Distance over the length we are
					float squish = delta / (overscrollMovement * 0.4f);
					vel = -delta * (squish * 0.1f);
					vel = UnityEngine.Mathf.Max(vel, -1000);
					vel = UnityEngine.Mathf.Min(vel, -1);
					if (delta < 2.0f) {
						scrollPosition = totalLength;
						vel = 0.0f;
					}
					//Debug.Log("Overscroll");
				} else if (scrollPosition < 0.0f) {
					float delta = UnityEngine.Mathf.Abs(scrollPosition);//Distance over the length we are
					float squish = delta / (overscrollMovement * 0.4f);
					vel = delta * (squish * 0.1f);
					vel = UnityEngine.Mathf.Min(vel, 1000);
					vel = UnityEngine.Mathf.Max(vel, 1);
					if (delta < 2.0f) {
						scrollPosition = 0.0f;
						vel = 0.0f;
					}
				} else {
					vel = UnityEngine.Mathf.Lerp(vel, 0.0f, 4.0f * Time.deltaTime);
					if (UnityEngine.Mathf.Abs(vel) < 0.25f)
						vel = 0.0f;

					startVelocity = vel;
				}

				//Preventing super tiny value propogation


				// scrollPosition = UnityEngine.Mathf.Clamp(scrollPosition, 0, (items.Count * spacing) - length);
				// if (scrollPosition == 0.0f || scrollPosition >= (items.Count * spacing) - length) {
				// 	scrollVelocity = Vector3.zero;
				// }

				scrollPosition += vel;
				lastScrollPosition = scrollPosition;

				if (scrollDirection == ScrollDirection.Vertical)
					scrollVelocity.y = vel;
				if (scrollDirection == ScrollDirection.Horizontal)
					scrollVelocity.x = vel;
			}
		}
	}
}