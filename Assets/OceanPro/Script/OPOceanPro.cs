using System;
using UnityEngine;

namespace OceanPro
{
	[ExecuteInEditMode]
	public class OPOceanPro : MonoBehaviour
	{
		public float windDirection = 1;
		public float windSpeed = 4.0f;
		public float wavesAmount = 1.5f;
		public float wavesSize = 0.75f;
		public float waterLevel = 0;

		[NonSerialized]
		private OPOceanObject oceanObject;

		void Start()
		{
			if (!CheckSupport())
			{
				enabled = false;
			}
		}

		private bool CheckSupport()
		{
			if(!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
				return false;

			return true;
		}

		void Update()
		{
			ResetOceanObject(false);
		}

		private void ResetOceanObject(bool forceReset)
		{
			if(oceanObject != null)
			{
				if (forceReset)
					oceanObject.ResetOcean(SetupOceanParam());
				return;
			}

			var name = "OceanObject";
			var go = GameObject.Find(name);

			if(go == null)
			{
				go = new GameObject(name);
				go.transform.parent = transform;
				go.hideFlags = HideFlags.DontSave;
			}

			oceanObject = go.GetComponent<OPOceanObject>();
			if(oceanObject == null)
			{
				oceanObject = go.AddComponent<OPOceanObject>();
			}

			oceanObject.ResetOcean(SetupOceanParam());
		}

		private OPOceanParam SetupOceanParam()
		{
			return new OPOceanParam 
			{
				param0 = new Vector4(windDirection, windSpeed, 0, wavesAmount),
				param1 = new Vector4(wavesSize, Mathf.Cos(windDirection), Mathf.Sin(windDirection), waterLevel),
			};
		}

	}

}
