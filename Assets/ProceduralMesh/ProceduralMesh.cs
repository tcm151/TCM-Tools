using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace ProceduralMesh
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class ProceduralMesh : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private Mesh mesh;

        public Material material;

        public List<Vertex> vertices = new List<Vertex>();
        public List<int> triangleList = new List<int>();
        
        
        private void Awake()
        {
            this.meshFilter = GetComponent<MeshFilter>();
            this.meshRenderer = GetComponent<MeshRenderer>();
            this.meshCollider = GetComponent<MeshCollider>();

            this.meshRenderer.sharedMaterial = material;
            this.mesh = meshFilter.sharedMesh = meshCollider.sharedMesh
                = new Mesh { name = "Mesh.0", indexFormat = IndexFormat.UInt32,};
        }
        
        public void Apply()
        {
            mesh.vertices = vertices.ConvertAll(v => v.ToVector3()).ToArray();
            mesh.triangles = triangleList.ToArray();
        }

        public void AddTriangle(ref Vertex v1, ref Vertex v2, ref Vertex v3)
        {
            var i = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangleList.Add(i + 0);
            triangleList.Add(i + 1);
            triangleList.Add(i + 2);
        }

        public void AddQuad(ref Vertex v1, ref Vertex v2, ref Vertex v3, ref Vertex v4)
        {
            var i = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangleList.Add(i + 0);
            triangleList.Add(i + 2);
            triangleList.Add(i + 1);
            triangleList.Add(i + 1);
            triangleList.Add(i + 2);
            triangleList.Add(i + 3);
        }
    }
}