Shader "Hidden/OceanPro/OceanWireframe"
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
			#pragma geometry OceanDebugGS
			#pragma fragment OceanDebugFS

            #include "OceanWireframe.cginc"
			ENDCG
		}
	}

    CustomEditor "OceanShaderGUI"
}
