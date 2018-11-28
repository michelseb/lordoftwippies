/*
Copyright(c) 2017 Untitled Games
Written by Chris Bellini

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files(the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions :

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

Shader "Custom/Heightlerp" 
{
	Properties
	{
		_MainTex("Main texture", 2D) = "white" {}
		_Texture1("Texture 1", 2D) = "white" {}
		_Height1Shift("Height 1 Shift", Range(200000, 10000000)) = 0
		_Texture2("Texture 2", 2D) = "white" {}
		_Height2Shift("Height 1 Shift", Range(200000, 10000000)) = 0
		_Texture3("Texture 3", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 3.0
		#include "heightblend.cginc"

		sampler2D _MainTex;

		sampler2D _Texture1;
		float _Height1Shift;

		sampler2D _Texture2;
		float _Height2Shift;

		sampler2D _Texture3;
		
		struct Input 
		{
			float2 uv_Texture1;
			float4 color : COLOR;
		};

		void vert(inout appdata_full v){
			float4 objectOrigin = unity_ObjectToWorld[3];
            float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
            float dist = distance(objectOrigin, worldPos);
            float cFactor = saturate((dist * dist * dist * dist * dist * dist * dist * dist)/(_Height1Shift + 0.5));
			float4 tex = tex2Dlod (_Texture1, float4(v.texcoord.xy, 0, 0));
			float4 tex2 = tex2Dlod (_Texture2, float4(v.texcoord.xy, 0, 0));
            v.color = lerp(tex.rgba, tex2.rgba, cFactor);
        }

		void surf(Input IN, inout SurfaceOutput o)
		{
			float2 uv = IN.uv_Texture1;
			float t = uv.x;
			o.Albedo = tex2D(_MainTex, uv).rgba * IN.color;
		}


		ENDCG
	}

	FallBack "Diffuse"
}
