#ifndef OCEAN_COMMON
#define OCEAN_COMMON

#include "UnityCG.cginc"

struct a2v
{
	float4 vertex : POSITION;
};

struct v2f
{
#if OP_NO_TESSELATION
	float4 vertex : SV_POSITION;
#endif

	half4 viewDir : TEXCOORD0;
	float4 posWorld : TEXCOORD1;
	half4 normal : TEXCOORD2;
	float4 oceanParam : TEXCOORD3;
};

struct v2f_hs
{
	float4 vertex : SV_POSITION;

	half4 viewDir : TEXCOORD0;
	float4 posWorld : TEXCOORD1;
	half4 normal : TEXCOORD2;
	float4 oceanParam : TEXCOORD3;
};

struct v2f_gs
{
	v2f_hs data;
	float2 bc : TEXCOORD9; //barycentricCoordinates
};

sampler2D_float _DispTex;
float4 _DispTex_TexelSize;
float4 _OceanParam0;
float4 _OceanParam1;

v2f OceanVS (a2v v)
{
	v2f o;
	UNITY_INITIALIZE_OUTPUT(v2f, o);

	float4 wpos = v.vertex;
	wpos.w = wpos.y; // store edge information

	wpos.y = _OceanParam1.w/* + (4.5f * (wpos.w > 0.98f) ? 1 : 0) */;
	wpos.xz = (wpos.xz * 2.0f - 1.0f) * ((wpos.w > 0.98f) ? _ProjectionParams.z * 2.0f : 500.0f);
	wpos.xz += _WorldSpaceCameraPos.xz - frac(_WorldSpaceCameraPos.xz);

	o.posWorld = wpos;

	o.viewDir.xyz = _WorldSpaceCameraPos.xyz - wpos.xyz;
	o.viewDir.w = sign(o.viewDir.z);

	o.oceanParam.x = _OceanParam1.x;
	o.oceanParam.w = _OceanParam0.w;

#if OP_NO_TESSELATION
	o.vertex = UnityObjectToClipPos(wpos);
#endif

	return o;
}

struct hs_const_output
{
    float edges[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

hs_const_output OceanPCF (InputPatch<v2f, 3> patch) 
{
	hs_const_output o;

	float maxFactor = 16;
	float factorEdge = 32;
	float factorInside = 32;
	float scale = 1;
	float fMinDistance = 0;
	float fMaxDistance = 300;

	float edge = factorEdge * scale;
	float inside = factorInside * scale;
	float4 factors = float4(edge, edge, edge, inside);

	float3 p0 = _WorldSpaceCameraPos.xyz - patch[0].viewDir.xyz;
	float3 p1 = _WorldSpaceCameraPos.xyz - patch[1].viewDir.xyz;
	float3 p2 = _WorldSpaceCameraPos.xyz - patch[2].viewDir.xyz;

	factors.xyz = distance(p2, p1);
	factors.x /= distance((p2 + p1) / 2, _WorldSpaceCameraPos.xyz);
	factors.y = distance(p2, p0);
	factors.y /= distance((p2 + p0) / 2, _WorldSpaceCameraPos.xyz);
	factors.z = distance(p0, p1);
	factors.z /= distance((p0 + p1) / 2, _WorldSpaceCameraPos.xyz);

	float avg = max(max(factors.x, factors.y), factors.z) ;
	factors.w = saturate(avg);
	factors.xyz *= edge;
	factors.w *= inside;

	o.edges[0] = clamp(factors.x, 1, maxFactor);
	o.edges[1] = clamp(factors.y, 1, maxFactor);
	o.edges[2] = clamp(factors.z, 1, maxFactor);
	o.inside   = clamp(factors.w, 1, maxFactor);
	return o;
}

[UNITY_domain("tri")]
[UNITY_outputcontrolpoints(3)]
[UNITY_outputtopology("triangle_cw")]
[UNITY_partitioning("fractional_odd")]
[UNITY_patchconstantfunc("OceanPCF")]
v2f OceanHS (InputPatch<v2f, 3> patch, uint id : SV_OutputControlPointID) 
{
	return patch[id];
}

float4 BarycentricInterp(float3 bcs, float4 p0, float4 p1, float4 p2)
{
	return bcs.x * p0 + bcs.y * p1 + bcs.z * p2;
}

[UNITY_domain("tri")]
v2f_hs OceanDS (hs_const_output factors, OutputPatch<v2f, 3> patch, float3 barycentricCoordinates : SV_DomainLocation) 
{
	v2f_hs o;
	UNITY_INITIALIZE_OUTPUT(v2f_hs, o);

	o.viewDir = BarycentricInterp(barycentricCoordinates, patch[0].viewDir, patch[1].viewDir, patch[2].viewDir);
	o.posWorld = BarycentricInterp(barycentricCoordinates, patch[0].posWorld, patch[1].posWorld, patch[2].posWorld);   

	float4 posWorld = float4(_WorldSpaceCameraPos.xyz - o.viewDir.xyz, 1);

	float camDist = length(posWorld.xyz - _WorldSpaceCameraPos.xyz);
	float atten = saturate(camDist * 0.5);
	atten *= atten;

	float2 uv = (posWorld.xz) * 0.0125 * patch[0].oceanParam.w;
	float4 disp = tex2Dlod(_DispTex, float4(uv, 0.0, 0.0));
	disp += tex2Dlod(_DispTex, float4(uv * 2, 0.0, 0.0)) * float4(1.5, 1.5, 1.0, 1.0);
	disp *= atten;
	posWorld.xyz += disp * 0.06 * patch[0].oceanParam.x;

	float4 disp2 = tex2Dlod(_DispTex, float4(uv + float2(_DispTex_TexelSize.x, 0), 0.0, 0.0));
	disp2 += tex2Dlod(_DispTex, float4(uv * 2 + float2(_DispTex_TexelSize.x, 0), 0.0, 0.0)) * float4(1.5, 1.5, 1.0, 1.0);
	disp2 *= atten;

	float4 edgeLen = disp2 - disp;
	posWorld.w = saturate(-(edgeLen.x + edgeLen.y) * 0.035);

	posWorld.w = lerp(posWorld.w, 0, saturate(o.posWorld.w * 1.5) ) ;

	o.posWorld = posWorld;
	o.vertex = UnityObjectToClipPos(posWorld);

	return o;
}



[maxvertexcount(3)]
void OceanGS(triangle v2f_hs i[3], inout TriangleStream<v2f_gs> stream)
{
	v2f_gs g0, g1, g2;

	g0.data = i[0];
	g1.data = i[1];
	g2.data = i[2];

	g0.bc = float2(1, 0);
	g1.bc = float2(0, 1);
	g2.bc = float2(0, 0);

	stream.Append(g0);
	stream.Append(g1);
	stream.Append(g2);
}


half4 OceanFS (v2f_gs i) : SV_Target
{
	half4 col = half4(0.5, 0.5, 0.5, 1);

	float3 bary = float3(i.bc, 1 - i.bc.x - i.bc.y);

	float m = min(bary.x, min(bary.y, bary.z));
	float delta = fwidth(m);
	m = smoothstep(0, delta * 0.5, m);

	col.xyz = col * m;

	return col;
}



#endif // #ifndef OCEAN_COMMON
