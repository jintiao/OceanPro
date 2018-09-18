using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	public class OceanRenderer
	{
		private MeshRenderer meshRenderer;
		private Mesh oceanMesh;
		private Material[] oceanMaterials;
		private Camera oceanCamera;

		public OceanRenderer(GameObject go, int gridSize)
		{
			Reset(go, gridSize);
		}

		public void Reset(GameObject go, int gridSize)
		{
			CreateMesh(gridSize);
			CreateMaterial();
			SetupRenderer(go);
		}

		private void CreateMesh(int gridSize)
		{

			if(gridSize < 2)
				gridSize = 2;

			int verticesCount = gridSize * gridSize;
			var vertices = new Vector3[verticesCount];
			var uvs = new Vector2[verticesCount];

			float rcpGridSize = 1.0f / (gridSize - 1.0f);
			int count = 0;
			for(int z = 0; z < gridSize; z++)
			{
				for(int x = 0; x < gridSize; x++)
				{
					var pos = new Vector3();
					pos.x = x * rcpGridSize;
					pos.z = z * rcpGridSize;
					// "edge factor" 
					pos.y = Mathf.Max(Mathf.Abs(pos.x * 2 - 1), Mathf.Abs(pos.z * 2 - 1));
					vertices[count] = pos;

					var uv = new Vector2();
					uv.x = x * rcpGridSize;
					uv.y = z * rcpGridSize;
					uvs[count] = uv;

					count++;
				}
			}

			int indicesCount = (gridSize - 1) * (gridSize - 1) * 6;
			var indices = new int[indicesCount];

			count = 0;
			for(int z = 0; z < gridSize - 1; z++)
			{
				for(int x = 0; x < gridSize - 1; x++)
				{
					var n = z * gridSize + x;

					indices[count] = n;
					indices[count + 1] = n + gridSize;
					indices[count + 2] = n + 1;

					indices[count + 3] = n + gridSize;
					indices[count + 4] = n + gridSize + 1;
					indices[count + 5] = n + 1;

					count += 6;
				}
			}

			oceanMesh = new Mesh();
			oceanMesh.name = "OceanMesh";
			oceanMesh.vertices = vertices;
			oceanMesh.triangles = indices;
			oceanMesh.uv = uvs;
		}

		private void CreateMaterial()
		{
			oceanMaterials = new Material[2];

			var mat = new Material(Shader.Find("Hidden/OceanPro/Ocean"));
			mat.name = "OceanMaterial";
			oceanMaterials[0] = mat;

			var mat1 = new Material(Shader.Find("Hidden/OceanPro/OceanWireframe"));
			mat1.name = "OceanWireframeMaterial";
			oceanMaterials[1] = mat1;
		}

		private void SetupRenderer(GameObject go)
		{
			go.GetOrAddComponent<MeshFilter>().mesh = oceanMesh;
			meshRenderer = go.GetOrAddComponent<MeshRenderer>();
		}

		public void Update()
		{
			if(oceanMesh == null)
				return;
			
			if (oceanCamera == null)
			{
				oceanCamera = Camera.main;
			}

			oceanMesh.bounds = new Bounds(oceanCamera.transform.position, new Vector3(oceanCamera.farClipPlane * 2, 1, oceanCamera.farClipPlane * 2));
		}

		public void SetWireframeMode(bool wireframeMode)
		{
			meshRenderer.material = oceanMaterials[wireframeMode ? 1 : 0];
		}
	}
}