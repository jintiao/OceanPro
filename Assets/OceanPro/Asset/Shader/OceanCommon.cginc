#ifndef OCEAN_COMMON
#define OCEAN_COMMON

#include "UnityCG.cginc"

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};

struct v2f
{
	float2 uv : TEXCOORD0;
	UNITY_FOG_COORDS(1)
	float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _MainTex_ST;

v2f OceanVS (appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	UNITY_TRANSFER_FOG(o,o.vertex);
	return o;
}

#ifdef OCEAN_WIREFRAME_MODE

fixed4 OceanFS (v2f i) : SV_Target
{
	fixed4 col = fixed4(0, 0, 1, 1);
	return col;
}

#else

fixed4 OceanFS (v2f i) : SV_Target
{
	fixed4 col = fixed4(1, 0, 0, 1);
	return col;
}
#endif

#endif // #ifndef OCEAN_COMMON
