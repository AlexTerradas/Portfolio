using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TCTerrainCreator : MonoBehaviour
{
    public Texture2D m_Heightmap;
	public float m_Width;
	public float m_Height;
	public float m_Length;
	public int m_Resolution=10;

    void Start()
    {
		GenerateMesh();   
    }
	void GenerateMesh()
	{
		List<Vector3> l_Vertexs=new List<Vector3>();
		List<Vector2> l_Uvs=new List<Vector2>();
		List<Vector3> l_Normals=new List<Vector3>();
		for(int z=0; z<=m_Resolution; ++z)
		{
			for(int x=0; x<=m_Resolution; ++x)
			{
				l_Vertexs.Add(GetVertexPosition(x, z));
				l_Uvs.Add(GetUVs(x, z));
				l_Normals.Add(GetNormal(x, z));
			}
		}
		List<int> l_Indices=new List<int>();
		for(int z=0; z<m_Resolution; ++z)
		{
			for(int x=0; x<m_Resolution; ++x)
			{
				int l_IdVertex=x+(m_Resolution+1)*z;
				l_Indices.Add(l_IdVertex+1);
				l_Indices.Add(l_IdVertex+m_Resolution+1);
				l_Indices.Add(l_IdVertex+m_Resolution+2);
				
				l_Indices.Add(l_IdVertex);
				l_Indices.Add(l_IdVertex+m_Resolution+1);
				l_Indices.Add(l_IdVertex+1);
			}
		}
		Mesh l_Mesh=new Mesh();
		l_Mesh.vertices=l_Vertexs.ToArray();
		l_Mesh.normals=l_Normals.ToArray();
		l_Mesh.SetUVs(0, l_Uvs.ToArray());
		l_Mesh.SetIndices(l_Indices, MeshTopology.Triangles, 0);
		GetComponent<MeshFilter>().sharedMesh=l_Mesh;
	}
	Vector2 GetUVs(int x, int z)
	{
		float l_PctX=x/(float)m_Resolution;
		float l_PctZ=z/(float)m_Resolution;
		
		return new Vector2(l_PctX, l_PctZ);
	}
	float GetHeight(Vector2 UV)
	{
		Color l_Color=m_Heightmap.GetPixel((int)(UV.x*m_Heightmap.width), (int)(UV.y*m_Heightmap.height));
		return l_Color.r*m_Height;
	}
	Vector3 GetVertexPosition(int x, int z)
	{
		float l_PctX=x/(float)m_Resolution;
		float l_PctZ=z/(float)m_Resolution;
		return new Vector3(l_PctX*m_Width, GetHeight(new Vector2(l_PctX, l_PctZ)), l_PctZ*m_Length);
	}
	Vector3 GetNormal(Vector3 V1, Vector3 V2)
	{
		V1.Normalize();
		V2.Normalize();
		Vector3 l_Normal=Vector3.Cross(V1, V2);
		if(l_Normal.y<0.0f)
			Debug.Log("error");
		return l_Normal;
	}
	Vector3 GetNormal(int x, int z)
	{
		Vector3 l_Normal=Vector3.zero;
		Vector3 l_Position0=GetVertexPosition(x, z);
		bool l_Position1Valid=x<m_Resolution;
		Vector3 l_Position1=l_Position1Valid ? GetVertexPosition(x+1, z) : Vector3.zero;
		bool l_Position2Valid=z<m_Resolution;
		Vector3 l_Position2=l_Position2Valid ? GetVertexPosition(x, z+m_Resolution+1) : Vector3.zero;
		bool l_Position3Valid=x>0;
		Vector3 l_Position3=l_Position3Valid ? GetVertexPosition(x-1, z) : Vector3.zero;
		bool l_Position4Valid=z>0;
		Vector3 l_Position4=l_Position4Valid ? GetVertexPosition(x, z-(m_Resolution+1)) : Vector3.zero;
		if(l_Position1Valid && l_Position2Valid)
			l_Normal+=GetNormal(l_Position2-l_Position0, l_Position1-l_Position0);
		if(l_Position2Valid && l_Position3Valid)
			l_Normal+=GetNormal(l_Position3-l_Position0, l_Position2-l_Position0);
		if(l_Position3Valid && l_Position4Valid)
			l_Normal+=GetNormal(l_Position4-l_Position0, l_Position3-l_Position0);
		if(l_Position4Valid && l_Position1Valid)
			l_Normal+=GetNormal(l_Position1-l_Position0, l_Position4-l_Position0);
		l_Normal.Normalize();
		return l_Normal;
	}	
}
