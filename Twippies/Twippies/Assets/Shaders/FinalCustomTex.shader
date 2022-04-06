// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FinalCustomTex" {
    Properties {
		_MainTex1 ("Tex1", 2D) = "white" {}
		_MainTex2 ("Tex2", 2D) = "white" {}
		_MainTex3 ("Tex3", 2D) = "white" {}
		_MainTex4 ("Tex4", 2D) = "white" {}
		_MainTex5 ("Tex5", 2D) = "white" {}
		_RED("Red", Color) = (1.0, 0, 0, 0)
		_GREEN("Green", Color) = (0, 1.0, 0, 0)
		_BLUE("Blue", Color) = (0, 0, 1.0, 0)
		_PURPLE("Purple", Color) = (1.0, 0, 1.0, 0)
		_ALPHA("Alpha", Color) = (0, 0, 0, 1)
		_Rayon ("Rayon", FLOAT) = 10
		_Step1 ("Step1", FLOAT) = 1
		_Step2 ("Step2", FLOAT) = 1
		_Step3 ("Step3", FLOAT) = 1
		_TextureScale ("Texture Scale", float) = 1
		_TriplanarBlendSharpness ("Blend Sharpness", float) = 1

		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("BumpMap", 2D) = "bump" {}
        _BumpScale ("BumpScale", Float) = 1
    }

    SubShader {    
		Tags { "RenderType" = "Transparent" }
      	 LOD 300
		Blend SrcAlpha OneMinusSrcAlpha
      	CGPROGRAM

		#pragma exclude_renderers gles
      	#pragma surface surf Lambert vertex:vert 
     	#pragma target 5.0
   
      struct Input 
	  {
          float2 uv_MainTex1;
		  float2 uv_MainTex2;
		  float2 uv_MainTex3;
		  float2 uv_MainTex4;
		  float2 uv_MainTex5;
          half4 COL;
		  half4 COL1;
		  half4 zoneColor;
		  float3 worldPos;
		  float3 worldNormal;

		  float2 uv_MainTex;
      }; 

      float4 _RED;
	  float4 _GREEN;
	  float4 _BLUE;
	  float4 _PURPLE;
	  float4 _ALPHA;
	  float _Rayon;
	  float _Step1;
	  float _Step2;
	  float _Step3;

	  fixed4 _Color;
      sampler2D _MainTex;
      sampler2D _BumpMap;
      uniform float _BumpScale;


	  // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //UNITY_INSTANCING_BUFFER_START(Props)
        //    // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)
 
		//hash for randomness
		float2 hash2D2D (float2 s)
		{
			//magic numbers
			return frac(sin(fmod(float2(dot(s, float2(127.1,311.7)), dot(s, float2(269.5,183.3))), 3.14159))*43758.5453);
		}
 
		//stochastic sampling
		float4 tex2DStochastic(sampler2D tex, float2 UV)
		{
			//triangle vertices and blend weights
			//BW_vx[0...2].xyz = triangle verts
			//BW_vx[3].xy = blend weights (z is unused)
			float4x3 BW_vx;
 
			//uv transformed into triangular grid space with UV scaled by approximation of 2*sqrt(3)
			float2 skewUV = mul(float2x2 (1.0 , 0.0 , -0.57735027 , 1.15470054), UV * 3.464);
 
			//vertex IDs and barycentric coords
			float2 vxID = float2 (floor(skewUV));
			float3 barry = float3 (frac(skewUV), 0);
			barry.z = 1.0-barry.x-barry.y;
 
			BW_vx = ((barry.z>0) ? 
				float4x3(float3(vxID, 0), float3(vxID + float2(0, 1), 0), float3(vxID + float2(1, 0), 0), barry.zyx) :
				float4x3(float3(vxID + float2 (1, 1), 0), float3(vxID + float2 (1, 0), 0), float3(vxID + float2 (0, 1), 0), float3(-barry.z, 1.0-barry.y, 1.0-barry.x)));
 
			//calculate derivatives to avoid triangular grid artifacts
			float2 dx = ddx(UV);
			float2 dy = ddy(UV);
 
			//blend samples with calculated weights
			return mul(tex2D(tex, UV + hash2D2D(BW_vx[0].xy), dx, dy), BW_vx[3].x) + 
					mul(tex2D(tex, UV + hash2D2D(BW_vx[1].xy), dx, dy), BW_vx[3].y) + 
					mul(tex2D(tex, UV + hash2D2D(BW_vx[2].xy), dx, dy), BW_vx[3].z);
		}



      	  
	  void vert (inout appdata_full v, out Input o) 
	  {
          UNITY_INITIALIZE_OUTPUT(Input,o);
		  float dist = length(v.vertex.xyz);
		  o.zoneColor = v.color;
		  if (dist <= _Rayon){
		  	  v.color = _RED;
		  }else if (dist > _Rayon && dist <= _Rayon+_Step1){
			  v.color = _GREEN;
		  }else if (dist > _Rayon+_Step1 && dist <= _Rayon+_Step1+_Step2){
			  v.color = _BLUE;
		  }else if (dist > _Rayon+_Step1+_Step2 && dist <= _Rayon+_Step1+_Step2+_Step3){
			  v.color = _PURPLE;
		  }
		  else{
			  v.color = _ALPHA;
		  }
          o.COL.r = max(v.color.r-v.color.b*v.color.b-v.color.g*v.color.g-v.color.a*v.color.a ,0.0) ;
          o.COL.g = max(v.color.b-v.color.r*v.color.r-v.color.g*v.color.g-v.color.a*v.color.a ,0.0) ;
          o.COL.b = max(v.color.g-v.color.r*v.color.r-v.color.b*v.color.b-v.color.a*v.color.a ,0.0) ;
		  o.COL.a = max(v.color.a-v.color.r*v.color.r-v.color.g*v.color.g-v.color.b*v.color.b ,0.0) ;
          o.COL1.r = max(v.color.r+v.color.b-v.color.g*v.color.g-v.color.a*v.color.a ,0.0) ;
      }      

      sampler2D _MainTex1;
      sampler2D _MainTex2;
      sampler2D _MainTex3;
      sampler2D _MainTex4;
      sampler2D _MainTex5;
	  float _TextureScale;
	  float _TriplanarBlendSharpness;

      void surf (Input IN, inout SurfaceOutput o) {
      
		float4 bumpSample;
		float4 albedoSample = 1;

		albedoSample = tex2DStochastic(_MainTex, IN.uv_MainTex);
		bumpSample = tex2DStochastic(_BumpMap, IN.uv_MainTex);


		// get scale from matrix
        float3 scale = float3(
            length(unity_WorldToObject._m00_m01_m02),
            length(unity_WorldToObject._m10_m11_m12),
            length(unity_WorldToObject._m20_m21_m22)
            );
 
        // get translation from matrix
        float3 pos = unity_WorldToObject._m03_m13_m23 / scale;
 
        // get unscaled rotation from matrix
        float3x3 rot = float3x3(
            normalize(unity_WorldToObject._m00_m01_m02),
            normalize(unity_WorldToObject._m10_m11_m12),
            normalize(unity_WorldToObject._m20_m21_m22)
            );

        // make box mapping with rotation preserved
        float3 map = mul(rot, IN.worldPos) + pos;
        float3 norm = mul(rot, IN.worldNormal);

		float3 blend = abs(norm) / dot(abs(norm), float3(1,1,1));
        float2 uv;
        if (blend.x > max(blend.y, blend.z)) {
            uv = map.yz;
        } else if (blend.z > blend.y) {
            uv = map.xy;
        } else {
            uv = map.xz;
        }
        //fixed4 c = tex2D(_MainTex, uv * (1/_TexScale));



		// Find our UVs for each axis based on world position of the fragment.
		half2 yUV = mul(rot, IN.worldPos.xz) / _TextureScale;
		half2 xUV = mul(rot, IN.worldPos.zy) / _TextureScale;
		half2 zUV = mul(rot, IN.worldPos.xy) / _TextureScale;


		fixed4 c = (IN.COL.r) * tex2DStochastic(_MainTex1, uv * (1 / _TextureScale));
		fixed4 c2 = (IN.COL.g) * tex2DStochastic(_MainTex2, uv * (1 / _TextureScale));
		fixed4 c3 = (IN.COL.b) * tex2DStochastic(_MainTex3, uv * (1 / _TextureScale));
		fixed4 c4 = (IN.COL.a) * tex2DStochastic(_MainTex4, uv * (1 / _TextureScale));
		fixed4 c5 = (IN.COL1.r) * tex2DStochastic(_MainTex5, uv * (1 / _TextureScale));

		o.Albedo = c + c2 + c3 + c4 + c5;

		// Now do texture samples from our diffuse map with each of the 3 UV set's we've just made.
		//half3 yDiff = (IN.COL.r) * tex2DStochastic (_MainTex1, yUV);
		//half3 xDiff = (IN.COL.r) * tex2DStochastic (_MainTex1, xUV);
		//half3 zDiff = (IN.COL.r) * tex2DStochastic (_MainTex1, zUV);

		//half3 y2Diff = (IN.COL.g) * tex2DStochastic (_MainTex2, yUV);
		//half3 x2Diff = (IN.COL.g) * tex2DStochastic (_MainTex2, xUV);
		//half3 z2Diff = (IN.COL.g) * tex2DStochastic (_MainTex2, zUV);

		//half3 y3Diff = (IN.COL.b) * tex2DStochastic (_MainTex3, yUV);
		//half3 x3Diff = (IN.COL.b) * tex2DStochastic (_MainTex3, xUV);
		//half3 z3Diff = (IN.COL.b) * tex2DStochastic (_MainTex3, zUV);

		//half3 y4Diff = (IN.COL.a) * tex2DStochastic (_MainTex4, yUV);
		//half3 x4Diff = (IN.COL.a) * tex2DStochastic (_MainTex4, xUV);
		//half3 z4Diff = (IN.COL.a) * tex2DStochastic (_MainTex4, zUV);

		//half3 y5Diff = (IN.COL1.r) * tex2DStochastic (_MainTex5, yUV);
		//half3 x5Diff = (IN.COL1.r) * tex2DStochastic (_MainTex5, xUV);
		//half3 z5Diff = (IN.COL1.r) * tex2DStochastic (_MainTex5, zUV);

		//half3 blendWeights = pow (abs(IN.worldNormal), _TriplanarBlendSharpness);
		//blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
		//o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z + 
		//x2Diff * blendWeights.x + y2Diff * blendWeights.y + z2Diff * blendWeights.z + 
		//x3Diff * blendWeights.x + y3Diff * blendWeights.y + z3Diff * blendWeights.z + 
		//x4Diff * blendWeights.x + y4Diff * blendWeights.y + z4Diff * blendWeights.z + 
		//x5Diff * blendWeights.x + y5Diff * blendWeights.y + z5Diff * blendWeights.z; 


		//o.Alpha = albedoSample.a;
		//o.Albedo *= albedoSample.rgb;
		//o.Normal = UnpackScaleNormal(bumpSample, _BumpScale);
		//o.Smoothness = smoothnessSample.r;

		//half3 Col1 = (IN.COL.r) * tex2D (_MainTex1, IN.uv_MainTex1).rgb;
		//half3 Col2 = (IN.COL.g) * tex2D (_MainTex2, IN.uv_MainTex2).rgb;
		//half3 Col3 = (IN.COL.b) * tex2D (_MainTex3, IN.uv_MainTex3).rgb;
		//half3 Col4 = (IN.COL.a) * tex2D (_MainTex4, IN.uv_MainTex4).rgb;
		//half3 Col5 = (IN.COL1.r) * tex2D (_MainTex5, IN.uv_MainTex5).rgb;
		
		
		//o.Albedo = Col1 + Col2 + Col3 + Col4 + Col5;
		o.Albedo *= IN.zoneColor;
		//o.Alpha = 0;


		//o.Emission = IN.Color;
		// pour afficher les Vertex  Color : //
		//o.Albedo = IN.couleur;

      }
      ENDCG
    }

	Fallback "Diffuse"
}
