using System.Runtime.InteropServices;
using UnityEngine;

namespace Dugan.NativeExtentions.iOS {
	public class Wrapper : MonoBehaviour {
		#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern bool _IsDarkModeSet();
		[DllImport("__Internal")]
		private static extern float _GetSystemFontScale();

		public static bool bDarkmodeEnabled {get; private set;}
		private static void GetDarkmodeEnabled() {
			bool mode = _IsDarkModeSet();
			if (bDarkmodeEnabled != mode) {
				bDarkmodeEnabled = mode;
				if (Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged != null)
						Dugan.NativeExtentions.Plugin.onSystemDarkModeChanged(bDarkmodeEnabled);
			}
		}

		public static float fontScale {get; private set;}
		private static void GetSystemFontScale() {
			float scale = _GetSystemFontScale();
			if (fontScale != scale) {
				fontScale = scale;
				if (Dugan.NativeExtentions.Plugin.onSystemFontScaleChanged != null)
						Dugan.NativeExtentions.Plugin.onSystemFontScaleChanged(fontScale);
			}
		}


		private void OnApplicationFocus(bool hasFocus) {
			GetDarkmodeEnabled();
			GetSystemFontScale();
		}

		//Instance specific functions here
		private static Dugan.NativeExtentions.iOS.Wrapper instance = null;

		public static void Init() {
			if (instance != null)
				return;
			
			GameObject go = new GameObject("Dugan.NativeExtensions.iOS.Wrapper");
			instance = go.AddComponent<Dugan.NativeExtentions.iOS.Wrapper>();
		}
		#endif
	}
}
