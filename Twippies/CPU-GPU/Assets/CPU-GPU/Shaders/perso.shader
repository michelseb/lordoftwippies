Shader "Custom/perso" {
	Properties {


	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float4 col : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.col;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
