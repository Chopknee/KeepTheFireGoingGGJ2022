using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
    public class Screen : MonoBehaviour {

		public enum Orientation {Landscape, Portrait};
		
		public static Orientation orientation {get; private set; }

		public static Vector2 screenSizeInUnits {get; private set; }

		public static Vector2 layoutSize {
			get {
				return screenSizeInUnits * 2.0f;
			}
		}

		private int screenWidth = -1;
		private int screenHeight = -1;

		public delegate void Event();
		public static Event OnResize = null;
		public static Event OnEarlyResize = null;

		public void OnEnable() {
			//Bleh
			Dugan.PreScriptExecution.EarlyUpdate += EarlyUpdate;
			Resize();
		}

		private void OnDisable() {
			Dugan.PreScriptExecution.EarlyUpdate -= EarlyUpdate;
		}

		private void EarlyUpdate() {
			if (screenWidth != UnityEngine.Screen.width || screenHeight != UnityEngine.Screen.height)
				Resize();
		}

		private void Resize() {
			screenWidth = UnityEngine.Screen.width;
			screenHeight = UnityEngine.Screen.height;

			if (UnityEngine.Screen.width >= UnityEngine.Screen.height)
				orientation = Orientation.Landscape;
			else 
				orientation = Orientation.Portrait;

			if (referenceSize.x >= referenceSize.y) {
				if (orientation == Orientation.Landscape) {
					screenSizeInUnits = new Vector2(
						referenceSize.x + GetScreenRatioDifferenceInUnits(false),
						referenceSize.y
					);
				}
				else {
					float length = referenceSize.y;
					float ratio = (float)screenHeight / (float)screenWidth;
					screenSizeInUnits = new Vector2(
						length,
						length * ratio
					);
				}
			}
			else {
				if (orientation == Orientation.Landscape) {
					float length = referenceSize.x;
					float ratio = (float)screenWidth / (float)screenHeight;
					screenSizeInUnits = new Vector2(
						length * ratio,
						length
					);
				}
				else {
					screenSizeInUnits = new Vector2(
						referenceSize.x,
						referenceSize.y + GetScreenRatioDifferenceInUnits(false)
					);
				}
			}

			//intended for global app utilities
			if (OnEarlyResize != null)
				OnEarlyResize();
			
			//intended for layout updates
			if (OnResize != null)
				OnResize();
		}

        public static Vector2 referenceSize = new Vector2(1920.0f, 1080.0f);

        public static float minLetterboxRatio = 0.5625f;

        public static float GetScreenRatioPercentOfDefaultRatio(bool letterbox) {
            float ratio;
            if (UnityEngine.Screen.width >= UnityEngine.Screen.height)
                ratio = (float)UnityEngine.Screen.height / (float)UnityEngine.Screen.width;
            else
                ratio = (float)UnityEngine.Screen.width / (float)UnityEngine.Screen.height;

            float defaultRatio = GetDefaultRatio();
            float defaultRatioPercentOfRatio = defaultRatio / ratio;
            float letterboxPercent = defaultRatio / minLetterboxRatio;

            float p = defaultRatioPercentOfRatio;
            if (letterbox && p > letterboxPercent) {
                p = letterboxPercent;
            }
            return p;
        }

        public static float GetScreenRatioDifferenceInUnits(bool letterbox) {
            float p = GetScreenRatioPercentOfDefaultRatio(letterbox);
            float pixels = 1f - p;
            if (referenceSize.x >= referenceSize.y)
                pixels *= referenceSize.x;
            else
                pixels *= referenceSize.y;
            return -pixels;
        }

        public static float GetDefaultRatio() {
            if (referenceSize.x >= referenceSize.y)
                return referenceSize.y / referenceSize.x;
            else
                return referenceSize.x / referenceSize.y;
        }

        public static bool IsScreenPointInsideCamera(Vector3 point, Camera c) {
            if (point.x < c.pixelRect.xMin)
                return false;
            if (point.x > c.pixelRect.xMax)
                return false;
            if (point.y < c.pixelRect.yMin)
                return false;
            if (point.y > c.pixelRect.yMax)
                return false;
            return true;
        }

        public static float GetCameraFieldOfViewAsVertical(Camera c, float ratioWidth = 9.0f, float ratioHeight = 16.0f) {
            if (c == null)
                return 0.0f;
            if (ratioWidth >= ratioHeight)
                return c.fieldOfView;
            if (c.orthographic)
                return c.fieldOfView;

            float defaultFieldOfView = c.fieldOfView;
            /*
			if (defaultSize.x >= defaultSize.y)
				defaultFieldOfView *= (ratioHeight / ratioWidth) / GetDefaultRatio();
			*/
            float p = GetDefaultRatio() / (ratioWidth / ratioHeight);
            float radAngle = defaultFieldOfView * UnityEngine.Mathf.Deg2Rad;
            float radHFOV = 2.0f * UnityEngine.Mathf.Atan(UnityEngine.Mathf.Tan(radAngle * 0.5f) * c.aspect * p);
            float hFOV = UnityEngine.Mathf.Rad2Deg * radHFOV;
            return 2.0f * UnityEngine.Mathf.Rad2Deg * UnityEngine.Mathf.Atan(UnityEngine.Mathf.Tan(0.5f * UnityEngine.Mathf.Deg2Rad * hFOV) / c.aspect);
        }

		public static float GetOrthographicSize() {
			float size = 0;
			if (UnityEngine.Screen.width > UnityEngine.Screen.height)
				size = (float)UnityEngine.Screen.height * 0.5f;
			else
				size = (float)UnityEngine.Screen.width * 0.5f;

			return size;
		}

		public static float GetReferenceOrthographicSize() {
			if (referenceSize.x > referenceSize.y) {
				return referenceSize.x;
			} else {
				return referenceSize.y;
			}
		}

		private static Screen instance = null;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static Screen Instance() {
			if (instance == null) {
				instance = (new GameObject("Dugan.Screen")).AddComponent<Screen>();
				DontDestroyOnLoad(instance.gameObject);
			}
			return instance;
		}

    }
}