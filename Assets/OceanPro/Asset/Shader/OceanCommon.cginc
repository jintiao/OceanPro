#ifndef OCEAN_COMMON
#define OCEAN_COMMON

#include "UnityCG.cginc"

struct VSInput
{
	float4 vertex : POSITION;
};

struct HSInput
{
	float4 vertex : INTERNALTESSPOS;
};

HSInput OceanVS (VSInput v)
{
	HSInput o;
	UNITY_INITIALIZE_OUTPUT(HSInput, o);

	float4 vPos = v.vertex;
	vPos.w = vPos.y; // store edge information
	vPos.xz = (vPos.xz * 2.0f - 1.0f) * ((vPos.w > 0.98f) ? _ProjectionParams.z * 2.0f : 500.0f);

	float2 cpos = _WorldSpaceCameraPos.xz;
	cpos.xy -= frac(cpos.xy);
	vPos.xz += cpos;

	vPos.y = 1;

	o.vertex = UnityObjectToClipPos(vPos);

	return o;
}

struct TessFactor
{
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

TessFactor OceanPCF (InputPatch<HSInput, 3> patch) 
{
	TessFactor f;
    f.edge[0] = 1;
    f.edge[1] = 1;
    f.edge[2] = 1;
	f.inside = 1;
	return f;
}

[UNITY_domain("tri")]
[UNITY_outputcontrolpoints(3)]
[UNITY_outputtopology("triangle_cw")]
[UNITY_partitioning("fractional_odd")]
[UNITY_patchconstantfunc("OceanPCF")]
HSInput OceanHS (
	InputPatch<HSInput, 3> patch,
	uint id : SV_OutputControlPointID) 
{
	return patch[id];
}

struct GSInput
{
	float4 vertex : SV_POSITION;
};

[UNITY_domain("tri")]
GSInput OceanDS (TessFactor factors, OutputPatch<HSInput, 3> patch, float3 barycentricCoordinates : SV_DomainLocation) 
{
	GSInput data;
	data.vertex =
		patch[0].vertex * barycentricCoordinates.x +
		patch[1].vertex * barycentricCoordinates.y +
		patch[2].vertex * barycentricCoordinates.z;
	return data;
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
	m = smoothstep(0, delta, m);

	col.xyz = col * m;

	return col;
}



#endif // #ifndef OCEAN_COMMON
