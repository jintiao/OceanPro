Shader "Hidden/OceanPro/OPOcean"
{
    Properties
    {
        _DispTex ("Water Displacement Texture", 2D) = "white" {}
		_OceanParam0 ("Ocean Param 0", Vector) = (0,0,0,0) 
		_OceanParam1 ("Ocean Param 1", Vector) = (0,0,0,0) 
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

			#pragma multi_compile OP_TESSELATION OP_NO_TESSELATION

			#pragma vertex OceanVS
			#pragma hull OceanHS
			#pragma domain OceanDS
			#pragma geometry OceanGS
			#pragma fragment OceanFS



            #include "OPOceanDebug.cginc"
            ENDCG
        }
    }
}
