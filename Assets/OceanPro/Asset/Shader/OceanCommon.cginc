#ifndef OCEAN_COMMON
#define OCEAN_COMMON

#include "UnityCG.cginc"

struct OVertexInput
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};

struct OVertexOutput
{
	UNITY_POSITION(pos);
	float2 tex : TEXCOORD0;
};

OVertexOutput OceanVS (OVertexInput v)
{
	OVertexOutput o;
	UNITY_INITIALIZE_OUTPUT(OVertexOutput, o);

	o.pos = UnityObjectToClipPos(v.vertex);
	o.tex = v.uv;

	return o;
}

half4 OceanFS (OVertexOutput i) : SV_Target
{
	half4 col = half4(0, 0, 1, 1);
	return col;
}



#endif // #ifndef OCEAN_COMMON
