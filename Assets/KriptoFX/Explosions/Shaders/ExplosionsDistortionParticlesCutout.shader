// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/Explosions/Distortion/ParticlesCutOut" {
Properties {
        //_TintColor ("Tint Color", Color) = (1,1,1,1)
		//_MainTex ("Base (RGB) Gloss (A)", 2D) = "black" {}
		//_CutOut ("CutOut (A)", 2D) = "white" {}
        _MainTex ("Normalmap & CutOut", 2D) = "black" {}
		//_ColorStrength ("Color Strength", Float) = 1
		_BumpAmt ("Distortion", Float) = 10
		_InvFade ("Soft Particles Factor", Float) = 0.5
}

Category {

	Tags { "Queue"="Transparent-10"}
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off

	ZWrite Off
	Fog { Mode Off}

	SubShader {

		Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile_particles
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
	fixed4 color : COLOR;
};

struct v2f {
	float4 vertex : POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	//float2 uvmain : TEXCOORD2;
	//float2 uvcutout : TEXCOORD3;
	fixed4 color : COLOR;
	#ifdef SOFTPARTICLES_ON
		float4 projPos : TEXCOORD3;
	#endif
};

//sampler2D _MainTex;
//sampler2D _CutOut;
sampler2D _MainTex;

float _BumpAmt;
//float _ColorStrength;
sampler2D _CameraOpaqueTexture;

//fixed4 _TintColor;


float4 _MainTex_ST;
//float4 _MainTex_ST;
//float4 _CutOut_ST;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	#ifdef SOFTPARTICLES_ON
		o.projPos = ComputeScreenPos (o.vertex);
		COMPUTE_EYEDEPTH(o.projPos.z);
	#endif
	o.color = v.color;
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.w;
#if UNITY_SINGLE_PASS_STEREO
	o.uvgrab.xy = TransformStereoScreenSpaceTex(o.uvgrab.xy, o.uvgrab.w);
#endif
	o.uvgrab.z /= distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
	o.uvbump = TRANSFORM_TEX( v.texcoord, _MainTex );
	//o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	//o.uvcutout = TRANSFORM_TEX( v.texcoord, _CutOut );

	return o;
}

sampler2D _CameraDepthTexture;
float _InvFade;

half4 frag( v2f i ) : COLOR
{

	half3 bump = UnpackNormal(tex2D( _MainTex, i.uvbump ));
	half alphaBump = saturate((abs(bump.r + bump.g) - 0.01) * 5);

	clip(step(0.5, alphaBump) - 0.1);

	float2 offset = bump * _BumpAmt  * i.color.a * alphaBump * 0.005;
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;




	half4 col = tex2Dproj( _CameraOpaqueTexture, UNITY_PROJ_COORD(i.uvgrab));
	//fixed4 tex = tex2D(_MainTex, i.uvmain) * i.color;
	//fixed4 cut = tex2D(_CutOut, i.uvcutout);
    col.a = saturate(col.a * alphaBump);
	return col;
}
ENDCG
		}
	}
}

}
