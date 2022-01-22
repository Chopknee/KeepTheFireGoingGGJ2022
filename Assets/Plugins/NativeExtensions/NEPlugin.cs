using UnityEngine;

namespace Dugan.NativeExtentions {
	public class NEPlugin {

		public enum MessageTime {Short, Long};
		public static bool defaultDarkmodeSetting = false;
		public static float defaultFontScaleSetting = 1.0f;

		public delegate void DarkModeChanged(bool bIsDarkModeEnabled);
		public static DarkModeChanged onSystemDarkModeChanged;

		public delegate void FontScaleChanged(float fontScale);
		public static FontScaleChanged onSystemFontScaleChanged;

		public static Dugan.NativeExtentions.Plugin instance = null;


		public static void ShowMessage(string message, MessageTime time) {

			#if UNITY_EDITOR
			#elif UNITY_ANDROID
			Dugan.NativeExtentions.Android.Wrapper.ToastLength length = Dugan.NativeExtentions.Android.Wrapper.ToastLength.LENGTH_LONG;
			if (time == MessageTime.Short) {
				length = Dugan.NativeExtentions.Android.Wrapper.ToastLength.LENGTH_SHORT;
			}
			Dugan.NativeExtentions.Android.Wrapper.Toast(message, length);
			#endif

		}

		public static bool IsDarkmodeEnabled() {
			#if UNITY_EDITOR
			return defaultDarkmodeSetting;
			#elif UNITY_ANDROID
			return Dugan.NativeExtentions.Android.Wrapper.bDarkmodeEnabled;
			#elif UNITY_IPHONE || UNITY_IOS
			return Dugan.NativeExtentions.iOS.Wrapper.bDarkmodeEnabled;
			#else
			return defaultDarkmodeSetting;
			#endif

		}

		public static float GetSystemFontScale() {
			#if UNITY_EDITOR
			return defaultFontScaleSetting;
			#elif UNITY_ANDROID
			return Dugan.NativeExtentions.Android.Wrapper.fontScale;
			#elif UNITY_IPHONE || UNITY_IOS
			return Dugan.NativeExtentions.iOS.Wrapper.fontScale;
			#else
			return defaultFontScaleSetting;
			#endif
		}

		public static bool IsNetworkConnected() {
			#if UNITY_EDITOR
			//throw new System.NotImplementedException("Not implemented yet!");
			return true;
			#elif UNITY_ANDROID
			return Dugan.NativeExtentions.Android.Wrapper.IsNetworkAvailable();
			#elif UNITY_IPHONE || UNITY_IOS
			throw new System.NotImplementedException("Not implemented yet!");
			#else
			return true;
			#endif
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {
			#if UNITY_EDITOR
			#elif UNITY_ANDROID
			Dugan.NativeExtentions.Android.Wrapper.Init();
			#elif UNITY_IPHONE || UNITY_IOS
			Dugan.NativeExtentions.iOS.Wrapper.Init();
			#endif

		}
	}
}
