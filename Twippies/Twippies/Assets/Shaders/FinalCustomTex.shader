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
		_ALPHA("Alpha", Color) = (0, 0, 0, 1.0)
		_Rayon ("Rayon", FLOAT) = 10
		_Step1 ("Step1", FLOAT) = 1
		_Step2 ("Step2", FLOAT) = 1
		_Step3 ("Step3", FLOAT) = 1

    }
    SubShader {    

      	Tags { "RenderType" = "Opaque" }
      	
      	 LOD 300
      	CGPROGRAM
      	#pragma surface surf Lambert vertex:vert 
     	#pragma target 5.0
            
      struct Input {
          float2 uv_MainTex1;
		  float2 uv_MainTex2;
		  float2 uv_MainTex3;
		  float2 uv_MainTex4;
		  float2 uv_MainTex5;
          half4 COL;
		  half4 COL1;  
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
      void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input,o);
		  /*float4 objectOrigin = unity_ObjectToWorld[3];
          float4 worldPos = mul(unity_ObjectToWorld, v.vertex);*/
		  float dist = length(v.vertex.xyz);
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
      void surf (Input IN, inout SurfaceOutput o) {
      
		half3 Col1 = (IN.COL.r) * tex2D (_MainTex1, IN.uv_MainTex1).rgb;
		half3 Col2 = (IN.COL.g) * tex2D (_MainTex2, IN.uv_MainTex2).rgb;
		half3 Col3 = (IN.COL.b) * tex2D (_MainTex3, IN.uv_MainTex3).rgb;
		half3 Col4 = (IN.COL.a) * tex2D (_MainTex4, IN.uv_MainTex4).rgb;
		half3 Col5 = (IN.COL1.r) * tex2D (_MainTex5, IN.uv_MainTex5).rgb;
		// pour afficher les textures : //
		o.Albedo = Col1 + Col2 + Col3 + Col4 + Col5;
		
		// pour afficher les Vertex  Color : //
		//o.Albedo = IN.couleur;

      }
      ENDCG
    }
    
    Fallback "Diffuse"
  }
