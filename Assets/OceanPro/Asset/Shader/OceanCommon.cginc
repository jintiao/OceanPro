#ifndef OCEAN_COMMON
#define OCEAN_COMMON

#include "UnityCG.cginc"

struct VSInput
{
	float4 vertex : POSITION;
};

struct HSInput
{
	float4 viewDir : TEXCOORD0;
	float4 worldPos : TEXCOORD1;
};


sampler2D _DispTex;
float4 _OceanParam;

HSInput OceanVS (VSInput v)
{
	HSInput o;
	UNITY_INITIALIZE_OUTPUT(HSInput, o);

	float4 wpos = v.vertex;
	wpos.w = wpos.y; // store edge information
	wpos.y = _OceanParam.w/* + (4.5f * (wpos.w > 0.98f) ? 1 : 0) */;
	wpos.xz = (wpos.xz * 2.0f - 1.0f) * ((wpos.w > 0.98f) ? _ProjectionParams.z * 2.0f : 500.0f);
	wpos.xz += _WorldSpaceCameraPos.xz - frac(_WorldSpaceCameraPos.xz);
	o.worldPos = wpos;

	o.viewDir.xyz = _WorldSpaceCameraPos.xyz - wpos.xyz;
	o.viewDir.w = sign(o.viewDir.z);

	return o;
}

struct HSConstant
{
    float edges[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

HSConstant OceanPCF (InputPatch<HSInput, 3> patch) 
{
	HSConstant o;

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
HSInput OceanHS (InputPatch<HSInput, 3> patch, uint id : SV_OutputControlPointID) 
{
	return patch[id];
}




struct GSInput
{
	float4 vertex : SV_POSITION;
	float4 viewDir : TEXCOORD0;
	float4 worldPos : TEXCOORD1;
};

float4 BarycentricInterp(float3 bcs, float4 p0, float4 p1, float4 p2)
{
	return bcs.x * p0 + bcs.y * p1 + bcs.z * p2;
}

[UNITY_domain("tri")]
GSInput OceanDS (HSConstant factors, OutputPatch<HSInput, 3> patch, float3 barycentricCoordinates : SV_DomainLocation) 
{
	GSInput o;
	UNITY_INITIALIZE_OUTPUT(GSInput, o);

	o.viewDir = BarycentricInterp(barycentricCoordinates, patch[0].viewDir, patch[1].viewDir, patch[2].viewDir);
	//o.worldPos = BarycentricInterp(barycentricCoordinates, patch[0].worldPos, patch[1].worldPos, patch[2].worldPos);   
	float4 worldPos = float4(_WorldSpaceCameraPos.xyz - o.viewDir.xyz, 1);

	float2 uv = (worldPos.xz) * 0.0125;
	float4 disp = tex2Dlod(_DispTex, float4(uv, 0.0, 0.0));

	worldPos.xyz += disp * 8;
	o.vertex = UnityObjectToClipPos(worldPos);

	return o;
}


struct FSInput
{
	GSInput data;
	float2 bc : TEXCOORD9; //barycentricCoordinates
};

[maxvertexcount(3)]
void OceanGS(triangle GSInput i[3], inout TriangleStream<FSInput> stream)
{
	FSInput g0, g1, g2;

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


half4 OceanFS (FSInput i) : SV_Target
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
