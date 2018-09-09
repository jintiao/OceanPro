using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	public static class GameObjectMethodExtension
	{
		static public T GetOrAddComponent<T>(this GameObject go) where T : Component
		{
			T result = go.GetComponent<T>();
			if(result == null)
				result = go.AddComponent<T>();
			return result;
		}
	}
}