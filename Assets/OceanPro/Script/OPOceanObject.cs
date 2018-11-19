using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	[ExecuteInEditMode]
	public class OPOceanObject : MonoBehaviour
	{
		private const int texSize = 64;
			
		private Mesh oceanMesh;
		private MeshRenderer oceanRenderer;
		private Material oceanMat;
		private Texture2D oceanTex;
		private OPWaveSim waveSim;

		private float waterLevel;

		public void ResetOcean(OPOceanParam param)
		{
			waveSim = new OPWaveSim();

			CreateMaterial(param);
			CreateMesh();
		}

		private void CreateMesh()
		{
			int gridSize = 20;
			float rcpGridSize = 1.0f / (gridSize - 1.0f);

			var vertices = new List<Vector3>();
			for(int z = 0; z < gridSize; z++)
			{
				for(int x = 0; x < gridSize; x++)
				{
					var pos = new Vector3();
					pos.x = x * rcpGridSize;
					pos.z = z * rcpGridSize;
					// "edge factor"
					pos.y = Mathf.Max(Mathf.Abs(pos.x * 2 - 1), Mathf.Abs(pos.z * 2 - 1));
					vertices.Add(pos);
				}
			}

			var indices = new List<int>();
			for(int z = 0; z < gridSize - 1; z++)
			{
				for(int x = 0; x < gridSize - 1; x++)
				{
					var n = z * gridSize + x;

					indices.Add(n);
					indices.Add(n + gridSize);
					indices.Add(n + 1);

					indices.Add(n + gridSize);
					indices.Add(n + gridSize + 1);
					indices.Add(n + 1);
				}
			}

			oceanMesh = new Mesh();
			oceanMesh.name = "OceanMesh";
			oceanMesh.vertices = vertices.ToArray();
			oceanMesh.triangles = indices.ToArray();

			var mf = GetComponent<MeshFilter>();
			if(mf == null)
				mf = gameObject.AddComponent<MeshFilter>();
			mf.mesh = oceanMesh;

			oceanRenderer = GetComponent<MeshRenderer>();
			if(oceanRenderer == null)
				oceanRenderer = gameObject.AddComponent<MeshRenderer>();
			oceanRenderer.material = oceanMat;
		}

		private void CreateMaterial(OPOceanParam op)
		{
			oceanTex = new Texture2D(texSize, texSize, TextureFormat.RGBAFloat, false);

			oceanMat = new Material(Shader.Find("Hidden/OceanPro/OPOcean"));
			oceanMat.SetTexture("_DispTex", oceanTex);
			oceanMat.SetVector("_OceanParam0", op.param0);
			oceanMat.SetVector("_OceanParam1", op.param1);
			//oceanMat.EnableKeyword("OP_NO_TESSELATION");

			waterLevel = op.param1.w;
		}

		void Update()
		{
			UpdateRenderer();
			UpdateBound();
			UpdateWave();
		}

		private void UpdateRenderer()
		{
			if(oceanRenderer == null)
				return;

			oceanRenderer.enabled = (waterLevel < Camera.main.transform.position.y);
		}

		private void UpdateBound()
		{
			if(oceanMesh == null)
				return;

			var cam = Camera.main;
			var camPos = cam.transform.position;
			oceanMesh.bounds = new Bounds(new Vector3(camPos.x, waterLevel, camPos.z), new Vector3(cam.farClipPlane, 1, cam.farClipPlane));
		}

		private void UpdateWave()
		{
			if(waveSim == null)
				return;
			
			if(oceanTex == null)
				return;
			
			var pixels = oceanTex.GetRawTextureData<Color>();
			waveSim.Update(Time.time, pixels, texSize);
			oceanTex.Apply(false);
		}
	}
}
