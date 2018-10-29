using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro.Tutorial
{
	[ExecuteInEditMode]
	public class OceanWave : MonoBehaviour
	{
		public int waveTexSize = 64;

		private Texture2D waveTex;

		private WaveSim waveSim;

		private void Start()
		{
			var tf = TextureFormat.RGBA32;
			if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
			{
				Debug.Log("RGBAFloat");
				//tf = TextureFormat.RGBAFloat;
			}
			else
			{
				Debug.LogWarning("RGBA32");
			}
			waveTex = new Texture2D(waveTexSize, waveTexSize, tf, false);
			waveSim = new WaveSim(waveTexSize, 1, 1, 1, 1, 1024);
		}

		private void Update()
		{
			if(waveTex == null || waveSim == null)
				return;

			waveSim.Update(Time.time);

			waveTex.SetPixels(waveSim.texture);
			waveTex.Apply(false);
		}

		private void OnGUI()
		{
			GUI.DrawTexture(new Rect(100, 100, waveTexSize, waveTexSize), waveTex);
		}
	}
}