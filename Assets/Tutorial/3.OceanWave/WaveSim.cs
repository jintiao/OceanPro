using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro.Tutorial
{
	public class WaveConstant
	{
		public Vector2 k;
		public float kLen;
		public float freq;
		public ComplexF kLenX;
		public ComplexF kLenZ;
		public ComplexF amp;
	}

	public class WaveSim
	{
		public Color[] texture;

		private const float g = 9.81f;
		private const float epsilon = 0.0001f;

		private int gridSize;
		private int gridSizeLog2;

		private float acc;
		private float windSpeed;
		private float maxWaveSize;
		private float choppyWaveScale;
		private float worldSize;

		private WaveConstant[] waveConstantLUT;
		private ComplexF[] heightField;
		private ComplexF[] dispFieldX;
		private ComplexF[] dispFieldZ;

		private FFT fftHelper = new FFT();

		public WaveSim(int gridSize, float acc, float wind, int waveSize, float waveScale, float worldSize)
		{

			this.gridSize = gridSize;
			gridSizeLog2 = (int)Mathf.Log(gridSize, 2);

			this.acc = acc;
			windSpeed = wind;
			maxWaveSize = waveSize;
			choppyWaveScale = waveScale;
			this.worldSize = worldSize;

			var gridSize2 = gridSize * gridSize;
			texture = new Color[gridSize2];
			waveConstantLUT = new WaveConstant[gridSize2];
			heightField = new ComplexF[gridSize2];
			dispFieldX = new ComplexF[gridSize2];
			dispFieldZ = new ComplexF[gridSize2];

			InitConstantLUT();
		}

		private void InitConstantLUT()
		{

			for(int y = 0; y < gridSize; y++)
			{
				var ky = Grid2World(y);
				for(int x = 0; x < gridSize; x++)
				{
					var kx = Grid2World(x);
					var data = new WaveConstant();
					data.k = new Vector2(kx, ky);
					data.kLen = Mathf.Sqrt(kx * kx + ky * ky);
					data.kLenX = (Mathf.Abs(data.kLen) < epsilon ? ComplexF.zero : new ComplexF(0, (-kx - ky) / data.kLen));
					data.kLenZ = (Mathf.Abs(data.kLen) < epsilon ? ComplexF.zero : new ComplexF(0, (- ky) / data.kLen));
					data.freq = Mathf.Sqrt(data.kLen * g);

					waveConstantLUT[Grid2Index(x, y)] = data;
				}
			}
		}

		private void InitFourierAmps()
		{
			float c = 1.0f / Mathf.Sqrt(2);
			Vector2 w = new Vector2(Mathf.Cos(windSpeed), Mathf.Sin(windSpeed)) * -1;


			for(int y = 0; y < gridSize; y++)
			{
				for(int x = 0; x < gridSize; x++)
				{
					var index = Grid2Index(x, y);

					ComplexF e = new ComplexF(Random.value, Random.value);
					e *= c * Mathf.Sqrt(ComputePhillipsSpec(waveConstantLUT[index].k, w));
					waveConstantLUT[index].amp = e;
				}
			}
		}

		private float ComputePhillipsSpec(Vector2 pk, Vector2 pw)
		{
			float k2 = pk.sqrMagnitude;
			if(Mathf.Abs(k2) < epsilon)
				return 0;

			float k4 = k2 * k2;
			float kw = Vector2.Dot(pk, pw);
			float kw2 = kw * kw;
			float w2 = pw.sqrMagnitude;
			float l = w2 / g;
			float l2 = l * l;

			float spec = acc * (Mathf.Exp(-1.0f / (k2 * l2)) / k4) * (kw2 / k2 * w2);
			return spec;
		}

		private int Grid2Index(int x, int y)
		{
			return y * gridSize + x;
		}

		private int Grid2MirrorIndex(int x, int y)
		{
			x = (gridSize - x) & (gridSize - 1);
			y = (gridSize - y) & (gridSize - 1);
			return Grid2Index(x, y);
		}

		private float Grid2World(int grid)
		{
			float pi2ByWorldSize = (2.0f * Mathf.PI) / worldSize;
			return (grid - (gridSize / 2.0f)) * pi2ByWorldSize;
		}

		public void Update(float time)
		{
			int halfYSize = (gridSize / 2) + 1;
			for(int y = 0; y < halfYSize; y++)
			{
				for(int x = 0; x < gridSize; x++)
				{
					var index = Grid2Index(x, y);
					var data = waveConstantLUT[index];
					var mirrorIndex = Grid2MirrorIndex(x, y);
					var mirrorData = waveConstantLUT[mirrorIndex];

					var freq = data.freq * time;
					var freqSin = Mathf.Sin(freq);
					var freqCos = Mathf.Cos(freq);

					var ep = new ComplexF(freqCos, freqSin);
					var em = ep.conj;

					var wave = data.amp * ep + mirrorData.amp.conj * em;
					heightField[index] = wave;
					dispFieldX[index] = wave * data.kLenX;
					dispFieldZ[index] = wave * data.kLenZ;

					if (y != halfYSize - 1)
					{
						heightField[mirrorIndex] = heightField[index].conj;
						dispFieldX[mirrorIndex] = dispFieldX[index].conj;
						dispFieldZ[mirrorIndex] = dispFieldZ[index].conj;
					}
				}
			}

			fftHelper.DoFFT2D(heightField);
			fftHelper.DoFFT2D(dispFieldX);
			fftHelper.DoFFT2D(dispFieldZ);


			for(int y = 0; y < gridSize; y++)
			{
				for(int x = 0; x < gridSize; x++)
				{
					var index = Grid2Index(x, y);
					var sign = PowNeg1(x + y);

					heightField[index] *= sign * maxWaveSize;
					dispFieldX[index] *= sign * choppyWaveScale;
					dispFieldZ[index] *= sign * choppyWaveScale;

					texture[index] = new Color(EncodeFloat(dispFieldX[index].real),
											   EncodeFloat(-heightField[index].real),
											   EncodeFloat(dispFieldZ[index].real));
				}
			}
		}

		private float EncodeFloat(float f)
		{
			return f;
		}

		private int PowNeg1(int n)
		{
			return ((n & 1) == 0 ? 1 : -1);
		}
	}
}