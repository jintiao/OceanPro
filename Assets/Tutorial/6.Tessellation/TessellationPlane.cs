using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanPro
{
	[ExecuteInEditMode]
	public class TessellationPlane : MonoBehaviour
	{
		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;

		void Start()
		{

			if(!meshFilter)
			{
				meshFilter = GetComponent<MeshFilter>();

				if(!meshFilter)
				{
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
			}

			if(!meshRenderer)
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
			int scrGridSizeX = 2;
			int scrGridSizeZ = 2;

			float rcpScrGridSizeX = 1.0f / (scrGridSizeX - 1.0f);
			float rcpScrGridSizeZ = 1.0f / (scrGridSizeZ - 1.0f);

			int verticesCount = scrGridSizeX * scrGridSizeZ;
			var vertices = new Vector3[verticesCount];

			int count = 0;
			for(int z = 0; z < scrGridSizeZ; z++)
			{
				var vvz = z * rcpScrGridSizeZ;

				for(int x = 0; x < scrGridSizeX; x++)
				{
					var pos = new Vector3();
					pos.x = x * rcpScrGridSizeX;
					pos.y = 0;
					pos.z = vvz;
					vertices[count] = pos;

					count++;
				}
			}

			int indicesCount = (scrGridSizeX - 1) * (scrGridSizeZ - 1) * 6;
			var indices = new int[indicesCount];

			count = 0;
			for(int z = 0; z < scrGridSizeZ - 1; z++)
			{
				for(int x = 0; x < scrGridSizeX - 1; x++)
				{
					var n = z * scrGridSizeX + x;

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

			return mesh;
		}
	}
}