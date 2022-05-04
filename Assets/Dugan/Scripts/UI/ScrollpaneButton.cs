using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.UI {
	public class ScrollpaneButton : Dugan.UI.Button {

		private bool bIsScrolling = false;

		private float holdTimeBeforeHighlighting = 0.25f;

		public void SetScrolling(bool bValue) {
			bIsScrolling = bValue;
			if (bIsScrolling)
				bSelected = false;
		}

		public bool GetScrolling() {
			return bIsScrolling;
		}

		protected override void OnPointerUpInternal() {
			//If we scrolling, don't fire events.
			if (bIsScrolling) {
				bIsScrolling = false;
				return;
			}

			base.OnPointerUpInternal();
		}

		private float overTime = 0.0f;
		private bool bUpdatedHighlight = false;

		protected override void Update() {
			base.Update();

			if (bSelected) {
				if (!bUpdatedHighlight) {
					overTime += Time.unscaledDeltaTime;
					if (overTime >= holdTimeBeforeHighlighting) {
						bUpdatedHighlight = true;
						UpdateHighlight();
					}
				}	
			} else {
				overTime = 0.0f;
				bUpdatedHighlight = false;
			}
		}

		protected override void UpdateHighlight() {
			if (bSelected && base.GetPointerOver() && bInteractive) {
				if (!bUpdatedHighlight)
					return;
				//Make everything go dark
				for (int i = 0; i < buttonParts.Count; i++) {
					buttonParts[i].SetTint(heldColorMultiplier);
					
				}
			} else {
				//Make everything go light
				for (int i = 0; i < buttonParts.Count; i++) {
					buttonParts[i].SetTint(0.0f);
				}
			}
		}

	}
}