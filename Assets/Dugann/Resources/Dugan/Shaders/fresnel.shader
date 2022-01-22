Shader "Dugan/Fresnel"  {
	Properties {
		_InnerColor ("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,1.0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
		_Texture ("Texture (2D)", 2D) = "white" {}
		_RimTexture ("Rim Texture (2D", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		
	   	Cull Back
		//Blend One One
	   
		CGPROGRAM
		#pragma surface surf Lambert
	   
		sampler2D _Texture;
		sampler2D _RimTexture;

		struct Input  {
			float2 uv_Texture;
			float2 uv_RimTexture;
			float3 viewDir;
		};
	   
		float4 _InnerColor;
		float4 _RimColor;
		float _RimPower;
	   
		void surf (Input IN, inout SurfaceOutput o)  {
			o.Albedo = _InnerColor.rgb * tex2D(_Texture, IN.uv_Texture);
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow (rim, _RimPower) * tex2D(_RimTexture, IN.viewDir.xz + IN.uv_RimTexture * IN.viewDir.y);
		}
		ENDCG
	} 
	Fallback "Diffuse"
}