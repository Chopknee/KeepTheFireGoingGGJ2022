using UnityEngine;

namespace Dugan {
	public class RendererExtensions {
		
		private static string[] properties = new string[] {
			"_Color",
			"_FaceColor",
			"_TintColor"
		};
		
		public static void SetMaterialAlpha(Renderer renderer, float value) {
			for (int i = 0; i < properties.Length; i++) {
				if (!renderer.material.HasProperty(properties[i]))
					continue;
				
				Color color = renderer.material.GetColor(properties[i]);
				color.a = value;
				renderer.material.SetColor(properties[i], color);
				
				//return;
			}
		}
		
		public static void SetMaterialColor(Renderer renderer, Color color) {
			for (int i = 0; i < properties.Length; i++) {
				if (!renderer.material.HasProperty(properties[i]))
					continue;
				
				renderer.material.SetColor(properties[i], color);
				
				return;
			}
		}
		
		public static Color GetMaterialColor(Renderer renderer) {
			for (int i = 0; i < properties.Length; i++) {
				if (!renderer.material.HasProperty(properties[i]))
					continue;

				return renderer.material.GetColor(properties[i]);
			}
			return new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}
}
