using UnityEngine;
using System.Collections;

namespace Dugan.NativeExtentions.Android {
	public class Wrapper : MonoBehaviour {
		#if UNITY_ANDROID && !UNITY_EDITOR

		public enum ToastLength {LENGTH_LONG=1, LENGTH_SHORT=0};

		/**
			Sends an android toast message.
			This should never go in an update function. It GC allocs 2kb/frame for the native java conversions.
		**/
		public static void Toast(string message, ToastLength length) {
			try {
				object[] toastParams = new object[3];
				toastParams[0] = GetAppActivity();
				toastParams[1] = message;
				toastParams[2] = (int)length;
				AndroidJavaObject toastObject = GetToastClass().CallStatic<AndroidJavaObject>("makeText", toastParams);
				toastObject.Call("show");
			} catch {}
		}

		/**
			Gets the current user setting for darkmode. If not supported (for older android versions) a configurable default is returned.
			This should never go in an update function. It GC allocs 2kb/frame for the native java conversions.
		**/
		public static bool bDarkmodeEnabled { get; private set; }
		private static void GetDarkmodeEnabled() {
			try {
				bool mode = GetFunctionsClass().CallStatic<bool>("IsDarkThemeOn", GetAppActivity());
				if (bDarkmodeEnabled != mode) {
					bDarkmodeEnabled = mode;
					if (Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged != null)
						Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged(bDarkmodeEnabled);
				}
			} catch {
				bDarkmodeEnabled = Dugan.NativeExtentions.Plugin.defaultDarkmodeSetting;
			}
		}

		public static float fontScale {get; private set;}
		private static void GetSystemFontScale() {
			try {
				float scale = GetFunctionsClass().CallStatic<float>("GetFontScale", GetAppActivity());
				if (fontScale != scale) {
					fontScale = scale;
					if (Dugan.NativeExtentions.Plugin.onSystemFontScaleChanged != null)
						Dugan.NativeExtentions.Plugin.onSystemFontScaleChanged(fontScale);
				}
			} catch {
				fontScale = Dugan.NativeExtentions.Plugin.defaultFontScaleSetting;
			}
		}

		public static bool IsNetworkAvailable() {
			try {
				return GetFunctionsClass().CallStatic<bool>("IsNetworkAvailable", GetAppActivity());
			} catch {
				return false;
			}
		}

		private void OnApplicationFocus(bool hasFocus) {
			GetDarkmodeEnabled();
			GetSystemFontScale();
		}

		//Instance specific functions here
		private static Dugan.NativeExtentions.Android.Wrapper instance = null;

		public static void Init() {
			if (instance != null)
				return;
			
			GameObject go = new GameObject("Dugan.NativeExtensions.Android.Wrapper");
			instance = go.AddComponent<Dugan.NativeExtentions.Android.Wrapper>();
		}

		private void Awake() {
			//Force the values to update for start of app.
			OnApplicationFocus(false);
		}


		//REQUIRED JAVA CLASSES


		private static AndroidJavaClass appClass = null;
		private static AndroidJavaClass GetAppClass() {
			if (appClass == null)
				appClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

			return appClass;
		}

		private static AndroidJavaObject appActivity = null;
		private static AndroidJavaObject GetAppActivity() {
			if (appActivity == null)
				appActivity = GetAppClass().GetStatic<AndroidJavaObject>("currentActivity");

			return appActivity;
		}

		private static AndroidJavaClass darkModeClass = null;
		private static AndroidJavaClass GetFunctionsClass() {
			if (darkModeClass == null)
				darkModeClass = new AndroidJavaClass("com.dugan.nativeextentions.StaticFunctions");
			
			return darkModeClass;
		}

		private static AndroidJavaClass toastClass = null;
		private static AndroidJavaClass GetToastClass() {
			if (toastClass == null)
				toastClass = new AndroidJavaClass("android.widget.Toast");
			return toastClass;
		}

		#endif
	}
}
