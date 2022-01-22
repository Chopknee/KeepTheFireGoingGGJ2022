using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
	public class Filef : MonoBehaviour {
		
		public static void WriteFileInData(string localUrl, byte[] bytes) {
			string url = GetDataUrl(localUrl, true);

			System.IO.FileStream file = System.IO.File.Create(url);
			file.Write(bytes, 0, bytes.Length);
			file.Close();

			#if !UNITY_EDITOR && UNITY_IPHONE
				UnityEngine.iOS.Device.SetNoBackupFlag(url);
			#endif
		}

		public static void DeleteFileInData(string localUrl) {
			string url = GetDataUrl(localUrl);

			try {
				System.IO.File.Delete(url);

				#if !UNITY_EDITOR && UNITY_IPHONE
					UnityEngine.iOS.Device.SetNoBackupFlag(url);
				#endif
			} catch {}
		}

		public static byte[] LoadFileFromDataAsBytes(string localUrl) {
			string url = GetDataUrl(localUrl);
			byte[] bytes;

			try {
				bytes = System.IO.File.ReadAllBytes(url);
			} catch { bytes = new byte[0]; }

			return bytes;
		}

		public static byte[] LoadFileFromStreamingAssetsAsBytes(string localUrl) {
			string url = GetStreamingAssetsUrl(localUrl);
			byte[] bytes = null;
			try {
				#if UNITY_ANDROID && !UNITY_EDITOR
					using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url)) {
						webRequest.SendWebRequest();
						while (!webRequest.isDone) {}
						if (string.IsNullOrEmpty(webRequest.error) && webRequest.downloadHandler.data != null && webRequest.downloadHandler.data.Length > 0)
							bytes = webRequest.downloadHandler.data;
					}

				#else
					bytes = System.IO.File.ReadAllBytes(url);
				#endif
			} catch { bytes = new byte[0]; }

			return bytes;
		}

		public static string GetDataUrl(string localUrl, bool bCreateDirectory = false) {
			string path = Application.persistentDataPath;

			string directory = path;

			localUrl = localUrl.Replace("\\", "/");
			string[] localUrlArray = localUrl.Split("/"[0]);
			for (int i = 0; i < localUrlArray.Length - 1; i++) {
				directory += "/";
				directory += localUrlArray[i];
			}

			if (bCreateDirectory && !System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);

			string url = directory + "/" + localUrlArray[localUrlArray.Length - 1];

			return url;
		}

		public static string GetStreamingAssetsUrl(string localUrl) {
			string path = "";

			#if !UNITY_EDITOR && UNITY_WEBPLAYER
				path = "StreamingAssets/";
			#else
				path = Application.streamingAssetsPath;
			#endif

			return System.IO.Path.Combine(path, localUrl);
		}

		public static bool ExistsInData(string localUrl) {
			return System.IO.File.Exists(GetDataUrl(localUrl));
		}

		public static bool ExistsInStreamingAssets(string localUrl) {
			return System.IO.File.Exists(GetStreamingAssetsUrl(localUrl));
		}

		public static T JsonBytesToObject<T>(byte[] bytes) where T : class {
			try {
				string json = System.Text.Encoding.UTF8.GetString(bytes);
				T obj = JsonUtility.FromJson<T>(json);
				return obj;
			} catch {
				return null;
			}
		}
	}
}