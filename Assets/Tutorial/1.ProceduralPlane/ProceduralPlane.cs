using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro.Tutorial
{
	[ExecuteInEditMode]
	public class ProceduralPlane : MonoBehaviour
	{
		public int gridSize = 10;
		private int lastGridSize;

		public float distance = 100;
		private float lastDistance;

		private void Start()
		{
			RegenMesh();
		}

		private void Update()
		{
			if(gridSize <= 1)
				gridSize = 2;
			if(distance <= 0)
				distance = 1;
			
			if(gridSize != lastGridSize || distance != lastDistance)
				RegenMesh();
		}

		private void RegenMesh()
		{
			lastGridSize = gridSize;
			lastDistance = distance;

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
					pos.x = (x * rcpGridSize - 0.5f) * distance;
					pos.z = (z * rcpGridSize - 0.5f) * distance;
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

			var mesh = new Mesh();
			mesh.name = "ProcedualMesh";
			mesh.vertices = vertices;
			mesh.triangles = indices;
			mesh.uv = uvs;

			gameObject.GetOrAddComponent<MeshFilter>().mesh = mesh;
			gameObject.GetOrAddComponent<MeshRenderer>();
		}
	}
}

