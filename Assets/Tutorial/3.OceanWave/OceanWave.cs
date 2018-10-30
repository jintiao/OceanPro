using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro.Tutorial
{
	//[ExecuteInEditMode]
	public class OceanWave : MonoBehaviour
	{
		public int waveTexSize = 64;

		private Texture2D waveTex;

		private WaveSim waveSim;

		private void Start()
		{
			//if(SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
			var tf = TextureFormat.RGBAFloat;
			waveTex = new Texture2D(waveTexSize, waveTexSize, tf, false);
			waveSim = new WaveSim(waveTexSize, 1, 1, 10, 5, 1);
		}

		private void Update()
		{
			if(waveTex == null || waveSim == null)
				return;

			var pixels = waveTex.GetRawTextureData<Color>();
			waveSim.Update(Time.time * 0.1f, pixels);

			waveTex.Apply(false);
		}

		private void OnGUI()
		{
			GUI.DrawTexture(new Rect(100, 100, 256, 256), waveTex);
		}
	}
}