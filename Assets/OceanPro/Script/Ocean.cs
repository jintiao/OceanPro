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

		m_fRECustomData[0] = p3DEngine->m_oceanWindDirection;
		m_fRECustomData[1] = p3DEngine->m_oceanWindSpeed;
		m_fRECustomData[2] = 0.0f; // used to be m_oceanWavesSpeed
		m_fRECustomData[3] = p3DEngine->m_oceanWavesAmount;
		m_fRECustomData[4] = p3DEngine->m_oceanWavesSize;
		sincos_tpl(p3DEngine->m_oceanWindDirection, &m_fRECustomData[6], &m_fRECustomData[5]);
		m_fRECustomData[7] = fWaterLevel;
		*/

		public bool wireframeMode = false;

		public int waterTessellationAmount = 10;

		public float oceanLevel = 1;
		public float waveAmount = 1.5f;
		public float waveSize = 0.75f;
		public Texture2D waveTex;

		private OceanRenderer oceanRenderer;

		private void OnEnable()
		{
			ResetOcean();
		}

		public void ResetOcean()
		{
			oceanRenderer = new OceanRenderer(this);
			SetWireframeMode(wireframeMode);
		}

		private void Update()
		{
			if (oceanRenderer != null)
			{
				oceanRenderer.oceanHeight = oceanLevel;
				oceanRenderer.Update();
			}
		}

		public void SetWireframeMode(bool wireframe)
		{
			wireframeMode = wireframe;
			oceanRenderer.SetWireframeMode(wireframeMode);
		}
	}
}
