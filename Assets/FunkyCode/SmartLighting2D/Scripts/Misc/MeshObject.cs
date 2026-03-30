using UnityEngine;

namespace FunkyCode
{
    public class MeshObject
    {
        public Mesh mesh;
        public int[] triangles;
        public Vector2[] uv;
        public Vector3[] vertices;

        public static MeshObject Get(Mesh meshOrigin)
        {
            if (meshOrigin.isReadable)
            {
                var meshObject = new MeshObject();
                meshObject.vertices = meshOrigin.vertices;
                meshObject.uv = meshOrigin.uv;
                meshObject.triangles = meshOrigin.triangles;
                meshObject.mesh = meshOrigin;

                return meshObject;
            }

            return null;
        }
    }
}