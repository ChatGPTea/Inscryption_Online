using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode
{
    public class MeshBrush
    {
        public List<Color> colors = new();

        private readonly Mesh mesh;
        public List<int> triangles = new();
        private int tris;
        public List<Vector2> uv = new();
        public List<Vector3> vertices = new();

        public MeshBrush()
        {
            mesh = new Mesh();
        }

        public void Clear()
        {
            vertices.Clear();
            uv.Clear();
            triangles.Clear();
            colors.Clear();

            tris = 0;
        }

        public void AddMesh(Mesh mesh, Vector3 offset)
        {
            for (var i = 0; i < mesh.vertices.Length; i++) vertices.Add(mesh.vertices[i] + offset);

            for (var i = 0; i < mesh.uv.Length; i++) uv.Add(mesh.uv[i]);

            for (var i = 0; i < mesh.triangles.Length; i++) triangles.Add(mesh.triangles[i] + tris);

            tris += mesh.vertices.Length;
        }

        public Mesh Export()
        {
            if (mesh == null) return null;
            mesh.triangles = null;
            mesh.vertices = null;
            mesh.uv = null;
            mesh.colors = null;

            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();

            if (colors.Count > 0) mesh.colors = colors.ToArray();

            //Debug.Log(triangles.Count / 3);

            return mesh;
        }
    }
}