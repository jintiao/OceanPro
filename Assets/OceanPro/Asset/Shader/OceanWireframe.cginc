#ifndef OCEAN_WIREFRAME
#define OCEAN_WIREFRAME

#include "UnityCG.cginc"
#include "OceanCommon.cginc"



struct OGeometryOutput
{
	OVertexOutput data;
	float2 bc : TEXCOORD9; //barycentricCoordinates
};


[maxvertexcount(3)]
void OceanDebugGS(triangle OVertexOutput i[3], inout TriangleStream<OGeometryOutput> stream)
{
	OGeometryOutput g0, g1, g2;

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

half4 OceanDebugFS (OGeometryOutput i) : SV_Target
{
	half4 col = half4(1, 1, 1, 1);

	float3 bary = float3(i.bc, 1 - i.bc.x - i.bc.y);

	float m = min(bary.x, min(bary.y, bary.z));
	float delta = fwidth(m);
	m = smoothstep(0, delta, m);

	col.xyz = col * m;

	return col;
}


#endif // #ifndef OCEAN_WIREFRAME
