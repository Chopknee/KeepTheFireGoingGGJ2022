using UnityEngine;

namespace Dugan {
	public class Util : MonoBehaviour {

		public static float maxNormalizedVector3Magnitude = 1.732050808f;

		public static void SetLayerOnGameObjectTree(int layer, GameObject go) {
			go.layer = layer;
			Transform[] children = go.GetComponentsInChildren<Transform>();
			for (int i = 0; i < children.Length; i++) {
				children[i].gameObject.layer = layer;
			}
		}

		public static float GetColorMagnitude(Color color, bool bIncludeAlpha=false) {
			float mag = 0.0f;
			mag = UnityEngine.Mathf.Sqrt((color.r * color.r) + (color.g * color.g) + (color.b * color.b) + (bIncludeAlpha? color.a * color.a : 0.0f));
			return mag;
		}

		public static Texture2D TextureFromSprite(Sprite sprite) {
			if(sprite.textureRect.width != sprite.texture.width) {
				Texture2D newText = new Texture2D((int)sprite.textureRect.width,(int)sprite.textureRect.height);
				Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height );
				newText.SetPixels(newColors);
				newText.Apply();
				return newText;
			} else {
				return sprite.texture;
			}
		}
	}
}
