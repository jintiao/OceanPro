using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	public class FFT
	{
		private float[] realCache;
		private float[] imagCache;

		public void DoFFT2D(ComplexF[] data)
		{
			int size = (int)Mathf.Log(data.Length, 2);
			if(realCache == null || realCache.Length != size)
			{
				realCache = new float[size];
				imagCache = new float[size];
			}

			for(int y = 0; y < size; y++)
			{
				for(int x = 0; x < size; x++)
				{
					var c = data[GetIndex(x, y, size)];
					realCache[x] = c.real;
					imagCache[x] = c.imag;
				}

				DoFFT1D(realCache, imagCache);

				for(int x = 0; x < size; x++)
				{
					data[GetIndex(x, y, size)] = new ComplexF(realCache[x], imagCache[x]);
				}
			}

			for(int x = 0; x < size; x++)
			{
				for(int y = 0; y < size; y++)
				{
					var c = data[GetIndex(x, y, size)];
					realCache[y] = c.real;
					imagCache[y] = c.imag;
				}

				DoFFT1D(realCache, imagCache);

				for(int y = 0; y < size; y++)
				{
					data[GetIndex(x, y, size)] = new ComplexF(realCache[y], imagCache[y]);
				}

			}
		}

		void DoFFT1D(float[] real, float[] imag)
		{
			int nn, i, i1, j, k, i2, l, l1, l2;
			float c1, c2, tx, ty, t1, t2, u1, u2, z;

			nn = real.Length;
			i2 = nn >> 1;
			j = 0;
			for (i = 0; i < nn - 1; i++)
			{
				if (i < j)
				{
					tx = real[i];
					ty = imag[i];
					real[i] = real[j];
					imag[i] = imag[j];
					real[j] = tx;
					imag[j] = ty;
				}

				k = i2;
				while(k <= j)
				{
					j -= k;
					k >>= 1;
				}

				j += k;
			}

			c1 = -1.0f;
			c2 = 0.0f;
			l2 = 1;
			int nnlog2 = (int)Mathf.Log(nn, 2);
			for(l = 0; l < nnlog2; ++l)
			{
				l1 = l2;
				l2 <<= 1;
				u1 = 1.0f;
				u2 = 0.0f;
				for(j = 0; j<l1; ++j)
				{
					for(i = j; i<nn; i += l2)
					{
						i1 = i + l1;
						t1 = u1 * real[i1] - u2 * imag[i1];
						t2 = u1 * imag[i1] + u2 * real[i1];
						real[i1] = real[i] - t1;
						imag[i1] = imag[i] - t2;
						real[i] += t1;
						imag[i] += t2;
					}

					z = u1 * c1 - u2 * c2;
					u2 = u1 * c2 + u2 * c1;
					u1 = z;
				}

				c2 = Mathf.Sqrt((1.0f - c1) * 0.5f);
				c1 = Mathf.Sqrt((1.0f + c1) * 0.5f);
			}
		}

		private static int GetIndex(int x, int y, int size)
		{
			return (y * size + x);
		}
	}
}
