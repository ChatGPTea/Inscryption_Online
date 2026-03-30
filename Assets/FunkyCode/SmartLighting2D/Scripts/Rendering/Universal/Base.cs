using UnityEngine;

namespace FunkyCode.Rendering.Universal
{
    public class Base
    {
        public static Vector2[] meshUV;
        private static Mesh preRenderMesh;

        public static Mesh GetRenderMesh()
        {
            if (preRenderMesh)
                return preRenderMesh;

            var mesh = new Mesh();

            mesh.vertices = new[] { new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1) };
            mesh.triangles = new[] { 0, 1, 2, 2, 3, 0 };
            mesh.uv = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            meshUV = mesh.uv;

            preRenderMesh = mesh;

            return preRenderMesh;
        }
    }
}