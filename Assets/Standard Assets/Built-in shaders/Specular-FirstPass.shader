// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nature/Terrain/Specular" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

		// set by terrain engine
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		// used in fallback on old cards & base map
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)

		_SizeEffect ("Size of the effect", Range (0.0, 30)) = 10
		_SpeedEffect ("Speed of the effect", Range (0.0, 30)) = 1
		_HeightEffect("Height of the effect", Range (0.0, 30)) = 1
		_HighLightColor("Effect Color", Color) = (1,0,0,1)
		_EffectColorRadius("Effect color radius", Range (0.0, 1.5)) = 0.8
	}

	SubShader {
		Tags {
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer
		#pragma multi_compile_fog
		#pragma multi_compile __ _TERRAIN_NORMAL_MAP
		#pragma target 3.0
		// needs more than 8 texcoords
		#pragma exclude_renderers gles

		#ifndef TERRAIN_SPLATMAP_COMMON_CGINC_INCLUDED
		#define TERRAIN_SPLATMAP_COMMON_CGINC_INCLUDED

		struct Input
		{
			float3 worldPos;
		    float2 uv_Splat0 : TEXCOORD0;
		    float2 uv_Splat1 : TEXCOORD1;
		    float2 uv_Splat2 : TEXCOORD2;
		    float2 uv_Splat3 : TEXCOORD3;
		    float2 tc_Control : TEXCOORD4;  // Not prefixing '_Contorl' with 'uv' allows a tighter packing of interpolators, which is necessary to support directional lightmap.
		    UNITY_FOG_COORDS(5)
		};

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3;

		float4 _Color;
		float4 _HighLightColor;
		float _SizeEffect;
		float _SpeedEffect;
		float _HeightEffect;
		float _EffectColorRadius;

		float _AgentAmount;
		float3 _AgentPositions[50];

		#ifdef _TERRAIN_NORMAL_MAP
		    sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
		#endif

		void SplatmapVert(inout appdata_full v, out Input data)
		{
		    UNITY_INITIALIZE_OUTPUT(Input, data);
		    data.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);  // Need to manually transform uv here, as we choose not to use 'uv' prefix for this texcoord.
		    float4 pos = UnityObjectToClipPos (v.vertex);
		    UNITY_TRANSFER_FOG(data, pos);

		#ifdef _TERRAIN_NORMAL_MAP
		    v.tangent.xyz = cross(v.normal, float3(0,0,1));
		    v.tangent.w = -1;
		#endif

			float size = _SizeEffect;
		    if(size > 0){
		    	float3 _PlayerPos;
		    	for(int i=0; i< _AgentAmount; i++)
			    {
			    	_PlayerPos = _AgentPositions[i];
			    	_PlayerPos.x += 250;
			    	_PlayerPos.z -= 100;
				    float dist = distance(_PlayerPos.xyz , v.vertex.xyz);
				    float sinVal = sin(_SpeedEffect * _SinTime * 4 * 1/(size*2)*(1-(size-clamp(dist, 0.0, size))))* _HeightEffect * 1/(size*2);
				    v.vertex.y += (size-clamp(dist, 0.0, size))* sinVal;
			    }
		    }
		}

		#ifdef TERRAIN_STANDARD_SHADER
		void SplatmapMix(Input IN, half4 defaultAlpha, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
		#else
		void SplatmapMix(Input IN, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
		#endif
		{
		    splat_control = tex2D(_Control, IN.tc_Control);
		    weight = dot(splat_control, half4(1,1,1,1));

		    #if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
		        clip(weight - 0.0039 /*1/255*/);
		    #endif

		    // Normalize weights before lighting and restore weights in final modifier functions so that the overal
		    // lighting result can be correctly weighted.
		    splat_control /= (weight + 1e-3f);

		    mixedDiffuse = 0.0f;
		    #ifdef TERRAIN_STANDARD_SHADER
		        mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.uv_Splat0) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
		        mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.uv_Splat1) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
		        mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.uv_Splat2) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
		        mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.uv_Splat3) * half4(1.0, 1.0, 1.0, defaultAlpha.a);
		    #else
		        mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.uv_Splat0);
		        mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.uv_Splat1);
		        mixedDiffuse += splat_control.b * tex2D(_Splat2, IN.uv_Splat2);
		        mixedDiffuse += splat_control.a * tex2D(_Splat3, IN.uv_Splat3);
		    #endif

		    #ifdef _TERRAIN_NORMAL_MAP
		        fixed4 nrm = 0.0f;
		        nrm += splat_control.r * tex2D(_Normal0, IN.uv_Splat0);
		        nrm += splat_control.g * tex2D(_Normal1, IN.uv_Splat1);
		        nrm += splat_control.b * tex2D(_Normal2, IN.uv_Splat2);
		        nrm += splat_control.a * tex2D(_Normal3, IN.uv_Splat3);
		        mixedNormal = UnpackNormal(nrm);
		    #endif
		}

		#ifndef TERRAIN_SURFACE_OUTPUT
		    #define TERRAIN_SURFACE_OUTPUT SurfaceOutput
		#endif

		void SplatmapFinalColor(Input IN, TERRAIN_SURFACE_OUTPUT o, inout fixed4 color)
		{
		    color *= o.Alpha;
		    #ifdef TERRAIN_SPLAT_ADDPASS
		        UNITY_APPLY_FOG_COLOR(IN.fogCoord, color, fixed4(0,0,0,0));
		    #else
		        UNITY_APPLY_FOG(IN.fogCoord, color);
		    #endif
		}

		void SplatmapFinalPrepass(Input IN, TERRAIN_SURFACE_OUTPUT o, inout fixed4 normalSpec)
		{
		    normalSpec *= o.Alpha;
		}

		void SplatmapFinalGBuffer(Input IN, TERRAIN_SURFACE_OUTPUT o, inout half4 diffuse, inout half4 specSmoothness, inout half4 normal, inout half4 emission)
		{
		    diffuse.rgb *= o.Alpha;
		    specSmoothness *= o.Alpha;
		    normal.rgb *= o.Alpha;
		    emission *= o.Alpha;
		}

		#endif // TERRAIN_SPLATMAP_COMMON_CGINC_INCLUDED

		half _Shininess;

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;
			SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);
			float3 _PlayerPos;
			float minDist = _SizeEffect;
	    	for(int i=0; i< _AgentAmount; i++)
		    {
		    	_PlayerPos = _AgentPositions[i];
		    	float dist = clamp(distance(_PlayerPos,IN.worldPos),0.0,_SizeEffect);
		    	if(dist < minDist) minDist = dist;
		    }
			o.Albedo = mixedDiffuse.rgb * _Color - 0.5*(_HighLightColor*(_SizeEffect-minDist))*_EffectColorRadius;
			o.Alpha = weight;
			o.Gloss = mixedDiffuse.a;
			o.Specular = _Shininess;
		}
		ENDCG
	}

	Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Specular-AddPass"
	Dependency "BaseMapShader" = "Hidden/TerrainEngine/Splatmap/Specular-Base"

	Fallback "Nature/Terrain/Diffuse"
}
