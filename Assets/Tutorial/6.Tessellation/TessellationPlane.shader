Shader "OceanPro/Tutorial//TessellationPlane"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 4.6
			#pragma vertex vert
			#pragma hull hul
			#pragma domain dom
			#pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2h
			{
				float4 vertex : INTERNALTESSPOS;
			};

			struct tf
			{
			    float edge[3] : SV_TessFactor;
			    float inside : SV_InsideTessFactor;
			};

			struct d2g
			{
				float4 vertex : SV_POSITION;
			};

			struct g2f
			{
				d2g data;
				float2 bc : TEXCOORD9; //barycentricCoordinates
			};
			
			v2h vert (appdata v)
			{
				v2h o;
				UNITY_INITIALIZE_OUTPUT(v2h, o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			[UNITY_domain("tri")]
			[UNITY_outputcontrolpoints(3)]
			[UNITY_outputtopology("triangle_cw")]
			[UNITY_partitioning("fractional_odd")]
			[UNITY_patchconstantfunc("pcf")]
			v2h hul (
				InputPatch<v2h, 3> patch,
				uint id : SV_OutputControlPointID) 
			{
				return patch[id];
			}

			tf pcf (InputPatch<v2h, 3> patch) 
			{
				tf f;
			    f.edge[0] = 3;
			    f.edge[1] = 3;
			    f.edge[2] = 3;
				f.inside = 5;
				return f;
			}

			[UNITY_domain("tri")]
			d2g dom (
				tf factors,
				OutputPatch<v2h, 3> patch,
				float3 barycentricCoordinates : SV_DomainLocation) 
			{
				d2g data;
				data.vertex =
					patch[0].vertex * barycentricCoordinates.x +
					patch[1].vertex * barycentricCoordinates.y +
					patch[2].vertex * barycentricCoordinates.z;
				return data;
			}

			[maxvertexcount(3)]
			void geom(
				triangle d2g i[3],
				inout TriangleStream<g2f> stream)
			{
				g2f g0, g1, g2;

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
			
			fixed4 frag (g2f i) : SV_Target
			{
				fixed4 col = fixed4(0.5, 0.5, 0.5, 1);

				float3 bary = float3(i.bc, 1 - i.bc.x - i.bc.y);

				float m = min(bary.x, min(bary.y, bary.z));
				float delta = fwidth(m);
				m = smoothstep(0, delta, m);

				col.xyz = col * m;

				return col;
			}
			ENDCG
		}
	}
}
