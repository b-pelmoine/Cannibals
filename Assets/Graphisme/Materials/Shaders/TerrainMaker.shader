Shader "Custom/TerrainMarker" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}

			// Splat Map Control Texture
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		 
		// Textures
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		 
		// Normal Maps
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}

		// used in fallback on old cards
		_Color ("Main Color", Color) = (1,1,1,1)
		_SizeEffect ("Size of the effect", Range (0.0, 30)) = 10
		_SpeedEffect ("Speed of the effect", Range (0.0, 30)) = 1
		_HeightEffect("Height of the effect", Range (0.0, 30)) = 1
		_HighLightColor("Effect Color", Color) = (1,0,0,1)
		_EffectColorRadius("Effect color radius", Range (0.0, 1.5)) = 0.8
}
	SubShader { 
		Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry-100"
		}
		LOD 200

		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert
		
		sampler2D _MainTex;
		half _Shininess;
		float4 _Color;
		float4 _HighLightColor;
		float _SizeEffect;
		float _SpeedEffect;
		float _HeightEffect;
		float _EffectColorRadius;

		float _AgentAmount;
		float3 _AgentPositions[50];

		void vert (inout appdata_full v) {
		    float size = _SizeEffect;
		    if(size > 0){
		    	float3 _PlayerPos;
		    	for(int i=0; i< _AgentAmount; i++)
			    {
			    	_PlayerPos = _AgentPositions[i];
			    	_PlayerPos.xz += 250;
				    float dist = distance(_PlayerPos.xyz , v.vertex.xyz);
				    float sinVal = sin(_SpeedEffect * _SinTime * 4 * 1/(size*2)*(1-(size-clamp(dist, 0.0, size))))* _HeightEffect * 1/(size*2);
				    v.vertex.y += (size-clamp(dist, 0.0, size))* sinVal;
			    }
		    }
		    
		}

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			float3 _PlayerPos;
			float minDist = _SizeEffect;
	    	for(int i=0; i< _AgentAmount; i++)
		    {
		    	_PlayerPos = _AgentPositions[i];
		    	float dist = clamp(distance(_PlayerPos,IN.worldPos),0.0,_SizeEffect);
		    	if(dist < minDist) minDist = dist;
		    }
			o.Albedo = tex.rgb * _Color - 0.5*(_HighLightColor*(_SizeEffect-minDist))*_EffectColorRadius;
			o.Gloss = tex.a;
			o.Alpha = 1.0f;
			o.Specular = _Shininess;
		}
		ENDCG
	}

	FallBack "Legacy Shaders/Specular"
}
