using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.UI {
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(BoxCollider))]
	public class BoxColliderHelper : MonoBehaviour {

		private RectTransform rect = null;
		private Vector2 rectSizeDelta = Vector2.zero;
		private Vector2 lastSizeDelta = Vector2.zero;
		private BoxCollider boxCollider = null;

		private void Awake() {
			rect = GetComponent<RectTransform>();
			boxCollider = GetComponent<BoxCollider>();
			CenterAndSizeBoxCollider();

			enabled = rect != null && boxCollider != null;//Disables component update if no box collider or rect is in place.
		}

		private void Update() {
			if (boxCollider == null || rect == null)
				return;
			//Doing this for the case when the rect is set to stretch mode.
			rectSizeDelta.x = rect.rect.width;
			rectSizeDelta.y = rect.rect.height;

			if (Dugan.Mathf.Vector.Changed(lastSizeDelta, rectSizeDelta)) {
				CenterAndSizeBoxCollider();
			}
		}

		private void CenterAndSizeBoxCollider() {
			if (boxCollider == null || rect == null)
				return;
			boxCollider.size = new Vector3(rectSizeDelta.x, rectSizeDelta.y, boxCollider.size.z);

			Vector2 boxColliderCenter = (Vector2.one * 0.5f) - rect.pivot;
			boxColliderCenter = new Vector2(boxColliderCenter.x * rectSizeDelta.x, boxColliderCenter.y * rectSizeDelta.y);
			boxCollider.center = boxColliderCenter;

			lastSizeDelta = rectSizeDelta;
		}
	}
}
