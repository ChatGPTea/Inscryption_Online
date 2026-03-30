using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public class Polygon2Collider3D
    {
        public static int defaultCircleVerticesCount = 25;

        public static Polygon2 CreateFromBoxCollider(BoxCollider boxCollider)
        {
            var newPolygon = new Polygon2(4);

            var size = new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2);

            Vector2 offset = boxCollider.center;

            newPolygon.points[0] = new Vector2(-size.x, -size.y) + offset;
            newPolygon.points[1] = new Vector2(-size.x, size.y) + offset;
            newPolygon.points[2] = new Vector2(size.x, size.y) + offset;
            newPolygon.points[3] = new Vector2(size.x, -size.y) + offset;

            return newPolygon;
        }

        public static List<Polygon2> CreateFromMeshCollider(MeshCollider meshCollider)
        {
            var newPolygons = new List<Polygon2>();

            var mesh = meshCollider.sharedMesh;
            var length = mesh.triangles.Length;

            var meshVertices = mesh.vertices;
            var meshTriangles = mesh.triangles;

            for (var i = 0; i < length; i = i + 3)
            {
                Vector2 vecA = meshVertices[meshTriangles[i]];
                Vector2 vecB = meshVertices[meshTriangles[i + 1]];
                Vector2 vecC = meshVertices[meshTriangles[i + 2]];

                var poly = new Polygon2(3);
                poly.points[0] = vecA;
                poly.points[1] = vecB;
                poly.points[2] = vecC;

                newPolygons.Add(poly);
            }

            return newPolygons;
        }


        public static Polygon2 CreateFromSphereCollider(SphereCollider sphereCollider, int pointsCount = -1)
        {
            if (pointsCount < 1) pointsCount = defaultCircleVerticesCount;

            var newPolygon = new Polygon2D();

            var size = sphereCollider.radius;
            float i = 0;

            Vector2 offset = sphereCollider.center;

            while (i < 360)
            {
                newPolygon.AddPoint(new Vector2(Mathf.Cos(i * Mathf.Deg2Rad) * size,
                    Mathf.Sin(i * Mathf.Deg2Rad) * size) + offset);
                i += 360f / pointsCount;
            }

            return new Polygon2(newPolygon);
        }

        public static Polygon2 CreateFromCapsuleCollider(CapsuleCollider capsuleCollider, int pointsCount = -1)
        {
            if (pointsCount < 1) pointsCount = defaultCircleVerticesCount;

            var newPolygon = new Polygon2D();

            var radius = capsuleCollider.radius;
            var height = capsuleCollider.height / 2;

            var size = new Vector2(capsuleCollider.radius, capsuleCollider.radius);
            Vector2 offset = capsuleCollider.center;

            float off = 0;

            if (height > radius) off = height - radius;

            float i = 0;

            while (i < 180)
            {
                var v = new Vector2(Mathf.Cos(i * Mathf.Deg2Rad) * size.x, off + Mathf.Sin(i * Mathf.Deg2Rad) * size.x);
                newPolygon.AddPoint(v + offset);
                i += 360f / pointsCount;
            }

            while (i < 360)
            {
                var v = new Vector2(Mathf.Cos(i * Mathf.Deg2Rad) * size.x,
                    -off + Mathf.Sin(i * Mathf.Deg2Rad) * size.x);
                newPolygon.AddPoint(v + offset);
                i += 360f / pointsCount;
            }

            return new Polygon2(newPolygon);
        }
    }
}