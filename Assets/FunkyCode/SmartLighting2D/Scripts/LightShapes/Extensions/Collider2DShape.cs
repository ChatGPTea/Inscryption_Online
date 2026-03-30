using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.LightShape
{
    public class Collider2DShape : Base
    {
        public bool edgeCollider2D;

        public override List<MeshObject> GetMeshes()
        {
            if (Meshes == null)
            {
                var polygons = GetPolygonsLocal();

                if (polygons.Count > 0)
                {
                    Meshes = new List<MeshObject>();

                    foreach (var poly in polygons)
                    {
                        if (poly.points.Length < 3) continue;

                        var mesh = PolygonTriangulator2.Triangulate(poly, Vector2.zero, Vector2.zero,
                            PolygonTriangulator2.Triangulation.Advanced);

                        if (mesh)
                        {
                            var meshObject = MeshObject.Get(mesh);

                            if (meshObject != null) Meshes.Add(meshObject);
                        }
                    }
                }
            }

            return Meshes;
        }

        public override List<Polygon2> GetPolygonsLocal()
        {
            if (LocalPolygons != null) return LocalPolygons;

            if (transform == null) return LocalPolygons;

            // avoid GC if possible

            LocalPolygons = Polygon2ListCollider2D.CreateFromGameObject(transform.gameObject);

            if (LocalPolygons.Count > 0) edgeCollider2D = transform.GetComponent<EdgeCollider2D>() != null;

            return LocalPolygons;
        }

        public override List<Polygon2> GetPolygonsWorld()
        {
            if (WorldPolygons != null) return WorldPolygons;

            if (WorldCache != null)
            {
                WorldPolygons = WorldCache;

                Polygon2 poly;
                Polygon2 wPoly;

                var list = GetPolygonsLocal();

                for (var i = 0; i < list.Count; i++)
                {
                    poly = list[i];
                    wPoly = WorldPolygons[i];

                    for (var p = 0; p < poly.points.Length; p++) wPoly.points[p] = poly.points[p];

                    wPoly.ToWorldSpaceSelfUNIVERSAL(transform);
                }
            }
            else
            {
                WorldPolygons = new List<Polygon2>();

                if (GetPolygonsLocal() != null)
                    foreach (var poly in GetPolygonsLocal())
                        WorldPolygons.Add(poly.ToWorldSpace(transform));

                WorldCache = WorldPolygons;
            }

            return WorldPolygons;
        }
    }
}