Shader "OceanPro/Tutorial/ViewDependentPlane"
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
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _PlaneParam;
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				float3 wpos = 0;
				wpos.xz = (v.vertex.xz * 2 - 1) * _PlaneParam.w + _WorldSpaceCameraPos.xz;
				wpos.y = _PlaneParam.y;

				o.vertex = UnityWorldToClipPos(wpos);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0.7, 0.0, 0.0, 1);
			}
			ENDCG
		}
	}
}
