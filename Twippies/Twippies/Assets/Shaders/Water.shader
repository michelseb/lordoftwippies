Shader "Custom/Water" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		_BumpMap("Normal Map", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0,1)) = 0.6
		_Transparency("Transparency", Range(0,1)) = 0.3
	}
		SubShader{
			Tags {"RenderType" = "Opaque" "Queue" = "Transparent" "ForceNoShadowCasting" = "true" "IgnoreProjector" = "true"}
			LOD 200
			Cull off

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows alpha
			#pragma target 3.0

			float4 _Color;
			float4 _RimColor;
			float _RimPower;
			sampler2D _BumpMap;
			half _Smoothness;
			half _Transparency;
			uniform float4 _WaveScale4;

			struct Input {
				float2 uv_BumpMap;
				float3 viewDir;
			};

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed4 c = _Color;
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * 5.0;

				half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
				o.Emission = _RimColor.rgba * pow(rim, _RimPower);
				o.Smoothness = _Smoothness;
				o.Alpha = _Transparency;
			}
			ENDCG
		}
			FallBack "Diffuse"
}