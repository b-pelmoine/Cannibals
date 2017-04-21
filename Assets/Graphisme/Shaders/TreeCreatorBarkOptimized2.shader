Shader "Nature/Tree Creator Bark Optimized 2" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (RGB) Gloss(A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	[HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
	[HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
	[HideInInspector] _SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags { "RenderQueue" = "Transparent" "IgnoreProjector"="True" "RenderType"="TreeBark" }
	LOD 200
	
CGPROGRAM
#pragma surface surf BlinnPhong alpha vertex:TreeVertBark addshadow nolightmap 
#include "UnityBuiltin3xTreeLibrary.cginc"

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

//Variables for hiding effect
int _ObjectsLength = 0;
fixed4 _Objects[25];
float3 viewDir;
float _Distance;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 worldPos;
};

//helper functions for hiding effect
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


void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * IN.color.rgb * IN.color.a;
	
	fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
	o.Gloss = trngls.a * _Color.r;

	//Calculating hiding effect
	viewDir = UNITY_MATRIX_IT_MV[2].xyz;
	float outalpha = 1.0;
	float3 camToFrag = IN.worldPos - _WorldSpaceCameraPos;
	for (int i = 0; i < _ObjectsLength; i++) {
		float3 camToObj = _Objects[i] - _WorldSpaceCameraPos;
		if (isInFront(camToObj,camToFrag))
			continue;
		float3 nearest = ClosestPointOnLine(camToFrag, camToObj);
		float val = (magnitude(camToFrag - nearest) / _Distance) - 0.5;
		if (val<outalpha)
			outalpha = val;
	}
	o.Alpha = outalpha;
	
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
	o.Specular = norspc.r;
	o.Normal = UnpackNormalDXT5nm(norspc);
}
ENDCG
}

Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Bark Rendertex"
}
