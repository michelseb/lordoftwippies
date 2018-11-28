// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/HeightShader" {
     Properties {
         _MainTex ("Main Texture", 2D) = "white" {}
         _CenterHeight ("Center Height", Float) = 0.0
         _MaxVariance ("Maximum Variance", Float) = 3.0
         _HighColor ("High Color", Color) = (1.0, 1.0, 1.0, 1.0)
         _LowColor ("Low Color", Color) = (0.0, 0.0, 0.0, 1.0)
     }
     SubShader {
             Tags { "RenderType"="Opaque" }
             Cull Off
             
             CGPROGRAM
             #pragma surface surf Lambert vertex:vert
             #include <UnityCG.cginc>
     
             float _CenterHeight;
             float _MaxVariance;
             float4 _HighColor;
             float4 _LowColor;
             sampler2D _MainTex;
             
             struct Input{
                 float2 uv_MainTex;
                 float4 color : COLOR;
             };
             
             void vert(inout appdata_full v){
                 // Convert to world position
				 float4 objectOrigin = unity_ObjectToWorld[3];
                 float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                 float dist = distance(objectOrigin, worldPos);
				 //float diff = worldPos.y - _CenterHeight;
                 float cFactor = saturate((dist*dist*dist*dist)/(_MaxVariance + 0.5));
                 
                 //lerp by factor
                 v.color = lerp(_LowColor, _HighColor, cFactor);
             }
             
             void surf(Input IN, inout SurfaceOutput o){
                 o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * IN.color;
             }
             
             ENDCG
     }
     FallBack "Diffuse"
 }