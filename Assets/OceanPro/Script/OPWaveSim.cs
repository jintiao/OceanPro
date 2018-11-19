using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace OceanPro
{
	public class OPWaveSim
	{
		public void Update(float time, NativeArray<Color> pixels, int texSize)
		{
			float s = 1.0f / (float)texSize;

			int index = 0;
			for(int y = 0; y < texSize; y++)
			{
				for(int x = 0; x < texSize; x++)
				{
					pixels[index] = new Color(0, s * x * 5, 0);
					index++;
				}
			}
		}
	}
}