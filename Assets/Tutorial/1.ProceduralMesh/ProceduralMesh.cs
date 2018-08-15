using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	[ExecuteInEditMode]
	public class ProceduralMesh : MonoBehaviour
	{
		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;

		public int gridSizeX = 20;
		public int gridSizeY = 20;

		private Vector3[] vertices;

		private void Start()
		{
			if (!meshFilter)
			{
				meshFilter = GetComponent<MeshFilter>();

				if(!meshFilter)
				{
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
			}

			if (!meshRenderer)
			{
				meshRenderer = GetComponent<MeshRenderer>();

				if(!meshRenderer)
				{
					meshRenderer = gameObject.AddComponent<MeshRenderer>();
				}
			}

			meshFilter.mesh = CreateMesh();
		}

		private Mesh CreateMesh()
		{
			int scrGridSizeX = gridSizeX;
			int scrGridSizeY = gridSizeY;

			float rcpScrGridSizeX = 1.0f / (scrGridSizeX - 1.0f);
			float rcpScrGridSizeY = 1.0f / (scrGridSizeY - 1.0f);

			int verticesCount = scrGridSizeX * scrGridSizeY;
			vertices = new Vector3[verticesCount];
			var uvs = new Vector2[verticesCount];

			int count = 0;
			for(int y = 0; y < scrGridSizeY; y++)
			{
				var vvy = y * rcpScrGridSizeY * 10;

				for(int x = 0; x < scrGridSizeX; x++)
				{
					var pos = new Vector3();
					pos.x = x * rcpScrGridSizeX * 10;
					pos.y = vvy;
					pos.z = 0;
					vertices[count] = pos;

					var uv = new Vector2();
					uv.x = x * rcpScrGridSizeX;
					uv.y = y * rcpScrGridSizeY;
					uvs[count] = uv;

					count++;
				}
			}

			int indicesCount = (scrGridSizeX - 1) * (scrGridSizeY - 1) * 6;
			var indices = new int[indicesCount];

			count = 0;
			for(int y = 0; y < scrGridSizeY - 1; y++)
			{
				for(int x = 0; x < scrGridSizeX - 1; x++)
				{
					var n = y * scrGridSizeX + x;

					indices[count] = n;
					indices[count + 1] = n + scrGridSizeX;
					indices[count + 2] = n + 1;

					indices[count + 3] = n + scrGridSizeX;
					indices[count + 4] = n + scrGridSizeX + 1;
					indices[count + 5] = n + 1;

					count += 6;
				}
			}

			var mesh = new Mesh();
			mesh.name = "ProcedualMesh";
			mesh.vertices = vertices;
			mesh.triangles = indices;
			mesh.uv = uvs;

			return mesh;
		}
	}
}

