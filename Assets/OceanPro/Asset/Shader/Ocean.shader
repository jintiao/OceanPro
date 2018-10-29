Shader "Hidden/OceanPro/Ocean"
{
	Properties
	{
		_DispTex( "Water Displacement Texture", 2D ) = "white" {}
		_OceanParam ("Ocean Param", Vector) = (0,0,0,0) 
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
