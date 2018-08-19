Shader "OceanPro/Tutorial/WireFrameShader"
{
	Properties
	{
		_PlaneParam ("Plane Params", Vector) = (0,0,0,0) 
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
			};

			struct g2f
			{
				v2g data;
				float2 bc : TEXCOORD9; //barycentricCoordinates
			};

			float4 _PlaneParam;
			
			v2g vert (appdata v)
			{
				v2g o;
				UNITY_INITIALIZE_OUTPUT(v2g, o);

				float3 wpos = 0;
				wpos.xz = (v.vertex.xz * 2 - 1) * _PlaneParam.w + _WorldSpaceCameraPos.xz;
				wpos.y = _PlaneParam.y;

				o.vertex = UnityWorldToClipPos(wpos);
				return o;
			}

			[maxvertexcount(3)]
			void geom(
				triangle v2g i[3],
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
				fixed4 col = fixed4(1, 1, 1, 1);

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
