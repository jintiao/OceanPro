using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	public class OPOceanParam
	{
		public float windDirection = 1;
		public float windSpeed = 4.0f;
		public float wavesAmount = 1.5f;
		public float wavesSize = 0.75f;
		public float waterLevel = 0;

		public Vector4 param0 
		{
			get
			{
				return new Vector4(windDirection, windSpeed, 0, wavesAmount);
			}
		}

		public Vector4 param1
		{
			get
			{
				return new Vector4(wavesSize, Mathf.Cos(windDirection), Mathf.Sin(windDirection), waterLevel);
			}
		}

		public OPOceanParam(float windDirection, float windSpeed, float wavesAmount, float wavesSize, float waterLevel)
		{
			this.windDirection = windDirection;
			this.windSpeed = windSpeed;
			this.wavesAmount = wavesAmount;
			this.wavesSize = wavesSize;
			this.waterLevel = waterLevel;
		}
	}
}
