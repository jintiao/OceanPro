using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	[ExecuteInEditMode]
	public class Ocean : MonoBehaviour
	{
		/*
		m_oceanCausticsMultiplier = (float) atof(GetXMLAttribText(pInputNode, "Ocean", "CausticsMultiplier", "0.85"));
		m_oceanCausticsDistanceAtten = (float) atof(GetXMLAttribText(pInputNode, "Ocean", "CausticsDistanceAtten", "100.0"));
		m_oceanCausticsTilling = (float) atof(GetXMLAttribText(pInputNode, "Ocean", "CausticsTilling", "1.0"));

		m_oceanWindDirection = (float) atof(GetXMLAttribText(pInputNode, "OceanAnimation", "WindDirection", "1.0"));
		m_oceanWindSpeed = (float) atof(GetXMLAttribText(pInputNode, "OceanAnimation", "WindSpeed", "4.0"));
		m_oceanWavesAmount = (float) atof(GetXMLAttribText(pInputNode, "OceanAnimation", "WavesAmount", "1.5"));
		m_oceanWavesAmount = clamp_tpl<float>(m_oceanWavesAmount, 0.4f, 3.0f);
		m_oceanWavesSize = (float) atof(GetXMLAttribText(pInputNode, "OceanAnimation", "WavesSize", "0.75"));
		m_oceanWavesSize = clamp_tpl<float>(m_oceanWavesSize, 0.0f, 3.0f);

		// disabled temporarily - we'll use fixed height instead with tweaked attenuation function
		m_oceanCausticHeight = 0.0f;
		//m_oceanCausticHeight = (float) atof( GetXMLAttribText(pInputNode, "Ocean", "CausticHeight", "2.5"));
		m_oceanCausticDepth = (float) atof(GetXMLAttribText(pInputNode, "Ocean", "CausticDepth", "8.0"));
		m_oceanCausticIntensity = (float) atof(GetXMLAttribText(pInputNode, "Ocean", "CausticIntensity", "1.0"));

		WaterWavesTessellationAmount = 5;
		WaterTessellationSwathWidth = 12;
		
		public float oceanWaterLevel;
		*/

		public bool wireframeMode = false;

		public int waterTessellationAmount = 10;

		private OceanRenderer oceanRenderer;

		private void OnEnable()
		{
			ResetOcean();
		}

		public void ResetOcean()
		{
			oceanRenderer = new OceanRenderer(gameObject, waterTessellationAmount);
			SetWireframeMode(wireframeMode);
		}

		private void Update()
		{
			oceanRenderer?.Update();
		}

		public void SetWireframeMode(bool wfm)
		{
			wireframeMode = wfm;
			if(oceanRenderer != null && oceanRenderer.oceanMaterial != null)
			{
				if (wfm)
				{
					oceanRenderer.oceanMaterial.EnableKeyword("OCEAN_WIREFRAME_MODE");
				}
				else
				{
					oceanRenderer.oceanMaterial.DisableKeyword("OCEAN_WIREFRAME_MODE");
				}
			}
		}
	}
}
