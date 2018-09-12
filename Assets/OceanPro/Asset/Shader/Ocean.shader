Shader "Hidden/OceanPro/Ocean"
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
			// target 5.0 doesn't work on mac
			#pragma target gl4.1

			#pragma vertex OceanVS
			#pragma hull OceanHS
			#pragma domain OceanDS
			#pragma geometry OceanGS
			#pragma fragment OceanFS

            #include "OceanCommon.cginc"
			ENDCG
		}
	}

    CustomEditor "OceanShaderGUI"
}
