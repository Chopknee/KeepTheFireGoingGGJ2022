Shader "Dugan/TransparentFresnel" {
	Properties {
		_InnerColor ("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
		_Texture ("Texture (2D)", 2D) = "white" {}
		_RimTexture ("Rim Texture (2D", 2D) = "white" {}
		_RimTextureTiling ("Rim Texture Tiling", Vector) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "Queue" = "Transparent" }

		ZWrite Off
		Cull Back
		Blend SrcColor OneMinusSrcColor
	   
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

		float2 _RimTextureTiling;

		void surf (Input IN, inout SurfaceOutput o)  {
        	o.Albedo = _InnerColor.rgb * tex2D(_Texture, IN.uv_Texture);
        	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));

			float2 uv = _RimTextureTiling;
			uv.x +=  sin(3.141592654 * 2 * _Time * 650) * 95;
			
			float4 texCol = tex2D(_RimTexture, IN.viewDir.xyz * uv);
        	o.Emission = (_RimColor.rgb * pow (rim, _RimPower) * texCol.rgb) * texCol.a;
		}
		ENDCG
	} 
	Fallback "Diffuse"
}