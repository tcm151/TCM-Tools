using System.Collections.Generic;
using UnityEngine;


namespace EasyMesh
{
    [System.Serializable]
    [UnityEngine.RequireComponent(typeof(UnityEngine.MeshFilter))]
    public class EasyMesh : MonoBehaviour
    {
        public UnityEngine.Mesh UnityMesh {get; private set;}

        public List<Vertex> vertexList = new List<Vertex>();
        public List<int> triangleList = new List<int>();
        
        public void Create() => UnityMesh = new UnityEngine.Mesh();

        public void Apply()
        {
            UnityMesh.vertices = vertexList.ConvertAll(v => v.ToVector3()).ToArray();
            UnityMesh.triangles = triangleList.ToArray();
        }

        public void AddTriangle(ref Vertex v1, ref Vertex v2, ref Vertex v3)
        {
            var i = vertexList.Count;
            vertexList.Add(v1);
            vertexList.Add(v2);
            vertexList.Add(v3);
            triangleList.Add(i + 0);
            triangleList.Add(i + 1);
            triangleList.Add(i + 2);
        }

        public void AddQuad(ref Vertex v1, ref Vertex v2, ref Vertex v3, ref Vertex v4)
        {
            var i = vertexList.Count;
            vertexList.Add(v1);
            vertexList.Add(v2);
            vertexList.Add(v3);
            vertexList.Add(v4);
            triangleList.Add(i + 0);
            triangleList.Add(i + 2);
            triangleList.Add(i + 1);
            triangleList.Add(i + 1);
            triangleList.Add(i + 2);
            triangleList.Add(i + 3);
        }
    }
}