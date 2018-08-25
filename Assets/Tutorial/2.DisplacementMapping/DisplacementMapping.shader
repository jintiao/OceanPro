Shader "OceanPro/Tutorial/DisplacementMapping"
{
	Properties
	{
		_DispTex ("Displacement Texture", 2D) = "white" {}
		_MaxOffset ("Max Offset", float ) = 1
	}

	SubShader
	{
		Tags { "LightMode"="ForwardBase" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 diff : COLOR0;
				float2 uv : TEXCOORD0;
			};

			sampler2D _DispTex;
			float4 _DispTex_ST;
			float4 _DispTex_TexelSize;
			float _MaxOffset;
			
			v2f vert (appdata v)
			{
				v2f o;

				float4 h0 =  tex2Dlod(_DispTex, float4(v.uv.xy, 0, 0));
				float4 h10 =  tex2Dlod(_DispTex, float4(v.uv.x + _DispTex_TexelSize.x, v.uv.y, 0, 0));
				float4 h11 =  tex2Dlod(_DispTex, float4(v.uv.x, v.uv.y + _DispTex_TexelSize.y, 0, 0));

				float d0 = (h0.y - h10.y) + 0.5;
				float d1 = (h0.y - h11.y) + 0.5;
				//half3 weight = half3(h0.y, h10.y, h11.y);
				//float normal = half3(weight.x - weight.y, weight.x - weight.z, 1);
				//normal = normalize(normal);
				float3 v0 = h10.xyz - h0.xyz;
				float3 v1 = h11.xyz - h0.xyz;
				half3 normal = normalize(cross(v0, v1));
				//half nl = max(0, dot(normal, _WorldSpaceLightPos0.xyz));
                o.diff = fixed4(normal, 1);

				float h = h0.y * _MaxOffset;
				float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				wpos.y += h;

				o.vertex = UnityWorldToClipPos(wpos);
				o.uv = v.uv	;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.7, 0.7, 0.7, 1);
				col = i.diff;
				return col;
			}
			ENDCG
		}
	}
}
