Shader "Custom/PlayerOcclusion" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		
		CGPROGRAM
		#include "UnityCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		half _Distance = 5;
		fixed4 _Color;
		int _ObjectsLength = 0;
		fixed4 _Objects[25];
		float3 viewDir;

		float3 ClosestPointOnLine(float3 v1, float3 v2) {
			float3 result = dot(v1, v2) / dot(v2, v2) * v2;
			return result;
		}

		float magnitude(float3 v) {
			return v.x*v.x + v.y*v.y + v.z*v.z;
		}

		bool isInFront(float3 camToObj, float3 camToFrag) {
			camToObj.y = 0;
			camToFrag.y = 0; 
			camToObj = ClosestPointOnLine(camToObj, viewDir);
			camToFrag = ClosestPointOnLine(camToFrag, viewDir);
			return (magnitude(camToObj) < magnitude(camToFrag));
		}
			

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			viewDir = UNITY_MATRIX_IT_MV[2].xyz;
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			
			
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			float outalpha = 1.0;
			float3 camToFrag = IN.worldPos - _WorldSpaceCameraPos;
			for (int i = 0; i < _ObjectsLength; i++) {
				float3 camToObj = _Objects[i] - _WorldSpaceCameraPos;
				//if (isInFront(camToObj,camToFrag))
					//continue;
				float3 nearest = ClosestPointOnLine(camToFrag, camToObj);
				float val = (magnitude(camToFrag - nearest)/_Distance)-0.5;
				if(val<outalpha)
					outalpha = val;
			}
			o.Alpha = outalpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
