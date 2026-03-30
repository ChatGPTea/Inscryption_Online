using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public static class Polygon2DHelper
    {
        public static Mesh CreateMesh(List<Polygon2D> polygons, GameObject gameObject, Vector2 UVScale,
            Vector2 UVOffset,
            PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced)
        {
            if (gameObject.GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();

            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null) filter = gameObject.AddComponent<MeshFilter>();

            var combine = new CombineInstance[polygons.Count];
            for (var i = 0; i < polygons.Count; i++)
            {
                var poly = polygons[i];

                combine[i].mesh = PolygonTriangulator2D.Triangulate(poly, UVScale, UVOffset, triangulation);
                combine[i].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);
            }

            var mesh = new Mesh();
            mesh.CombineMeshes(combine);

            filter.sharedMesh = mesh;
            if (filter.sharedMesh == null) Object.Destroy(gameObject);

            return filter.sharedMesh;
        }

        public static Rect GetRect(List<Polygon2D> polygons)
        {
            var rect = new Rect();

            if (polygons == null) return rect;

            if (polygons.Count > 0)
            {
                float minX = 100000;
                float minY = 100000;
                float maxX = -100000;
                float maxY = -100000;

                foreach (var poly in polygons)
                {
                    var pointsCount = poly.pointsList.Count;
                    for (var i = 0; i < pointsCount; i++)
                    {
                        var id = poly.pointsList[i];

                        minX = Mathf.Min(minX, (float)id.x);
                        minY = Mathf.Min(minY, (float)id.y);
                        maxX = Mathf.Max(maxX, (float)id.x);
                        maxY = Mathf.Max(maxY, (float)id.y);
                    }
                }

                rect.x = minX;
                rect.y = minY;
                rect.width = maxX - minX;
                rect.height = maxY - minY;
            }

            return rect;
        }
    }
}