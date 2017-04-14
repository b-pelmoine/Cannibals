Shader "Nature/Tree Creator Leaves Optimized 2" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
	
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_ShadowTex ("Shadow (RGB)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R) Shadow Offset (B)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (B) Gloss(A)", 2D) = "white" {}

	// These are here only to provide default values
	[HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
	[HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
	[HideInInspector] _SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="TreeLeaf"
	}
	LOD 200
	
CGPROGRAM
#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:TreeVertLeaf nolightmap noforwardadd
#include "UnityBuiltin3xTreeLibrary.cginc"

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

half _Distance = 5;
int _ObjectsLength = 0;
fixed4 _Objects[25];
float3 viewDir;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR; // color.a = AO
	float3 worldPos;
};

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

void surf (Input IN, inout LeafSurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * IN.color.rgb * IN.color.a;
	
	fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
	o.Translucency = trngls.b;
	o.Gloss = trngls.a * _Color.r;
	o.Alpha = c.a;

	viewDir = UNITY_MATRIX_IT_MV[2].xyz;
	float outalpha = 1.0;
	float3 camToFrag = IN.worldPos - _WorldSpaceCameraPos;
	for (int i = 0; i < _ObjectsLength; i++) {
		float3 camToObj = _Objects[i] - _WorldSpaceCameraPos;
		//if (isInFront(camToObj,camToFrag))
		//continue;
		float3 nearest = ClosestPointOnLine(camToFrag, camToObj);
		float val = (magnitude(camToFrag - nearest) / _Distance);
		if (val<outalpha)
			outalpha = val;
	}
	if(outalpha<c.a)
		o.Alpha = outalpha;
	
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
	o.Specular = norspc.r;
	o.Normal = UnpackNormalDXT5nm(norspc);
}
ENDCG

	// Pass to render object as a shadow caster
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		
		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma multi_compile_shadowcaster
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "UnityBuiltin3xTreeLibrary.cginc"

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			float2 hip_pack0 : TEXCOORD1;
		};
		float4 _MainTex_ST;
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			TreeVertLeaf (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			return o;
		}
		fixed _Cutoff;
		float4 frag_surf (v2f_surf IN) : SV_Target {
			half alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;
			clip (alpha - _Cutoff);
			SHADOW_CASTER_FRAGMENT(IN)
		}
		ENDCG
	}
	
}

Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Leaves Rendertex"
}
