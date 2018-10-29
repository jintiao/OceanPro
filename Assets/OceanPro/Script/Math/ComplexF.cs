using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OceanPro
{
	public struct ComplexF
	{
		public float real;
		public float imag;

		public static ComplexF zero
		{
			get { return new ComplexF(0, 0); }
		}

		public ComplexF conj
		{
			get { return new ComplexF(real, -imag); }
		}

		public ComplexF(float r, float i)
		{
			real = r;
			imag = i;
		}

		public static ComplexF operator +(ComplexF lhs, ComplexF rhs)
		{
			return new ComplexF(lhs.real + rhs.real,
			                    lhs.imag * rhs.imag);
		}

		public static ComplexF operator *(ComplexF lhs, float n)
		{
			return new ComplexF(lhs.real * n, 
			                    lhs.imag * n);
		}

		public static ComplexF operator *(ComplexF lhs, ComplexF rhs)
		{
			return new ComplexF(lhs.real * rhs.real - lhs.imag * rhs.imag,
			                    lhs.real * rhs.imag + lhs.imag * rhs.real);
		}
	}
}
