using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan.Mathf {
	public class Vector : MonoBehaviour {

		public static float floatingPointCutoff = 0.0001f;

		public static bool Changed(Vector3 previous, Vector3 current) {
			return Changed(previous.x, current.x) || Changed(previous.y, current.y) || Changed(previous.z, current.z);
		}

		public static bool Changed(Vector2 previous, Vector2 current) {
			return Changed(previous.x, current.x) || Changed(previous.y, current.y);
		}

		private static bool Changed(float previous, float current) {
			return UnityEngine.Mathf.Abs(current - previous) > floatingPointCutoff;
		}

		public static UnityEngine.Vector2 Sign(UnityEngine.Vector2 vector) {
			return new UnityEngine.Vector2(UnityEngine.Mathf.Sign(vector.x), UnityEngine.Mathf.Sign(vector.x));
		}

		public static UnityEngine.Vector2 Multiply(UnityEngine.Vector2 a, UnityEngine.Vector2 b) {
			return new UnityEngine.Vector2(a.x * b.x, a.y * b.y);
		}

		public static UnityEngine.Vector2 Abs(UnityEngine.Vector2 vector) {
			return new UnityEngine.Vector2(UnityEngine.Mathf.Abs(vector.x), UnityEngine.Mathf.Abs(vector.y));
		}
	}
}
